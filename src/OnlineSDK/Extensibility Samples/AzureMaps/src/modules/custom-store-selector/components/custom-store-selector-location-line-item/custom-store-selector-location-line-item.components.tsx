/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { DeliveryOption, OrgUnitContact, OrgUnitLocation, StoreHours } from '@msdyn365-commerce/retail-proxy';
import { ArrayExtensions } from '@msdyn365-commerce-modules/retail-actions';
import {
    getPayloadObject,
    getTelemetryAttributes,
    IPayLoad,
    ITelemetryContent,
    TelemetryConstant
} from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { PickUpOptionList } from '../pickup-option-list';
import { buildStoreHours, IHoursDisplayInfo, secondsToTime } from './date-time-helpers';

/**
 * Store Locator line item resources.
 */
export interface IStoreSelectorLocationLineItemResources {
    contactInfoHeader: string;
    storeHoursHeader: string;
    availabilityInfoHeader: string;

    closedText: string;

    outOfStock: string;
    inStock: string;

    currentLocation: string;
    selectLocation: string;
    selectLocationAriaLabelFormat: string;
    setAsPreferredStoreText: string;
    setAsPreferredStoreTextAriaLabel: string;
    preferredStoreText: string;
    preferredStoreAriaLabel: string;
    pickupDeliveryOptionErrorMessage?: string;

    days: {
        monday: string;
        tuesday: string;
        wednesday: string;
        thursday: string;
        friday: string;
        saturday: string;
        sunday: string;
        mondayFull: string;
        tuesdayFull: string;
        wednesdayFull: string;
        thursdayFull: string;
        fridayFull: string;
        saturdayFull: string;
        sundayFull: string;
    };
}

/**
 * Store Locator line item props.
 */
export interface IStoreSelectorLocationLineItemProps {
    location: OrgUnitLocation;
    storeHours?: StoreHours;

    hideStockStatus?: boolean;
    isInStock?: boolean;
    isCurrentLocation?: boolean;

    resources: IStoreSelectorLocationLineItemResources;
    stockStatusLabel?: string;
    storeLocatorView?: boolean;
    isPreferredStoreEnabled?: boolean;
    index: string;
    preferredStoreLocationId: string | null;
    storePickUpOptionList?: DeliveryOption[];
    productPickUpOptionList?: DeliveryOption[];
    filteredPickupMode?: string;
    shouldShowIndex?: boolean;

    /**
     * The telemetry content.
     */
    telemetryContent?: ITelemetryContent;
    preferredDistanceUnit: string;
    isLocationDisabled?: boolean;
    handlers: {
        onSelected(location: OrgUnitLocation, deliveryMode?: string): void;
        onSetAsPreferredStore(locationId: string | undefined): void;
        onRemovePreferredStore(): void;
    };
}

/**
 * Store Locator line item action component.
 */
const storeSelectorLocationLineItemComponentActions = {
    onLocationSelected(props: IStoreSelectorLocationLineItemProps, deliveryMode?: string): void {
        props.handlers.onSelected(props.location, deliveryMode);
    },
    onSetAsPreferredStore(props: IStoreSelectorLocationLineItemProps): void {
        props.handlers.onSetAsPreferredStore(props.location.OrgUnitNumber);
    },
    onRemovePreferredStore(props: IStoreSelectorLocationLineItemProps): void {
        props.handlers.onRemovePreferredStore();
    }
};

/**
 * Function to render store hours.
 * @param storeHours - Store Hours object.
 * @param location - Store Unit.
 * @param resources - Resources.
 * @returns - Jsx element.
 */
