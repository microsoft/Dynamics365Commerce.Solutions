/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { IComponent, IComponentProps, msdyn365Commerce } from '@msdyn365-commerce/core';
import { ProductPrice } from '@msdyn365-commerce/retail-proxy';
import classnames from 'classnames';
import * as React from 'react';

const defaultPrice: number = 0;
const freePrice: number = 0;

/**
 * Interface for price component resources.
 * @param {string} priceRangeSeparator - The price range separator.
 */
export interface IPriceComponentResources {
    priceRangeSeparator?: string;
    salesAgreementPricePrompt?: string;
    salesAgreementExpirationDatePrompt?: string;
    salesAgreementCommittedQuantityPrompt?: string;
    salesAgreementRemainingQuantityPrompt?: string;
}

/**
 * Interface for price component props.
 * @param {boolean} isPriceMinMaxEnabled - Whether the price range feature is enabled or not.
 * @param {IPriceComponentResources} priceResources - The price range resources.
 */
export interface IPriceComponentProps extends IComponentProps<{ price: ProductPrice }> {
    className?: string;
    savingsText?: string;
    freePriceText?: string;
    originalPriceText?: string;
    currentPriceText?: string;
    isPriceMinMaxEnabled?: boolean;
    priceResources?: IPriceComponentResources;
    isSalesAgreementPriceFeatureEnabled?: boolean;
    salesAgreementLineId?: number;
}
export interface IPriceComponent extends IComponent<IPriceComponentProps> {
}

const priceComponentActions = {};

/**
 * Price.
 * @param props - The props.
 * @returns The JSX.Element.
 */
const Price: React.FC<IPriceComponentProps> = (props: IPriceComponentProps): JSX.Element | null => {
    // CustomerContextualPrice could be zero
    if (props.data.price.CustomerContextualPrice === undefined) {
        return null;
    }

    return (
        <span className={classnames('msc-price', props.className)}>
            {showStrikethroughPricing(props) ? renderCurrentPriceWithOriginalPrice(props) : renderCurrentPrice(props)}
        </span>
    );
};

/**
 * ShowStrikethroughPricing.
 * @param props - The props.
 * @returns True if showStrikethroughPricing, false otherwise.
 */
const showStrikethroughPricing = (props: IPriceComponentProps): boolean => {
    const originalPrice = getOriginalPrice(props);
    if (props.isPriceMinMaxEnabled &&
        props.data.price.MaxVariantPrice &&
        props.data.price.MinVariantPrice &&
        props.data.price.MaxVariantPrice > props.data.price.MinVariantPrice) {
        return props.data.price.MinVariantPrice < originalPrice;
    }

    if (originalPrice && props.data.price.CustomerContextualPrice) {
        return originalPrice > props.data.price.CustomerContextualPrice;
    }

    return false;
};

const getOriginalPrice = (props: IPriceComponentProps): number => {
    return Math.max(
        props.data.price.BasePrice ?? defaultPrice,
        props.data.price.TradeAgreementPrice ?? defaultPrice,
        props.data.price.AdjustedPrice ?? defaultPrice
    );
};

const formatCurrency = (props: IPriceComponentProps, price: number | undefined, shouldUseFreePriceText: boolean | undefined): string => {
    if (price === undefined) {
        return '';
    }

    if (shouldUseFreePriceText && price === freePrice && props.freePriceText) {
        return props.freePriceText;
    }

    return props.context.cultureFormatter.formatCurrency(price);
};

const renderCurrentPrice = (props: IPriceComponentProps): JSX.Element => {
    const initialPrice: string = formatCurrency(props, props.data.price.CustomerContextualPrice, true);
    const maxVariantPrice: string = formatCurrency(props, props.data.price.MaxVariantPrice, false);
    const minVariantPrice: string = formatCurrency(props, props.data.price.MinVariantPrice, false);
    if (props.isPriceMinMaxEnabled &&
        props.data.price.MaxVariantPrice &&
        props.data.price.MinVariantPrice &&
        props.data.price.MaxVariantPrice > props.data.price.MinVariantPrice) {
        return (
            <span className='msc-price__pricerange' itemProp='price'>
                {' '}
                <span className='msc-price__minprice' itemProp='price'>
                    {minVariantPrice}
                </span>
                <span className='msc-price__separator' itemProp='price'>
                    {props.priceResources?.priceRangeSeparator}
                </span>
                <span className='msc-price__maxprice' itemProp='price'>
                    {maxVariantPrice}
                </span>
            </span>
        );
    }
    return (<span className='msc-price__actual' itemProp='price'>
        {initialPrice}
    </span>);
};

const renderCurrentPriceWithOriginalPrice = (props: IPriceComponentProps): JSX.Element => {
    const originalPrice: number = getOriginalPrice(props);
    const initialPrice: string = formatCurrency(props, originalPrice, true);

    return (
        <>
            <span className='sr-only'>
                {' '}
                {props.originalPriceText}
                {' '}
                {initialPrice}
                {' '}
                {props.currentPriceText}
                {' '}
                {renderCurrentPrice(props)}
            </span>
            <span className='msc-price__strikethrough' aria-hidden='true'>
                {initialPrice}
            </span>
            <span aria-hidden='true'>
                {renderCurrentPrice(props)}
            </span>
            {props.savingsText && <span className='msc-price__savings'>
                {props.savingsText}
            </span>}
        </>
    );
};

export const PriceComponent: React.FunctionComponent<IPriceComponentProps> = msdyn365Commerce.createComponentOverride<IPriceComponent>(
    'Price',
    { component: Price, ...priceComponentActions }
);


export default PriceComponent;