/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * ICookieTest contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface ICookieTestConfig extends Msdyn365.IModuleConfig {
    showText?: string;
}

export interface ICookieTestResources {
    resourceKey: string;
}

export interface ICookieTestProps<T> extends Msdyn365.IModule<T> {
    resources: ICookieTestResources;
    config: ICookieTestConfig;
}
