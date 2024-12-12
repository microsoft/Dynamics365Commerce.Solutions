/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as Msdyn365 from '@msdyn365-commerce/core';
import { IQueryResultSettings, ProductSearchResult, ProductSearchCriteria, CacheType } from '@msdyn365-commerce/retail-proxy';
import { searchByCriteriaAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/ProductsDataActions.g';
import { buildCacheKey, QueryResultSettingsProxy } from '@msdyn365-commerce-modules/retail-actions';
import { createObservableDataAction, ICommerceApiSettings } from '@msdyn365-commerce/core';
/**
 * GetCategoryProducts Input Action
 */
export class GetCategoryProductsInput implements Msdyn365.IActionInput {
    public categoryId: number;
    public itemsPerPage: number;
    public queryResultSettings: IQueryResultSettings;
    private readonly apiSettings: ICommerceApiSettings;
    public constructor(
        categoryId: number,
        itemsPerPage: number,
        queryResultSettings: IQueryResultSettings,
        apiSettings: ICommerceApiSettings
    ) {
        this.categoryId = categoryId;
        this.itemsPerPage = itemsPerPage;
        this.queryResultSettings = queryResultSettings;
        this.apiSettings = apiSettings;
    }

    // TODO: Determine if the results of this get action should cache the results and if so provide
    // a cache object type and an appropriate cache key
    public getCacheKey = () => buildCacheKey(`Category-${this.categoryId}`, this.apiSettings);
    public getCacheObjectType = () => 'CategoryProducts';
    public dataCacheType = (): CacheType => 'request';
}

/**
 * TODO: Use this function to create the input required to make the action call
 */
const createInput = (args: Msdyn365.ICreateActionContext<{ categoryId: number; itemsPerPage: number }>): Msdyn365.IActionInput => {
    const queryResultSettings = QueryResultSettingsProxy.fromInputData(args).QueryResultSettings;
    return new GetCategoryProductsInput(
        args.config?.categoryId || 0,
        args.config?.itemsPerPage || 0,
        queryResultSettings,
        args.requestContext.apiSettings
    );
};

/**
 * TODO: Use this function to call your action and process the results as needed
 */

export async function getCategoryProducts(input: GetCategoryProductsInput, ctx: Msdyn365.IActionContext): Promise<ProductSearchResult[]> {
    const searchCriteriaInput: ProductSearchCriteria = {};
    searchCriteriaInput.Context = { ChannelId: ctx.requestContext.apiSettings.channelId, CatalogId: 0 };
    searchCriteriaInput.CategoryIds = [input.categoryId || 0];

    // Set Top
    if (input.queryResultSettings.Paging && input.itemsPerPage) {
        input.queryResultSettings.Paging.Top = input.itemsPerPage || 1;
    }

    // Set Skip
    if (input.queryResultSettings.Paging && ctx.requestContext.query && ctx.requestContext.query.skip) {
        input.queryResultSettings.Paging.Skip = +ctx.requestContext.query.skip;
    }

    const result: ProductSearchResult[] = await searchByCriteriaAsync(
        {
            callerContext: ctx,
            queryResultSettings: input.queryResultSettings
        },
        searchCriteriaInput
    );
    return result;
}

export default createObservableDataAction({
    action: <Msdyn365.IAction<ProductSearchResult[]>>getCategoryProducts,
    id: 'GetCategoryProducts',
    input: createInput
});
