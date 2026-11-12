/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IProductFeature contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export const enum imageAlignment {
    left = 'left',
    right = 'right'
}

export interface IProductFeatureConfig extends Msdyn365.IModuleConfig {
    recordId?: string;
    imageAlignment?: imageAlignment;
    productTitle?: string;
    productDetails?: Msdyn365.RichText;
    productImage?: Msdyn365.IImageData;
    buttonText?: string;
}

export interface IProductFeatureProps<T> extends Msdyn365.IModule<T> {
    config: IProductFeatureConfig;
}
