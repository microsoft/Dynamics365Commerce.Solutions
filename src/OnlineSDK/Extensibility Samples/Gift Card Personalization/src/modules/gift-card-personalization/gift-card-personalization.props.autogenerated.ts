/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IGiftCardPersonalization contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

// eslint-disable-next-line @typescript-eslint/no-empty-interface
export interface IGiftCardPersonalizationConfig extends Msdyn365.IModuleConfig {}

export interface IGiftCardPersonalizationResources {}

export interface IGiftCardPersonalizationProps<T> extends Msdyn365.IModule<T> {
    resources: IGiftCardPersonalizationResources;
    config: IGiftCardPersonalizationConfig;
}
