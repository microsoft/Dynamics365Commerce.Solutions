/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { ArrayExtensions } from '@msdyn365-commerce-modules/retail-actions';
import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { ISelectedStoreViewProps, IStoreSelectorViewProps } from './index';

/**
 * Renders the pickup Delivery options.
 * @param props -- Selected store view props.
 * @returns -- HTML.
 */
const renderDeliveryOptions: React.FC<ISelectedStoreViewProps> = props => {
    const { canSelectLocation, deliveryOptions, pickUpDeliveryOptionButton, pickupModes } = props;

    return (
        <>
            {/* eslint-disable-next-line no-nested-ternary -- Need this.*/}
            {canSelectLocation ? (ArrayExtensions.hasElements(deliveryOptions) ? pickupModes : pickUpDeliveryOptionButton) : false}
        </>
    );
};

/**
 * Render the selected store.
 * @param props -- Selected store view props.
 * @returns -- HTML.
 */
const renderSelectedStore: React.FC<ISelectedStoreViewProps> = props => {
    const {
        storeDetailsMain,
        storeHeader,
        storeName,
        storeDetailsCloseButton,
        storeDistance,
        storeDetailsAvailabilityContainer,
        productAvailability,
        stockStatus,
        isCurrentLocation,
        currentLocation,
        isPreferredStore,
        preferredStoreButton,
        removePreferredStoreButton
    } = props;

    return (
        <Node {...storeDetailsMain}>
            <Node {...storeHeader}>
                {storeDetailsCloseButton}
                {storeName}
                {storeDistance}
            </Node>
            <Node {...storeDetailsAvailabilityContainer}>
                {productAvailability}
                {stockStatus}
            </Node>
            {isCurrentLocation ? currentLocation : renderDeliveryOptions(props)}
            {isPreferredStore ? preferredStoreButton : removePreferredStoreButton}
        </Node>
    );
};

/**
 * Render the body of the popup.
 * @param props -- Store selector view props.
 * @returns -- Returns the html.
 */
const renderBody: React.FC<IStoreSelectorViewProps> = props => {
    const {
        BodyContainerProps,
        locationsMessage,
        search,
        state,
        spinner,
        locationsList,
        noLocationsMessage,
        maps,
        bodyWrapperProps,
        storeResultContainerProps,
        mapContainerProps,
        isMobileView,
        storeDetailsModal,
        selectedLocation
    } = props;

    if (isMobileView) {
        return (
            <Node {...BodyContainerProps}>
                {locationsMessage}
                {search}
                <Node {...bodyWrapperProps!}>
                    <Node {...storeResultContainerProps!}>
                        {state.isSearchInProgress ? spinner : <>{locationsList ? locationsList : noLocationsMessage}</>}
                        {maps}
                        <Node {...storeDetailsModal!}>{selectedLocation && renderSelectedStore(selectedLocation)}</Node>
                    </Node>
                </Node>
            </Node>
        );
    }

    return (
        <Node {...BodyContainerProps}>
            <Node {...bodyWrapperProps!}>
                <Node {...storeResultContainerProps!}>
                    {locationsMessage}
                    {search}
                    {state.isSearchInProgress ? spinner : <>{locationsList ? locationsList : noLocationsMessage}</>}
                </Node>
                <Node {...mapContainerProps!}>{maps}</Node>
            </Node>
        </Node>
    );
};

/**
 * Store selector view props.
 * @param props - Store selector view props.
 * @returns - HTML.
 */
const StoreSelectorView: React.FC<IStoreSelectorViewProps> = props => {
    const {
        FooterContainerProps,
        HeaderContainerProps,
        HeaderProps,
        ModuleProps,

        terms
    } = props;

    return (
        <Module {...ModuleProps}>
            <Node {...HeaderContainerProps}>{HeaderProps}</Node>
            {renderBody(props)}
            <Node {...FooterContainerProps}>{terms}</Node>
        </Module>
    );
};

export default StoreSelectorView;
