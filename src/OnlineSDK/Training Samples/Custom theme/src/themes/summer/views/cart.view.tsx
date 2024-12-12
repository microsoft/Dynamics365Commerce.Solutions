/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import MsDyn365 from '@msdyn365-commerce/core';
import { ICartlinesViewProps, ICartResources, ICartViewProps, IOrderSummaryErrors } from '@msdyn365-commerce-modules/cart';
import { ICartLineItemViewProps } from '@msdyn365-commerce-modules/cart/dist/types/modules/cart/components/cart-line-items-promotion';
import { IInvoiceSummaryLines } from '@msdyn365-commerce-modules/invoice-payment-summary';
import { IOrderSummaryLines } from '@msdyn365-commerce-modules/order-summary-utilities';
import { ArrayExtensions } from '@msdyn365-commerce-modules/retail-actions';
import {
    Button,
    getPayloadObject,
    getTelemetryAttributes,
    INodeProps,
    ITelemetryContent,
    Node,
    TelemetryConstant
} from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

const CartView: React.FC<ICartViewProps> = (props: ICartViewProps) => (
    <div className={props.className} id={props.id} {...props.renderModuleAttributes(props)}>
        {props.title}
        {props.promotionOptions && _renderPromotions(props.promotionOptions)}
        {props.multiplePickUpEnabled ? (
            _renderCartLinesGroup(props, props.resources)
        ) : (
            <Node {...props.CartlinesWrapper}>
                {_renderCartlines(
                    props.cartlines,
                    props.resources,
                    props.storeSelector,
                    props.backToShoppingButton,
                    props.waitingComponent,
                    props.cartLoadingStatus,
                    props.cartDataResult,
                    props.telemetryContent,
                    props.multiplePickUpEnabled,
                    props.context.actionContext.requestContext.channel?.EmailDeliveryModeCode
                )}
            </Node>
        )}
        {props.orderSummaryHeading && (
            <Node {...props.OrderSummaryWrapper}>
                {props.orderSummaryHeading}
                {props.cart?.hasInvoiceLine
                    ? _renderInvoiceSummarylines(props.invoiceSummaryLineitems, props.OrderSummaryItems, props)
                    : _renderOrderSummarylines(props.orderSummaryLineitems, props.OrderSummaryItems, props)}
                {_renderErrorBlock(props.OrderSummaryErrors)}
                {props.checkoutAsSignInUserButton}
                {props.checkoutAsGuestButton}
                {props.expressCheckoutButton && ArrayExtensions.hasElements(props.expressCheckoutButton) ? (
                    <Node {...props.ExpressCheckoutSectionWrapper}>{props.expressCheckoutButton}</Node>
                ) : null}
                {props.backToShoppingButton}
                {props.createTemplateFromCartButton}
            </Node>
        )}
        {props.storeSelector}
    </div>
);

const _renderPromotions = (promotions: ICartLineItemViewProps): JSX.Element | undefined => {
    return (
        <>
            <Node {...promotions.promotionMessageWrapper}>
                {promotions.promotionMessage}
                {promotions.promotionSeeAllLink}
                {promotions.promotionDialog}
            </Node>
        </>
    );
};

const _renderCartlines = (
    cartLines: ICartlinesViewProps[] | undefined,
    resources: ICartResources,
    storeSelector: React.ReactNode | undefined,
    backToShoppingButton: React.ReactNode,
    waitingComponent: React.ReactNode,
    cartLoadingStatus: string,
    cartDataResult: boolean,
    telemetryContent?: ITelemetryContent,
    mulitplePickUp?: boolean,
    emailDeliveryModeCode?: string
): JSX.Element[] | JSX.Element => {
    const { emptyCartText } = resources;

    if (cartLoadingStatus) {
        return <>{cartLoadingStatus}</>;
    }
    if (cartLines) {
        return cartLines.map((cartLine, index) => {
            return (
                <div className='msc-cart-lines-item' key={`${index}-${MsDyn365.isBrowser.toString()}`}>
                    {cartLine.cartline}
                    {mulitplePickUp ? null : _renderBOPISBlock(cartLine, resources, storeSelector, telemetryContent, emailDeliveryModeCode)}
                    {mulitplePickUp ? (
                        <Node className='msc-cart-line-group__extra-actions'>
                            {_renderGroupBOPISBlock(cartLine, resources, storeSelector, telemetryContent, emailDeliveryModeCode)}
                            {cartLine.addToOrderTemplate}
                            {cartLine.addToWishlist}
                            {cartLine.remove}
                        </Node>
                    ) : cartLine.addToOrderTemplate ? (
                        <Node className='msc-cart-line__extra-actions'>
                            {cartLine.addToOrderTemplate}
                            {cartLine.addToWishlist}
                            {cartLine.remove}
                        </Node>
                    ) : (
                        <>
                            {cartLine.addToWishlist}
                            {cartLine.remove}
                        </>
                    )}
                </div>
            );
        });
    }
    return cartDataResult ? (
        <div className='msc-cart__empty-cart'>
            <p className='msc-cart-line'>{emptyCartText}</p>
            {backToShoppingButton}
        </div>
    ) : (
        <>{waitingComponent}</>
    );
};

