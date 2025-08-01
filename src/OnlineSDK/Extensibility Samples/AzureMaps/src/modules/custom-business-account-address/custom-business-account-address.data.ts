/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import { AddressPurpose, AsyncResult, CountryRegionInfo, StateProvinceInfo } from '@msdyn365-commerce/retail-proxy';

/**
 * Interface for ICustomBusinessAccountAddressData.
 */
export interface ICustomBusinessAccountAddressData {
    countryRegions: AsyncResult<CountryRegionInfo[]>;
    countryStates: AsyncResult<StateProvinceInfo[]>;
    addressPurposes: AsyncResult<AddressPurpose[]>;
}
