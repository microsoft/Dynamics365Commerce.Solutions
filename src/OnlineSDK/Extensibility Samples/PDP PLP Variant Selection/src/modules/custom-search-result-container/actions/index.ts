/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { ProductSearchResult, SortColumn } from '@msdyn365-commerce/retail-proxy';
import getCollectionProducts, { GetFullProductsByCollectionInput } from './get-full-products-by-collection';
import getMappedSearchConfiguration, { MappedSearchInput, sortOptions } from './get-mapped-search-configuration';
import getCollectionRefinersAction, { RefinersByCollectionInput } from './get-refiners-for-collection';

export * from './base-collection-action';
export * from './url-utils';

export {
    getCollectionProducts,
    getCollectionRefinersAction,
    GetFullProductsByCollectionInput,
    getMappedSearchConfiguration,
    IFullProductsSearchResultsWithCount,
    IMappedSearchConfiguration,
    MappedSearchInput,
    RefinersByCollectionInput,
    sortOptions
};

interface IFullProductsSearchResultsWithCount {
    products: ProductSearchResult[];
    count: number;
    channelInventoryConfigurationId?: number;
}

interface IMappedSearchConfiguration {
    key: string;
    sortColumn: SortColumn;
}
