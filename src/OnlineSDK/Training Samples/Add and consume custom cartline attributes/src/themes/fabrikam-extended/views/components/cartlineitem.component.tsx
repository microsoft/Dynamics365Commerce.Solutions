/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import MsDyn365, { IComponentProps, IGridSettings, IImageSettings, Image, msdyn365Commerce } from '@msdyn365-commerce/core';
import { ICartState } from '@msdyn365-commerce/global-state';
import { ChannelDeliveryOptionConfiguration, SimpleProduct } from '@msdyn365-commerce/retail-proxy';
import { AttributeValueBase, CartLine, ProductCatalog, SalesLine } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import { ArrayExtensions, getFallbackImageUrl, ObjectExtensions } from '@msdyn365-commerce-modules/retail-actions';
import { getPayloadObject, getTelemetryAttributes, ITelemetryContent } from '@msdyn365-commerce-modules/utilities';
import classNames from 'classnames';
import * as React from 'react';
import { CartLinePriceEditor, ICartLinePriceEditorResources, PriceComponent } from '@msdyn365-commerce/components';

/**
 * ICartlineResourceString: Interface for specifying the
 * resource strings that the component needs.
 */
export interface ICartlineResourceString {
    /**
     * Display string for discount label.
     */
    discountStringText: string;

    /**
     * String for size key.
     */
    sizeString: string;

    /**
     * String for color key.
     */
    colorString: string;

    /**
     * String associated with the configuration product dimension.
     */
    configString: string;

    /**
     * String for style key.
     */
    styleString: string;

    /**
     * String for amount key.
     */
    amountString?: string;

    /**
     * Display string for quantity label.
     */
    quantityDisplayString: string;

    /**
     * Display string for quantity label.
     */
    inputQuantityAriaLabel: string;

    /**
     * Aria label for the decrement button in quantity component.
     */
    decrementButtonAriaLabel?: string;

    /**
     * Aria label for the increment button in quantity component.
     */
    incrementButtonAriaLabel?: string;

    /**
     * Original text screen reader.
     */
    originalPriceText: string;

    /**
     * Current text screen reader.
     */
    currentPriceText: string;

    /**
     * Shipping Charges Text.
     */
    shippingChargesText: string;

    priceEditorResources?: ICartLinePriceEditorResources;
}

/**
 * The data about the cart line and products.
 */
export interface ICartLineData {
    cartLine: CartLine | SalesLine;
    cartState?: ICartState;
    product?: SimpleProduct;
    catalogs?: ProductCatalog[];
}

export type CartLineDisplayMode = 'DEFAULT' | 'COMPACT';

/**
 * ICartLineProps: The props required to render cartLineitem.
 */
export interface ICartLineProps extends IComponentProps<ICartLineData> {
    /**
     * The flag to change the quantity component from interactivity to static.
     */
    disableQuantityInteractivity?: boolean;

    /**
     * The primary image url.
     */
    primaryImageUrl?: string;

    /**
     * The product url.
     */
    productUrl?: string;

    /**
     * GridSettings for the product image in cartLine.
     */
    gridSettings: IGridSettings;

    /**
     * ImageSettings for the product image in cartLine.
     */
    imageSettings: IImageSettings;

    /**
     * Boolean flag to indicate if the item is out of stock.
     */
    isOutOfStock?: boolean;

    /**
     * Flag to make quantity section editable.
     */
    isQuantityEditable?: boolean;

    /**
     * Max quantity for line item.
     */
    maxQuantity?: number;

    /**
     * Current quantity for line item.
     */
    currentQuantity?: number;

    /**
     * Resource string for the component.
     */
    resources: ICartlineResourceString;

    /**
     * SalesLine flag.
     */
    isSalesLine?: boolean;

    /**
     * Error message to show in place of quantity.
     */
    errorMessage?: string;

    /**
     * Display mode to use.
     */
    displayMode?: CartLineDisplayMode;

    /**
     * Inventory information label.
     */
    inventoryInformationLabel?: string;

    /**
     * Inventory information class name.
     */
    inventoryLabelClassName?: string;

    /**
     * Flag to show/hide shipping charges for line items.
     */
    showShippingChargesForLineItems?: boolean;

    /**
     * Boolean flag to indicate if cart state status is ready.
     */
    isCartStateReady?: boolean;

    /**
     * Chanel Delivery Option configuration is from api.
     */
    channelDeliveryOptionConfig?: ChannelDeliveryOptionConfiguration;

    /**
     * The telemetry content.
     */
    telemetryContent?: ITelemetryContent;

    /**
     * The cart line index.
     */
    lineIndex?: number;

