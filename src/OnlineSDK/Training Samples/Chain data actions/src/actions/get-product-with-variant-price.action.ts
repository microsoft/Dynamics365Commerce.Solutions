/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as Msdyn365 from '@msdyn365-commerce/core';
import { ProductPrice, SimpleProduct } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import { getSelectedProductIdFromActionInput } from '@msdyn365-commerce-modules/retail-actions';

import {
    CacheType,
    getCatalogId,
    IAction,
    IActionContext,
    IAny,
    ICreateActionContext,
    IGeneric,
    IRequestContext
} from '@msdyn365-commerce/core';
import getSimpleProductsAction, { ProductInput } from './get-product.action';
import getVariantPrice from './get-variant-price.action';

// eslint-disable-next-line @typescript-eslint/naming-convention
export interface SimpleProductWithPrice extends SimpleProduct {
    variantPrice?: ProductPrice | null;
}
/**
 * ChainDataAction Input Action
 */
export class ChainDataActionInput implements Msdyn365.IActionInput {
    public productId: number;

    public channelId: number;

    public catalogId?: number;

    public constructor(productId: number | string, channelId?: number, requestContext?: IRequestContext, catalogId?: number) {
        this.productId = +productId;
        this.channelId = channelId ?? 0;
        this.catalogId = catalogId;

        if (requestContext && catalogId === undefined) {
            this.catalogId = getCatalogId(requestContext);
        }
    }

    /**
     * Property to get cache key.
     * @returns Cache object name.
     */
    public getCacheKey = (): string => 'chainDataActionInput';

    /**
     * Property to get cache object type.
     * @returns Cache object type.
     */
    public getCacheObjectType = (): string => 'ChainProduct';

    /**
     * Property to get data cache type.
     * @returns Data cache type.
     */
    public dataCacheType = (): CacheType => 'none';
}

export const createChainDataActionInput = (inputData: ICreateActionContext<IGeneric<IAny>>): Msdyn365.IActionInput => {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const productId: any = getSelectedProductIdFromActionInput(inputData);
    return new ProductInput(productId, undefined, inputData.requestContext);
};
export async function getProductWithVariantPriceAction(
    input: ProductInput,
    ctx: IActionContext
): Promise<SimpleProductWithPrice | undefined> {
    // First we get the product
    let product: SimpleProductWithPrice;
    const pro = new ProductInput(input.productId, undefined, ctx.requestContext, 0);
    try {
        product = await getSimpleProductsAction(pro, ctx);
        // If we successfully get the product, then we try to get its availability information.
        if (product) {
            // Get the price information
            product.variantPrice = await getVariantPrice(input, ctx);
            return product;
        } else {
            return <SimpleProductWithPrice>{};
        }
        // eslint-disable-next-line no-empty
    } catch (err) {}
    return undefined;
}

export default Msdyn365.createObservableDataAction({
    id: 'ChainDataAction',
    action: getProductWithVariantPriceAction as IAction<SimpleProductWithPrice | null>,
    input: createChainDataActionInput
});
