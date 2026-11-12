/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import {
    CacheType,
    createObservableDataAction,
    getCatalogId,
    IAction,
    IActionContext,
    IActionInput,
    IAny,
    ICreateActionContext,
    IGeneric,
    IRequestContext
} from '@msdyn365-commerce/core';
import { SimpleProduct } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import { ProductsDataActions } from '@msdyn365-commerce/retail-proxy';

import { QueryResultSettingsProxy, getSelectedProductIdFromActionInput } from '@msdyn365-commerce-modules/retail-actions';

/**
 * Product Input.
 */
export class ProductInput implements IActionInput {
    public productId: number;

    public channelId?: number;

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
    public getCacheKey = (): string => 'getProductDetail';

    /**
     * Property to get cache object type.
     * @returns Cache object type.
     */
    public getCacheObjectType = (): string => 'FeatureProduct';

    /**
     * Property to get data cache type.
     * @returns Data cache type.
     */
    public dataCacheType = (): CacheType => 'none';
}

/**
 * Creates the input required to make the retail api call.
 * @param inputData
 */
export const createSimpleProductsInput = (inputData: ICreateActionContext<IGeneric<IAny>>): IActionInput => {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const productId: any = getSelectedProductIdFromActionInput(inputData);
    return new ProductInput(productId, undefined, inputData.requestContext);
};

/**
 * Calls the Retail API and returns the product based on the passed ProductInput.
 * @param inputs
 * @param ctx
 */
export async function getSimpleProductsAction(inputs: ProductInput, ctx: IActionContext): Promise<SimpleProduct> {
    const result = await ProductsDataActions.getByIdsAsync(
        {
            callerContext: ctx,
            queryResultSettings: QueryResultSettingsProxy.getPagingFromInputDataOrDefaultValue(ctx)
        },
        inputs.channelId || 0,
        [inputs.productId],
        null,
        inputs.catalogId ?? 0
    );
    return result[0];
}

export default createObservableDataAction({
    id: '@msdyn365-commerce-modules/retail-actions/get-simple-products',
    action: <IAction<SimpleProduct>>getSimpleProductsAction,
    input: createSimpleProductsInput
});