const renderStoreHours = (
    storeHours: StoreHours | undefined,
    location: OrgUnitLocation,
    resources: IStoreSelectorLocationLineItemResources
): JSX.Element | null => {
    // First, use the store hours. This will allow listing day by day houts
    if (storeHours?.RegularStoreHours) {
        const storeHoursList: IHoursDisplayInfo[] = buildStoreHours(storeHours.RegularStoreHours, resources);

        return (
            <div className='ms-store-select__store-hours-details-group'>
                {storeHoursList.map(displayTime => (
                    <div className='ms-store-select__store-hours-details' key={JSON.stringify(displayTime)}>
                        {displayTime.endDayShort ? (
                            <div className='ms-store-select__store-hours-day'>
                                <abbr title={displayTime.startDayFull}>{displayTime.startDayShort}</abbr>–
                                <abbr title={displayTime.endDayFull}>{displayTime.endDayShort}</abbr>
                            </div>
                        ) : (
                            <div className='ms-store-select__store-hours-day'>
                                <abbr title={displayTime.startDayFull}>{displayTime.startDayShort}</abbr>
                            </div>
                        )}
                        <div className='ms-store-select__store-hours-time'>{displayTime.timeRange}</div>
                    </div>
                ))}
            </div>
        );
    }

    // If that fails, fall back to using the OpenFrom and OpenTo properties on the location
    const storeOpenFrom: string | undefined = secondsToTime(location.OpenFrom);
    const storeOpenTo: string | undefined = secondsToTime(location.OpenTo);

    return (
        <div className='ms-store-select__store-hours-details'>
            {storeOpenFrom}-{storeOpenTo}
        </div>
    );
};

/**
 * Function to return distance measure unit.
 * @param distance - Store location within radius.
 * @param preferredDistanceUnit - Unit configuration.
 * @param isLocationDisabled - Flag to check user location.
 * @returns - Unit of distance.
 */
export const buildDistanceString = (
    distance: number | undefined,
    preferredDistanceUnit: string,
    isLocationDisabled?: boolean
): string | undefined => {
    if (distance === undefined || isLocationDisabled) {
        return undefined;
    } else if (distance < 1) {
        return `(<1 ${preferredDistanceUnit})`;
    }
    return `(${Math.floor(distance)} ${preferredDistanceUnit})`;
};

/**
 * Function to store contact.
 * @param contacts - Store location within radius.
 * @returns - Return primary contact number of store.
 */
const extractPhoneNumber = (contacts: OrgUnitContact[] | undefined): string | undefined => {
    if (!contacts) {
        return undefined;
    }

    const allPhoneContacts = contacts.filter(contact => contact.ContactTypeValue === 1 && !contact.IsPrivate);

    if (ArrayExtensions.hasElements(allPhoneContacts)) {
        const primaryPhoneContact = allPhoneContacts.find(contact => contact.IsPrimary);

        if (primaryPhoneContact) {
            return primaryPhoneContact.Locator;
        }

        return allPhoneContacts[0].Locator;
    }

    return undefined;
};

/**
 * On Store locator click functionality.
 * @param props -Store locator location items.
 * @param pickupMode -Store locator location items.
 * @returns Store locator selected location line item action.
 */
const onPickupModeSelectHandler = (props: IStoreSelectorLocationLineItemProps, pickupMode?: string) => () => {
    storeSelectorLocationLineItemComponentActions.onLocationSelected(props, pickupMode);
};

/**
 * Set Preferred store functionality.
 * @param props -Store locator location items.
 * @returns Store locator selected location line item action.
 */
const onSetPreferredStoreClickHandler = (props: IStoreSelectorLocationLineItemProps) => () => {
    storeSelectorLocationLineItemComponentActions.onSetAsPreferredStore(props);
};

/**
 * Remove preferred store functionality.
 * @param props -Store locator location items.
 * @returns Store locator selected location line item action.
 */
const removePreferredStoreHandler = (props: IStoreSelectorLocationLineItemProps) => () => {
    storeSelectorLocationLineItemComponentActions.onRemovePreferredStore(props);
};

/**
 * On Change functionality.
 * @param props -Store locator location items.
 * @returns Store locator selected location line item action.
 */
const onChangeHandler = (props: IStoreSelectorLocationLineItemProps) => async (deliveryCode: string): Promise<void> => {
    storeSelectorLocationLineItemComponentActions.onLocationSelected(props, deliveryCode);
    return Promise.resolve();
};

/**
 * Method to render pickup modes.
 * @param props - StoreSelector Line item props.
 * @param deliveryOptionList - Product DeliveryOption List.
 * @param pickupInStoreButtonText - Pickup store button text.
 * @param pickupInStoreAriaLabel - Pickup store aria label.
 * @returns Jsx component.
 */
