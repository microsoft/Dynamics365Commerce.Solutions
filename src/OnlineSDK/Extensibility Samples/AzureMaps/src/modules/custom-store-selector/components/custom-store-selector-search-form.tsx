/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import * as Msdyn365 from '@msdyn365-commerce/core';
import { DeliveryOption, ChannelConfiguration } from '@msdyn365-commerce/retail-proxy';
import {
    getPayloadObject,
    getTelemetryAttributes,
    IPayLoad,
    ITelemetryContent,
    KeyCodes,
    TelemetryConstant
} from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';
import { SearchURL, MapsURL, SubscriptionKeyCredential, Aborter } from 'azure-maps-rest';

import { StorePickUpOptionList } from './store-pickup-option-list';
import { IMapClient, ISuggestion } from '../interfaces';

/**
 * Store Selector Search Form Props interface.
 */
export interface IStoreSelectorSearchFormProps {
    resources: {
        searchInputAriaLabel: string;
        searchButtonAriaLabel: string;
        searchPlaceholderText: string;
        seeAllStoresText: string;
        viewListText: string;
        viewMapText: string;
        pickupFilterByHeading?: string;
        pickupFilterMenuHeading?: string;
    };

    id: string;
    value: string;
    showAllStores?: boolean;
    displayList?: boolean;
    locatorView?: boolean;
    productPickUpDeliveryOptions?: DeliveryOption[];
    hasEnabledPickupFilterToShowStore?: boolean;
    filteredPickupMode?: string;
    autoSuggestionEnabled?: boolean;
    autoSuggestOptions?: Microsoft.Maps.IAutosuggestOptions;
    telemetry?: Msdyn365.ITelemetry;
    channel?: ChannelConfiguration;
    locale?: string;
    isMapAPILoaded?: boolean;

    /**
     * The telemetry content.
     */
    telemetryContent?: ITelemetryContent;
    onShowAllStores(): void;
    onToggleListMapViewState(): void;
    performSearch(searchTerm: string, latitude?: number, longitude?: number): Promise<void>;
    searchTermChanged(searchTerm: string): Promise<void>;
    filterPickupModeSelected(pickupMode: string): void;
    onSuggestionsSelected(suggestion: ISuggestion): void; // Update type to support both Bing and Azure Maps
}

interface IStoreSelectorSearchFormState {
    suggestions: ISuggestion[];
    activeSuggestionId: string;
    activeSuggestionIndex: number;
    value: string;
    isSearchResultOpened: boolean;
}

class AzureMapsClient implements IMapClient {
    private azureMapsSearchClient: SearchURL;
    private locale: string | undefined;
    private channel: ChannelConfiguration | undefined;
    private maxResults: number;
    private telemetry: Msdyn365.ITelemetry;

    constructor(
        channel: ChannelConfiguration | undefined,
        telemetry: Msdyn365.ITelemetry,
        locale?: string,
        autoSuggestOptions?: Microsoft.Maps.IAutosuggestOptions
    ) {
        const pipeline = MapsURL.newPipeline(new SubscriptionKeyCredential(channel?.BingMapsApiKey || ''));
        this.azureMapsSearchClient = new SearchURL(pipeline);
        this.locale = locale;
        this.channel = channel;
        this.telemetry = telemetry;
        this.maxResults = autoSuggestOptions?.maxResults || 5;
    }

    public initialize(isMapAPILoaded: boolean): void {
        // No initialization needed for Azure Maps
    }

    public async getSuggestions(query: string): Promise<ISuggestion[]> {
        return new Promise((resolve, reject) => {
            this.azureMapsSearchClient
                .searchFuzzy(Aborter.timeout(10000), query, {
                    limit: this.maxResults,
                    maxFuzzyLevel: 1,
                    language: this.locale || '',
                    typeahead: true,
                    entityType: ['Municipality'],
                    ...(this.channel?.ChannelCountryRegionISOCode ? { countrySet: [this.channel.ChannelCountryRegionISOCode] } : {})
                })
                .then(response => {
                    return resolve(
                        response.results?.map(result => ({
                            formattedSuggestion: result.address?.freeformAddress || '',
                            title: result.address?.freeformAddress || '',
                            subtitle: [result.address?.municipality, result.address?.countrySubdivisionName, result.address?.country]
                                .filter((part): part is string => !!part)
                                .join(', '),
                            latitude: result.position?.lat,
                            longitude: result.position?.lon
                        })) || []
                    );
                })
                .catch((error: Error) => {
                    this.telemetry.error('Error fetching suggestions from Azure Maps:', error);
                    console.log('Error fetching suggestions from Azure Maps:', error);
                    reject([]);
                });
        });
    }
}

