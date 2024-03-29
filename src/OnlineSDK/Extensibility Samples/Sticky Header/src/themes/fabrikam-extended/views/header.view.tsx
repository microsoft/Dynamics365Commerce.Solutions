/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { IHeaderViewProps } from '@msdyn365-commerce-modules/header';
import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';
import { IHeaderProps as IHeaderExtentionProps } from '../definition-extensions/header.ext.props.autogenerated';

const headerView: React.FC<IHeaderViewProps & IHeaderExtentionProps<{}>> = (props: IHeaderViewProps & IHeaderExtentionProps<{}>) => {
    const { useStickyHeader } = props.config;
    React.useEffect(() => {
        if (useStickyHeader) {
            _stickyHeaderSetup();
        }
    }, [props]);

    const {
        HeaderTag,
        HeaderContainer,
        HeaderTopBarContainer,
        MobileMenuContainer,
        MobileMenuBodyContainer,
        MobileMenuLinksContainer,
        Divider
    } = props;
    return (
        <Module {...HeaderTag}>
            <Node {...HeaderContainer}>
                <Node {...HeaderTopBarContainer}>
                    {props.navIcon}
                    {props.logo}
                    {_renderReactFragment(props.search)}
                    {_renderDesktopAccountBlock(props)}
                    {props.wishListIconDesktop}
                    <Node {...Divider} />
                    {props.cartIcon}
                </Node>
                <Node {...MobileMenuContainer}>
                    <Node {...MobileMenuBodyContainer}>
                        {props.MobileMenuHeader}
                        {_renderReactFragment(props.menuBar)}
                        <Node {...MobileMenuLinksContainer}>
                            {props.accountLinks ? props.accountLinks.map(link => link) : false}
                            {props.wishListIconMobile}
                            {props.signInLink}
                            {props.signOutLink}
                        </Node>
                    </Node>
                </Node>
                {_renderReactFragment(props.menuBar)}
            </Node>
        </Module>
    );
};

/* Sticky header logic*/
function _stickyHeaderSetup(): void {
    // Set header update methods to trigger on scroll
    window.addEventListener('scroll', () => {
        _updateHeader();
    });
    _updateHeader();
}

/* Update header*/
function _updateHeader(): void {
    // Get heights of cookie and promotion banners
    const defaultValue = 0;
    const headerAlertsContainer: HTMLElement | null = document.querySelector('.ms-promo-banner');
    const bannerHeights = headerAlertsContainer ? headerAlertsContainer.offsetHeight : defaultValue;

    // Triggers opacity change of header
    const headerElement = document.querySelector('.ms-header');
    if (headerElement) {
        if (document.documentElement.scrollTop > bannerHeights) {
            headerElement.classList.add('lock-opaque');
        } else {
            headerElement.classList.remove('lock-opaque');
        }
    }

    // Update sticky header position and opacity
    const stickyHeader: HTMLElement | null = document.querySelector('.ms-header__desktop-view');
    const headerLogo: HTMLElement | null = document.querySelector('.ms-header__logo');

    if (stickyHeader && headerLogo) {
        // Fix center sticky header
        const navStickyPos = headerLogo.offsetHeight + bannerHeights;
        if (document.documentElement.scrollTop > navStickyPos) {
            stickyHeader.classList.add('fixed');
        } else {
            stickyHeader.classList.remove('fixed');
        }
    }
}
function _renderDesktopAccountBlock(props: IHeaderViewProps): JSX.Element | null {
    const {
        AccountInfoDropdownParentContainer,
        AccountInfoDropdownPopoverConentContainer,
        accountInfoDropdownButton,
        signOutLink,
        signInLink,
        accountLinks
    } = props;

    if (AccountInfoDropdownParentContainer) {
        if (AccountInfoDropdownPopoverConentContainer) {
            return (
                <Node {...AccountInfoDropdownParentContainer}>
                    {accountInfoDropdownButton}
                    <Node {...AccountInfoDropdownPopoverConentContainer}>
                        {accountLinks ? accountLinks.map(link => link) : false}
                        {signOutLink}
                    </Node>
                </Node>
            );
        } else if (signInLink) {
            return <Node {...AccountInfoDropdownParentContainer}>{signInLink}</Node>;
        }
    }

    return null;
}

function _renderReactFragment(items: React.ReactNode[]): JSX.Element | null {
    return (
        <React.Fragment>
            {items && items.length
                ? items.map((slot: React.ReactNode, index: number) => {
                      return <React.Fragment key={index}>{slot}</React.Fragment>;
                  })
                : null}
        </React.Fragment>
    );
}

export default headerView;
