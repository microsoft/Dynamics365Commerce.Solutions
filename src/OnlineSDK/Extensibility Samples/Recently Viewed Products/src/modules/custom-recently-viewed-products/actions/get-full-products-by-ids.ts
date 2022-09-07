/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { generateProductImageUrl } from '@msdyn365-commerce-modules/retail-actions';
import {
    CacheType,
    createObservableDataAction,
    IAction,
    IActionContext,
    IActionInput,
    IAny,
    ICreateActionContext,
    IGeneric
} from '@msdyn365-commerce/core';
import { AsyncResult, ProductSearchCriteria, ProductSearchResult } from '@msdyn365-commerce/retail-proxy';
import { searchByCriteriaAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/ProductsDataActions.g';
import { GetCookie } from '../../../shared-utils/cookie-utilities';

export interface IFullProductsSearchResultsWithCount {
    products: ProductSearchResult[];
    count: number;
}

/**
 * GetFullProductsByIds Action Input
 */
export class GetFullProductsByIdsInput implements IActionInput {
    public maxRecentlyViewedItemsCount: number;

    constructor(maxRecentlyViewedItemsCount: number) {
        this.maxRecentlyViewedItemsCount = maxRecentlyViewedItemsCount;
    }
    public getCacheKey = () => 'FullProductSearchResultByIds';
    public getCacheObjectType = () => 'FullProductSearchResultByIds';
    public dataCacheType = (): CacheType => 'none';
}

const createInput = (inputData: ICreateActionContext<IGeneric<IAny>>, maxRecentlyViewedItemsCount: number): GetFullProductsByIdsInput => {
    return new GetFullProductsByIdsInput(maxRecentlyViewedItemsCount);
};

export async function getFullProductsByIds(
    input: GetFullProductsByIdsInput,
    ctx: IActionContext
): Promise<IFullProductsSearchResultsWithCount> {
    let productSearchResults: ProductSearchResult[];
    let promise: AsyncResult<ProductSearchResult[]>;
    const searchCriteriaInput: ProductSearchCriteria = {};
    searchCriteriaInput.Context = {
        ChannelId: ctx.requestContext.apiSettings.channelId,
        CatalogId: ctx.requestContext.apiSettings.catalogId
    };
    searchCriteriaInput.IncludeAttributes = true;
    const cookieName: string = '_msdyn365___recently_viewed_products';
    const cookieValue = GetCookie(ctx.requestContext.cookies, cookieName);
    const productIds = (cookieValue && cookieValue.split(',').map(x => +x)) || [0];
    searchCriteriaInput.Ids = productIds;
    promise = searchByCriteriaAsync(
        {
            callerContext: ctx
        },
        searchCriteriaInput
    );

    productSearchResults = await promise;

    const mappedProducts = productSearchResults.map(productSearchResult => {
        const newImageUrl = generateProductImageUrl(productSearchResult, ctx.requestContext.apiSettings);

        if (newImageUrl) {
            productSearchResult.PrimaryImageUrl = newImageUrl;
        }

        return productSearchResult;
    });

    const refinedProductSearchResult = refineProductSearchResults(
        ctx,
        productIds.reverse(),
        input.maxRecentlyViewedItemsCount,
        mappedProducts
    );

    return {
        products: refinedProductSearchResult,
        count: promise.metadata.count || 0
    };
}

function refineProductSearchResults(
    context: IActionContext,
    productIds: number[],
    maxRecentlyViewedItemsCount: number,
    productSearchResult: ProductSearchResult[]
): ProductSearchResult[] {
    const newproductSearchResult: ProductSearchResult[] = [];
    const pathName = context.requestContext.url.requestUrl.pathname;
    const splitPathName = pathName.split('/') || [];
    const lastPathName = splitPathName[splitPathName.length - 1] || '';
    const isPdp = (lastPathName && lastPathName.indexOf('.p')) || -1;
    if (isPdp !== -1) {
        const productId = lastPathName.split('.');
        const index = productIds.indexOf(Number(productId[0]));
        if (index !== -1) {
            productIds.splice(index, 1);
        }
    }
    if (productIds.length > maxRecentlyViewedItemsCount) {
        productIds.splice(maxRecentlyViewedItemsCount, productIds.length - maxRecentlyViewedItemsCount);
    }
    productIds.map(id => {
        const productById = productSearchResult.filter(product => product.RecordId === id) || [];
        productById.length > 0 && newproductSearchResult.push(productById[0]);
    });
    return newproductSearchResult;
}

export default createObservableDataAction({
    id: '@msdyn365-commerce-modules/search-result-container/get-full-products-by-ids',
    action: <IAction<IFullProductsSearchResultsWithCount>>getFullProductsByIds,
    input: createInput
});
