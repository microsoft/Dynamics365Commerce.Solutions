/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * ISampleCurrentConditions contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface ISampleCurrentConditionsConfig extends Msdyn365.IModuleConfig {
    apiKey?: string;
}

export interface ISampleCurrentConditionsResources {
    cardHeader: string;
}

export interface ISampleCurrentConditionsProps<T> extends Msdyn365.IModule<T> {
    resources: ISampleCurrentConditionsResources;
    config: ISampleCurrentConditionsConfig;
}
