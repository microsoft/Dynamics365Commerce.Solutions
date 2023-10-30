/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { buildHydratedMockActionContext, buildMockRequest, ICoreContext, IRequestContext } from '@msdyn365-commerce/core';

const mockRequest: IRequestContext = buildMockRequest();
mockRequest.gridSettings = {
    xs: { w: 767, h: 0 },
    sm: { w: 991, h: 0 },
    md: { w: 1199, h: 0 },
    lg: { w: 1599, h: 0 },
    xl: { w: 1600, h: 0 }
};
mockRequest.user.isAuthenticated = true;
mockRequest.user.signInUrl = '/signin';
mockRequest.user.signOutUrl = '/signout';
mockRequest.user.signUpUrl = '/signup';

const mockAnonRequest = buildMockRequest();
mockAnonRequest.user.isAuthenticated = false;
mockAnonRequest.user.signInUrl = '/signin';
mockAnonRequest.user.signOutUrl = '/signout';
mockAnonRequest.user.signUpUrl = '/signup';

const mockActionContext = buildHydratedMockActionContext();

/**
 * Mocked resources shared for test cases.
 */
// @ts-expect-error partial mock
export const mockAuthContext: ICoreContext = {
    actionContext: mockActionContext,
    request: mockRequest,
    app: {
        config: {
            disableTooltip: true
        },
        routes: {
            wishlist: '/wishlist'
        }
    }
};

/**
 * Mocked resources shared for test cases.
 */
// @ts-expect-error partial mock
export const mockAnonContext: ICoreContext = {
    actionContext: mockActionContext,
    request: mockAnonRequest,
    app: {
        config: {
            disableTooltip: true
        },
        routes: {
            wishlist: '/wishlist'
        }
    }
};

/**
 * Mocked resources shared for test cases.
 */
export const mockHeaderConfig = {
    logoLink: {
        linkUrl: {
            destinationUrl: 'https://ppe.fabrikam.com/fe'
        },
        ariaLabel: 'fabrikam'
    },
    logoImage: {
        src: 'https://img-prod-cms-mr-microsoft-com.akamaized.net/cms/api/fabrikam/imageFileData/MA1G3L'
    }
};

/**
 * Mocked resources shared for test cases.
 */
export const mockResources = {
    mobileHamburgerAriaLabel: 'Mobile view hamburger aria label',
    wishlistTooltipText: 'My wishlist',
    cartLabel: 'Shopping bag, ({0}) items',
    cartQtyLabel: '({0})',
    signInLinkText: 'Sign In',
    signInLinkAriaText: 'Sign In',
    signOutLinkText: 'Sign Out',
    signOutLinkAriaText: 'Sign Out',
    headerPreferredStoreText: 'Select store',
    headerPreferredStoreAriaLabel: 'select preferred store',
    signUpCustomerNotFoundTitle: 'Error message if cutsomer not found',
    signUpMultipleCustomerFoundTitle: 'Multiple customers found error msg',
    closeNotificationLabel: 'Close',
    wishlistCountLabel: '{(0)}',
    shoppingAsText: 'Shopping',
    switchCustomerLinkText: 'Switch',
    switchCustomerLinkAriaText: 'Switch Aria Text',
    headerAriaLabel: 'Header',
    headerPreferredDistributorText: 'Preferred distributor',
    headerPreferredDistributorAriaLabel: 'Preferred distributor'
};
