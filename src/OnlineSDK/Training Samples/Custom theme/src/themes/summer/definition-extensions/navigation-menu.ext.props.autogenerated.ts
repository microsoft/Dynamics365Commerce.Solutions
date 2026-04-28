/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * INavigationMenu contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export const enum navigationMenuSource {
    all = 'all',
    retailServerOnly = 'retailServerOnly',
    cmsOnly = 'cmsOnly'
}

export interface INavigationMenuConfig extends Msdyn365.IModuleConfig {
    navigationMenuSource?: navigationMenuSource;
    cmsNavItems?: ICmsNavItemsData[];
    enableMultilevelMenu?: boolean;
    enabletopMenu?: boolean;
    menuLevelSupport?: number;
    rootMenuNavigation?: string;
    displayCategoryImage?: boolean;
    displayPromotionalImage?: boolean;
    categoryPromotionalContent?: ICategoryPromotionalContentData[];
    categoryImageSettings?: Msdyn365.IImageSettings;
    className?: string;
    clientRender?: boolean;
}

export interface INavigationMenuResources {
    menuAriaLabel: string;
    backButtonAriaLabel: string;
    allCategoryMenuText: string;
}

export interface ICmsNavItemsData {
    linkText?: string;
    linkUrl?: Msdyn365.ILinkData;
    image?: Msdyn365.IImageData;
    imageLink?: Msdyn365.ILinkData;
    ariaLabel?: string;
    openInNewTab?: boolean;
    subMenus?: ISubMenusData[];
}

export interface ICategoryPromotionalContentData {
    categoryName?: string;
    image: Msdyn365.IImageData;
    text?: string;
    linkUrl: Msdyn365.ILinkData;
}

export interface ISubMenusData {
    linkText?: string;
    linkUrl: Msdyn365.ILinkData;
    ariaLabel?: string;
    image?: Msdyn365.IImageData;
    imageLink?: Msdyn365.ILinkData;
    subMenus?: ISubMenusData[];
}

export interface ISubMenusData {
    linkText?: string;
    linkUrl: Msdyn365.ILinkData;
    ariaLabel?: string;
    image?: Msdyn365.IImageData;
    imageLink?: Msdyn365.ILinkData;
}

export interface INavigationMenuProps<T> extends Msdyn365.IModule<T> {
    resources: INavigationMenuResources;
    config: INavigationMenuConfig;
}