const renderPickupModes = (
    props: IStoreSelectorLocationLineItemProps,
    deliveryOptionList: DeliveryOption[],
    pickupInStoreButtonText: string,
    pickupInStoreAriaLabel: string
): JSX.Element | null => {
    if (props.filteredPickupMode === undefined) {
        return (
            <PickUpOptionList
                pickupListOptionMenuText={pickupInStoreButtonText}
                pickupmodeList={deliveryOptionList}
                onChange={onChangeHandler(props)}
            />
        );
    }
    return (
        <button
            className='ms-store-select__location-line-select-store'
            aria-label={pickupInStoreAriaLabel}
            onClick={onPickupModeSelectHandler(props, props.filteredPickupMode)}
        >
            {pickupInStoreButtonText}
        </button>
    );
};

/**
 * Method to check if we have atleast one common delivery code between product and channel.
 * @param productdeliveryOptions - Product DeliveryOption List.
 * @param storePickUpOptionList - Channel DeliveryOption List.
 * @returns DeliveryOptionList.
 */
export const matchDeliveryOptions = (
    productdeliveryOptions: DeliveryOption[] | undefined,
    storePickUpOptionList: DeliveryOption[] | undefined
): DeliveryOption[] => {
    const deliveryOption: DeliveryOption[] = [];
    productdeliveryOptions?.map(delivery => {
        const pickup = storePickUpOptionList?.find(_delivery => _delivery.Code === delivery.Code);
        if (pickup) {
            deliveryOption.push(pickup);
        }
    });

    return deliveryOption;
};

/**
 * Renders store location line items.
 * @param props - Store location line items props.
 * @returns - HTML.
 */
