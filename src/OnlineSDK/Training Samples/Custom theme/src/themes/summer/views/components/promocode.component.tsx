/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { PriceComponent } from '@msdyn365-commerce/components';
import { IComponentProps } from '@msdyn365-commerce/core';
import { ICartState } from '@msdyn365-commerce/global-state';
import { Coupon } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import {
    Button,
    format,
    getPayloadObject,
    getTelemetryAttributes,
    IPayLoad,
    ITelemetryContent,
    TelemetryConstant
} from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

export interface IPromoCodeProps extends IComponentProps<{}> {
    cart: ICartState | undefined;
    promoCodeHeadingText: string;
    appliedPromoCodeHeadingText: string;
    removePromoAriaLabelFormat: string;
    promoPlaceholderText: string;
    promoCodeApplyButtonText: string;
    collapseTimeOut: number;
    removePromoText: string;
    invalidPromoCodeErrorText: string;
    failedToAddPromoCodeErrorText: string;
    duplicatePromoCodeErrorText: string;
    failedToRemovePromoCodeErrorText: string;

    /**
     * The telemetry content
     */
    telemetryContent?: ITelemetryContent;
    promoCodeApplyCallback?(): void;
}

interface IPromoCodeState {
    isCollapseOpen: boolean;
    promoCodeInputValue: string;
    error: string;
    canApply: boolean;
}

/**
 *
 * The PromoCode component renders the promocode section.
 * @extends {React.PureComponent<IRefineSubmenuProps>}
 */
class PromoCode extends React.Component<IPromoCodeProps, IPromoCodeState> {
    private readonly payLoad: IPayLoad;

    constructor(props: IPromoCodeProps, state: IPromoCodeState) {
        super(props);
        this.payLoad = getPayloadObject('click', this.props.telemetryContent!, TelemetryConstant.ApplyPromoCode);
        this.state = {
            isCollapseOpen: false,
            promoCodeInputValue: '',
            error: '',
            canApply: false
        };
    }

    public shouldComponentUpdate(nextProps: IPromoCodeProps, nextState: IPromoCodeState): boolean {
        if (this.state === nextState && this.props.data === nextProps.data) {
            return false;
        }
        return true;
    }

    public render(): JSX.Element {
        return (
            <div>
                <div className='msc-promo-code-heading'>{this.props.promoCodeHeadingText}</div>
                {this._renderForm(this.props.promoPlaceholderText, this.props.promoCodeApplyButtonText, this.props.cart)}
                <p className={this.state.error ? 'msc-alert-danger' : ''} aria-live='assertive'>
                    {this.state.error}
                </p>
                {this._renderAppliedPromoCode(this.props)}
            </div>
        );
    }

    private readonly _onInputChangeHandler = (e: React.ChangeEvent<HTMLInputElement>) => {
        const error = e.target.value === '' ? '' : this.state.error;
        this.setState({
            promoCodeInputValue: e.target.value,
            error,
            canApply: !!e.target.value
        });
    };

    private readonly _applyPromotion = (cartState: ICartState | undefined) => {
        if (!cartState || !cartState.cart) {
            return;
        }
        const appliedPromo = this.state.promoCodeInputValue;

        cartState
            .addPromoCode({ promoCode: appliedPromo })
            .then(result => {
                if (result.status === 'SUCCESS') {
                    // Show success text
                    this.setState({ promoCodeInputValue: '', error: '', canApply: false });
                } else if (result.substatus === 'ALREADYADDED') {
                    this.setState({ error: this.props.duplicatePromoCodeErrorText });
                } else {
                    this.setState({ error: this.props.invalidPromoCodeErrorText });
                }
            })
            .catch(error => {
                this.setState({ error: this.props.failedToAddPromoCodeErrorText });
            });
    };

    /**
     * On Submit function.
     * @param cartState -Cartstate.
     * @returns Apply promotion.
     */
    private readonly _onSubmitHandler = (cartState: ICartState | undefined) => (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        this._applyPromotionHandler(cartState);
    };

    /**
     * On apply promotion function.
     * @param cartState -Cartstate.
     * @returns Apply promotion.
     */
    private readonly _applyPromotionHandler = (cartState: ICartState | undefined) => () => {
        this._applyPromotion(cartState);
    };

