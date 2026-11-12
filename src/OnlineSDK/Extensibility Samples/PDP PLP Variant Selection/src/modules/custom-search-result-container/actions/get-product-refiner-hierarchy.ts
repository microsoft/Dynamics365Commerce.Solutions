/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { IProductRefinerHierarchy } from '@msdyn365-commerce/commerce-entities';
import { IActionContext } from '@msdyn365-commerce/core';
import { IQueryResultSettings, ProductRefiner, ProductSearchCriteria } from '@msdyn365-commerce/retail-proxy';
import { getProductSearchRefinersAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/ProductsDataActions.g';

/**
 * Uses Bulk refiner API to get refiners and child refiner values.
 * @param searchCriteria Product search criteria.
 * @param maxItems Max items.
 * @param queryResultSettings
 * @param ctx Action context.
 */
export async function getProductRefinerHierarchy(
    searchCriteria: ProductSearchCriteria,
    queryResultSettings: IQueryResultSettings,
    ctx: IActionContext
): Promise<IProductRefinerHierarchy[]> {
    return getProductSearchRefinersAsync({ callerContext: ctx, queryResultSettings }, searchCriteria).then(
        (productRefiners: ProductRefiner[]) => {
            if (!productRefiners) {
                ctx.trace('[getRefinersByCriteriaAction] returned no refiners');
                return <IProductRefinerHierarchy[]>[];
            }

            const result = (productRefiners || []).map((productRefiner: ProductRefiner) => <IProductRefinerHierarchy>productRefiner);

            if (result.length === 0) {
                ctx.trace('[getRefinersByCriteriaAction] returned no IProductRefinerHierarchy objects');
                return <IProductRefinerHierarchy[]>[];
            }

            return result;
        }
    );
}
