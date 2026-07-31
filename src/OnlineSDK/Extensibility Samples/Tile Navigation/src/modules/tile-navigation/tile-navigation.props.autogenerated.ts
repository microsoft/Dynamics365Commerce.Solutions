/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * ITileNavigation contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface ITileNavigationConfig extends Msdyn365.IModuleConfig {
    categoryId?: number;
    imageSettings?: Msdyn365.IImageSettings;
}

export interface ITileNavigationResources {
    noSubcategoryMsg: string;
}

export interface ITileNavigationProps<T> extends Msdyn365.IModule<T> {
    resources: ITileNavigationResources;
    config: ITileNavigationConfig;
}
