/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { buildMockModuleProps } from '@msdyn365-commerce/core';
import { IHeaderViewProps } from '@msdyn365-commerce-modules/header';
import { render } from 'enzyme';
import * as React from 'react';

import HeaderView from '../../views/header.view';
import * as MockUtillities from './utilities/mock-utilities';

const resources = {
    mobileHamburgerAriaLabel: '',
    wishlistTooltipText: '',
    cartLabel: '',
    cartQtyLabel: '',
    signInLinkText: '',
    signInLinkAriaText: '',
    signOutLinkText: '',
    signOutLinkAriaText: '',
    headerPreferredStoreText: '',
    headerPreferredStoreAriaLabel: '',
    signUpCustomerNotFoundTitle: '',
    signUpMultipleCustomerFoundTitle: '',
    closeNotificationLabel: '',
    wishlistCountLabel: '',
    shoppingAsText: '',
    switchCustomerLinkText: '',
    switchCustomerLinkAriaText: '',
    headerAriaLabel: ''
};
describe('Header view tests', () => {
    it('render correctly no sign in info', () => {
        const moduleProps = buildMockModuleProps({}, {}, MockUtillities.mockHeaderConfig);

        // @ts-expect-error
        const mockProps: IHeaderViewProps = {
            ...buildMockModuleProps(MockUtillities.mockResources, MockUtillities.mockAnonContext),
            ...moduleProps,
            logo: '{LogoComponent}',
            resources,
            wishListIconDesktop: '{WishlistIconDesktop}',
            wishListIconMobile: '{WishlistIconMobile}',
            cartIcon: '{CartIcon}',
            navIcon: '{NavIcon}',
            mobileMenuCollapsed: false,
            signinPopoverOpen: true,
            className: 'mock-class',
            menuBar: ['{NavigationMenu}'],
            search: ['{Search}'],
            siteOptions: ['{siteOptions}'],
            HeaderTag: { moduleProps, className: 'module-class-HeaderTag' },
            HeaderContainer: { className: 'module-class-HeaderContainer' },
            HeaderTopBarContainer: { className: 'module-class-HeaderTopBar' },
            MobileMenuContainer: { className: 'module-class-MobileMenu' },
            MobileMenuBodyContainer: { className: 'module-class-MobileMenuBody' },
            MobileMenuLinksContainer: { className: 'module-class-MobileMenuLinks' },
            MobileMenuHeader: 'mobile-menu-header',
            Divider: { className: 'module-class-Divider' }
        };

        const view = render(<HeaderView {...mockProps} />);
        expect(view).toMatchSnapshot();
    });

    it('render correctly no signed out', () => {
        const moduleProps = buildMockModuleProps({}, {}, MockUtillities.mockHeaderConfig);

        const mockProps: IHeaderViewProps = {
            ...moduleProps,
            logo: '{LogoComponent}',
            wishListIconDesktop: '{WishlistIconDesktop}',
            wishListIconMobile: '{WishlistIconMobile}',
            cartIcon: '{CartIcon}',
            navIcon: '{NavIcon}',
            resources: MockUtillities.mockResources,
            context: MockUtillities.mockAnonContext,
            mobileMenuCollapsed: false,
            signinPopoverOpen: true,
            className: 'mock-class',
            menuBar: ['{NavigationMenu}'],
            search: ['{Search}'],
            data: {
                // @ts-expect-error
                accountInformation: {
                    result: {
                        AccountNumber: '1'
                    }
                }
            },
            siteOptions: ['{SiteOptions}'],
            MobileMenuContainer: { className: 'module-class-MobileMenu' },
            MobileMenuBodyContainer: { className: 'module-class-MobileMenuBody' },
            MobileMenuLinksContainer: { className: 'module-class-MobileMenuLinks' },
            MobileMenuHeader: 'mobile-menu-header',
            HeaderTag: { moduleProps, className: 'module-class-HeaderTag' },
            HeaderContainer: { className: 'module-class-HeaderContainer' },
            HeaderTopBarContainer: { className: 'module-class-HeaderTopBar' },
            AccountInfoDropdownParentContainer: { className: 'module-class-AccountInfoDropdownParent' },
            signInLink: '{SignIn}',
            Divider: { className: 'module-class-Divider' }
        };

        const view = render(<HeaderView {...mockProps} />);
        expect(view).toMatchSnapshot();
    });

    it('render correctly signed in', () => {
        const moduleProps = buildMockModuleProps({}, {}, MockUtillities.mockHeaderConfig);

        const mockProps: IHeaderViewProps = {
            ...moduleProps,
            logo: '{LogoComponent}',
            wishListIconDesktop: '{WishlistIconDesktop}',
            wishListIconMobile: '{WishlistIconMobile}',
            cartIcon: '{CartIcon}',
            navIcon: '{NavIcon}',
            resources: MockUtillities.mockResources,
            context: MockUtillities.mockAnonContext,
            mobileMenuCollapsed: false,
            data: {
                // @ts-expect-error
                accountInformation: {
                    result: {
                        AccountNumber: '1',
                        FirstName: 'First Name'
                    }
                }
            },
            signinPopoverOpen: true,
            className: 'mock-class',
            menuBar: ['{NavigationMenu}'],
            search: ['{Search}'],
            siteOptions: ['{SiteOptions}'],
            MobileMenuContainer: { className: 'module-class-MobileMenu' },
            MobileMenuBodyContainer: { className: 'module-class-MobileMenuBody' },
            MobileMenuLinksContainer: { className: 'module-class-MobileMenuLinks' },
            MobileMenuHeader: 'mobile-menu-header',
            HeaderTag: { moduleProps, className: 'module-class-HeaderTag' },
            HeaderContainer: { className: 'module-class-HeaderContainer' },
            HeaderTopBarContainer: { className: 'module-class-HeaderTopBar' },
            AccountInfoDropdownParentContainer: { className: 'module-class-AccountInfoDropdownParent' },
            AccountInfoDropdownPopoverConentContainer: { className: 'module-class-AccountInfoDropdownPopoverConent' },
            accountInfoDropdownButton: '{accountInfoDropdownButton}',
            signOutLink: '{SignOut}',
            accountLinks: ['{accountLink1}', '{accountLink2}', '{accountLink3}'],
            Divider: { className: 'module-class-Divider' }
        };

        const view = render(<HeaderView {...mockProps} />);
        expect(view).toMatchSnapshot();
    });
    it('render correctly signed in with parameter', () => {
        const moduleProps = buildMockModuleProps({}, {}, MockUtillities.mockHeaderConfig);

        const mockProps: IHeaderViewProps = {
            ...moduleProps,
            logo: '{LogoComponent}',
            wishListIconDesktop: '{WishlistIconDesktop}',
            wishListIconMobile: '{WishlistIconMobile}',
            cartIcon: '{CartIcon}',
            navIcon: '{NavIcon}',
            resources: MockUtillities.mockResources,
            context: MockUtillities.mockAnonContext,
            mobileMenuCollapsed: false,
            data: {
                // @ts-expect-error
                accountInformation: {
                    result: {
                        AccountNumber: '1'
                    }
                }
            },
            signinPopoverOpen: true,
            className: 'mock-class',
            menuBar: ['{NavigationMenu}'],
            search: ['{Search}'],
            siteOptions: ['{SiteOptions}'],
            MobileMenuContainer: { className: 'module-class-MobileMenu' },
            MobileMenuBodyContainer: { className: 'module-class-MobileMenuBody' },
            MobileMenuLinksContainer: { className: 'module-class-MobileMenuLinks' },
            MobileMenuHeader: 'mobile-menu-header',
            HeaderTag: { moduleProps, className: 'module-class-HeaderTag' },
            HeaderContainer: { className: 'module-class-HeaderContainer' },
            HeaderTopBarContainer: { className: 'module-class-HeaderTopBar' },
            AccountInfoDropdownParentContainer: { className: 'module-class-AccountInfoDropdownParent' },
            AccountInfoDropdownPopoverConentContainer: { className: 'module-class-AccountInfoDropdownPopoverConent' },
            accountInfoDropdownButton: '{accountInfoDropdownButton}',
            signOutLink: '{SignOut}',
            accountLinks: ['{accountLink1}', '{accountLink2}', '{accountLink3}'],
            Divider: { className: 'module-class-Divider' }
        };

        const view = render(<HeaderView {...mockProps} />);
        expect(view).toMatchSnapshot();
    });
    it('render correctly signed in with with customer', () => {
        const moduleProps = buildMockModuleProps({}, {}, MockUtillities.mockHeaderConfig);

        const mockProps: IHeaderViewProps = {
            ...moduleProps,
            logo: '{LogoComponent}',
            wishListIconDesktop: '{WishlistIconDesktop}',
            wishListIconMobile: '{WishlistIconMobile}',
            cartIcon: '{CartIcon}',
            navIcon: '{NavIcon}',
            resources: MockUtillities.mockResources,
            context: MockUtillities.mockAnonContext,
            mobileMenuCollapsed: false,
            data: {
                // @ts-expect-error
                accountInformation: undefined
            },
            signinPopoverOpen: true,
            className: 'mock-class',
            menuBar: ['{NavigationMenu}'],
            search: [],
            siteOptions: ['{SiteOptions}'],
            MobileMenuContainer: { className: 'module-class-MobileMenu' },
            MobileMenuBodyContainer: { className: 'module-class-MobileMenuBody' },
            MobileMenuLinksContainer: { className: 'module-class-MobileMenuLinks' },
            MobileMenuHeader: 'mobile-menu-header',
            HeaderTag: { moduleProps, className: 'module-class-HeaderTag' },
            HeaderContainer: { className: 'module-class-HeaderContainer' },
            HeaderTopBarContainer: { className: 'module-class-HeaderTopBar' },
            AccountInfoDropdownParentContainer: { className: 'module-class-AccountInfoDropdownParent' },
            AccountInfoDropdownPopoverConentContainer: { className: 'module-class-AccountInfoDropdownPopoverConent' },
            accountInfoDropdownButton: '{accountInfoDropdownButton}',
            signOutLink: '{SignOut}',
            accountLinks: undefined,
            Divider: { className: 'module-class-Divider' }
        };

        const view = render(<HeaderView {...mockProps} />);
        expect(view).toMatchSnapshot();
    });
});
