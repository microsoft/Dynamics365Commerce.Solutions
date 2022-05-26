/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import {
    CacheType,
    createObservableDataAction,
    IAction,
    IActionContext,
    IActionInput,
    ICreateActionContext
} from '@msdyn365-commerce/core';
import { getCustomer, GetCustomerInput } from '@msdyn365-commerce-modules/retail-actions';
import customDataAction, {
    CustomGetFullProductsByCollectionInput,
    IFullProductsSearchResultsWithCount
} from './custom-get-full-products-by-collection.action';
import { BaseCollectionInput, createBaseCollectionInput } from './base-collection.action';

let productCollectionArgs: ICreateActionContext<{ itemsPerPage: number; includedAttributes: boolean | undefined }>;

/**
 * GetFullProductsByCollection Action Input.
 */
export class GetFullProductsByCollectionInput extends BaseCollectionInput implements IActionInput {
    public getCacheKey = () => 'FullProductByCollection';
    /**
     * The cache object type.
     * @returns The cache object type.
     */
    public getCacheObjectType = (): string => 'FullProductSearchResult';

    /**
     * The data cache type.
     * @returns The data cache type.
     */
    public dataCacheType = (): CacheType => {
        return 'none';
    };
}

/**
 * CreateInput function which creates and actionInput used to fetch products for a list page.
 * @param args - The API arguments.
 * @returns IActionInput - The action input.
 */
const createInput = (args: ICreateActionContext<{ itemsPerPage: number; includedAttributes: boolean | undefined }>): IActionInput => {
    productCollectionArgs = args;

    return createBaseCollectionInput(args, GetFullProductsByCollectionInput);
};

/**
 * Action function to fetch products for a list page.
 * @param input - The input.
 * @param context - The context.
 * @returns IFullProductsSearchResultsWithCount - The full product search result with count.
 */
async function action(input: GetFullProductsByCollectionInput, context: IActionContext): Promise<IFullProductsSearchResultsWithCount> {
    let customerType: string = '';
    let products: IFullProductsSearchResultsWithCount = {
        products: [],
        count: 0
    };
    const isUserAuthenticated = context.requestContext.user.isAuthenticated;
    if (isUserAuthenticated) {
        const customerInput = new GetCustomerInput(context.requestContext.apiSettings, context.requestContext.user.customerAccountNumber);
        const customer = await getCustomer(customerInput, context);

        // Read the extension property and set the customerType value as required
        const customerTypeFromExtension: string =
            customer?.ExtensionProperties?.find(x => x.Key === 'CMISRecommend')?.Value?.StringValue || '';

        if (customerTypeFromExtension === 'CMISTemple') {
            customerType = 'Temple';
        } else if (customerTypeFromExtension === 'CMISEndowment') {
            customerType = 'Endowment';
        }
    } else {
        customerType = 'Anonymous';
    }

    const updatedArgs: ICreateActionContext = {
        requestContext: context.requestContext,
        config: productCollectionArgs.config,
        data: productCollectionArgs.data
    };

    const baseCollectionInput: BaseCollectionInput = createBaseCollectionInput(
        updatedArgs,
        CustomGetFullProductsByCollectionInput,
        customerType
    );

    if (input.refiners.length > 0) {
        baseCollectionInput.refiners = input.refiners;
    } else {
        baseCollectionInput.refiners = [];
    }
    // Set Top
    if (baseCollectionInput.queryResultSettings.Paging && productCollectionArgs.config) {
        baseCollectionInput.queryResultSettings.Paging.Top = productCollectionArgs.config.itemsPerPage || 1;
    }

    // Set Skip
    if (
        baseCollectionInput.queryResultSettings.Paging &&
        productCollectionArgs.requestContext.query &&
        productCollectionArgs.requestContext.query.skip
    ) {
        baseCollectionInput.queryResultSettings.Paging.Skip = +productCollectionArgs.requestContext.query.skip;
    }

    if (input.queryResultSettings.Sorting && input.queryResultSettings.Sorting.Columns) {
        baseCollectionInput.queryResultSettings.Sorting = input.queryResultSettings.Sorting;
    }

    baseCollectionInput.queryResultSettings.count = true;

    products = await customDataAction(baseCollectionInput, context);

    return products;
}

export const actionDataAction = createObservableDataAction({
    id: '@msdyn365-commerce-modules/search-result-container/get-full-products-by-collection',
    action: <IAction<IFullProductsSearchResultsWithCount>>action,
    input: createInput
});

export default actionDataAction;
