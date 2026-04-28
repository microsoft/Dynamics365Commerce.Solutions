/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * ICustomBusinessAccountAddress contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface ICustomBusinessAccountAddressConfig extends Msdyn365.IModuleConfig {
    className?: string;
    clientRender?: boolean;
}

export interface ICustomBusinessAccountAddressResources {
    addressAddButtonText: string;
    addressChangeCheckboxAriaLabel: string;
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
    addressNameLabelOptional: string;
    addressNameRequiredErrorMessage: string;
    addressPhoneLabel: string;
    addressPhoneRequiredErrorMessage: string;
    addressPhoneFormatErrorMessage: string;
    addressZipCodeLabel: string;
    addressZipCodeLabelOptional: string;
    addressZipCodeRequiredErrorMessage: string;
    addressAddressTypeValueLabel: string;
    addressAddressTypeValueLabelOptional: string;
    addressCityLabel: string;
    addressCityLabelOptional: string;
    addressCityRequiredErrorMessage: string;
    addressCountyLabel: string;
    addressCountyLabelOptional: string;
    addressCountyRequiredErrorMessage: string;
    addressStateLabel: string;
    addressStateLabelOptional: string;
    addressStateRequiredErrorMessage: string;
    addressThreeLetterISORegionNameLabel: string;
    addressThreeLetterISORegionNameLabelOptional: string;
    addressThreeLetterISORegionNameRequiredErrorMessage: string;
    addressStreetLabel: string;
    addressStreetLabelOptional: string;
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
}

export interface ICustomBusinessAccountAddressProps<T> extends Msdyn365.IModule<T> {
    resources: ICustomBusinessAccountAddressResources;
    config: ICustomBusinessAccountAddressConfig;
}
