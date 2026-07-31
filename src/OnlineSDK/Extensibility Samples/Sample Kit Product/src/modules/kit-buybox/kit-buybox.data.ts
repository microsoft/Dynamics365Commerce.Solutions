/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import { RatingsSummary } from '@msdyn365-commerce/commerce-entities';
import { ICartState } from '@msdyn365-commerce/global-state';
import {
    AsyncResult,
    ChannelDeliveryOptionConfiguration,
    CommerceList,
    Customer,
    FeatureState,
    ProductDeliveryOptions,
    ProductPrice,
    SimpleProduct
} from '@msdyn365-commerce/retail-proxy';
import { IStoreSelectorStateManager } from '@msdyn365-commerce-modules/bopis-utilities';
import { IDimensionForSelectedVariant, IProductInventoryInformation, OrderTemplate } from '@msdyn365-commerce-modules/retail-actions';

/**
 * Interface for BuyBox data.
 */
export interface IKitBuyboxData {
    deliveryOptions: AsyncResult<ProductDeliveryOptions | undefined>;
    product: AsyncResult<SimpleProduct>;
    productDimensions: AsyncResult<IDimensionForSelectedVariant[]>;
    productPrice: AsyncResult<ProductPrice>;
    ratingsSummary: AsyncResult<RatingsSummary>;
    storeSelectorStateManager: AsyncResult<IStoreSelectorStateManager>;
    wishlists: AsyncResult<CommerceList[]>;
    orderTemplates: AsyncResult<OrderTemplate[]>;
    cart: AsyncResult<ICartState>;
    productAvailableQuantity: AsyncResult<IProductInventoryInformation[]>;
    customerInformation: AsyncResult<Customer>;
    featureState: AsyncResult<FeatureState[]>;
    channelDeliveryOptionConfig: AsyncResult<ChannelDeliveryOptionConfiguration>;
}