    /**
     * Quantity onChange callback.
     */
    quantityOnChange?(cartLine: CartLine, newQuantity: number, lineIndex?: number): void;
}

interface IDimensionStrings {
    /**
     * String for size key.
     */
    sizeString: string;

    /**
     * String for color key.
     */
    colorString: string;

    /**
     * String associated with the configuration product dimension.
     */
    configString: string;

    /**
     * String for style key.
     */
    styleString: string;

    /**
     * String for amount key.
     */
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
                            {DimensionStrings.colorString}
                            <span className='name'>{productDimension.DimensionValue && productDimension.DimensionValue.Value}</span>
                        </span>
                    </div>
                );
            }

            if (productDimension.DimensionTypeValue === 2) {
                return (
                    <div key={`${Product.RecordId}ProductDimensions2`} className='msc-cart-line__product-variant-item'>
                        <span className='msc-cart-line__product-configuration'>
                            {DimensionStrings.configString}
                            <span className='name'>{productDimension.DimensionValue?.Value}</span>
                        </span>
                    </div>
                );
            }

            if (productDimension.DimensionTypeValue === 3) {
                return (
                    <div key={`${Product.RecordId}ProductDimensions3`} className='msc-cart-line__product-variant-item'>
                        <span className='msc-cart-line__product-variant-size'>
                            {DimensionStrings.sizeString}
                            <span className='name'>{productDimension.DimensionValue?.Value}</span>
                        </span>
                    </div>
                );
            }

            if (productDimension.DimensionTypeValue === 4) {
                return (
                    <div key={`${Product.RecordId}ProductDimensions4`} className='msc-cart-line__product-variant-item'>
                        <span className='msc-cart-line__product-variant-style'>
                            {Product.IsGiftCard ? DimensionStrings.amountString : DimensionStrings.styleString}
                            <span className='name'>{productDimension.DimensionValue?.Value}</span>
                        </span>
                    </div>
                );
            }
            return null;
        });
    },
    renderDiscountLines: (props: ICartLineProps) => {
        if (!props.data.cartLine.DiscountLines || props.data.cartLine.DiscountLines.length === 0) {
            return null;
        }

        return props.data.cartLine.DiscountLines.map((discountLine, index) => {
            return (
                <div key={discountLine.OfferId || index} className='msc-cart-line-item-product-discount'>
                    <span className='msc-cart-line__product-savings-label'>
                        {discountLine.OfferName ? discountLine.OfferName : ''}:{` ${props.resources.discountStringText || 'Discount'} `}
                    </span>
                    <span className='msc-cart-line__promo-codes'>
                        {discountLine.DiscountCost && (
                            <>
                                <PriceComponent
                                    data={
                                        props.isSalesLine
                                            ? {
                                                  price: {
                                                      // @ts-expect-error
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
                                    className='msc-cart-line__discount-value'
                                />
                            </>
                        )}
                    </span>
                    <span>{` (${discountLine.EffectivePercentage !== undefined ? discountLine.EffectivePercentage : ''}%)`}</span>
                </div>
            );
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

    /**
     * Gets the react node for product unit of measure display.
     * @param  props - ICartLineProps props.
     * @returns The node representing markup for unit of measure component.
     */
    renderUnitOfMeasure: (props: ICartLineProps) => {
        // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access -- Do not need type check for appsettings
        if (
            props.context.app.config &&
            (!props.context.app.config.unitOfMeasureDisplayType || props.context.app.config.unitOfMeasureDisplayType === 'none')
        ) {
            return undefined;
        }

        const product = props.data.product;
        if (!product || !product.DefaultUnitOfMeasure) {
            return undefined;
        }

        return (
            <div className='msc-cartline__product-unit-of-measure'>
                <span>{product.DefaultUnitOfMeasure}</span>
            </div>
        );
    },
    renderShippingLabel: (props: ICartLineProps) => {
        const pickupDeliveryModeCode = props.context.request.channel?.PickupDeliveryModeCode;
        const channelDeliveryOptionConfig = props.channelDeliveryOptionConfig;
        const cartline = props.data.cartLine;
        let hasShippingMethod = false;
        if (channelDeliveryOptionConfig !== undefined) {
            hasShippingMethod = !!(
                cartline.DeliveryMode &&
                channelDeliveryOptionConfig.PickupDeliveryModeCodes?.some(deliveryMode => deliveryMode !== cartline.DeliveryMode)
            );
        } else {
            hasShippingMethod = !!(cartline.DeliveryMode && cartline.DeliveryMode !== pickupDeliveryModeCode);
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
                        CustomerContextualPrice: freightFee
                    }
                }}
                freePriceText='Free'
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

/**
 * Renders catalog label for the cart line.
 * @param props - Cart line props.
 * @returns Catalog label.
 */
const CatalogLabelComponent: React.FC<ICartLineProps> = (props: ICartLineProps) => {
    const catalogId = props.data.cartLine.CatalogId;

    if (!props.context.request.user.isB2b || !catalogId || !ArrayExtensions.hasElements(props.data.catalogs)) {
        return null;
    }

    const catalog = props.data.catalogs.find(item => item.RecordId === catalogId);

    if (!catalog || !catalog.Name) {
        return null;
    }

    return <div className='msc-cart-line__catalog-label'>{catalog.Name}</div>;
};

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
    const imageSettings = props.imageSettings;
    imageSettings.cropFocalRegion = !ObjectExtensions.isNullOrUndefined(imageSettings);
    const { inputQuantityAriaLabel } = props.resources;
    const productDimensions = product && CartLineItemFunctions.renderProductDimensions(product, destructDimensionStrings);
    const renderDisountLines = CartLineItemFunctions.renderDiscountLines(props);
    const renderInventoryLabel = CartLineItemFunctions.renderInventoryLabel(props);
    const renderUnitOfMeasure = CartLineItemFunctions.renderUnitOfMeasure(props);
    const renderShippingLabel = CartLineItemFunctions.renderShippingLabel(props);
    const renderOtherCharges = CartLineItemFunctions.renderOtherCharges(props);
    const payLoad = getPayloadObject('click', props.telemetryContent!, '', product?.RecordId.toString());
    const prodAttribute = getTelemetryAttributes(props.telemetryContent!, payLoad);
    const productName = product?.Name || cartLine.Description;
    const imgeClassName = props.data.cartLine.IsInvoiceLine ? 'msc-cart-line__invoice-image' : 'msc-cart-line__product-image';

    const customCartlineAttributeKey: string = 'customCartlineAttribute';
    const [customCartlineAttributeText, SetCustomCartlineAttributeText] = React.useState<string>('');

    const getAttributeValue = (cartLine: CartLine, attributeName: string): string => {
        /* eslint-disable  @typescript-eslint/no-explicit-any */
        const attribute: any =
            cartLine.AttributeValues &&
            cartLine.AttributeValues.find((attributeValueBase: AttributeValueBase) => attributeValueBase.Name === attributeName);
        if (attribute && attribute.TextValue) {
            return attribute.TextValue;
        }
        return '';
    };

    React.useEffect(() => {
        SetCustomCartlineAttributeText(getAttributeValue(props.data.cartLine, customCartlineAttributeKey));
    }, [props.data.cartLine]);

    const renderCustomCartlineAttributeText = () => {
        return <label className='msc-cart-line__attribute-value'>{customCartlineAttributeText}</label>;
    };

    const _generateErrorMessage = (): JSX.Element | null => {
        if (props.errorMessage) {
            return (
                <div className='msc-cart-line__error-message msc-alert__header'>
                    <span className='msi-exclamation-triangle' />
                    <span>{props.errorMessage}</span>
                </div>
            );
        }

        return null;
    };

    const _updateQuantity = (event: React.ChangeEvent<HTMLSelectElement>) => {
        if (props.quantityOnChange) {
            props.quantityOnChange(props.data.cartLine, Number.parseInt(event.target.value, 10), props.lineIndex);
        }
    };

    const _generateMenu = (quantity: number) => {
        const nodes = [];

        for (let i = 1; i <= quantity; i++) {
            nodes.push(
                <option className='msc-cart-line__quantity__select-menu__item' value={i}>
                    {i}
                </option>
            );
        }

        return nodes;
    };

    const _generateSelectMenu = (quantity: number, currentQuantity: number | undefined): JSX.Element => {
        return (
            <select
                className='msc-cart-line__quantity__select-menu'
                aria-label={inputQuantityAriaLabel}
                value={currentQuantity}
                onChange={_updateQuantity}
            >
                {_generateMenu(quantity)}
            </select>
        );
    };

    const _generateQuantityAndPrice = (): JSX.Element[] | null => {
        const nodes = [];

        if (props.isOutOfStock) {
            return null;
        }

        // No quantity selector for invoice line
        if (!props.data.cartLine.IsInvoiceLine) {
            if (props.isQuantityEditable && !props.data.product?.IsGiftCard) {
                nodes.push(_generateSelectMenu(props.maxQuantity || 10, props.currentQuantity));
            } else {
                nodes.push(
                    <div className={classNames('msc-cart-line__quantity', { 'single-quantity': props.data.cartLine.Quantity === 1 })}>
                        <label className='quantity-label'>{resources.quantityDisplayString}</label>
                        <span className='quantity-value'>{props.data.cartLine.Quantity}</span>
                    </div>
                );
            }
        }

        nodes.push(
            <div className='msc-cart-line__product-price'>
                <PriceComponent
                    data={
                        isSalesLine
                            ? {
                                  price: {
                                      // @ts-expect-error
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
                    className='discount-value'
                    originalPriceText={resources.originalPriceText}
                    currentPriceText={resources.currentPriceText}
                />
            </div>
        );

        return nodes;
    };

    if (props.displayMode === 'COMPACT') {
        const reducedDimensions: string = product?.Dimensions
            ? product.Dimensions.reduce<string>((str, productDimension) => {
                  if (productDimension.DimensionValue?.Value) {
                      if (str) {
                          return `${str}, ${productDimension.DimensionValue.Value}`;
                      }

                      return `${productDimension.DimensionValue.Value}`;
                  }

                  return str;
              }, '')
            : '';

        return (
            <div className='msc-cart-line msc-cart-line__compact'>
                <div className={imgeClassName}>
                    <Image
                        requestContext={props.context.actionContext.requestContext}
                        src={props.primaryImageUrl ?? 'empty'}
                        fallBackSrc={fallbackImageUrl}
                        altText={productName}
                        gridSettings={props.gridSettings}
                        imageSettings={imageSettings}
                        loadFailureBehavior='empty'
                    />
                </div>
                <div className='msc-cart-line__content'>
                    {MsDyn365.isBrowser ? (
                        <a className='msc-cart-line__product-title' {...prodAttribute} href={productUrl} key={productUrl}>
                            {productName}
                        </a>
                    ) : null}
                    {reducedDimensions !== '' ? <div className='msc-cart-line__product-variants'>{reducedDimensions}</div> : ''}
                    {renderUnitOfMeasure}
                    {_generateQuantityAndPrice()}
                    {props.data.cartLine.IsInvoiceLine && props.data.cartState && props.resources.priceEditorResources && (
                        <CartLinePriceEditor
                            className='msc-cart-line__price-editor-container'
                            context={props.context}
                            resources={props.resources.priceEditorResources}
                            cartState={props.data.cartState}
                            cartLine={props.data.cartLine}
                        />
                    )}
                </div>
                {_generateErrorMessage()}
            </div>
        );
    }

    return (
        <>
            <div className='msc-cart-line'>
                <div className={imgeClassName}>
                    <Image
                        requestContext={props.context.actionContext.requestContext}
                        src={props.primaryImageUrl ?? ''}
                        fallBackSrc={fallbackImageUrl}
                        altText={productName}
                        gridSettings={props.gridSettings}
                        imageSettings={imageSettings}
                        loadFailureBehavior='empty'
                    />
                </div>
                <div className='msc-cart-line__content'>
                    <div className='msc-cart-line__product'>
                        <CatalogLabelComponent {...props} />
                        {MsDyn365.isBrowser ? (
                            <a className='msc-cart-line__product-title' {...prodAttribute} href={productUrl} key={productUrl}>
                                {productName}
                            </a>
                        ) : null}
                        {ArrayExtensions.hasElements(productDimensions) ? (
                            <div className='msc-cart-line__product-variants'>{productDimensions}</div>
                        ) : (
                            ''
                        )}
                        {renderUnitOfMeasure}
                        {renderDisountLines}
                        {renderInventoryLabel}
                        {props.showShippingChargesForLineItems && <div className='msc-cart-line__freight'>{renderShippingLabel}</div>}
                        {renderOtherCharges}
                        {renderCustomCartlineAttributeText()}
                    </div>
                    {_generateQuantityAndPrice()}
                    {props.data.cartLine.IsInvoiceLine && props.data.cartState && props.resources.priceEditorResources && (
                        <CartLinePriceEditor
                            className='msc-cart-line__price-editor-container'
                            context={props.context}
                            resources={props.resources.priceEditorResources}
                            cartState={props.data.cartState}
                            cartLine={props.data.cartLine}
                        />
                    )}
                </div>
            </div>
            {_generateErrorMessage()}
        </>
    );
};

// @ts-expect-error
export const CartLineItemComponent: React.FunctionComponent<ICartLineProps> = msdyn365Commerce.createComponentOverride<ICartline>(
    'CartLineItem',
    { component: CartLine, ...CartLineItemFunctions }
);

export default CartLineItemComponent;