const storeSelectorLocationLineItem: React.FC<IStoreSelectorLocationLineItemProps> = (props: IStoreSelectorLocationLineItemProps) => {
    const {
        hideStockStatus,
        isCurrentLocation,
        isInStock,
        location,
        resources,
        storeHours,
        stockStatusLabel,
        storeLocatorView,
        index,
        preferredStoreLocationId,
        isPreferredStoreEnabled,
        isLocationDisabled,
        telemetryContent,
        storePickUpOptionList,
        productPickUpOptionList,
        shouldShowIndex
    } = props;

    const storeName: string | undefined = location.OrgUnitName;
    const distanceAsString: string | undefined = buildDistanceString(location.Distance, props.preferredDistanceUnit, isLocationDisabled);
    const storeAddress: string | undefined = location.Address;
    const phoneNumber: string | undefined = extractPhoneNumber(location.Contacts);

    const resourceLabel = isInStock ? resources.inStock : resources.outOfStock;
    const stockLabel = stockStatusLabel ? stockStatusLabel : undefined;
    const stockText: string | undefined = hideStockStatus ? undefined : stockLabel ? stockLabel : resourceLabel;
    const canSelectLocation = hideStockStatus || isInStock;
    const pickupInStoreButtonText: string = resources.selectLocation;
    const pickupInStoreAriaLabel: string = resources.selectLocationAriaLabelFormat.replace('{StoreName}', storeName || '');
    const isPreferredStore = preferredStoreLocationId === location.OrgUnitNumber;
    const payLoad: IPayLoad = getPayloadObject('click', telemetryContent!, TelemetryConstant.PickupInStore);
    const attributes = getTelemetryAttributes(telemetryContent!, payLoad);
    payLoad.contentAction.etext = TelemetryConstant.PreferredStore;
    const preferredStoreAttributes = getTelemetryAttributes(telemetryContent!, payLoad);
    payLoad.contentAction.etext = TelemetryConstant.RemovePreferredStore;
    const removePreferredStoreAttributes = getTelemetryAttributes(telemetryContent!, payLoad);
    const deliveryOptionList = matchDeliveryOptions(productPickUpOptionList, storePickUpOptionList);
    const [isError, setError] = React.useState(false);

    const pickUpButtonClickHandler = (props: IStoreSelectorLocationLineItemProps) => () => {
        if (deliveryOptionList.length === 0) {
            setError(true);
        } else {
            storeSelectorLocationLineItemComponentActions.onLocationSelected(props);
        }
    };

    /* eslint-disable prettier/prettier */
    return (
        <div className='ms-store-select__location-line-item'>
            <div className='ms-store-select__location-line-item-header'>
                {(shouldShowIndex || storeLocatorView) && <span className='ms-store-select__location-line-item-store-index'>{index}</span>}
                <span className='ms-store-select__location-line-item-store-name'>{storeName}</span>
                {distanceAsString ? <span className='ms-store-select__location-line-item-store-distance'>
                {' '}
                {distanceAsString}
                </span> : false}
            </div>
            <div className='ms-store-select__location-line-item-content'>
                <div className='ms-store-select__location-line-item-contact-info'>
                    <p className='ms-store-select__location-line-header'>{resources.contactInfoHeader}</p>
                    {storeAddress && (
                        <div className='ms-store-select__location-line-shop-address-section'>
                            <span className='msi msi-map-marker-alt ms-store-select__location-line-shop-address-glyph-icon' />
                            <span className='ms-store-select__location-line-shop-address-text'>{storeAddress}</span>
                        </div>
                    )}
                    {phoneNumber && (
                        <div className='ms-store-select__location-line-shop-phone-section'>
                            <span className='msi msi-phone ms-store-select__location-line-shop-phone-icon' />
                            <span className='ms-store-select__location-line-shop-phone-text'>{phoneNumber}</span>
                        </div>
                    )}
                </div>
                <div className='ms-store-select__location-line-item-store-hours'>
                    <p className='ms-store-select__location-line-header'>{resources.storeHoursHeader}</p>
                    {renderStoreHours(storeHours, location, resources)}
                </div>
                {!storeLocatorView && (
                    <div className='ms-store-select__location-line-item-store-availability'>
                        {!hideStockStatus && <p className='ms-store-select__location-line-header'>{resources.availabilityInfoHeader}</p>}
                        {stockText && <p className='ms-store-select__location-line-stock-status'>{stockText}</p>}
                        {isCurrentLocation ? (
                            <p className='ms-store-select__location-line-current-location'>{resources.currentLocation}</p>
                        ) : canSelectLocation ? (
                            deliveryOptionList.length === 0 ? (
                                <button
                                    className='ms-store-select__location-line-select-store'
                                    aria-label={pickupInStoreAriaLabel}
                                    onClick={pickUpButtonClickHandler(props)}
                                    {...attributes}
                                >
                                    {pickupInStoreButtonText}
                                </button>
                            ) : (
                                renderPickupModes(props, deliveryOptionList, pickupInStoreButtonText, pickupInStoreAriaLabel)
                            )
                        ) : (
                            false
                        )}
                    </div>
                )}
                {isPreferredStoreEnabled ? (
                    isPreferredStore ? (
                        <button
                            role='checkbox'
                            className='ms-store-select__location-line-item-preferred-store'
                            aria-live='polite'
                            aria-checked='true'
                            aria-label={resources.preferredStoreAriaLabel}
                            onClick={removePreferredStoreHandler(props)}
                            {...removePreferredStoreAttributes}
                        >
                            {resources.preferredStoreText}
                        </button>
                    ) : (
                        <button
                            role='checkbox'
                            className='ms-store-select__location-line-item-set-as-preferred-store'
                            aria-live='polite'
                            aria-checked='false'
                            aria-label={resources.setAsPreferredStoreTextAriaLabel}
                            {...preferredStoreAttributes}
                            onClick={onSetPreferredStoreClickHandler(props)}
                        >
                            {resources.setAsPreferredStoreText}
                        </button>
                    )
                ) : null}
            </div>
            {isError ? (
                <span className='ms-store-select__location-line-pickup-list-error msc-alert msc-alert-noborder msc-alert-danger'>
                    <span className='msi-exclamation-triangle' aria-hidden='true' />
                    <span>{props.resources.pickupDeliveryOptionErrorMessage}</span>
                </span>
            ) : ('')}
        </div>
    );
};

/**
 * Store location line item component.
 */
export const StoreSelectorLocationLineItemComponent = storeSelectorLocationLineItem as (
    props: IStoreSelectorLocationLineItemProps
) => JSX.Element;
