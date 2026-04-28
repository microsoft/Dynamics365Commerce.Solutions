/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * ICustomAccountManagementAddress contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface ICustomAccountManagementAddressConfig extends Msdyn365.IModuleConfig {
    heading?: IHeadingData;
    primaryAddressSectionHeading?: IPrimaryAddressSectionHeadingData;
    otherAddressSectionHeading?: IOtherAddressSectionHeadingData;
    addAddressHeading?: IAddAddressHeadingData;
    editAddressHeading?: IEditAddressHeadingData;
    showAddressType?: boolean;
    autoSuggestionEnabled?: boolean;
    autoSuggestOptions?: IAutoSuggestOptionsData;
    className?: string;
    clientRender?: boolean;
}

export interface ICustomAccountManagementAddressResources {
    accountProcessingPendingInfoMessage: string;
    accountProcessingPendingInfoMessageCanAddAddress: string;
    addressChangeCheckboxAriaLabel: string;
    addressAddButtonText: string;
    addressAddButtonAriaLabel: string;
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
    addressIsPrimaryLabel: string;
    removeAddressNotification: string;
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

export const enum PrimaryAddressSectionHeadingTag {
    h1 = 'h1',
    h2 = 'h2',
    h3 = 'h3',
    h4 = 'h4',
    h5 = 'h5',
    h6 = 'h6'
}

export interface IPrimaryAddressSectionHeadingData {
    text: string;
    tag?: PrimaryAddressSectionHeadingTag;
}

export const enum OtherAddressSectionHeadingTag {
    h1 = 'h1',
    h2 = 'h2',
    h3 = 'h3',
    h4 = 'h4',
    h5 = 'h5',
    h6 = 'h6'
}

export interface IOtherAddressSectionHeadingData {
    text: string;
    tag?: OtherAddressSectionHeadingTag;
}

export const enum AddAddressHeadingTag {
    h1 = 'h1',
    h2 = 'h2',
    h3 = 'h3',
    h4 = 'h4',
    h5 = 'h5',
    h6 = 'h6'
}

export interface IAddAddressHeadingData {
    text: string;
    tag?: AddAddressHeadingTag;
}

export const enum EditAddressHeadingTag {
    h1 = 'h1',
    h2 = 'h2',
    h3 = 'h3',
    h4 = 'h4',
    h5 = 'h5',
    h6 = 'h6'
}

export interface IEditAddressHeadingData {
    text: string;
    tag?: EditAddressHeadingTag;
}

export interface IAutoSuggestOptionsData {
    maxResults?: number;
}

export interface ICustomAccountManagementAddressProps<T> extends Msdyn365.IModule<T> {
    resources: ICustomAccountManagementAddressResources;
    config: ICustomAccountManagementAddressConfig;
}
