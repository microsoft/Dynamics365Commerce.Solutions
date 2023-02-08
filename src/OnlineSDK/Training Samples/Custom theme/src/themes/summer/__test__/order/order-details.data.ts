/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import { AsyncResult,
    ChannelDeliveryOptionConfiguration,
    ChannelIdentity, FeatureState, LoyaltyCard,
    OrgUnitLocation, ProductDeliveryOptions, SalesOrder,
    SimpleProduct, TenderType } from '@msdyn365-commerce/retail-proxy';

/**
 * Mocked IOrderDetailsData.
 */
export interface IOrderDetailsData {
    orderHydration: AsyncResult<{
        salesOrder: SalesOrder;
        products: SimpleProduct[];
        deliveryOptions?: ProductDeliveryOptions[];
    }>;
    tenderTypes: AsyncResult<TenderType[]>;
    loyaltyCard: AsyncResult<LoyaltyCard>;
    channels: AsyncResult<{
        channelIdentities: ChannelIdentity[];
    }>;
    orgUnitLocations: AsyncResult<{
        orgUnitLocations: OrgUnitLocation[];
    }>;
    featureState: AsyncResult<FeatureState[]>;
    channelDeliveryOptionConfig: AsyncResult<ChannelDeliveryOptionConfiguration>;
}
