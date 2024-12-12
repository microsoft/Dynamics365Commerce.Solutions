/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { AsyncResult } from '@msdyn365-commerce/retail-proxy';
import { IFullProductsSearchResultsWithCount } from './actions/get-full-products-by-ids';

export interface ICustomRecentlyViewedProductsData {
    fullProductsSearchResultsWithCount: AsyncResult<IFullProductsSearchResultsWithCount>;
}
