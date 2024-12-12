/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { getProductPageUrlSync } from '@msdyn365-commerce-modules/retail-actions';
import {
    format,
    getPayloadObject,
    getTelemetryAttributes,
    ITelemetryContent,
    onTelemetryClick
} from '@msdyn365-commerce-modules/utilities';
import { PriceComponent, RatingComponent } from '@msdyn365-commerce/components';
import {
    IComponent,
    IComponentProps,
    ICoreContext,
    IGridSettings,
    IImageData,
    IImageSettings,
    Image,
    msdyn365Commerce
} from '@msdyn365-commerce/core';
import { ProductPrice, ProductSearchResult } from '@msdyn365-commerce/retail-proxy';
import React from 'react';

export interface IProductComponentProps extends IComponentProps<{ product?: ProductSearchResult }> {
    className?: string;
    imageSettings?: IImageSettings;
    savingsText?: string;
    freePriceText?: string;
    originalPriceText?: string;
    currentPriceText?: string;
    ratingAriaLabel?: string;
    allowBack?: boolean;
    telemetryContent?: ITelemetryContent;
    quickViewButton?: React.ReactNode;
}

export interface IProductComponent extends IComponent<IProductComponentProps> {}

const PriceComponentActions = {};

const ProductCard: React.FC<IProductComponentProps> = ({
    data,
    context,
    imageSettings,
    savingsText,
    freePriceText,
    originalPriceText,
    currentPriceText,
    ratingAriaLabel,
    allowBack,
    typeName,
    id,
    telemetryContent,
    quickViewButton
}) => {
    const product = data.product;
    let imageOrientation: string = '';
    if (!product) {
        return null;
    }
    if (product.AttributeValues!.length > 0) {
        product.AttributeValues!.map(property => {
            if (property.Name === 'Image Orientation') {
                imageOrientation = property.TextValue!;
            }
        });
        imageOrientation = 'Landscape';
    }
    const productImageSettings = imageOrientation === 'Landscape' ? getLandscapeImageSettings(imageSettings) : imageSettings;
    let productUrl = getProductPageUrlSync(product.Name || '', product.RecordId, context && context.actionContext, undefined);
    if (allowBack) {
        productUrl = updateProductUrl(productUrl, context);
    }

    // Construct telemetry attribute to render
    const payLoad = getPayloadObject('click', telemetryContent!, '', product.RecordId.toString());

    const attribute = getTelemetryAttributes(telemetryContent!, payLoad);

    return (
        <>
            <a
                href={productUrl}
                onClick={onTelemetryClick(telemetryContent!, payLoad, product.Name!)}
                aria-label={renderLabel(
                    product.Name,
                    context.cultureFormatter.formatCurrency(product.Price),
                    product.AverageRating,
                    ratingAriaLabel
                )}
                className='msc-product'
                {...attribute}
            >
                <div className='msc-product__image'>
                    {renderProductPlacementImage(productImageSettings, context.request.gridSettings, product.PrimaryImageUrl, product.Name)}
                </div>
                <div className='msc-product__details'>
                    <h4 className='msc-product__title'>{product.Name}</h4>
                    {renderPrice(
                        context,
                        typeName,
                        id,
                        product.BasePrice,
                        product.Price,
                        savingsText,
                        freePriceText,
                        originalPriceText,
                        currentPriceText
                    )}
                    {renderDescription(product.Description)}
                    {!context.app.config.hideRating &&
                        renderRating(context, typeName, id, product.AverageRating, product.TotalRatings, ratingAriaLabel)}
                </div>
            </a>
            {quickViewButton && renderQuickView(quickViewButton, product.RecordId)}
        </>
    );
};
function getLandscapeImageSettings(cmsImageSettings?: IImageSettings): IImageSettings {
    // tslint:disable-next-line:no-unnecessary-local-variable
    const landscapeImageSettings: IImageSettings = {
        viewports: {
            xs: { q: 'w=735&h=481&q=80&m=6&f=jpg', w: 735, h: 481 },
            sm: { q: 'w=467&h=303&q=80&m=6&f=jpg', w: 467, h: 303 },
            md: { q: 'w=563&h=366&q=80&m=6&f=jpg', w: 563, h: 366 },
            lg: { q: 'w=670&h=431&q=80&m=6&f=jpg', w: 670, h: 431 },
            xl: { q: 'w=811&h=518&q=80&m=6&f=jpg', w: 811, h: 518 }
        },
        disableLazyLoad: cmsImageSettings && cmsImageSettings.disableLazyLoad,
        lazyload: cmsImageSettings && cmsImageSettings.lazyload
    };

    return landscapeImageSettings;
}
function renderLabel(name?: string, price?: string, rating?: number, ratingAriaLabel?: string): string {
    name = name || '';
    price = price || '';
    return `${name} ${price} ${getRatingAriaLabel(rating, ratingAriaLabel)}`;
}

