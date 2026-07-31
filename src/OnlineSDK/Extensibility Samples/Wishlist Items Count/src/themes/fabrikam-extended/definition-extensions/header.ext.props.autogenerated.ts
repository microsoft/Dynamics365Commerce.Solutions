/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IHeader containerModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';
import * as React from 'react';

export interface IHeaderConfig extends Msdyn365.IModuleConfig {
    logoImage: Msdyn365.IImageData;
    logoLink?: ILogoLinkData;
    myAccountLinks?: IMyAccountLinksData[];
    className?: string;
    signUpCustomerNotFound?: string;
    signUpMultipleCustomerFound?: string;
    shouldShowWishlistCount?: boolean;
    clientRender?: boolean;
}

export interface IHeaderResources {
    mobileHamburgerAriaLabel: string;
    wishlistTooltipText: string;
    cartLabel: string;
    cartQtyLabel: string;
    signInLinkText: string;
    signInLinkAriaText: string;
    signOutLinkText: string;
    signOutLinkAriaText: string;
    shoppingAsText: string;
    switchCustomerLinkText: string;
    switchCustomerLinkAriaText: string;
    headerPreferredStoreText: string;
    headerPreferredStoreAriaLabel: string;
    signUpCustomerNotFoundTitle: string;
    signUpMultipleCustomerFoundTitle: string;
    closeNotificationLabel: string;
    wishlistCountLabel: string;
    headerAriaLabel: string;
}

export interface ILogoLinkData {
    linkText?: string;
    linkUrl: Msdyn365.ILinkData;
    ariaLabel?: string;
    openInNewTab?: boolean;
}

export interface IMyAccountLinksData {
    linkText?: string;
    linkUrl: Msdyn365.ILinkData;
    ariaLabel?: string;
    openInNewTab?: boolean;
}

export interface IHeaderProps<T> extends Msdyn365.IModule<T> {
    resources: IHeaderResources;
    config: IHeaderConfig;
    slots: {
        menuBar: React.ReactNode[];
        search: React.ReactNode[];
        cartIcon: React.ReactNode[];
        siteOptions: React.ReactNode[];
        storeSelector: React.ReactNode[];
    };
}
