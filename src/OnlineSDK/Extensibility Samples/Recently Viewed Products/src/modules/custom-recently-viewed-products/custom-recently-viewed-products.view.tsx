/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { INodeProps, Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';
import { IProductCollectionViewProps, IProductComponentViewProps } from './custom-recently-viewed-products';

const ProductCollectionView: React.FC<IProductCollectionViewProps> = props => {
    const { heading, ProductCollectionContainer, products, SingleSlideCarouselComponentProps, GridComponentProps, isCarousel } = props;
    if (products) {
        return (
            <Module {...ProductCollectionContainer}>
                {heading}
                {isCarousel ? _renderCarousel(SingleSlideCarouselComponentProps, products) : _renderGrid(GridComponentProps, products)}
            </Module>
        );
    }
    return null;
};

const _renderCarousel = (carouselContainer: INodeProps, items: IProductComponentViewProps[]): JSX.Element => {
    return <Node {...carouselContainer}>{items && items.map(_renderProduct)}</Node>;
};

const _renderGrid = (gridContainer: INodeProps, items: IProductComponentViewProps[]): JSX.Element => {
    return <Node {...gridContainer}>{items && items.map(_renderProduct)}</Node>;
};

const _renderProduct = (product: IProductComponentViewProps): JSX.Element => {
    const { ProductContainer, productComponent } = product;

    return <Node {...ProductContainer}>{productComponent}</Node>;
};

export default ProductCollectionView;
