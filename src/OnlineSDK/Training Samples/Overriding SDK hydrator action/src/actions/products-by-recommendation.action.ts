/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable @typescript-eslint/no-explicit-any */
import { createObservableDataAction } from '@msdyn365-commerce/action-internal';
import { getCatalogIdForSdk, IAction, IActionContext, versionGte } from '@msdyn365-commerce/core-internal';
import { IContext } from '@msdyn365-commerce/retail-proxy';
import { readAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/CartsDataActions.g';
import { searchByCriteriaAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/ProductsDataActions.g';
import { getElementsAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/RecommendationsDataActions.g';
import {
    Cart,
    ProductSearchCriteria,
    ProductSearchResult,
    RecommendationCriteria,
    RecommendedElement
} from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import { ProductInput } from '@msdyn365-commerce-modules/retail-actions';
// import { orderProductSearchResults } from '../../utils/product-list-utils';

import { getProductImageUrls, ProductListInput } from '../actions/hydrate';

const actionInputError = (missingType: string) => {
    throw new Error(`Input ${missingType} is missing and required to run GetProductsByRecommendationId action`);
};

const enum CartTokenPrefix {
    Auth = 't',
    Anon = 'p'
}

/**
 * Constructs the category context for a recommendation call. If page context is selected the category dervided from page context
 * (current page) will be used. If instead a spefic category is chosen as context that category will be used instead.
 *
 * @param recommendationCriteria The criteria that will be used for the recommendation API call
 * @param input The ProductListInput provided to the action
 * @param context The current action context
 */
const getCategoryIdForReco = (recommendationCriteria: RecommendationCriteria, input: ProductListInput, context: IActionContext) => {
    let categoryIds: number[] = [];

    if (input.listMetadata.includePageContext || !input.listMetadata.categoryIds || input.listMetadata.categoryIds.length === 0) {
        const urlTokens = context.requestContext.urlTokens;

        if (urlTokens && urlTokens.pageType && urlTokens.pageType.toLowerCase() === 'category') {
            if (urlTokens.recordId) {
                categoryIds = [+urlTokens.recordId];
            }
        }
    } else {
        // Use category from a specific context if provided
        if (input.listMetadata.categoryIds && input.listMetadata.categoryIds.length > 0) {
            categoryIds = input.listMetadata.categoryIds;
        }
    }

    recommendationCriteria.CategoryIds = categoryIds;
};

/**
 * Fetches the current active cart to be used when cart context
 * is selected for a reco list
 *
 * @param ctx The current action context
 */
const getCart = async (ctx: IActionContext): Promise<Cart | undefined> => {
    let cart;
    if (ctx.requestContext && ctx.requestContext.cookies) {
        const cookies = ctx.requestContext && ctx.requestContext.cookies;
        const cartCookie = cookies.getCartCookie();
        const cartCookieParts = cartCookie.split(':');
        if (cartCookieParts && cartCookieParts.length === 2) {
            if (
                (ctx.requestContext.user.isAuthenticated && cartCookieParts[0] === CartTokenPrefix.Auth) ||
                (!ctx.requestContext.user.isAuthenticated && cartCookieParts[0] === CartTokenPrefix.Anon)
            ) {
                const readCart = await readAsync({ callerContext: ctx }, cartCookieParts[1]);
                if (readCart && readCart.Id) {
                    cart = readCart;
                }
            }
        }
    }
    return cart;
};

/**
 * Constructs the product context for a recommendation call. If cart context is selected, products currently in the active cart will
 * be used as the product ids for the recommendation call. Otherwise, if page context is selected the product derived from page context will be used.
 * Finally, if specific context is selected, a product id will be given to us from the ProductListInput which will be used to seed the product id for the reco call.
 *
 * @param recommendationCriteria The criteria that will be used for the recommendation API call
 * @param input The ProductListInput provided to the action
 * @param context The current action context
 */
const getProductIdForReco = async (recommendationCriteria: RecommendationCriteria, input: ProductListInput, context: IActionContext) => {
    let productIds: number[] = [];
    // If include cart is set true, fetch the product ids in the cart to append to the product ID list for the recommendation call
    if (input.listMetadata.includeCart) {
        const cart = await getCart(context);
        if (cart && cart.CartLines) {
            const validCartLines = cart.CartLines.filter(element => element.ProductId !== undefined);
            const cartProductIds = validCartLines.map(product => product.ProductId);
            context.telemetry.debug(`Products in cart: ${cartProductIds}`);
            // If the cart isn't empty, assign the products in the cart to the productIds for seeding
            if (cartProductIds && cartProductIds.length > 0) {
                productIds = <any>cartProductIds;
            }
        }
    } else if (input.listMetadata.includePageContext || !input.listMetadata.productIds || input.listMetadata.productIds.length === 0) {
        const urlTokens = context.requestContext.urlTokens;

        if (urlTokens && urlTokens.recordId && urlTokens.pageType && urlTokens.pageType.toLowerCase() === 'product') {
            productIds = [+urlTokens.recordId];
        }
    } else {
        // Use the product from specfic context
        if (input.listMetadata.productIds && input.listMetadata.productIds.length > 0) {
            productIds = [input.listMetadata.productIds[0]];
        }
    }

    return productIds;
};

const getProductsByNewSearchByCriteria = async (input: ProductListInput, callerContext: IContext): Promise<ProductSearchResult[]> => {
    const context = <IActionContext>callerContext.callerContext;
    const customerAccountNumber =
        context.requestContext && context.requestContext.user && context.requestContext.user.customerAccountNumber;
    const searchCriteria: ProductSearchCriteria = {
        // @ts-ignore
        CustomerAccountNumber: customerAccountNumber,
        RecommendationListId: input.listMetadata.recommendationListId,
        Context: {
            ChannelId: +context.requestContext.apiSettings.channelId,
            CatalogId: +input.catalogId
        },
        SkipVariantExpansion: true,
        IncludeAttributes: true
    };
    const rsVersion =
        !process.env.MSDyn365Commerce_RSVERSION || process.env.MSDyn365Commerce_RSVERSION === '--'
            ? '0.0'
            : process.env.MSDyn365Commerce_RSVERSION;

    // after 9.24 version, we dont need to use two retail server call - getElement + searchByCriteria to get all recommendation products.
    // we just need to use searchByCriteria.

    const sequentialCallForRecoProducts = versionGte(rsVersion, '9.24');
    if (sequentialCallForRecoProducts && !context.requestContext?.features?.disable_sequential_call_for_recoProducts) {
        try {
            getCategoryIdForReco(searchCriteria, input, context);
            searchCriteria.Ids = await getProductIdForReco(searchCriteria, input, context);
            let productSearchResults = await searchByCriteriaAsync(
                { callerContext: context, queryResultSettings: { Paging: { Top: input.listMetadata.pageSize || 10 } } },
                searchCriteria
            );
            productSearchResults.map(pro => {
                pro.Name = `${pro.Name} testing`;
            });
            if (productSearchResults.length) {
                let productSearchResultsTmp: ProductSearchResult[] = [];
                productSearchResultsTmp = productSearchResults.filter(item => {
                    // @ts-ignore
                    const isVariantProduct = !item.IsMasterProduct && item.MasterProductId !== 0;
                    return !isVariantProduct;
                });
                productSearchResults = productSearchResultsTmp;
                // If the ProductSearchResult API finds the products then populate the product image urls and return
                // otherwise if the API does not exist or does not return products proceed to the legacy flows for legacy/backward compatibility reasons
                if (productSearchResults.length > 0) {
                    productSearchResults.map(pro => {
                        pro.Name = `Recommended Product: ${pro.Name}`;
                    });
                    getProductImageUrls(productSearchResults, context.requestContext.apiSettings);
                    return productSearchResults;
                }
            }
        } catch (e) {
            // In case of an error fall back to legacy flow
            context.telemetry.error(`Error while getting productSearchResult: ${e}`);
        }
    }
    return [];
};

export const getProductsByRecommendationList = async (input: ProductListInput, context: IActionContext): Promise<ProductSearchResult[]> => {
    // If the recommendation list is picks for you and the user is not signed do not make the reco API call and return empty list of products
    const customerAccountNumber =
        context.requestContext && context.requestContext.user && context.requestContext.user.customerAccountNumber;
    if (input.listMetadata.recommendationListId === 'picks' && !customerAccountNumber) {
        return [];
    }

    const recommendationCriteria: RecommendationCriteria = {};
    const proxyContext: IContext = { callerContext: context, queryResultSettings: { Paging: { Top: input.listMetadata.pageSize || 10 } } };
    recommendationCriteria.CatalogId = input.catalogId;

    let reccomendationListId;
    if (input.listMetadata.recommendationListId) {
        reccomendationListId = input.listMetadata.recommendationListId;
    } else {
        actionInputError(`recommendationListId (from listmetadata)`);
    }
    recommendationCriteria.ProductIds = await getProductIdForReco(recommendationCriteria, input, context);

    if ((input.listMetadata.personalization || reccomendationListId === 'picks') && customerAccountNumber) {
        recommendationCriteria.CustomerAccountNumber = customerAccountNumber;
        proxyContext.bypassCache = 'get';
    }

    const products = await getProductsByNewSearchByCriteria(input, proxyContext);
    if (products.length > 0) {
        return products;
    }

    context.telemetry.debug(`Recommendation Criteria: ${JSON.stringify(recommendationCriteria)}`);

    // For FBT lists, a product or category seed must be provided or the request will error out. So we check to see if any have been provided and return empty list if none have
    if (reccomendationListId === 'fbt') {
        if (
            recommendationCriteria.ProductIds &&
            recommendationCriteria.ProductIds.length === 0 &&
            recommendationCriteria.CategoryIds &&
            recommendationCriteria.CategoryIds.length === 0
        ) {
            return [];
        }
    }
    if (reccomendationListId) {
        return getElementsAsync(proxyContext, reccomendationListId, recommendationCriteria)
            .then(async (recommendedElements: RecommendedElement[]) => {
                if (recommendedElements) {
                    const productInputs: ProductInput[] = [];
                    const productTypeId: string[] = [];
                    const catalogIdNumber = getCatalogIdForSdk(context.requestContext, null);
                    // Grab all the elements that are products and store the product ids
                    for (let i = 0; i < recommendedElements.length; i++) {
                        const element = recommendedElements[i];
                        // Element type value of 1 indicates product type
                        if (element.ElementId && element.ElementTypeValue === 1) {
                            productInputs[i] = new ProductInput(+element.ElementId, context.requestContext.apiSettings, catalogIdNumber);
                            productTypeId[i] = element.ElementId;
                        }
                    }
                    context.telemetry.debug(`Running recommendation action for list ${input.listMetadata.recommendationListId}`);
                    context.telemetry.debug(`Number of products returned: ${productTypeId.length}`);
                    context.telemetry.debug('Product ids returned', productTypeId);

                    if (productInputs.length) {
                        const itemIds: number[] = productInputs.map(value => value.productId);
                        const productSearchCriteria: ProductSearchCriteria = {
                            Ids: itemIds,
                            Context: {
                                ChannelId: +context.requestContext.apiSettings.channelId,
                                CatalogId: +input.catalogId
                            },
                            SkipVariantExpansion: true,
                            IncludeAttributes: true
                        };
                        try {
                            const productSearchResults = await searchByCriteriaAsync(
                                { callerContext: context, queryResultSettings: { Paging: { Top: input.listMetadata.pageSize || 10 } } },
                                productSearchCriteria
                            );
                            // If the ProductSearchResult API finds the products then populate the product image urls and return
                            // otherwise if the API does not exist or does not return products proceed to the legacy flows for legacy/backward compatibility reasons
                            context.telemetry.debug('Product search results returned', JSON.stringify(productSearchResults));
                            if (productSearchResults.length > 0) {
                                getProductImageUrls(productSearchResults, context.requestContext.apiSettings);
                                // productSearchResults = orderProductSearchResults(itemIds, productSearchResults);
                                return productSearchResults;
                            }
                        } catch (e) {
                            // In case of an error fall back to legacy flow
                            context.telemetry.error(`Error while getting productSearchResult: ${e}`);
                        }
                    }
                }
                return [];
            })
            .catch(error => {
                context.telemetry.error(`Error running productByRecommendation action: ${error}`);
                throw new Error(error);
            });
    }
    return [];
};

export const getProductsByRecommendationListAction = createObservableDataAction({
    action: <IAction<ProductSearchResult[]>>getProductsByRecommendationList,
    id: '@msdyn365-commerce/products-by-recommendation'
});
