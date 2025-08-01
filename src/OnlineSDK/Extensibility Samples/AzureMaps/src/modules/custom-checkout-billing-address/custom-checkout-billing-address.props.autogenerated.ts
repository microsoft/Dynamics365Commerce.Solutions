/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * ICustomCheckoutBillingAddress contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface ICustomCheckoutBillingAddressConfig extends Msdyn365.IModuleConfig {
    heading?: IHeadingData;
    showAddressType?: boolean;
    paymenTenderType?: string;
    autoSuggestionEnabled?: boolean;
    autoSuggestOptions?: IAutoSuggestOptionsData;
    className?: string;
    clientRender?: boolean;
}

export interface ICustomCheckoutBillingAddressResources {
    addressChangeCheckboxAriaLabel: string;
    addressBillingAddressHeading: string;
    addressSameAsShippingAddressAriaLabel: string;
    addressSameAsShippingAddressText: string;
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
    errorMessageTitle: string;
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

export interface ICustomCheckoutBillingAddressProps<T> extends Msdyn365.IModule<T> {
    resources: ICustomCheckoutBillingAddressResources;
    config: ICustomCheckoutBillingAddressConfig;
}
