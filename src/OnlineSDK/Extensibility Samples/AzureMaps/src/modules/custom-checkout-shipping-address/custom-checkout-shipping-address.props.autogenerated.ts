/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * ICustomCheckoutShippingAddress contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface ICustomCheckoutShippingAddressConfig extends Msdyn365.IModuleConfig {
    heading?: IHeadingData;
    showAddressType?: boolean;
    autoSuggestionEnabled?: boolean;
    imageSettings?: Msdyn365.IImageSettings;
    className?: string;
    clientRender?: boolean;
    multipleAddressShippingEnabled?: boolean;
    forceReselectShippingAddressOnDeletedSavedAddressEnabled?: boolean;
    autoSuggestOptions?: IAutoSuggestOptionsData;
}

export interface ICustomCheckoutShippingAddressResources {
    addressChangeCheckboxAriaLabel: string;
    addressAddButtonText: string;
    addressAddButtonAriaLabel: string;
    addressAddNewButtonText: string;
    addressAddNewButtonAriaLabel: string;
    addressPrimaryButtonText: string;
    addressPrimaryButtonAriaLabel: string;
    addressEditButtonText: string;
    addressEditButtonAriaLabel: string;
    addressRemoveButtonText: string;
    addressRemoveButtonAriaLabel: string;
    addressSaveButtonText: string;
    addressSaveButtonAriaLabel: string;
    addressCancelButtonText: string;
    addressCancelButtonAriaLabel: string;
    addressStateDefaultSelectionText: string;
    addressNameLabel: string;
    addressNameRequiredErrorMessage: string;
    addressPhoneLabel: string;
    addressPhoneRequiredErrorMessage: string;
    addressPhoneFormatErrorMessage: string;
    addressPhoneMaxLengthErrorMessage: string;
    addressZipCodeLabel: string;
    addressAddressTypeValueLabel: string;
    addressIsPrimaryLabel: string;
    addressZipCodeRequiredErrorMessage: string;
    addressCityLabel: string;
    addressCityRequiredErrorMessage: string;
    addressCountyLabel: string;
    addressCountyRequiredErrorMessage: string;
    addressStateLabel: string;
    addressStateRequiredErrorMessage: string;
    addressThreeLetterISORegionNameLabel: string;
    addressThreeLetterISORegionNameRequiredErrorMessage: string;
    addressStreetLabel: string;
    addressStreetRequiredErrorMessage: string;
    addressDistrictLabel: string;
    addressDistrictNameLabel: string;
    addressDistrictRequiredErrorMessage: string;
    addressStreetNumberLabel: string;
    addressStreetNumberRequiredErrorMessage: string;
    addressBuildingComplimentLabel: string;
    addressBuildingComplimentRequiredErrorMessage: string;
    addressPostboxLabel: string;
    addressPostboxRequiredErrorMessage: string;
    addressHouseRULabel: string;
    addressHouseRURequiredErrorMessage: string;
    addressFlatRULabel: string;
    addressFlatRURequiredErrorMessage: string;
    addressCountryOKSMCodeRULabel: string;
    addressCountryOKSMCodeRURequiredErrorMessage: string;
    addressErrorMessageTitle: string;
    addressGenericErrorMessage: string;
    addressEmptyListAddressMessage: string;
    itemsText: string;
    singleItemText: string;
    headingImages: string;
    productQuantityInfo: string;
    errorMessageTitle: string;
    addressSelectAllRowsText: string;
    addressProductNumberText: string;
    addressProductText: string;
    addressProductUnitPriceText: string;
    addressProductQuantityText: string;
    addressProductAddressText: string;
    addressShipMultipleText: string;
    addressShipMultipleAriaLabel: string;
    saveAndContinueBtnLabel: string;
    addressShipSingleText: string;
    addressShipSingleAriaLabel: string;
    clearSelectionButtonText: string;
    clearSelectionButtonAriaLabel: string;
    chooseAddressForSelectedItemsText: string;
    chooseAddressForSelectedItemsAriaLabel: string;
    noAddressSelectedAriaLabel: string;
    unavailableProductErrorMessage: string;
    outOfStockProductErrorMessage: string;
    headingAfterMultiSelectAddressSelect: string;
}

export const enum HeadingTag {
    h1 = 'h1',
    h2 = 'h2',
    h3 = 'h3',
    h4 = 'h4',
    h5 = 'h5',
    h6 = 'h6'
}

export interface IHeadingData {
    text: string;
    tag?: HeadingTag;
}

export interface IAutoSuggestOptionsData {
    maxResults?: number;
}

export interface ICustomCheckoutShippingAddressProps<T> extends Msdyn365.IModule<T> {
    resources: ICustomCheckoutShippingAddressResources;
    config: ICustomCheckoutShippingAddressConfig;
}
