/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { getUrlSync, IComponent, IComponentProps, msdyn365Commerce } from '@msdyn365-commerce/core';
import {
    Button,
    getPayloadObject,
    getTelemetryAttributes,
    ITelemetryContent,
    onTelemetryClick,
    UncontrolledTooltip
} from '@msdyn365-commerce-modules/utilities';
import classname from 'classnames';
import * as React from 'react';

export interface IWishlistIconComponentProps extends IComponentProps<{}> {
    className?: string;
    wishlistTooltipText?: string;
    showButtonTooltip?: boolean;
    telemetryContent?: ITelemetryContent;
    wishlistCountLabel?: string;
    wishlistCount?: number;
    isDispayWishlistCount?: boolean;
}

export interface IWishlistIconComponent extends IComponent<IWishlistIconComponentProps> {}

const WishlistIconComponentActions = {};

/**
 * WishlistIcon component.
 * @param props
 * @extends {React.PureComponent<IWishlistIconProps>}
 */
const WishlistIcon: React.FC<IWishlistIconComponentProps> = (props: IWishlistIconComponentProps) => {
    const wishlistIconRef: React.RefObject<HTMLButtonElement> = React.createRef();
    const text = props.wishlistTooltipText;
    const showButtonIconTooltip = props.showButtonTooltip;
    const wishlistUrl = getUrlSync('wishlist', props.context.actionContext);
    const signInUrl = `${props.context.request.user.signInUrl}?ru=${wishlistUrl}`;
    const url = props.context.request.user.isAuthenticated ? wishlistUrl : signInUrl;
    const showTooltip = showButtonIconTooltip !== undefined ? showButtonIconTooltip : true;
    // const shouldShowCount = props.isDispayWishlistCount !== undefined ? props.isDispayWishlistCount : false;
    // const wishlistCountlbl = props.wishlistCountLabel !== undefined ? props.wishlistCountLabel : '';
    const wishlistItemCount = props.wishlistCount !== undefined ? props.wishlistCount : '';
    // const countLabel = format(wishlistCountlbl, wishlistItemCount);

    // Construct telemetry attribute to render
    const payLoad = getPayloadObject('click', props.telemetryContent!, text || '', '');
    const attributes = getTelemetryAttributes(props.telemetryContent!, payLoad);
    const formattedWishlistCount =
        props.className === 'ms-header__wishlist-mobile' ? `${text} ` + `(${wishlistItemCount})` : `(${wishlistItemCount})`;

    return (
        <>
            <Button
                className={classname('msc-wishlist-icon', props.className)}
                href={url}
                aria-label={text}
                innerRef={wishlistIconRef}
                {...attributes}
                onClick={onTelemetryClick(props.telemetryContent!, payLoad, text || '')}
            >
                {<span className='msc-wishlist-icon__textcount'>{formattedWishlistCount}</span>}
            </Button>
            {showTooltip && (
                <UncontrolledTooltip trigger='hover focus' target={wishlistIconRef}>
                    {text}
                </UncontrolledTooltip>
            )}
        </>
    );
};

export const WishListIconComponent: React.FunctionComponent<IWishlistIconComponentProps> = msdyn365Commerce.createComponentOverride<
    IWishlistIconComponent
>('WishListIcon', { component: WishlistIcon, ...WishlistIconComponentActions });

export default WishListIconComponent;
