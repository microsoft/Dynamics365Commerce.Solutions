/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IContentBlock contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export const enum actionableRegion {
    imageAndLinks = 'imageAndLinks',
    linksOnly = 'linksOnly'
}

export const enum textplacement {
    left = 'left',
    right = 'right',
    center = 'center'
}

export const enum texttheme {
    dark = 'dark',
    light = 'light'
}

export interface IContentBlockConfig extends Msdyn365.IModuleConfig {
    heading?: IHeadingData;
    paragraph?: Msdyn365.RichText;
    image?: Msdyn365.IImageData;
    links?: ILinksData[];
    additionalContent?: IAdditionalContentData[];
    actionableRegion?: actionableRegion;
    imageLink?: Msdyn365.ILinkData;
    imageAriaLabel?: string;
    className?: string;
    clientRender?: boolean;
    textplacement?: textplacement;
    texttheme?: texttheme;
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

export interface ILinksData {
    linkText?: string;
    linkUrl: Msdyn365.ILinkData;
    ariaLabel?: string;
    openInNewTab?: boolean;
}

export interface IAdditionalContentData {
    heading?: string;
    subtext?: string;
    links?: ILinksData[];
}

export interface ILinksData {
    linkText?: string;
    linkUrl: Msdyn365.ILinkData;
    ariaLabel?: string;
    openInNewTab?: boolean;
}

export interface IContentBlockProps<T> extends Msdyn365.IModule<T> {
    config: IContentBlockConfig;
}
