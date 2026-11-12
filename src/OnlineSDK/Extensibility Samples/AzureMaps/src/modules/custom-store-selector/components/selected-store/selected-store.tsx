/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { DeliveryOption, OrgUnitLocation } from '@msdyn365-commerce/retail-proxy';
import { IFullOrgUnitAvailability } from '@msdyn365-commerce-modules/retail-actions';
import {
    Button,
    getPayloadObject,
    getTelemetryAttributes,
    INodeProps,
    IPayLoad,
    ITelemetryContent,
    NodeTag,
    TelemetryConstant
} from '@msdyn365-commerce-modules/utilities';
import React from 'react';

import { PickUpOptionList } from '../pickup-option-list';
import { buildDistanceString, matchDeliveryOptions } from '../custom-store-selector-location-line-item';

/**
 * Store Locator line item resources.
 */
export interface ISelectedStoreResources {
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
}

/**
 * Store Selector resources.
 */
export interface ISelectedStoreProps {
    className?: string;
    shouldHideStockStatus?: boolean;
    isCurrentLocation?: boolean;
    isInStock?: boolean;
    selectedStore?: IFullOrgUnitAvailability;
    resources: ISelectedStoreResources;
    stockStatusLabel?: string;
    preferredStoreLocationId: string | null;
    isPreferredStoreEnabled?: boolean;
    isLocationDisabled?: boolean;
    telemetryContent?: ITelemetryContent;
    storePickUpOptionList?: DeliveryOption[];
    productPickUpOptionList?: DeliveryOption[];
    preferredDistanceUnit: string;
    filteredPickupMode?: string;
    handlers: {
        onSelected(location: OrgUnitLocation, deliveryMode?: string): void;
        onSetAsPreferredStore(locationId: string | undefined): void;
        onRemovePreferredStore(): void;
        closeStoreDetailsModal(): Promise<void>;
    };
}

/**
 * Store Selector resources.
 */
export interface ISelectedStoreViewProps {
    isCurrentLocation?: boolean;
    canSelectLocation?: boolean;
    selectedStore?: IFullOrgUnitAvailability;
    deliveryOptions?: DeliveryOption[];
    isPreferredStore?: boolean;
    resources: ISelectedStoreResources;

    storeDetailsMain: INodeProps;
    storeHeader: INodeProps;
    storeName: React.ReactNode;
    storeDistance: React.ReactNode;
    storeDetailsAvailabilityContainer: INodeProps;
    productAvailability: React.ReactNode;
    stockStatus: React.ReactNode;
    currentLocation: React.ReactNode;
    pickUpDeliveryOptionButton: React.ReactNode;
    pickupModes?: React.ReactNode;
    preferredStoreButton?: React.ReactNode;
    removePreferredStoreButton?: React.ReactNode;
    storeDetailsCloseButton?: React.ReactNode;
    handlers: {
        onSelected(location: OrgUnitLocation, deliveryMode?: string): void;
        onSetAsPreferredStore(locationId: string | undefined): void;
        onRemovePreferredStore(): void;
        closeStoreDetailsModal(): Promise<void>;
    };
}

/**
 * On Store locator click functionality.
 * @param props -Store locator location items.
 * @returns Store locator selected location line item action.
 */
const onPickupInStoreClickHandler = (props: ISelectedStoreProps) => () => {
    if (props.selectedStore?.OrgUnitAvailability?.OrgUnitLocation) {
        props.handlers.onSelected(props.selectedStore.OrgUnitAvailability.OrgUnitLocation);
    }
};

/**
 * Set Preferred store functionality.
 * @param props -Store locator location items.
 * @returns Store locator selected location line item action.
 */
const onSetPreferredStoreClickHandler = (props: ISelectedStoreProps) => () => {
    props.handlers.onSetAsPreferredStore(props.selectedStore?.OrgUnitAvailability?.OrgUnitLocation?.OrgUnitNumber);
};

/**
 * On Change functionality.
 * @param props -Store locator location items.
 * @returns Store locator selected location line item action.
 */
const onChangeHandler = (props: ISelectedStoreProps) => async (deliveryCode: string): Promise<void> => {
    if (props.selectedStore?.OrgUnitAvailability?.OrgUnitLocation) {
        props.handlers.onSelected(props.selectedStore.OrgUnitAvailability.OrgUnitLocation, deliveryCode);
    }
    return Promise.resolve();
};

/**
 * Remove preferred store functionality.
 * @param props -Store locator location items.
 * @returns Store locator selected location line item action.
 */
const removePreferredStoreHandler = (props: ISelectedStoreProps) => () => {
    props.handlers.onRemovePreferredStore();
};

/**
 * On Store locator click functionality.
 * @param props -Store locator location items.
 * @param pickupMode -Store locator location items.
 * @returns Store locator selected location line item action.
 */
const onPickupModeSelectHandler = (props: ISelectedStoreProps, pickupMode?: string) => () => {
    if (props.selectedStore?.OrgUnitAvailability?.OrgUnitLocation) {
        props.handlers.onSelected(props.selectedStore.OrgUnitAvailability.OrgUnitLocation, pickupMode);
    }
};

