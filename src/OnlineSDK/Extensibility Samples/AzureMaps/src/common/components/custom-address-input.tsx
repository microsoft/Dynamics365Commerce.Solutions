/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { getPayloadObject, getTelemetryAttributes, ITelemetryContent } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';
import { ChannelConfiguration } from '@msdyn365-commerce/retail-proxy';
import { SearchURL, MapsURL, SubscriptionKeyCredential, Aborter } from 'azure-maps-rest'; // Azure Maps search dependencies

export interface IAdressInput {
    id?: string;
    name?: string;
    className: string;
    type: string;
    value: string;
    maxLength?: number;
    autoFocus?: boolean;
    additionalAddributes?: object;
    telemetryContent?: ITelemetryContent;
    onChange(event: React.ChangeEvent<HTMLInputElement>): void;

    // Azure Maps search related
    useAzureMaps?: boolean;
    autoSuggestionEnabled?: boolean;
    autoSuggestOptions?: Microsoft.Maps.IAutosuggestOptions;
    channel?: ChannelConfiguration;
    locale?: string;
    countryRegionId?: string;
    isMapAPILoaded?: boolean;
    onAzureMapsSuggestionSelected?(suggestion: ISuggestion): void;
}

interface IStoreSelectorSearchFormState {
    suggestions: ISuggestion[];
    activeSuggestionId: string;
    activeSuggestionIndex: number;
    value: string;
    isSearchResultOpened: boolean;
}

interface ISearchResultAddress {
    streetNumber?: string;
    streetName?: string;
    municipality?: string;
    countrySecondarySubdivision?: string;
    countrySubdivision?: string;
    postalCode?: string;
    extendedPostalCode?: string;
    countryCode?: string;
    country?: string;
    countryCodeISO3?: string;
    freeformAddress?: string;
    countrySubdivisionName?: string;
    localName?: string;
    countrySubdivisionCode?: string;
}

export interface ISuggestion {
    formattedSuggestion: string;
    title: string;
    subtitle: string;

    address: ISearchResultAddress;
}
/**
 * Address input.
 * @param props - Configuration of the functional component.
 * @returns React functional component.
 */
export const AddressInputFunctionComponent: React.FC<IAdressInput> = (props: IAdressInput) => {
    const {
        id: itemId,
        name,
        className,
        type,
        value,
        maxLength,
        additionalAddributes,
        autoFocus: shouldUseAutoFocus,
        telemetryContent,
        onChange,
        onAzureMapsSuggestionSelected,
        countryRegionId,
        locale,
        channel
    } = props;
    const [state, setState] = React.useState<IStoreSelectorSearchFormState>({
        suggestions: [],
        activeSuggestionId: '',
        activeSuggestionIndex: -1,
        value: props.value || '',
        isSearchResultOpened: false
    });

    const payLoad = getPayloadObject('click', telemetryContent!, name!);
    const attributes = getTelemetryAttributes(telemetryContent!, payLoad);

    const pipeline = MapsURL.newPipeline(new SubscriptionKeyCredential(channel?.BingMapsApiKey || ''));
    const azureMapsSearchClient: SearchURL = new SearchURL(pipeline);

    const onTextInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const inputValue: string = event.target.value;
        setState(prevState => ({
            ...prevState,
            value: inputValue
        }));
        if (inputValue && inputValue !== '' && name === 'Street') {
            getSuggestions(inputValue).then(suggestions => {
                setState(prevState => ({
                    ...prevState,
                    suggestions: suggestions,
                    isSearchResultOpened: true
                }));
            });
        } else if (inputValue === '') {
            setState(prevState => ({
                ...prevState,
                suggestions: [],
                isSearchResultOpened: false
            }));
        }
        onChange(event);
    };

    const _onSuggestionSelected = (suggestion: ISuggestion): void => {
        setState(prevState => ({
            ...prevState,
            isSearchResultOpened: false,
            value: suggestion.formattedSuggestion,
            suggestions: []
        }));
        if (onAzureMapsSuggestionSelected) {
            onAzureMapsSuggestionSelected(suggestion);
        }
    };

    const getSuggestions = async (query: string): Promise<ISuggestion[]> => {
        return new Promise((resolve, reject) => {
            azureMapsSearchClient
                .searchFuzzy(Aborter.timeout(10000), query, {
                    limit: 5,
                    maxFuzzyLevel: 1,
                    language: locale,
                    typeahead: true,
                    countrySet: [countryRegionId ?? 'US'],
                    entityType: ['Municipality']
                })
                .then(response => {
                    resolve(
                        response.results?.map(result => ({
                            formattedSuggestion: result.address?.freeformAddress || '',
                            title: result.address?.freeformAddress || '',
                            subtitle: result.address?.municipality || '',
                            address: result.address ?? {}
                        })) || []
                    );
                })
                .catch(error => {
                    console.error('Error fetching suggestions from Azure Maps:', error);
                    reject([]);
                });
        });
    };
    const suggestionListView = state.suggestions.map((suggestion: ISuggestion, index: number) => (
        <li
            key={index}
            id={`result-as-${index}`}
            role='listitem'
            aria-selected={state.activeSuggestionId === `result-as-${index}`}
            aria-label={`${suggestion.formattedSuggestion}, item ${index + 1}`}
            onClick={() => _onSuggestionSelected(suggestion)}
            tabIndex={0}
        >
            <div className={`suggestLink ${state.activeSuggestionId === `result-as-${index}` ? 'selected' : ''}`} data-tag='as_suggestion'>
                <div className='as_suggestion_root_inside' data-tag='as_suggestion_root_inside'>
                    <div className='as_img maps_address' data-tag='as_img'></div>
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
        <>
            <input
                name={name}
                id={itemId}
                className={`${className}__input ${className}__input-${type}`}
                type={type}
                autoFocus={shouldUseAutoFocus}
                value={value}
                maxLength={maxLength}
                {...(additionalAddributes || {})}
                {...attributes}
                onChange={onTextInputChange}
            />
            {name === 'Street' && (
                <div className='MicrosoftMap'>
                    <div
                        className='as_container_search as_container'
                        id={`as_containerSearch_${itemId}`}
                        data-tag='as_container'
                        style={{ display: state.isSearchResultOpened ? 'block' : 'none' }}
                    >
                        <div className='b_cards asOuterContainer'>
                            <ul role='list'>{suggestionListView}</ul>
                        </div>
                        <div className='clear'></div>
                    </div>
                </div>
            )}
        </>
    );
};

export default AddressInputFunctionComponent;