/**
 * Simple search form consisting of search text and a search button.
 */
export class StoreSelectorSearchForm extends React.PureComponent<IStoreSelectorSearchFormProps, IStoreSelectorSearchFormState> {
    public mapClient: IMapClient;

    private readonly searchBoxRef: React.RefObject<HTMLInputElement> = React.createRef<HTMLInputElement>();

    private readonly storeSearchAttributes: Msdyn365.IDictionary<string> | undefined;

    private readonly showAllStoreAttributes: Msdyn365.IDictionary<string> | undefined;

    private readonly toggleMapViewAttributes: Msdyn365.IDictionary<string> | undefined;

    private previousValue: string = '';

    public constructor(props: IStoreSelectorSearchFormProps) {
        super(props);
        this.state = {
            suggestions: [],
            activeSuggestionId: '',
            activeSuggestionIndex: -1,
            value: props.value || '',
            isSearchResultOpened: false
        };
        this._handleKeyPressPrev = this._handleKeyPressPrev.bind(this);
        const payLoad: IPayLoad = getPayloadObject('click', props.telemetryContent!, TelemetryConstant.SearchStore);
        this.storeSearchAttributes = getTelemetryAttributes(props.telemetryContent!, payLoad);
        payLoad.contentAction.etext = TelemetryConstant.ShowAllStore;
        this.showAllStoreAttributes = getTelemetryAttributes(props.telemetryContent!, payLoad);
        payLoad.contentAction.etext = TelemetryConstant.ToggleMapView;
        this.toggleMapViewAttributes = getTelemetryAttributes(props.telemetryContent!, payLoad);
        const telemetry = props.telemetry || ({} as Msdyn365.ITelemetry);
        this.mapClient = new AzureMapsClient(props.channel, telemetry, props.locale);
    }

    public componentDidMount(): void {
        this.mapClient.initialize(this.props.isMapAPILoaded || false);
    }

    public componentDidUpdate(prevProps: IStoreSelectorSearchFormProps): void {
        if (this.props.isMapAPILoaded && !prevProps.isMapAPILoaded) {
            this.mapClient.initialize(true);
        }
    }

