/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IApiTest contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface IApiTestConfig extends Msdyn365.IModuleConfig {
    showText?: string;
}

export interface IApiTestResources {
    resourceKey: string;
}

export interface IApiTestProps<T> extends Msdyn365.IModule<T> {
    resources: IApiTestResources;
    config: IApiTestConfig;
}
