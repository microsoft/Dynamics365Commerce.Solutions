/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { ProductSearchResult } from '@msdyn365-commerce/retail-proxy';
import * as React from 'react';
import { generateImageUrl } from '@msdyn365-commerce-modules/retail-actions';
import { IShowCategoryProductsViewProps } from './show-category-products';

export default (props: IShowCategoryProductsViewProps) => {
    const { products } = props;
    if (products && products.length > 0) {
        return (
            <div className='ms-search-result-container__Products'>
                <ul className='list-unstyled'>
                    {products.map((product: ProductSearchResult, index: number) => (
                        <li className='ms-product-search-result__item' key={index}>
                            <img
                                src={generateImageUrl(product.PrimaryImageUrl, props.context.request.apiSettings)}
                                alt='product'
                                className='img-fluid p-3'
                            />
                            <h4 className='msc-product__title'>{product.Name}</h4>
                            <span className='msc-price__price'>{product.BasePrice}</span>
                        </li>
                    ))}
                </ul>
            </div>
        );
    }
    return null;
};