    public render(): JSX.Element {
        const {
            resources: {
                searchButtonAriaLabel,
                searchPlaceholderText,
                seeAllStoresText,
                viewListText,
                viewMapText,
                pickupFilterByHeading,
                pickupFilterMenuHeading
            },
            id,
            value,
            channel,
            showAllStores,
            displayList,
            locatorView,
            onShowAllStores,
            productPickUpDeliveryOptions,
            hasEnabledPickupFilterToShowStore
        } = this.props;

        const isMapDisabled =
            !(channel?.BingMapsApiKey && channel.BingMapsEnabled) || !(channel?.BingMapsApiKey && channel.BingMapsEnabled);
        const { activeSuggestionId, suggestions } = this.state;
        const toggleButtonText = displayList ? viewMapText : viewListText;
        const suggestionListView = suggestions.map((suggestion: ISuggestion, index: number) => (
            <li
                key={index}
                id={`result-as-${index}`}
                role='listitem'
                aria-selected={this.state.activeSuggestionId === `result-as-${index}`}
                aria-label={`${suggestion.formattedSuggestion}, item ${index + 1}`}
                onClick={() => this.onSuggestionSelected(suggestion)}
                tabIndex={0}
            >
                <div
                    className={`suggestLink ${this.state.activeSuggestionId === `result-as-${index}` ? 'selected' : ''}`}
                    data-tag='as_suggestion'
                >
                    <div className='as_suggestion_root_inside' data-tag='as_suggestion_root_inside'>
                        <div className='as_img maps_address' data-tag='as_img' style={{ height: '37px' }}></div>
                        <div className='as_lines_root' data-tag='as_lines_root'>
                            <p className='line1' data-tag='as_suggestion_line'>
                                {suggestion.title}
                            </p>
                            <p className='line2' data-tag='as_suggestion_line'>
                                {suggestion.subtitle}
                            </p>
                        </div>
                    </div>
                </div>
            </li>
        ));

        return (
            <div className='ms-store-select__search'>
                <form
                    className='ms-store-select__search-form'
                    aria-label={searchButtonAriaLabel}
                    name='storeSelectorSearchForm'
                    autoComplete='off'
                    onSubmit={this.onSubmit}
                    id={`ms-store-select__search-box-container_${id}`}
                >
                    <input
                        type='search'
                        aria-label={this.props.resources.searchInputAriaLabel}
                        className='msc-form-control ms-store-select__search-input'
                        placeholder={searchPlaceholderText}
                        value={value}
                        onChange={this.searchTextChanged}
                        id={`ms-store-select__search-box_${id}`}
                        ref={this.searchBoxRef}
                        role='combobox'
                        aria-expanded={this.state.isSearchResultOpened}
                        aria-controls={`as_containerSearch_${id}`}
                        onKeyUp={this._handleInputKeyPress}
                        aria-activedescendant={activeSuggestionId}
                        autoCorrect='off'
                        autoComplete='street-address'
                        autoCapitalize='off'
                        aria-owns={`as_containerSearch_${id}`}
                        aria-autocomplete='list'
                        style={{ boxSizing: 'content-box' }}
                    />
                    <div className='MicrosoftMap' style={{ position: 'static' }}>
                        <div
                            className='as_container_search as_container'
                            id={`as_containerSearch_${id}`}
                            data-tag='as_container'
                            style={{ visibility: this.state.isSearchResultOpened ? 'visible' : 'hidden' }}
                        >
                            <div className='b_cards asOuterContainer'>
                                <ul role='list'>{suggestionListView}</ul>
                            </div>
                            <div className='clear'></div>
                        </div>
                    </div>
                    <button
                        className={`ms-store-select__search-button${isMapDisabled ? ' search-btn-disabled' : ''}`}
                        aria-label={searchButtonAriaLabel}
                        color='primary'
                        {...this.storeSearchAttributes}
                    />
                </form>
                {showAllStores && (
                    <div
                        className='ms-store-select__search-see-all-stores'
                        tabIndex={0}
                        role='button'
                        onKeyUp={this._handleKeyPressPrev}
                        onClick={onShowAllStores}
                        {...this.showAllStoreAttributes}
                    >
                        {seeAllStoresText}
                    </div>
                )}
                {this.renderPickupModesList(
                    this.props,
                    toggleButtonText,
                    productPickUpDeliveryOptions,
                    pickupFilterMenuHeading,
                    locatorView,
                    pickupFilterByHeading,
                    hasEnabledPickupFilterToShowStore,
                    displayList
                )}
            </div>
        );
    }

    /**
     * Method called on search text changed.
     * @param event - Input value.
     */
    public readonly searchTextChanged = async (event: React.ChangeEvent<HTMLInputElement>): Promise<void> => {
        const inputValue: string = event.target.value;

        this.setState({ value: inputValue });
        if (inputValue && inputValue !== '') {
            this.mapClient.getSuggestions(inputValue).then(suggestions => {
                this.setState({ suggestions, isSearchResultOpened: true });
            });
        } else if (inputValue === '') {
            this.setState({ isSearchResultOpened: false, suggestions: [] });
        }
        await this.props.searchTermChanged(inputValue);
    };

    /**
     * Method called on search suggestion selected.
     * @param suggestion - Selected suggestion.
     */
    public readonly onSuggestionSelected = (suggestion: ISuggestion): void => {
        this.searchBoxRef.current?.blur();
        this.props.onSuggestionsSelected({
            formattedSuggestion: suggestion.formattedSuggestion,
            title: suggestion.title,
            subtitle: suggestion.subtitle,
            latitude: suggestion.latitude,
            longitude: suggestion.longitude
        });
        this.setState({ isSearchResultOpened: false, value: suggestion.formattedSuggestion });
    };

    /**
     * Method called on search submit.
     * @param event - Input value.
     */
    public readonly onSubmit = async (event: React.SyntheticEvent): Promise<void> => {
        event.preventDefault(); // Prevents form submission
        this.searchBoxRef.current?.blur();
        const suggestions = await this.mapClient.getSuggestions(this.state.value);
        const suggestion = suggestions[0];
        await this.props.performSearch(suggestion?.formattedSuggestion || this.state.value, suggestion?.latitude, suggestion?.longitude);
        this.setState({ isSearchResultOpened: false });
    };

    /**
     * Method called on product pickup option change.
     * @returns - Void.
     */
    public readonly onChangeHandler = () => async (deliveryCode: string): Promise<void> => {
        this.props.filterPickupModeSelected(deliveryCode);
        return Promise.resolve();
    };

