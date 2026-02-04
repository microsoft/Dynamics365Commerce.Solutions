/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IAzureMaps contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface IAzureMapsConfig extends Msdyn365.IModuleConfig {
    heading?: IHeadingData;
    pushpinOptions?: IPushpinOptionsData;
    className?: string;
    clientRender?: boolean;
}

export interface IAzureMapsResources {
    resourceKey: string;
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

export interface IPushpinOptionsData {
    size?: number;
    color?: string;
    selectionColor?: string;
    showIndex?: boolean;
}

export interface IAzureMapsProps<T> extends Msdyn365.IModule<T> {
    resources: IAzureMapsResources;
    config: IAzureMapsConfig;
}
