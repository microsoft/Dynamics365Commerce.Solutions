/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IShowCategoryProducts contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface IShowCategoryProductsConfig extends Msdyn365.IModuleConfig {
    categoryId: number;
    itemsPerPage: number;
}

export interface IShowCategoryProductsProps<T> extends Msdyn365.IModule<T> {
    config: IShowCategoryProductsConfig;
}