function renderDescription(description?: string): JSX.Element | null {
    return <p className='msc-product__text'>{description}</p>;
}

function renderQuickView(quickview: React.ReactNode, item?: number): JSX.Element | undefined {
    if (quickview === null) {
        return undefined;
    }
    return React.cloneElement(quickview as React.ReactElement, { selectedProductId: item });
}

function getRatingAriaLabel(rating?: number, ratingAriaLabel?: string): string {
    if (rating && ratingAriaLabel) {
        const roundedRating = rating.toFixed(2);
        return format(ratingAriaLabel || '', roundedRating, '5');
    }
    return '';
}

function updateProductUrl(productUrl: string, context: ICoreContext): string {
    const srcUrl = new URL(productUrl, context.request.apiSettings.baseUrl);
    const queryString = 'back=true';
    if (srcUrl.search) {
        srcUrl.search += `&${queryString}`;
    } else {
        srcUrl.search += queryString;
    }

    const updatedUrl = new URL(srcUrl.href);
    return updatedUrl.pathname + srcUrl.search;
}

function renderRating(
    context: ICoreContext,
    typeName: string,
    id: string,
    avgRating?: number,
    totalRatings?: number,
    ariaLabel?: string
): JSX.Element | null {
    if (!avgRating) {
        return null;
    }

    const numRatings = (totalRatings && totalRatings.toString()) || undefined;
    const ratingAriaLabel = getRatingAriaLabel(avgRating, ariaLabel);

    return (
        <RatingComponent
            context={context}
            id={id}
            typeName={typeName}
            avgRating={avgRating}
            ratingCount={numRatings}
            readOnly
            ariaLabel={ratingAriaLabel}
            data={{}}
        />
    );
}

function renderPrice(
    context: ICoreContext,
    typeName: string,
    id: string,
    basePrice?: number,
    adjustedPrice?: number,
    savingsText?: string,
    freePriceText?: string,
    originalPriceText?: string,
    currentPriceText?: string
): JSX.Element | null {
    const price: ProductPrice = {
        BasePrice: basePrice,
        AdjustedPrice: adjustedPrice,
        CustomerContextualPrice: adjustedPrice
    };

    return (
        <PriceComponent
            context={context}
            id={id}
            typeName={typeName}
            data={{ price }}
            savingsText={savingsText}
            freePriceText={freePriceText}
            originalPriceText={originalPriceText}
        />
    );
}

function renderProductPlacementImage(
    imageSettings?: IImageSettings,
    gridSettings?: IGridSettings,
    imageUrl?: string,
    altText?: string
): JSX.Element | null {
    if (!imageUrl || !gridSettings || !imageSettings) {
        return null;
    }
    const img: IImageData = {
        src: imageUrl,
        altText: altText ? altText : ''
    };
    const imageProps = {
        gridSettings,
        imageSettings
    };
    imageProps.imageSettings.cropFocalRegion = true;
    return <Image {...img} {...imageProps} loadFailureBehavior='empty' />;
}

export const ProductComponent: React.FunctionComponent<IProductComponentProps> = msdyn365Commerce.createComponentOverride<
    IProductComponent
>('Product', { component: ProductCard, ...PriceComponentActions });

export default ProductComponent;
