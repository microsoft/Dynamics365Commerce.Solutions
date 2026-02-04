/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * ITextBlock contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export const enum alignment {
    left = 'left',
    right = 'right',
    center = 'center'
}

export const enum fontsize {
    small = 'small',
    medium = 'medium',
    large = 'large',
    extralarge = 'extralarge'
}

export interface ITextBlockConfig extends Msdyn365.IModuleConfig {
    paragraph?: Msdyn365.RichText;
    className?: string;
    clientRender?: boolean;
    alignment?: alignment;
    fontsize?: fontsize;
}

export interface ITextBlockProps<T> extends Msdyn365.IModule<T> {
    config: ITextBlockConfig;
}
