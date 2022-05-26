/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */
import { AsyncResult } from '@msdyn365-commerce/retail-proxy';
import { IGetProductReviewsData } from '../../actions/get-product-reviews.action'; 

export interface IProductFeatureData {
    productReviews: AsyncResult<IGetProductReviewsData>;
}
