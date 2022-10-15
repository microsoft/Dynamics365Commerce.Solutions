/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import { CategoryHierarchy, IProductRefinerHierarchy } from '@msdyn365-commerce/commerce-entities';
import { AsyncResult, FeatureState, SimpleProduct, Customer } from '@msdyn365-commerce/retail-proxy';

import { IFullProductsSearchResultsWithCount, MappedSearchConfiguration } from './actions';
import { ListPageState } from './list-page-state';

/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/**
 * Interface for search result container data.
 * @param {AsyncResult<FeatureState[]>} featureState - The feature state.
 */
export interface ICustomSearchResultContainerData {
    products: AsyncResult<IFullProductsSearchResultsWithCount>;
    category: AsyncResult<CategoryHierarchy>;
    listPageState: AsyncResult<ListPageState>;
    categoryHierarchy: AsyncResult<CategoryHierarchy[]>;
    searchConfiguration: AsyncResult<MappedSearchConfiguration[]>;
    refiners: AsyncResult<IProductRefinerHierarchy[]>;
    featureProduct: AsyncResult<SimpleProduct>;
    featureState: AsyncResult<FeatureState[]>;
    customerInformation: AsyncResult<Customer>;
}