    private readonly _renderForm = (promoPlaceholderText: string, promoCodeApplyButtonText: string, cartState: ICartState | undefined) => {
        const attributes = getTelemetryAttributes(this.props.telemetryContent!, this.payLoad);

        return (
            <form onSubmit={this._onSubmitHandler(cartState)} className='msc-promo-code__form-container'>
                <div className='msc-promo-code__group'>
                    <input
                        className='msc-promo-code__input-box'
                        onChange={this._onInputChangeHandler}
                        value={this.state.promoCodeInputValue}
                        placeholder={promoPlaceholderText}
                    />
                    <Button
                        title={promoCodeApplyButtonText}
                        className='msc-promo-code__apply-btn btn'
                        onClick={this._applyPromotionHandler(cartState)}
                        disabled={!this.state.canApply}
                        {...attributes}
                    >
                        {promoCodeApplyButtonText}
                    </Button>
                </div>
            </form>
        );
    };

    private readonly _removePromotion = (cartState: ICartState | undefined, event: React.MouseEvent) => {
        if (!cartState) {
            return;
        }
        const code = event.currentTarget.getAttribute('data-value') || '';
        cartState
            .removePromoCodes({
                promoCodes: [code]
            })
            .then(result => {
                if (result.status === 'SUCCESS') {
                    this.setState({ error: '' });
                }
            })
            .catch(() => {
                this.setState({ error: this.props.failedToRemovePromoCodeErrorText });
            });
    };

    private readonly _calculateDiscount = (code: string, cartState: ICartState | undefined) => {
        if (!cartState || !cartState.cart || !cartState.cart.CartLines || cartState.cart.CartLines.length === 0 || !code) {
            return;
        }
        let discountAmount = 0;
        for (const line of cartState.cart.CartLines) {
            if (line.DiscountLines) {
                for (const discountLine of line.DiscountLines) {
                    if (discountLine.DiscountCode === code) {
                        discountAmount += discountLine.DiscountCost!;
                    }
                }
            }
        }
        return discountAmount * -1;
    };

    private readonly _renderAppliedPromoCode = (props: IPromoCodeProps) => {
        if (!props.cart || !props.cart.cart || !props.cart.cart.Coupons || props.cart.cart.Coupons.length === 0) {
            return;
        }

        const removePromotionHandler = (event: React.MouseEvent<HTMLElement>) => {
            this._removePromotion(props.cart, event);
        };

        const promoCodTotalDiscount = props.cart.cart.Coupons.reduce((count: number, coupon: Coupon) => {
            return count + (this._calculateDiscount(coupon.Code || '', props.cart) || 0);
        }, 0);

        return (
            <>
                <div className='msc-promo-code__discount'>
                    <div className='msc-promo-code__discount-heading'>{this.props.appliedPromoCodeHeadingText}</div>
                    <PriceComponent
                        data={{
                            price: {
                                CustomerContextualPrice: promoCodTotalDiscount
                            }
                        }}
                        context={props.context}
                        id={props.id}
                        typeName={props.typeName}
                        className='msc-promo-code__discount-value'
                    />
                </div>
                {props.cart.cart.Coupons.map((coupon: Coupon) => {
                    const ariaLabel = props.removePromoAriaLabelFormat
                        ? format(props.removePromoAriaLabelFormat, props.removePromoText, coupon.Code)
                        : '';

                    return (
                        <div key={coupon.Code} className='msc-promo-code__line-container'>
                            <div className='msc-promo-code__line-value'>
                                {coupon.Code} (
                                <PriceComponent
                                    data={{
                                        price: {
                                            CustomerContextualPrice: this._calculateDiscount(coupon.Code || '', props.cart)
                                        }
                                    }}
                                    context={props.context}
                                    id={props.id}
                                    typeName={props.typeName}
                                    className='msc-promo-code__line-discount-value'
                                />
                                )
                            </div>
                            <Button
                                title={props.removePromoText}
                                className='msc-promo-code__line__btn-remove'
                                onClick={removePromotionHandler}
                                data-value={coupon.Code}
                                aria-label={ariaLabel}
                            >
                                {props.removePromoText}
                            </Button>
                        </div>
                    );
                })}
            </>
        );
    };
}

export default PromoCode;
