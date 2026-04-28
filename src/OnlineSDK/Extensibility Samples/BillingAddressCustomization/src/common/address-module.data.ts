/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { Address } from '@msdyn365-commerce/retail-proxy';
import { Heading } from '@msdyn365-commerce-modules/data-types';

export enum AddressOperation {
    Add = 'Add',
    Show = 'Show',
    Update = 'Update',
    List = 'List',
    BillingList = 'BillingList'
}

export enum AddressType {
    Shipping = 'Shipping',
    Billing = 'Billing',
    Company = 'Company'
}

export interface IDropdownDisplayData {
    [index: string]: { key?: string; value?: string }[];
}

export interface IAddressResponse {
    errorTitle?: string;
    errorMessage?: string;
    customerAddresses?: Address[];
    address?: Address;
}

export interface IAddressConfig {
    heading?: Heading;
    primaryAddressSectionHeading?: Heading;
    otherAddressSectionHeading?: Heading;
    addAddressHeading?: Heading;
    editAddressHeading?: Heading;
}

export interface IAddressResource {
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
    addressNameLabelOptional?: string;
    addressNameRequiredErrorMessage: string;
    addressPhoneLabel: string;
    addressPhoneLabelOptional?: string;
    addressPhoneRequiredErrorMessage: string;
    addressZipCodeLabel: string;
    addressZipCodeLabelOptional?: string;
    addressZipCodeRequiredErrorMessage: string;
    addressCityLabel: string;
    addressCityLabelOptional?: string;
    addressCityRequiredErrorMessage: string;
    addressCountyLabel: string;
    addressCountyLabelOptional?: string;
    addressCountyRequiredErrorMessage: string;
    addressStateLabel: string;
    addressStateLabelOptional?: string;
    addressStateRequiredErrorMessage: string;
    addressThreeLetterISORegionNameLabel: string;
    addressThreeLetterISORegionNameLabelOptional?: string;
    addressThreeLetterISORegionNameRequiredErrorMessage: string;
    addressStreetRequiredErrorMessage: string;
    addressDistrictLabel: string;
    addressDistrictLabelOptional?: string;
    addressDistrictNameLabel: string;
    addressDistrictNameLabelOptional?: string;
    addressDistrictRequiredErrorMessage: string;
    addressStreetNumberLabel: string;
    addressStreetNumberLabelOptional?: string;
    addressStreetNumberRequiredErrorMessage: string;
    addressBuildingComplimentLabel: string;
    addressBuildingComplimentLabelOptional?: string;
    addressBuildingComplimentRequiredErrorMessage: string;
    addressPostboxLabel: string;
    addressPostboxLabelOptional?: string;
    addressPostboxRequiredErrorMessage: string;
    addressHouseRULabel: string;
    addressHouseRULabelOptional?: string;
    addressHouseRURequiredErrorMessage: string;
    addressFlatRULabel: string;
    addressFlatRULabelOptional?: string;
    addressFlatRURequiredErrorMessage: string;
    addressCountryOKSMCodeRULabel: string;
    addressCountryOKSMCodeRURequiredErrorMessage: string;
    addressErrorMessageTitle: string;
    addressGenericErrorMessage: string;
    addressEmptyListAddressMessage: string;
    removeAddressNotification?: string;
}
