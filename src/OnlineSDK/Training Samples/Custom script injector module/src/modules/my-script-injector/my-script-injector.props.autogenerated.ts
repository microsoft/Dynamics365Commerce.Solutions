/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IMyScriptInjector scriptModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export const enum crossorigin {
    anonymous = 'anonymous',
    useCredentials = 'useCredentials'
}

export interface IMyScriptInjectorConfig {
    inlineScript: string;
    async?: boolean;
    defer?: boolean;
    crossorigin?: crossorigin;
}

export interface IMyScriptInjectorProps<T> extends Msdyn365.IModule<T> {
    config: IMyScriptInjectorConfig;
}