/**
 * On Toggle function.
 * @param cartLine -Cartline.
 * @param isBopisSelected -Boolean.
 * @returns Set state of button.
 */
const onToggleBopisHandler = (cartLine: ICartlinesViewProps, isBopisSelected: boolean) => () => {
    cartLine.pickUpInStore && cartLine.pickUpInStore.callbacks.toggleBopis(!isBopisSelected);
};

/**
 * On change store function.
 * @param cartLine -CartLine items prop.
 * @returns Set state of button.
 */
const onChangeStoreHandler = (cartLine: ICartlinesViewProps) => () => {
    cartLine.pickUpInStore && cartLine.pickUpInStore.callbacks.toggleBopis(true);
};

const _renderBOPISBlock = (
    cartLine: ICartlinesViewProps,
    resources: ICartResources,
    storeSelector: React.ReactNode | undefined,
    telemetryContent?: ITelemetryContent,
    emailDeliveryModeCode?: string
): JSX.Element | null => {
    // If it is electronic item cart line, then return null for BOPISBlock.
    if (!cartLine.pickUpInStore || !storeSelector || cartLine.data?.cartline.DeliveryMode === emailDeliveryModeCode) {
        return null;
    }

    const {
        shipInsteadDisplayText,
        shipToAddressDisplayText,
        pickItUpDisplayText,
        pickUpAtStoreWithLocationText,
        changeStoreDisplayText
    } = resources;

    const isBopisSelected = cartLine.pickUpInStore.isBopisSelected;

    const payLoad = getPayloadObject('click', telemetryContent!, TelemetryConstant.PickupInStore);
    const puckUpinStoreAttribute = getTelemetryAttributes(telemetryContent!, payLoad);

    return (
        <Node {...cartLine.pickUpInStore.ContainerProps}>
            <div className='msc-cart-line__bopis-method'>
                {isBopisSelected ? (
                    <span className='pick-up'>{pickUpAtStoreWithLocationText}</span>
                ) : (
                    <span className='ship'>{shipToAddressDisplayText}</span>
                )}
            </div>
            {isBopisSelected && (
                <div className='msc-cart-line__bopis-fullfilment'>
                    <span className='msc-cart-line__bopis-fullfilment-store'>{cartLine.pickUpInStore.orgUnitName}</span>
                    <Button role='link' className='msc-cart-line__bopis-changestore' onClick={onChangeStoreHandler(cartLine)}>
                        {changeStoreDisplayText}
                    </Button>
                </div>
            )}
            <Button
                className='msc-cart-line__bopis-btn'
                {...puckUpinStoreAttribute}
                onClick={onToggleBopisHandler(cartLine, isBopisSelected)}
            >
                {isBopisSelected ? shipInsteadDisplayText : pickItUpDisplayText}
            </Button>
        </Node>
    );
};

const _renderErrorBlock = (errorData: IOrderSummaryErrors | undefined): JSX.Element | null => {
    if (!errorData || errorData.errors.length === 0) {
        return null;
    }
    return (
        <Node {...errorData.Wrapper}>
            {errorData.header}
            {errorData.errors}
        </Node>
    );
};

const _renderOrderSummarylines = (
    orderSummaryLines: IOrderSummaryLines | undefined,
    OrderSummaryItems: INodeProps,
    props: ICartViewProps
): JSX.Element | null => {
    if (!orderSummaryLines) {
        return null;
    }
    return (
        <Node {...OrderSummaryItems}>
            {props.promoCode}
            {orderSummaryLines.subtotal}
            {orderSummaryLines.shipping}
            {orderSummaryLines.otherCharge}
            {orderSummaryLines.tax}
            {orderSummaryLines.totalDiscounts ? orderSummaryLines.totalDiscounts : null}
            {orderSummaryLines.orderTotal}
        </Node>
    );
};

const _renderInvoiceSummarylines = (
    invoiceSummaryLines: IInvoiceSummaryLines | undefined,
    OrderSummaryItems: INodeProps,
    props: ICartViewProps
): JSX.Element | null => {
    if (!invoiceSummaryLines) {
        props.context.telemetry.error('InvoiceSummary content is empty, module wont render');
        return null;
    }
    return (
        <Node {...OrderSummaryItems}>
            {invoiceSummaryLines.invoices}
            {invoiceSummaryLines.giftCard}
            {invoiceSummaryLines.loyalty}
            {invoiceSummaryLines.orderTotal}
        </Node>
    );
};

