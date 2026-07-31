/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IConfigVisibility contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export const enum layout {
    plainTextOnly = 'plainTextOnly',
    richTextOnly = 'richTextOnly',
    richTextWithImage = 'richTextWithImage'
}

export const enum imageAlignment {
    left = 'left',
    right = 'right'
}

export interface IConfigVisibilityConfig extends Msdyn365.IModuleConfig {
    productTitle?: string;
    layout?: layout;
    subTitle?: string;
    featureText?: string;
    featureRichText?: Msdyn365.RichText;
    featureImage?: Msdyn365.IImageData;
    imageAlignment?: imageAlignment;
}

export interface IConfigVisibilityProps<T> extends Msdyn365.IModule<T> {
    config: IConfigVisibilityConfig;
}
