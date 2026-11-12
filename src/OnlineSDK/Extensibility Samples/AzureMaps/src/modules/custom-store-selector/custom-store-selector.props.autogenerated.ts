/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * ICustomStoreSelector containerModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';
import * as React from 'react';

export const enum mode {
    findStores = 'findStores',
    pickupInStore = 'pickupInStore'
}

export const enum searchRadiusUnit {
    miles = 'miles',
    kilometers = 'kilometers'
}

export const enum style {
    inline = 'inline',
    dialog = 'dialog'
}

export interface ICustomStoreSelectorConfig extends Msdyn365.IModuleConfig {
    heading?: string;
    termsOfServiceLink?: ITermsOfServiceLinkData;
    mode?: mode;
    searchRadiusUnit?: searchRadiusUnit;
    lookupRadius: number;
    style?: style;
    setAsPreferredStore?: boolean;
    enablePickupFilterToShowStore?: boolean;
    showAllStores?: boolean;
    autoSuggestionEnabled?: boolean;
    autoSuggestOptions?: IAutoSuggestOptionsData;
    className?: string;
    clientRender?: boolean;
}

export interface ICustomStoreSelectorResources {
    searchPlaceholderText: string;
    searchButtonAriaLabel: string;
    searchInputAriaLabel: string;
    outOfStockText: string;
    inStockText: string;
    selectedStoreLocationText: string;
    emptyLocationsText: string;
    selectStoreText: string;
    selectStoreAriaFormatText: string;
    setAsPreferredStoreText: string;
    setAsPreferredStoreTextAriaLabel: string;
    preferredStoreText: string;
    preferredStoreAriaLabel: string;
    timeText: string;
    captionText: string;
    milesShortText: string;
    kilometersShortText: string;
    contactText: string;
    availabilityText: string;
    productDimensionTypeColor: string;
    productDimensionTypeConfiguration: string;
    productDimensionTypeSize: string;
    productDimensionTypeStyle: string;
    storeHoursClosedText: string;
    storeHoursMondayText: string;
    storeHoursTuesdayText: string;
    storeHoursWednesdayText: string;
    storeHoursThursdayText: string;
    storeHoursFridayText: string;
    storeHoursSaturdayText: string;
    storeHoursSundayText: string;
    storeHoursMondayFullText: string;
    storeHoursTuesdayFullText: string;
    storeHoursWednesdayFullText: string;
    storeHoursThursdayFullText: string;
    storeHoursFridayFullText: string;
    storeHoursSaturdayFullText: string;
    storeHoursSundayFullText: string;
    storePhoneAriaLabel: string;
    storeAddressAriaLabel: string;
    storeCountMessage: string;
    storeAllCountMessage: string;
    storeCountMessageInKm: string;
    storeSelectorHeaderText: string;
    storeLocatorHeaderText: string;
    seeAllStoresText: string;
    viewMapText: string;
    viewListText: string;
    pickupFilterByHeading: string;
    pickupFilterMenuHeading: string;
    pickupDeliveryOptionErrorMessage: string;
}

export interface ITermsOfServiceLinkData {
    linkText?: string;
    linkUrl: Msdyn365.ILinkData;
    ariaLabel?: string;
    openInNewTab?: boolean;
}

export interface IAutoSuggestOptionsData {
    maxResults?: number;
}

export interface ICustomStoreSelectorProps<T> extends Msdyn365.IModule<T> {
    resources: ICustomStoreSelectorResources;
    config: ICustomStoreSelectorConfig;
    slots: {
        maps: React.ReactNode[];
    };
}
