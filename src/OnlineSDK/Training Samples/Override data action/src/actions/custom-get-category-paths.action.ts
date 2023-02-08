/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import {
    CacheType,
    createObservableDataAction,
    getCatalogId,
    getCategoriesUrlSync,
    IAction,
    IActionContext,
    IActionInput,
    IAny,
    ICommerceApiSettings,
    ICreateActionContext,
    IGeneric,
    IRequestContext
} from '@msdyn365-commerce/core';
import { ICategoryPath, ICategoryUrl } from '@msdyn365-commerce/core-internal/dist/types/interfaces/ICategoryPathInterfaces';
import { getCategoryPathsAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/ProductsDataActions.g';
import { CategoryPathLookup } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';

import { buildCacheKey, getSelectedProductIdFromActionInput, QueryResultSettingsProxy } from '@msdyn365-commerce-modules/retail-actions';

/**
 * Category path input.
 */
export class GetCategoryPathsInput implements IActionInput {
    public readonly ChannelId: number;

    public readonly CatalogId: number;

    public readonly categoryPathLooksups: CategoryPathLookup[];

    public readonly apiSettings: ICommerceApiSettings;

    private readonly locale: string;

    private constructedCacheKey: string;

    public constructor(context: IRequestContext, categoryPathLooksups: CategoryPathLookup[]) {
        this.ChannelId = context.apiSettings.channelId;
        this.CatalogId = getCatalogId(context);
        this.categoryPathLooksups = categoryPathLooksups;
        this.apiSettings = context.apiSettings;
        this.constructedCacheKey = '';
        categoryPathLooksups.forEach(categoryPath => {
            this.constructedCacheKey += `${categoryPath.ProductId && categoryPath.ProductId.toString()}|`;
        });
        this.constructedCacheKey += `${this.ChannelId.toString()}|`;
        this.constructedCacheKey += `${this.CatalogId.toString()}|`;
        this.locale = context.locale;
    }

    public getCacheKey = () => buildCacheKey(this.constructedCacheKey, this.apiSettings, this.locale);

    public getCacheObjectType = () => 'CategoryPath';

    public dataCacheType = (): CacheType => 'request';

    /**
     * Retrieves locale to use for the current API call.
     * @returns A string which represents locale.
     */
    public getLocale = (): string => this.locale;
}

/**
 * Creates the input required to make the getCategoryPath retail api call.
 * @param inputData
 */
export const createGetCategoryPathsInput = (inputData: ICreateActionContext<IGeneric<IAny>>): IActionInput => {
    const productId = getSelectedProductIdFromActionInput(inputData);
    if (productId) {
        return new GetCategoryPathsInput(inputData.requestContext, [{ ProductId: +productId }]);
    }
    throw new Error('Unable to create SelectedVariantInput, no productId found on module config or query');
};

/**
 * Calls the Retail API and returns the category path for a product.
 * @param input
 * @param ctx
 */
export async function getCategoryPathsAction(input: GetCategoryPathsInput, ctx: IActionContext): Promise<ICategoryUrl[]> {
    const categoryPathResults = await getCategoryPathsAsync(
        {
            callerContext: ctx,
            queryResultSettings: QueryResultSettingsProxy.getPagingFromInputDataOrDefaultValue(ctx)
        },
        input.ChannelId,
        input.CatalogId,
        input.categoryPathLooksups
    );
    const categoryPath = categoryPathResults[0].CategoryPath && categoryPathResults[0].CategoryPath[0];
    const categoryUrl = getCategoriesUrlSync(<ICategoryPath>categoryPath, ctx);
    if (categoryUrl) {
        const defaultCategory: ICategoryUrl = { Name: 'Custom path', Url: '/home' };
        categoryUrl.push(defaultCategory);
        /* Do not return the root category of the product*/
        return categoryUrl.slice(1);
    }
    return [];
}

export const getCategoryPathsActionDataAction = createObservableDataAction({
    id: '@msdyn365-commerce-modules/retail-actions/get-category-paths',
    action: <IAction<ICategoryUrl[]>>getCategoryPathsAction,
    input: createGetCategoryPathsInput
});

export default getCategoryPathsActionDataAction;
