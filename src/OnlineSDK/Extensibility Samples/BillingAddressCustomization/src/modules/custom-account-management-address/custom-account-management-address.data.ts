/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import { Address, AddressPurpose, AsyncResult, CountryRegionInfo } from '@msdyn365-commerce/retail-proxy';
import { IStoreSelectorStateManager } from '@msdyn365-commerce-modules/bopis-utilities';

/**
 * Interface for ICustomAccountManagementAddressData.
 */
export interface ICustomAccountManagementAddressData {
    storeSelectorStateManager: AsyncResult<IStoreSelectorStateManager>;
    address: AsyncResult<Address[]>;
    countryRegions: AsyncResult<CountryRegionInfo[]>;
    addressPurposes: AsyncResult<AddressPurpose[]>;
}
