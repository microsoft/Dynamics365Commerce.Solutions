/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { createObservableDataAction } from '@msdyn365-commerce/action-internal';
import { IAction, IActionContext } from '@msdyn365-commerce/core-internal';
import { searchByTextAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/ProductsDataActions.g';
import { ProductSearchResult } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import { getProductImageUrls, ProductListInput } from '../actions/hydrate';

export const getProductsBySearchList = async (input: ProductListInput, context: IActionContext): Promise<ProductSearchResult[]> => {
    let searchText: string;
    if (input.listMetadata.searchText) {
        searchText = input.listMetadata.searchText;
    } else if (context.requestContext.query && context.requestContext.query && context.requestContext.query.q) {
        searchText = context.requestContext.query.q;
    } else {
        throw new Error(
            'Search text missing for ProductsBySearch. Query string ?q={searchText} or search text in list meta data must be present.'
        );
    }

    return searchByTextAsync(
        { callerContext: context, queryResultSettings: { Paging: { Top: input.listMetadata.pageSize || 10 } } },
        input.channelId,
        input.catalogId,
        searchText
    )
        .then((products: ProductSearchResult[]) => {
            context.telemetry.debug('Products returned by productsBySearch action:', products);
            products.map(pro => {
                pro.Name = `Searched Product: ${pro.Name}`;
            });
            return getProductImageUrls(products, context.requestContext.apiSettings);
        })
        .catch(error => {
            context.telemetry.error(`Error running productsBySearch action: ${error}`);
            throw new Error(error);
        });
};

export const getProductsBySearchListAction = createObservableDataAction({
    action: <IAction<ProductSearchResult[]>>getProductsBySearchList,
    id: '@msdyn365-commerce/products-by-search'
});
