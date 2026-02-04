/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { ArrayExtensions, getFallbackImageUrl } from '@msdyn365-commerce-modules/retail-actions';
import { getPayloadObject, getTelemetryAttributes, IncrementalQuantity, ITelemetryContent } from '@msdyn365-commerce-modules/utilities';
import { PriceComponent } from '@msdyn365-commerce/components';
import { IComponentProps, IGridSettings, IImageSettings, Image } from '@msdyn365-commerce/core';
import { ChannelDeliveryOptionConfiguration, SimpleProduct } from '@msdyn365-commerce/retail-proxy';
import { CartLine, SalesLine } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import classNames from 'classnames';
import * as React from 'react';

/**
 * ICartlineResourceString: Interface for specifying the
 * resource strings that the component needs
 */
export interface ICartlineResourceString {
    /** Display string for discount label */
    discountStringText: string;
    /** String for size key  */
    sizeString: string;
    /** String for color key  */
    colorString: string;
    /** String associated with the configuration product dimension  */
    configString: string;
    /** String for style key  */
    styleString: string;
    /** String for amount key  */
    amountString?: string;
    /** Display string for quantity label  */
    quantityDisplayString: string;
    /** Display string for quantity label  */
    inputQuantityAriaLabel: string;
    /** Aria label for the decrement button in quantity component  */
    decrementButtonAriaLabel: string;
    /** Aria label for the increment button in quantity component  */
    incrementButtonAriaLabel: string;
    /** original text screenreader */
    originalPriceText: string;
    /** current text screenreader  */
    currentPriceText: string;
    /** shipping Charges Text  */
    shippingChargesText: string;
}

interface ICartLineData {
    cartLine: CartLine | SalesLine;
    product: SimpleProduct;
}

/**
 * ICartLineProps: The props required to render cartLineitem
 */
export interface ICartLineProps extends IComponentProps<ICartLineData> {
    /** The flag to change the quantity component from interactible to static  */
    disableQuantityInteractivity?: boolean;
    /** The primary image url  */
    primaryImageUrl?: string;
    /** The product url  */
    productUrl?: string;
    /** GridSettings for the product image in cartLine  */
    gridSettings: IGridSettings;
    /** ImageSettings for the product image in cartLine  */
    imageSettings: IImageSettings;
    /** Boolean flag to indicate if the item is out of stock  */
    isOutOfStock?: boolean;
    /** Flag to make quantity section editable  */
    isQuantityEditable?: boolean;
    /** Max quantity for line item  */
    maxQuantity?: number;
    /** Current quantity for line item  */
    currentQuantity?: number;
    /** Resource string for the component  */
    resources: ICartlineResourceString;
    /** SalesLine flag  */
    isSalesLine?: boolean;
    /** Error message to show in place of quantity */
    errorMessage?: string;
    /** Inventory information label  */
    inventoryInformationLabel?: string;
    /** Inventory information class name  */
    inventoryLabelClassName?: string;
    /** Flag to show/hide shipping charges for line items  */
    showShippingChargesForLineItems?: boolean;
    /** Boolean flag to indicate if cart state status is ready  */
    isCartStateReady?: boolean;
    /**  Chanel Delivery Option configuration is from api. */
    channelDeliveryOptionConfig?: ChannelDeliveryOptionConfiguration;
    /** The telemetry content */
    telemetryContent?: ITelemetryContent;
    /** Quantity onChange callback  */
    quantityOnChange?(cartLine: CartLine, newQuantity: number): boolean;
}

interface IDimensionStrings {
    /** String for size key  */
    sizeString: string;
    /** String for color key  */
    colorString: string;
    /** String associated with the configuration product dimension  */
    configString: string;
    /** String for style key  */
    styleString: string;
    /** String for amount key  */
    amountString?: string;
}

