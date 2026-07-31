/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IPromoBanner contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export const enum alignment {
    left = 'left',
    right = 'right',
    center = 'center'
}

export interface IPromoBannerConfig extends Msdyn365.IModuleConfig {
    bannerMessages?: IBannerMessagesData[];
    autoplay?: boolean;
    interval?: number;
    dismissEnabled?: boolean;
    hideFlipper?: boolean;
    className?: string;
    clientRender?: boolean;
    alignment?: alignment;
}

export interface IPromoBannerResources {
    closeButtonAriaLabel: string;
    closeButtonLabel: string;
    bannerText: string;
    bannerAriaLabel: string;
}

export interface IBannerMessagesData {
    text?: string;
    links?: ILinksData[];
}

export interface ILinksData {
    linkText?: string;
    linkUrl: Msdyn365.ILinkData;
    ariaLabel?: string;
    openInNewTab?: boolean;
}

export interface IPromoBannerProps<T> extends Msdyn365.IModule<T> {
    resources: IPromoBannerResources;
    config: IPromoBannerConfig;
}