const _renderCartLinesGroup = (props: ICartViewProps, resources: ICartResources): JSX.Element | undefined => {
    if (props.cartLinesGroup && props.cartLinesGroup.length > 0) {
        return (
            <div className='msc-cart-lines-group'>
                {props.cartLinesGroup.map(cartlines => {
                    return (
                        // eslint-disable-next-line react/jsx-key
                        <div className='msc-cart-lines-group-wraper'>
                            {_renderCartLinesGroupHeader(
                                cartlines[0],
                                props.storeSelector,
                                resources,
                                _countItems(cartlines),
                                props.context.actionContext.requestContext.channel?.EmailDeliveryModeCode
                            )}
                            {_renderCartlines(
                                cartlines,
                                props.resources,
                                props.storeSelector,
                                props.backToShoppingButton,
                                props.waitingComponent,
                                props.cartLoadingStatus,
                                props.cartDataResult,
                                props.telemetryContent,
                                props.multiplePickUpEnabled,
                                props.context.actionContext.requestContext.channel?.EmailDeliveryModeCode
                            )}
                        </div>
                    );
                })}
            </div>
        );
    }
    return props.cartDataResult ? (
        <div className='msc-cartline-wraper'>
            <div className='msc-cart__empty-cart'>
                <p className='msc-cart-line'>{props.resources.emptyCartText}</p>
                {props.backToShoppingButton}
            </div>
        </div>
    ) : (
        <>{props.waitingComponent}</>
    );
};
const _renderCartLinesGroupHeader = (
    cartLine: ICartlinesViewProps,
    storeSelector: React.ReactNode | undefined,
    resources: ICartResources,
    count: number,
    emailDeliveryModeCode?: string
) => {
    const isBopisSelected = cartLine.pickUpInStore?.isBopisSelected;
    const { pickUpText, shippingText, emailshippingText } = resources;
    let groupTitle: string;

    groupTitle = isBopisSelected ? pickUpText : shippingText;
    groupTitle = cartLine.data && cartLine.data.cartline.DeliveryMode === emailDeliveryModeCode ? emailshippingText : groupTitle;

    return (
        <>
            <div className='msc-cart-lines-group-wraper__bopis-heading'>
                <p className={`msc-cart-lines-group-wraper__bopis-heading-${groupTitle.toLowerCase()}-icon`} />
                <p className={`msc-cart-lines-group-wraper__bopis-heading-${groupTitle.toLowerCase()}`}>{groupTitle}</p>
                {_renderCartLinesGroupTitle(cartLine, resources, count)}
            </div>
        </>
    );
};

const _renderGroupBOPISBlock = (
    cartLine: ICartlinesViewProps,
    resources: ICartResources,
    storeSelector: React.ReactNode | undefined,
    telemetryContent?: ITelemetryContent,
    emailDeliveryModeCode?: string
): JSX.Element | null => {
    // If it is electronic item cart line, then return null for GroupBOPISBlock.
    if (!cartLine.pickUpInStore || !storeSelector || cartLine.data?.cartline.DeliveryMode === emailDeliveryModeCode) {
        return null;
    }

    const { changeStoreDisplayText, shipInsteadDisplayText, pickItUpDisplayText } = resources;

    const isBopisSelected = cartLine.pickUpInStore.isBopisSelected;

    const payLoad = getPayloadObject('click', telemetryContent!, TelemetryConstant.PickupInStore);
    const puckUpinStoreAttribute = getTelemetryAttributes(telemetryContent!, payLoad);

    return (
        <>
            {isBopisSelected ? (
                <Button className='msc-cart-line__remove-item msc-btn' onClick={onChangeStoreHandler(cartLine)} aria-live='polite'>
                    {changeStoreDisplayText}
                </Button>
            ) : null}
            <Button
                className='msc-cart-line__remove-item msc-btn'
                {...puckUpinStoreAttribute}
                onClick={onToggleBopisHandler(cartLine, isBopisSelected)}
            >
                {isBopisSelected ? shipInsteadDisplayText : pickItUpDisplayText}
            </Button>
        </>
    );
};

const _countItems = (cartLine: ICartlinesViewProps[]): number => {
    let countItem = 0;
    countItem = cartLine.reduce((count, item) => {
        return count + (item.data?.cartline.Quantity || 0);
    }, 0);
    return countItem;
};

const _renderCartLinesGroupTitle = (cartLine: ICartlinesViewProps, resources: ICartResources, count: number) => {
    const isBopisSelected = cartLine.pickUpInStore?.isBopisSelected;
    const { itemLabel, itemsLabel } = resources;
    const suffix = count > 1 ? itemsLabel : itemLabel;
    return (
        <>
            <Node className='msc-cart-lines-group-wraper__bopis-heading-title'>
                {isBopisSelected ? (
                    <p className='msc-cart-lines-group-wraper__bopis-heading-title-st'>
                        {cartLine.pickUpInStore?.deliveryOption}
                        {', '}
                        {cartLine.pickUpInStore?.orgUnitName}
                    </p>
                ) : null}
                <p className='msc-cart-lines-group-wraper__bopis-heading-title-ct'>
                    ({count} {suffix})
                </p>
            </Node>
        </>
    );
};

export default CartView;
