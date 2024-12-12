/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IReadAppSettings contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface IReadAppSettingsConfig extends Msdyn365.IModuleConfig {
    showText?: string;
}

export interface IReadAppSettingsResources {
    resourceKey: string;
}

export interface IReadAppSettingsProps<T> extends Msdyn365.IModule<T> {
    resources: IReadAppSettingsResources;
    config: IReadAppSettingsConfig;
}
