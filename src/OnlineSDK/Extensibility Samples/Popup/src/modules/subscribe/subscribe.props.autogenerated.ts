/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * ISubscribe contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface ISubscribeConfig extends Msdyn365.IModuleConfig {
    heading?: IHeadingData;
    paragraph?: Msdyn365.RichText;
    className?: string;
}

export interface ISubscribeResources {
    subscribeEmailPlaceholderText: string;
    subscribeButtonLabel: string;
    subscribeFormAriaLabel: string;
    subscribeFormEmailAriaLabel: string;
    subscribeFormSubmitAriaLabel: string;
}

export const enum HeadingTag {
    h1 = 'h1',
    h2 = 'h2',
    h3 = 'h3',
    h4 = 'h4',
    h5 = 'h5',
    h6 = 'h6'
}

export interface IHeadingData {
    text: string;
    tag?: HeadingTag;
}

export interface ISubscribeProps<T> extends Msdyn365.IModule<T> {
    resources: ISubscribeResources;
    config: ISubscribeConfig;
}
