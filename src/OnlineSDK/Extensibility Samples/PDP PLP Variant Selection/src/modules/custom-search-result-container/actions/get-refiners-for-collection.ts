/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { IProductRefinerHierarchy } from '@msdyn365-commerce/commerce-entities';
import { createObservableDataAction, IAction, IActionContext, IActionInput, ICreateActionContext } from '@msdyn365-commerce/core';

import { BaseCollectionInput, createBaseCollectionInput } from './base-collection-action';
import { getProductRefinerHierarchy } from './get-product-refiner-hierarchy';

/**
 * Default Category/Product Id Values.
 */
enum DefaultValues {
    defaultCategoryIdValue = 0,
    defaultProductIdValue = 0
}

/**
 * Refiners-by-Collection Input action.
 */
export class RefinersByCollectionInput extends BaseCollectionInput implements IActionInput {
    public getCacheObjectType = () => 'ProductRefiner';

    public dataCacheType = () => {
        if (
            this.pageType !== 'Category' ||
            (this.refiners && this.refiners.length > 0) ||
            (this.queryResultSettings &&
                this.queryResultSettings.Sorting &&
                this.queryResultSettings.Sorting.Columns &&
                this.queryResultSettings.Sorting.Columns.length > 0)
        ) {
            return 'request';
        }
        return 'application';
    };
}

/**
 * Create input method which creates an ActionInput for fetching list page refiners.
 * @param args
 */
const createInput = (args: ICreateActionContext): IActionInput => {
    return createBaseCollectionInput(args, RefinersByCollectionInput);
};

/**
 * Action method which fetches refiners for the given list page.
 * @param input
 * @param ctx
 */
async function action(input: RefinersByCollectionInput, ctx: IActionContext): Promise<IProductRefinerHierarchy[]> {
    let searchProductId;
    if (input.pageType === 'Category') {
        if (input.category) {
            return getProductRefinerHierarchy(
                {
                    CategoryIds: [input.category || DefaultValues.defaultCategoryIdValue],
                    Context: {
                        ChannelId: input.apiSettings.channelId,
                        CatalogId: input.catalogId
                    },
                    Refinement: input.isUpdateRefinerPanel ? input.refiners : []
                },
                input.queryResultSettings,
                ctx
            );
        }
        throw new Error('[GetRefinersForCollection]Category Page Detected, but no global categoryId found');
    } else {
        if (input.searchText && ctx.requestContext.query && ctx.requestContext.query.q) {
            return getProductRefinerHierarchy(
                {
                    SearchCondition: input.searchText,
                    Context: {
                        ChannelId: input.apiSettings.channelId,
                        CatalogId: input.catalogId
                    },
                    Refinement: input.isUpdateRefinerPanel ? input.refiners : []
                },
                input.queryResultSettings,
                ctx
            );
        }
        if (input.searchText && ctx.requestContext.query && ctx.requestContext.query.productId) {
            const searchObject = JSON.parse(input.searchText);
            searchProductId = Number(searchObject.ProductId);
            if (Number.isNaN(searchProductId)) {
                throw new Error('Failed to cast search product id into a number.');
            } else if (!searchObject.Recommendation) {
                throw new Error('Failed to retrieve the Recommendation.');
            } else {
                return getProductRefinerHierarchy(
                    {
                        Context: {
                            ChannelId: input.apiSettings.channelId,
                            CatalogId: input.catalogId
                        },
                        Refinement: input.isUpdateRefinerPanel ? input.refiners : [],
                        RecommendationListId: searchObject.Recommendation,
                        Ids: [searchProductId || DefaultValues.defaultProductIdValue]
                    },
                    input.queryResultSettings,
                    ctx
                );
            }
        } else {
            throw new Error('[GetFullProductsForCollection]Search Page Detected, but no q= or productId= query parameter found');
        }
    }
}

export const actionDataAction = createObservableDataAction({
    id: '@msdyn365-commerce-modules/search-result-container/get-refiners-for-collection',
    action: <IAction<IProductRefinerHierarchy[]>>action,
    input: createInput
});

export default actionDataAction;
