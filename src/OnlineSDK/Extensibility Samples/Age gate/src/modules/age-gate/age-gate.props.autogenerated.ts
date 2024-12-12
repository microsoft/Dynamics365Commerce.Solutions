/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IAgeGate containerModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';
import * as React from 'react';

export interface IAgeGateConfig extends Msdyn365.IModuleConfig {
    heading?: string;
    consentButtonText?: string;
    className?: string;
}

export interface IAgeGateResources {
    ageGateAriaLabel: string;
}

export interface IAgeGateProps<T> extends Msdyn365.IModule<T> {
    resources: IAgeGateResources;
    config: IAgeGateConfig;
    slots: {
        content: React.ReactNode[];
    };
}
