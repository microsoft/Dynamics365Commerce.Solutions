/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { createObservableDataAction } from '@msdyn365-commerce/action-internal';
import { IAction, IActionContext } from '@msdyn365-commerce/core-internal';
import { getRelatedProductsAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/ProductsDataActions.g';
import { ProductSearchResult } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';

import { getProductImageUrls, ProductListInput } from '../actions/hydrate';

const actionInputError = (missingType: string) => {
    throw new Error(`Input ${missingType} is missing and required to run GetProductsByRelationship action`);
};

export const getProductsByRelationshipIdList = async (input: ProductListInput, context: IActionContext): Promise<ProductSearchResult[]> => {
    let productId;
    let relationshipId;
    // Grab the product ID from the list meta data or context
    if (input.listMetadata.productIds && input.listMetadata.productIds.length > 0) {
        productId = input.listMetadata.productIds[0];
    } else if (context.requestContext.urlTokens && context.requestContext.urlTokens.itemId) {
        productId = +context.requestContext.urlTokens.itemId;
    } else {
        actionInputError('productId');
    }
    // The relationship ID must be provided with the list meta data
    if (input.listMetadata.relationshipId) {
        relationshipId = +input.listMetadata.relationshipId;
    } else {
        actionInputError('relationshipId');
    }
    context.telemetry.debug(`Product Id input to productByRelationship action: ${productId}`);
    context.telemetry.debug(`Relationship Id input to productByRelationship action: ${relationshipId}`);
    if (productId && relationshipId) {
        return getRelatedProductsAsync(
            { callerContext: context, queryResultSettings: { Paging: { Top: input.listMetadata.pageSize || 10 } } },
            productId,
            input.channelId,
            input.catalogId,
            relationshipId
        )
            .then((products: ProductSearchResult[]) => {
                products.map(pro => {
                    pro.Name = `Related Product: ${pro.Name}`;
                });
                context.telemetry.debug('Products returned by getRelatedProducts action', products);
                return getProductImageUrls(products, context.requestContext.apiSettings);
            })
            .catch(error => {
                context.telemetry.error(`Error running getRelatedProducts action: ${error}`);
                throw new Error(error);
            });
    }
    return [];
};

export const getProductsByRelationshipIdListAction = createObservableDataAction({
    action: <IAction<ProductSearchResult[]>>getProductsByRelationshipIdList,
    id: '@msdyn365-commerce/products-by-relationship-id'
});
