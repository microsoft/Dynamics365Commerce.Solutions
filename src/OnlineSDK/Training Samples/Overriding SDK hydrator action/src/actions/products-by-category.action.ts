/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { createObservableDataAction } from '@msdyn365-commerce/action-internal';
import { IAction, IActionContext, ICommerceApiSettings } from '@msdyn365-commerce/core-internal';
import { searchByCriteriaAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/ProductsDataActions.g';
import { ProductSearchCriteria, ProductSearchResult, SimpleProduct } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import { generateImageUrl } from '@msdyn365-commerce-modules/retail-actions';
import { ProductListInput } from '../actions/hydrate';
/**
 *
 */

export const getProductsByCategoryList = async (input: ProductListInput, context: IActionContext): Promise<ProductSearchResult[]> => {
    let categoryId: number;
    const urlTokens = context.requestContext.urlTokens;

    // If the category ID has been provided from the list metadata use it, otherwise grab the category ID from context
    if (input.listMetadata.categoryIds && input.listMetadata.categoryIds.length > 0) {
        categoryId = input.listMetadata.categoryIds[0];
    } else if (urlTokens && urlTokens.pageType && urlTokens.pageType.toLowerCase() === 'category' && urlTokens.recordId) {
        categoryId = +urlTokens.recordId;
    } else if (urlTokens && urlTokens.categories && urlTokens.categories.length > 0) {
        categoryId = +urlTokens.categories[0];
    } else {
        // Unable to determine category from list meta data or context
        throw new Error('CategoryId Missing for ProductsByCategory List');
    }
    context.telemetry.debug(`Category Id input to ProductsByCategory action : ${categoryId}`);
    const productSearchCriteria: ProductSearchCriteria = {
        Context: {
            ChannelId: +input.channelId,
            CatalogId: +input.catalogId
        },
        CategoryIds: [categoryId],
        SkipVariantExpansion: true,
        IncludeAttributes: true
    };

    return searchByCriteriaAsync(
        { callerContext: context, queryResultSettings: { Paging: { Top: input.listMetadata.pageSize || 10 } } },
        productSearchCriteria
    )
        .then((products: ProductSearchResult[]) => {
            context.telemetry.debug('Products returned by ProductByCategory action', products);
            products.map(pro => {
                pro.Name = `Category Product: ${pro.Name}`;
            });
            return getProductImageUrls(products, context.requestContext.apiSettings);
        })
        .catch(error => {
            context.telemetry.error(`Error running ProductByCategory action: ${error}`);
            throw new Error(error);
        });
};

export const getProductImageUrls = (
    products: SimpleProduct[] | ProductSearchResult[],
    apiSettings: ICommerceApiSettings
): SimpleProduct[] | ProductSearchResult[] => {
    const productsWithImageUrls: SimpleProduct[] | ProductSearchResult[] = [];

    products &&
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        products.forEach((product: any) => {
            if (product && product.RecordId) {
                product.PrimaryImageUrl = generateImageUrl(product.PrimaryImageUrl, apiSettings);
                productsWithImageUrls.push(product);
            }
        });

    return productsWithImageUrls;
};

export const getProductsByCategoryListAction = createObservableDataAction({
    action: <IAction<ProductSearchResult[]>>getProductsByCategoryList,
    id: '@msdyn365-commerce/products-by-category'
});