const CartLineItemFunctions = {
    renderProductDimensions: (Product: SimpleProduct, DimensionStrings: IDimensionStrings) => {
        if (!Product || !Product.Dimensions) {
            return [];
        }

        return Product.Dimensions.map(productDimension => {
            if (productDimension.DimensionTypeValue === 1) {
                return (
                    <div key={`${Product.RecordId}ProductDimensions1`} className='msc-cart-line__product-variant-item'>
                        <span className='msc-cart-line__product-variant-color'>
                            {DimensionStrings.colorString}:
                            <span className='name'>{productDimension.DimensionValue && productDimension.DimensionValue.Value}</span>
                        </span>
                    </div>
                );
            }

            if (productDimension.DimensionTypeValue === 2) {
                return (
                    <div key={`${Product.RecordId}ProductDimensions2`} className='msc-cart-line__product-variant-item'>
                        <span className='msc-cart-line__product-configuration'>
                            {DimensionStrings.configString}:
                            <span className='name'>{productDimension.DimensionValue && productDimension.DimensionValue.Value}</span>
                        </span>
                    </div>
                );
            }

            if (productDimension.DimensionTypeValue === 3) {
                return (
                    <div key={`${Product.RecordId}ProductDimensions3`} className='msc-cart-line__product-variant-item'>
                        <span className='msc-cart-line__product-variant-size'>
                            {DimensionStrings.sizeString}:
                            <span className='name'>{productDimension.DimensionValue && productDimension.DimensionValue.Value}</span>
                        </span>
                    </div>
                );
            }

            if (productDimension.DimensionTypeValue === 4) {
                return (
                    <div key={`${Product.RecordId}ProductDimensions4`} className='msc-cart-line__product-variant-item'>
                        <span className='msc-cart-line__product-variant-style'>
                            {Product.IsGiftCard ? DimensionStrings.amountString : DimensionStrings.styleString}:
                            <span className='name'>{productDimension.DimensionValue && productDimension.DimensionValue.Value}</span>
                        </span>
                    </div>
                );
            }
            return null;
        });
    },

    renderInventoryLabel: (props: ICartLineProps) => {
        if (!props.inventoryInformationLabel) {
            return null;
        }
        const inventoryCssName = props.inventoryLabelClassName
            ? `msc-cart-line__product-inventory-label ${props.inventoryLabelClassName}`
            : 'msc-cart-line__product-inventory-label';
        return <span className={inventoryCssName}>{props.inventoryInformationLabel}</span>;
    },
    renderShippingLabel: (props: ICartLineProps) => {
        const pickupDeliveryModeCode = props.context.request.channel?.PickupDeliveryModeCode;
        const channelDeliveryOptionConfig = props.channelDeliveryOptionConfig;
        const cartline = props.data.cartLine;
        let hasShippingMethod = false;
        if (channelDeliveryOptionConfig !== undefined) {
            hasShippingMethod =
                !!cartline.DeliveryMode &&
                channelDeliveryOptionConfig?.PickupDeliveryModeCodes?.some(deliveryMode => deliveryMode !== cartline.DeliveryMode)
                    ? true
                    : false;
        } else {
            hasShippingMethod = !!cartline.DeliveryMode && cartline.DeliveryMode !== pickupDeliveryModeCode ? true : false;
        }
        if (!hasShippingMethod) {
            return undefined;
        }

        const shippingChargeLines = (cartline.ChargeLines || []).filter(chargeLine => chargeLine.IsShipping);

        if (shippingChargeLines.length === 0) {
            return undefined;
        }

        const freightFee = shippingChargeLines.reduce((chargeTotal, chargeLine) => {
            return chargeTotal + (chargeLine.CalculatedAmount || 0);
        }, 0);

        const priceComponent = (
            <PriceComponent
                data={{
                    price: {
                        // @ts-ignore
                        CustomerContextualPrice: freightFee
                    }
                }}
                freePriceText={'Free'}
                context={props.context}
                id={props.id}
                typeName={props.typeName}
                className='msc-cart-line__freight-actual'
            />
        );
        return (
            <>
                <label className='msc-cart-line__freight-label'>{`${props.resources.shippingChargesText}:`}</label>
                <span className='shipping-value'>{priceComponent}</span>
            </>
        );
    },
    renderOtherCharges: (props: ICartLineProps) => {
        const cartline = props.data.cartLine;
        const otherCharges = cartline.ChargeLines?.filter(chargeline => !chargeline.IsShipping);

        return (
            (otherCharges &&
                otherCharges.map((otherCharge, key) => {
                    return otherCharge && otherCharge.CalculatedAmount ? (
                        <div className='msc-cart-line__other-charges' key={key}>
                            <label className='msc-cart-line__other-charges-label'>{`${otherCharge.Description}:`}</label>
                            <span className='other-charge-value'>
                                <PriceComponent
                                    data={{
                                        price: {
                                            // @ts-ignore
                                            CustomerContextualPrice: otherCharge.CalculatedAmount
                                        }
                                    }}
                                    context={props.context}
                                    id={props.id}
                                    typeName={props.typeName}
                                    className='msc-cart-line__other-charges-actual'
                                />
                            </span>
                        </div>
                    ) : (
                        ''
                    );
                })) ||
            undefined
        );
    }
};

