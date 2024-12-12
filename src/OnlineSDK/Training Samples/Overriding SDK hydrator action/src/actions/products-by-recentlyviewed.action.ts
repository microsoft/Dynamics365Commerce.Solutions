/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { createObservableDataAction } from '@msdyn365-commerce/action-internal';
import { getCatalogIdForSdk, IAction, IActionContext } from '@msdyn365-commerce/core-internal';
import { searchByCriteriaAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/ProductsDataActions.g';
import { ProductSearchCriteria, ProductSearchResult } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import { ICookieContext } from '@msdyn365-commerce/core';
import { RecentlyViewedProductItem } from '@msdyn365-commerce-modules/buybox/src/modules/recently-viewed/base';
import { getProductImageUrls, ProductListInput } from '../actions/hydrate';

export const getProductsByRecentlyViewedList = async (input: ProductListInput, ctx: IActionContext): Promise<ProductSearchResult[]> => {
    const searchCriteriaInput: ProductSearchCriteria = {};
    const cookieContext: ICookieContext = ctx.requestContext.cookies;
    const catalogId = getCatalogIdForSdk(ctx.requestContext, null);
    searchCriteriaInput.Context = {
        ChannelId: ctx.requestContext.apiSettings.channelId,
        CatalogId: catalogId
    };
    searchCriteriaInput.IncludeAttributes = false;
    searchCriteriaInput.SkipVariantExpansion = true;
    const cookieName: string = '_msdyn365___recently_viewed_products';
    const cookieValue = cookieContext.get<RecentlyViewedProductItem[] | undefined | null>(cookieName).value;
    const productIds = cookieValue?.filter(value => value.catalogId === catalogId).map(value => value.productId) || [0];
    searchCriteriaInput.Ids = productIds;

    return searchByCriteriaAsync(
        { callerContext: ctx, queryResultSettings: { Paging: { Top: input.listMetadata.pageSize || 10 } } },
        searchCriteriaInput
    )
        .then((products: ProductSearchResult[]) => {
            products.map(pro => {
                pro.Name = `Recent Product: ${pro.Name}`;
            });
            ctx.telemetry.debug('Products returned by ProductByRecentlyViewed action', products);
            return getProductImageUrls(products, ctx.requestContext.apiSettings);
        })
        .catch(error => {
            ctx.telemetry.error(`Error running ProductByRecentlyViewed action: ${error}`);
            throw new Error(error);
        });
};

export const getProductsByRecentlyViewedListAction = createObservableDataAction({
    action: <IAction<ProductSearchResult[]>>getProductsByRecentlyViewedList,
    id: '@msdyn365-commerce/products-by-recently-viewed'
});