/**
 * On store details close functionality.
 * @param props -Store locator location items.
 * @returns Void promise.
 */
const storeDetailsModalHandler = (props: ISelectedStoreProps) => async (): Promise<void> => {
    await props.handlers.closeStoreDetailsModal();
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
    props: ISelectedStoreProps,
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
 * Renders selected store details.
 * @param props - Store location details props.
 * @returns - Selected Store view props.
 */
const selectedStoreDetails = (props: ISelectedStoreProps): ISelectedStoreViewProps => {
    const {
        className,
        shouldHideStockStatus,
        isCurrentLocation,
        isInStock,
        selectedStore,
        resources,
        stockStatusLabel,
        preferredStoreLocationId,
        isPreferredStoreEnabled,
        isLocationDisabled,
        telemetryContent,
        storePickUpOptionList,
        productPickUpOptionList,
        preferredDistanceUnit
    } = props;

    const parentClassName = className ? className : 'ms-store-select__map__selected_store';
    const storeName: string | undefined = selectedStore?.OrgUnitAvailability?.OrgUnitLocation?.OrgUnitName;
    const location = selectedStore?.OrgUnitAvailability?.OrgUnitLocation;
    const distanceAsString: string | undefined = buildDistanceString(location?.Distance, preferredDistanceUnit, isLocationDisabled);

    const resourceLabel = isInStock ? resources.inStock : resources.outOfStock;
    const stockLabel = stockStatusLabel ? stockStatusLabel : undefined;
    // eslint-disable-next-line no-nested-ternary -- Required to check stock Label
    const stockText: string | undefined = shouldHideStockStatus ? undefined : stockLabel ? stockLabel : resourceLabel;
    const canSelectLocation = !shouldHideStockStatus && isInStock;
    const pickupInStoreButtonText: string = resources.selectLocation;
    const pickupInStoreAriaLabel: string = resources.selectLocationAriaLabelFormat.replace('{StoreName}', storeName ?? '');
    const isPreferredStore = preferredStoreLocationId === location?.OrgUnitNumber;
    const payLoad: IPayLoad = getPayloadObject('click', telemetryContent!, TelemetryConstant.PickupInStore);
    const attributes = getTelemetryAttributes(telemetryContent!, payLoad);
    payLoad.contentAction.etext = TelemetryConstant.PreferredStore;
    const preferredStoreAttributes = getTelemetryAttributes(telemetryContent!, payLoad);
    payLoad.contentAction.etext = TelemetryConstant.RemovePreferredStore;
    const removePreferredStoreAttributes = getTelemetryAttributes(telemetryContent!, payLoad);
    const deliveryOptionList = matchDeliveryOptions(productPickUpOptionList, storePickUpOptionList);

    const viewProps: ISelectedStoreViewProps = {
        selectedStore,
        isCurrentLocation,
        canSelectLocation,
        isPreferredStore,
        resources,
        deliveryOptions: deliveryOptionList,
        storeDetailsMain: {
            tag: 'div' as NodeTag,
            className: `${parentClassName}__main`
        },
        storeHeader: {
            tag: 'div' as NodeTag,
            className: `${parentClassName}__header`
        },
        storeName: <span className={`${parentClassName}__header_store_name`}>{storeName}</span>,
        storeDetailsCloseButton: <Button className={`${parentClassName}__header_close_button`} onClick={storeDetailsModalHandler(props)} />,
        // eslint-disable-next-line prettier/prettier
        storeDistance: distanceAsString ? <span className={`${parentClassName}__header_store-distance`}>
        {' '}
        {distanceAsString}</span> : '',
        storeDetailsAvailabilityContainer: {
            tag: 'div' as NodeTag,
            className: `${parentClassName}__container`
        },
        productAvailability: !shouldHideStockStatus && (
            <p className={`${parentClassName}__container_product-availability`}>{resources.availabilityInfoHeader}</p>
        ),
        stockStatus: stockText && <p className={`${parentClassName}__container_stock-status`}>{stockText}</p>,
        currentLocation: <p className={`${parentClassName}__container_current-location`}>{resources.currentLocation}</p>,
        pickUpDeliveryOptionButton: (
            <button
                className='ms-store-select__location-line-select-store'
                aria-label={pickupInStoreAriaLabel}
                onClick={onPickupInStoreClickHandler(props)}
                {...attributes}
            >
                {pickupInStoreButtonText}
            </button>
        ),
        pickupModes: renderPickupModes(props, deliveryOptionList, pickupInStoreButtonText, pickupInStoreAriaLabel),
        preferredStoreButton: isPreferredStoreEnabled ? (
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
        ) : null,
        removePreferredStoreButton: isPreferredStoreEnabled ? (
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
        ) : null,
        handlers: {
            onSelected: props.handlers.onSelected,
            onSetAsPreferredStore: props.handlers.onSetAsPreferredStore,
            onRemovePreferredStore: props.handlers.onRemovePreferredStore,
            closeStoreDetailsModal: props.handlers.closeStoreDetailsModal
        }
    };

    return viewProps;
};

/**
 * Store location line item component.
 */
export const SelectedStoreComponent = selectedStoreDetails as (props: ISelectedStoreProps) => ISelectedStoreViewProps;
