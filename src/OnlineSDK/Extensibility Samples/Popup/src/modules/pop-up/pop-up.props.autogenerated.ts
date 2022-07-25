/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IPopUp containerModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';
import * as React from 'react';

export interface IPopUpConfig extends Msdyn365.IModuleConfig {
    heading?: string;
    className?: string;
    isBackDropStatic?: boolean;
    isCookieEnabled?: boolean;
    shouldPopUpOnLoad?: boolean;
}

export interface IPopUpResources {
    popUpAriaLabel: string;
}

export interface IPopUpProps<T> extends Msdyn365.IModule<T> {
    resources: IPopUpResources;
    config: IPopUpConfig;
    slots: {
        content: React.ReactNode[];
    };
}
