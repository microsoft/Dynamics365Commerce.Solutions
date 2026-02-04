/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * ICustomRecentlyViewedProducts contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export const enum layout {
    carousel = 'carousel',
    grid = 'grid'
}

export interface ICustomRecentlyViewedProductsConfig extends Msdyn365.IModuleConfig {
    productCollection: Msdyn365.IProductList;
    heading?: IHeadingData;
    layout?: layout;
    imageSettings?: Msdyn365.IImageSettings;
    className?: string;
}

export interface ICustomRecentlyViewedProductsResources {
    priceFree: string;
    ratingAriaLabel: string;
    flipperNext: string;
    flipperPrevious: string;
    originalPriceText: string;
    currentPriceText: string;
}

export const enum HeadingTag {
    h1 = 'h1',
    h2 = 'h2',
    h3 = 'h3',
    h4 = 'h4',
    h5 = 'h5',
    h6 = 'h6'
}

export interface IHeadingData {
    text: string;
    tag?: HeadingTag;
}

export interface ICustomRecentlyViewedProductsProps<T> extends Msdyn365.IModule<T> {
    resources: ICustomRecentlyViewedProductsResources;
    config: ICustomRecentlyViewedProductsConfig;
}