// tslint:disable-next-line:max-func-body-length
// eslint-disable-next-line no-redeclare
const CartLine: React.FC<ICartLineProps> = (props: ICartLineProps) => {
    const { isSalesLine, productUrl, resources } = props;
    const { product, cartLine } = props.data;

    const destructDimensionStrings = {
        sizeString: resources.sizeString,
        colorString: resources.colorString,
        styleString: resources.styleString,
        configString: resources.configString,
        amountString: resources.amountString
    };

    const fallbackImageUrl = product && getFallbackImageUrl(product.ItemId, props.context.actionContext.requestContext.apiSettings);
    const productDimensions = product && CartLineItemFunctions.renderProductDimensions(product, destructDimensionStrings);
    const imageSettings = props.imageSettings;
    if (imageSettings) {
        imageSettings.cropFocalRegion = true;
    }
    // const renderDisountLines = CartLineItemFunctions.renderDiscountLines(props);
    const renderInventoryLabel = CartLineItemFunctions.renderInventoryLabel(props);
    const renderShippingLabel = CartLineItemFunctions.renderShippingLabel(props);
    const renderOtherCharges = CartLineItemFunctions.renderOtherCharges(props);
    const payLoad = getPayloadObject('click', props.telemetryContent!, '', product?.RecordId.toString());
    const prodAttribute = getTelemetryAttributes(props.telemetryContent!, payLoad);
    const productName = product?.Name || cartLine.Description;

    const onChange = (newValue: number): boolean => {
        if (props.quantityOnChange) {
            return props.quantityOnChange(props.data.cartLine, newValue);
        }
        return true;
    };

    const _generateErrorMessage = (): JSX.Element | null => {
        if (props.errorMessage) {
            return (
                <div className='msc-alert__header'>
                    <span className='msi-exclamation-triangle' />
                    <span>{props.errorMessage}</span>
                </div>
            );
        }

        return null;
    };

    const _generateQuantityandPrice = (): JSX.Element[] | null => {
        const nodes = [];
        if (props.isOutOfStock) {
            const errorMessage = _generateErrorMessage();

            if (errorMessage) {
                return [errorMessage];
            }

            return null;
        }

        // No quantity selector for invoice line
        if (!props.data.cartLine.IsInvoiceLine) {
            if (props.isQuantityEditable) {
                nodes.push(
                    <div className='msc-cart-line__product-quantity'>
                        <div className='msc-cart-line__product-quantity-label'>{resources.quantityDisplayString}</div>

                        {_generateErrorMessage()}

                        <IncrementalQuantity
                            id={`msc-cart-line__quantity_${props.data.product?.RecordId}-${props.data.cartLine?.DeliveryMode}-${props.data.cartLine?.LineId}`}
                            max={props.maxQuantity || 10}
                            currentCount={props.currentQuantity}
                            onChange={onChange}
                            inputQuantityAriaLabel={resources.inputQuantityAriaLabel}
                            decrementButtonAriaLabel={resources.decrementButtonAriaLabel}
                            incrementButtonAriaLabel={resources.incrementButtonAriaLabel}
                            key={props.data.cartLine.LineId}
                            disabled={!props.isCartStateReady}
                            isGiftCard={props.data.product.IsGiftCard}
                            telemetryContent={props.telemetryContent}
                        />
                    </div>
                );
            } else {
                nodes.push(
                    <div className={classNames('msc-cart-line__quantity', { 'single-quantity': props.data.cartLine.Quantity === 1 })}>
                        <label className='quantity-label'>{resources.quantityDisplayString}:</label>

                        {_generateErrorMessage()}

                        <span className='quantity-value'>{props.data.cartLine.Quantity}</span>
                    </div>
                );
            }
        }

        nodes.push(
            <div className='msc-cart-line__product-savings'>
                <PriceComponent
                    data={
                        isSalesLine
                            ? {
                                  price: {
                                      // @ts-ignore
                                      CustomerContextualPrice: props.data.cartLine.NetAmount,
                                      BasePrice: props.data.cartLine.Price
                                  }
                              }
                            : {
                                  price: {
                                      CustomerContextualPrice: props.data.cartLine.NetAmountWithoutTax,
                                      BasePrice: props.data.cartLine.NetPrice
                                  }
                              }
                    }
                    context={props.context}
                    id={props.id}
                    typeName={props.typeName}
                    className='msc-cart-line__product-savings-actual'
                    originalPriceText={resources.originalPriceText}
                    currentPriceText={resources.currentPriceText}
                />
                {(props.data.cartLine.DiscountAmount && props.data.cartLine.DiscountAmount > 0 && (
                    <>
                        <span className='msc-cart-line__product-savings-label'>{` ${props.resources.discountStringText ||
                            'Discount'} `}</span>
                        <PriceComponent
                            data={
                                props.isSalesLine
                                    ? {
                                          price: {
                                              // @ts-ignore
                                              CustomerContextualPrice: props.data.cartLine.PeriodicDiscount
                                          }
                                      }
                                    : {
                                          price: {
                                              CustomerContextualPrice: props.data.cartLine.DiscountAmountWithoutTax,
                                              BasePrice: props.data.cartLine.DiscountAmount
                                          }
                                      }
                            }
                            context={props.context}
                            id={props.id}
                            typeName={props.typeName}
                            className='msc-cart-line__product-savings-text'
                        />
                    </>
                )) ||
                    null}
            </div>
        );

        return nodes;
    };

    return (
        <div className='msc-cart-line'>
            <div className='msc-cart-line__product-image'>
                <Image
                    src={props.primaryImageUrl || ''}
                    fallBackSrc={fallbackImageUrl}
                    altText={productName}
                    gridSettings={props.gridSettings}
                    imageSettings={imageSettings}
                    loadFailureBehavior='empty'
                />
            </div>
            <div className='msc-cart-line__content'>
                <div className='msc-cart-line__product'>
                    <a className='msc-cart-line__product-title' href={productUrl} {...prodAttribute}>
                        {productName}
                    </a>
                    {ArrayExtensions.hasElements(productDimensions) ? (
                        <div className='msc-cart-line__product-variants'>{productDimensions}</div>
                    ) : (
                        ''
                    )}
                    <div className='msc-cart-line__product-price'>
                        <PriceComponent
                            data={
                                isSalesLine
                                    ? {
                                          price: {
                                              // @ts-ignore
                                              CustomerContextualPrice: props.data.cartLine.TotalAmount,
                                              BasePrice: props.data.cartLine.Price
                                          }
                                      }
                                    : {
                                          price: {
                                              CustomerContextualPrice: props.data.cartLine.TotalAmount,
                                              BasePrice: props.data.cartLine.NetPrice
                                          }
                                      }
                            }
                            context={props.context}
                            id={props.id}
                            typeName={props.typeName}
                            className='discount-value'
                        />
                    </div>
                    {renderInventoryLabel}
                    {props.showShippingChargesForLineItems && <div className='msc-cart-line__freight'>{renderShippingLabel}</div>}
                    {renderOtherCharges}
                </div>
                {_generateQuantityandPrice()}
            </div>
        </div>
    );
};

export default CartLine;
