/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */
import { AsyncResult, SimpleProduct } from '@msdyn365-commerce/retail-proxy';

export interface IProductFeatureData {
    products: AsyncResult<SimpleProduct>[];
}
