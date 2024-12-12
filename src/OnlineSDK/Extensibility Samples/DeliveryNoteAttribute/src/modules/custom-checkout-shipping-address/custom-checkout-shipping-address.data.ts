/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import { ICartState, ICheckoutState } from '@msdyn365-commerce/global-state';
import {
    Address,
    AddressPurpose,
    AsyncResult,
    ChannelDeliveryOptionConfiguration,
    CountryRegionInfo,
    FeatureState,
    StateProvinceInfo
} from '@msdyn365-commerce/retail-proxy';
import { IStoreSelectorStateManager } from '@msdyn365-commerce-modules/bopis-utilities';

/**
 * Interface for ICustomCheckoutShippingAddressData.
 */
export interface ICustomCheckoutShippingAddressData {
    storeSelectorStateManager: AsyncResult<IStoreSelectorStateManager>;
    checkout: AsyncResult<ICheckoutState>;
    address: AsyncResult<Address[]>;
    countryRegions: AsyncResult<CountryRegionInfo[]>;
    addressPurposes: AsyncResult<AddressPurpose[]>;
    countryStates: AsyncResult<StateProvinceInfo[]>;
    channelDeliveryOptionConfig: AsyncResult<ChannelDeliveryOptionConfiguration>;
    featureState: AsyncResult<FeatureState[]>;
    cart: AsyncResult<ICartState>;
}