    /**
     * Handles the key press on the input box.
     * @param event - Event object.
     */
    private readonly _handleInputKeyPress = (event: React.KeyboardEvent): void => {
        const { suggestions, activeSuggestionIndex } = this.state;
        if (event.keyCode === KeyCodes.ArrowDown) {
            event.preventDefault();
            const newIndex = (activeSuggestionIndex + 1) % suggestions.length;
            this.setState({
                activeSuggestionIndex: newIndex,
                activeSuggestionId: `result-as-${newIndex}`
            });
        } else if (event.keyCode === KeyCodes.ArrowUp) {
            event.preventDefault();
            const newIndex = (activeSuggestionIndex - 1 + suggestions.length) % suggestions.length;
            this.setState({
                activeSuggestionIndex: newIndex,
                activeSuggestionId: `result-as-${newIndex}`
            });
        } else if (event.keyCode === KeyCodes.Enter && activeSuggestionIndex >= 0) {
            event.preventDefault();
            const suggestion = suggestions.at(activeSuggestionIndex);
            if (suggestion) {
                this.onSuggestionSelected(suggestion);
            }
        }

        // When the user navigates through the up and down arrow on the result returned by the auto suggest and press enter.
        // At that time, two requests were made to fetch the store details due to which incorrect result is shown and causes flickering.
        // In order to avoid that. Checking if the search result is open or not.
        if (event.keyCode === KeyCodes.ArrowUp || event.keyCode === KeyCodes.ArrowDown) {
            const activeDescedantValue = (event.target as HTMLInputElement).getAttribute('aria-activedescendant');
            if (activeDescedantValue) {
                if (this.previousValue === '') {
                    this.previousValue = activeDescedantValue;
                    this.setState({ isSearchResultOpened: true });
                    return;
                }

                if (activeDescedantValue !== this.previousValue) {
                    this.previousValue = activeDescedantValue;
                    this.setState({ isSearchResultOpened: true });
                } else {
                    this.setState({ isSearchResultOpened: false });
                }
            }
        }
    };

    /**
     * Calls method display result.
     * @param event - The first number.
     */
    private readonly _handleKeyPressPrev = (event: React.KeyboardEvent): void => {
        if (event.keyCode === KeyCodes.Enter || event.keyCode === KeyCodes.Space) {
            this.props.onShowAllStores();
        }
    };

    /**
     * Method to render pickup mode list.
     * @param props - Store selector search form props.
     * @param toggleButtonText - Locator view button text.
     * @param productPickUpDeliveryOptions - Product DeliveryOption List.
     * @param pickupFilterMenuHeading - Pickup store button text.
     * @param locatorView - Locator View is on\off.
     * @param pickupFilterByHeading - Pickup list label text.
     * @param hasEnabledPickupFilterToShowStore - Flag to check header filter is enabled.
     * @param shouldDisplayList - Display List.
     * @returns Jsx component.
     */
    private readonly renderPickupModesList = (
        props: IStoreSelectorSearchFormProps,
        toggleButtonText: string,
        productPickUpDeliveryOptions: DeliveryOption[] | undefined,
        pickupFilterMenuHeading: string | undefined,
        locatorView: boolean | undefined,
        pickupFilterByHeading: string | undefined,
        hasEnabledPickupFilterToShowStore: boolean | undefined,
        shouldDisplayList?: boolean
    ): JSX.Element | null => {
        const toggleButtonClass = shouldDisplayList ? 'view-map' : 'list-view';
        if (hasEnabledPickupFilterToShowStore) {
            return (
                <div className='ms-store-select__search-header'>
                    {locatorView && (
                        <button
                            className={`ms-store-select__toggle-view ${toggleButtonClass}`}
                            onClick={props.onToggleListMapViewState}
                            {...this.toggleMapViewAttributes}
                        >
                            {toggleButtonText}
                        </button>
                    )}
                    <StorePickUpOptionList
                        productPickupListOptionMenuText={pickupFilterMenuHeading}
                        defaultOptionText={props.filteredPickupMode ? undefined : pickupFilterMenuHeading}
                        productPickupModeList={productPickUpDeliveryOptions}
                        onChange={this.onChangeHandler()}
                    />
                    <div className='ms-store-select__search-header-heading'>{pickupFilterByHeading}</div>
                </div>
            );
        }
        if (locatorView) {
            return (
                <button
                    className={`ms-store-select__toggle-view ${toggleButtonClass}`}
                    onClick={props.onToggleListMapViewState}
                    {...this.toggleMapViewAttributes}
                >
                    {toggleButtonText}
                </button>
            );
        }
        return null;
    };
}
