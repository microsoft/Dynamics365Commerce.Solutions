# Dynamics 365 Commerce - online extensibility samples

## License
License is listed in the [LICENSE](./LICENSE) file.

# Sample - Add to cart product on product card

## Overview
This sample will show how to add an "Add to cart" button on the product cards in the product collection and/or product list/category page. If an item is out of stock, it will show as unavailable.
![Overview](docs/image1.png)

## Starter kit license
License for starter kit is listed in the [LICENSE](./module-library/LICENSE) .

## Prerequisites
Follow the instructions mentioned in [document](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/setup-dev-environment) to set up the development environment.

### Procedure to create custom theme
Follow the instructions mentioned in [document](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/create-theme) to create the custom theme,in this sample, we'll assume a custom theme has been cloned from the fabrikam theme named "fabrikam-extended".

## Detailed Steps

### 1. Override product.component.tsx in the src/themes/fabrikam-extended/views/components
Override product.component.tsx component using this command **yarn msdyn365 add-component-override fabrikam-extended Product**.Open the new product.component.tsx under **src/themes/fabrikam-extended/views/components** and replace existing code with below code.In this file added logic to show addtocart button and out of stock.

```typescript

/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { IProductsDimensionsAvailabilities } from '@msdyn365-commerce/commerce-entities';
import {
    IComponent, IComponentProps, ICoreContext, IGridSettings,
    IImageData, IImageProps, IImageSettings, Image, IRequestContext, msdyn365Commerce
} from '@msdyn365-commerce/core';
import { AttributeSwatch, ProductDimension, ProductPrice, ProductSearchResult, ProductAvailableQuantity, SimpleProduct } from '@msdyn365-commerce/retail-proxy';
import {
    ArrayExtensions, checkIfShouldDisplayAsSwatch, convertDimensionTypeToProductDimensionType,
    Dictionary,
    DimensionSwatchDisplayTypes, DimensionTypes, generateImageUrl,
    getProductPageUrlSync, IDimensionsApp, StringExtensions
} from '@msdyn365-commerce-modules/retail-actions';
import { format, getPayloadObject, getTelemetryAttributes, ITelemetryContent, Node, onTelemetryClick } from '@msdyn365-commerce-modules/utilities';
import React, { useState } from 'react';

import { IPriceComponentResources, PriceComponent, ISwatchItem, RatingComponent, ProductComponentSwatchComponent, AddToCartComponent } from '@msdyn365-commerce/components';


export interface IProductComponentProps extends IComponentProps<{ product?: ProductSearchResult }> {
    className?: string;
    imageSettings?: IImageSettings;
    savingsText?: string;
    freePriceText?: string;
    originalPriceText?: string;
    currentPriceText?: string;
    ratingAriaLabel?: string;
    ratingCountAriaLabel?: string;
    allowBack?: boolean;
    telemetryContent?: ITelemetryContent;
    quickViewButton?: React.ReactNode;
    inventoryLabel?: string;
    isPriceMinMaxEnabled?: boolean;
    priceResources?: IPriceComponentResources;
    dimensionAvailabilities?: IProductsDimensionsAvailabilities[];
    swatchItemAriaLabel?: string;
}

export interface IProductComponent extends IComponent<IProductComponentProps> { }

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
    ratingCountAriaLabel,
    allowBack,
    typeName,
    id,
    telemetryContent,
    quickViewButton,
    inventoryLabel,
    isPriceMinMaxEnabled,
    priceResources,
    dimensionAvailabilities,
    swatchItemAriaLabel
}) => {
    const product = data.product;

    /**
     * Updates the product url link to product details page.
     * @param  productDetailsPageUrl - Product page url.
     * @param  coreContext - Context of the module using the component.
     * @param  queryString - Querystring to be added to the URL.
     * @returns The update product page url.
     */
    function updateProductUrl(productDetailsPageUrl: string, coreContext: ICoreContext, queryString: string): string {
        const sourceUrl = new URL(productDetailsPageUrl, coreContext.request.apiSettings.baseUrl);
        if (sourceUrl.search) {
            sourceUrl.search += `&${queryString}`;
        } else {
            sourceUrl.search += queryString;
        }

        const updatedUrl = new URL(sourceUrl.href);
        return updatedUrl.pathname + sourceUrl.search;
    }

    /**
     * Gets the product page url from the default swatch selected.
     * @param  productData - Product card to be rendered.
     * @returns The default color swatch selected if any.
     */
    function getDefaultColorSwatchSelected(productData?: ProductSearchResult): AttributeSwatch | null {
        if (!productData || !productData.AttributeValues) {
            return null;
        }

        const colorAttribute = productData.AttributeValues.find(attributeValue => attributeValue.KeyName?.toLocaleLowerCase() === DimensionTypes.color);
        if (!ArrayExtensions.hasElements(colorAttribute?.Swatches)) {
            return null;
        }

        const defaultSwatch = colorAttribute!.Swatches.find(item => item.IsDefault === true) ?? colorAttribute!.Swatches[0];
        return defaultSwatch;
    }

    /**
     * Gets the product image from the default swatch selected.
     * @param  coreContext - Context of the module using the component.
     * @param  productData - Product card to be rendered.
     * @returns The product card image url.
     */
    function getProductImageUrlFromDefaultColorSwatch(coreContext: ICoreContext, productData?: ProductSearchResult): string | undefined {
        const siteContext = coreContext as ICoreContext<IDimensionsApp>;
        const dimensionToPreSelectInProductCard = siteContext.app.config.dimensionToPreSelectInProductCard;
        if (dimensionToPreSelectInProductCard === DimensionTypes.none) {
            return productData?.PrimaryImageUrl;
        }
        const defaultSwatch = getDefaultColorSwatchSelected(productData);
        return defaultSwatch && ArrayExtensions.hasElements(defaultSwatch.ProductImageUrls) ? generateImageUrl(
            defaultSwatch.ProductImageUrls[0], coreContext.request.apiSettings) : productData?.PrimaryImageUrl;
    }

    /**
     * Gets the product page url from the default swatch selected.
     * @param  coreContext - Context of the module using the component.
     * @param productUrl - Product page url for the product card.
     * @param  productData - Product card to be rendered.
     * @returns The product card image url.
     */
    function getProductPageUrlFromDefaultSwatch(coreContext: ICoreContext, productUrl: string, productData?: ProductSearchResult): string | undefined {
        const siteContext = coreContext as ICoreContext<IDimensionsApp>;
        const dimensionToPreSelectInProductCard = siteContext.app.config.dimensionToPreSelectInProductCard;
        if (dimensionToPreSelectInProductCard === DimensionTypes.none) {
            return productUrl;
        }
        const defaultSwatch = getDefaultColorSwatchSelected(productData);
        if (!defaultSwatch || !defaultSwatch.SwatchValue) {
            return productUrl;
        }

        const queryString = `color=${defaultSwatch.SwatchValue}`;
        return updateProductUrl(productUrl, coreContext, queryString);
    }

    let productUrl = product ? getProductPageUrlSync(product.Name ?? '', product.RecordId, context.actionContext, undefined) : '';
    if (allowBack && productUrl) {
        productUrl = updateProductUrl(productUrl, context, 'back=true');
    }
    const productImageUrlFromSwatch = getProductImageUrlFromDefaultColorSwatch(context, product) ?? product?.PrimaryImageUrl;
    const productPageUrlFromSwatch = getProductPageUrlFromDefaultSwatch(context, productUrl, product) ?? productUrl;
    const [productPageUrl, setProductPageUrl] = useState<string>(productPageUrlFromSwatch);
    const [productImageUrl, setProductImageUrl] = useState<string | undefined>(productImageUrlFromSwatch);
    const [selectedSwatchItems] = useState(new Dictionary<DimensionTypes, ISwatchItem>());
    // eslint-disable-next-line @typescript-eslint/no-unsafe-assignment,@typescript-eslint/no-unsafe-member-access -- app context is generic
    const enableStockCheck = context.app.config.enableStockCheck;

    /**
     * Updates the product page and Image url based on swatch selected.
     * @param coreContext - Context of the caller.
     * @param swatchItem - Dimension swatch selected.
     */
    const updatePageAndImageUrl = React.useCallback((coreContext: ICoreContext, swatchItem: ISwatchItem) => {
        const dimensionType = swatchItem.dimensionType;
        selectedSwatchItems.setValue(dimensionType, swatchItem);
        if (StringExtensions.isNullOrWhitespace(swatchItem.value)) {
            return;
        }
        const queryString = `${dimensionType}=${swatchItem.value}`;
        let productPageUrlWithSwatch = '';
        if (productPageUrl.includes(dimensionType)) {
            const newUrl = new URL(productPageUrl, coreContext.request.apiSettings.baseUrl);
            newUrl.searchParams.delete(dimensionType);
            productPageUrlWithSwatch = updateProductUrl(newUrl.toString(), context, queryString);
        } else {
            productPageUrlWithSwatch = updateProductUrl(productPageUrl, context, queryString);
        }
        setProductPageUrl(productPageUrlWithSwatch);
        if (dimensionType === DimensionTypes.color) {
            const swatchProductImageUrl = ArrayExtensions.hasElements(swatchItem.productImageUrls) ? swatchItem.productImageUrls[0] : undefined;
            const newImageUrl = generateImageUrl(swatchProductImageUrl, coreContext.request.apiSettings);
            setProductImageUrl(newImageUrl);
        }
    }, [selectedSwatchItems, context, productPageUrl]);

    if (!product) {
        return null;
    }

    const swatchItems = ArrayExtensions.validValues(product.AttributeValues?.map(item => {
        const dimensionTypeValue = item.KeyName?.toLocaleLowerCase() ?? '';
        const shouldDisplayAsSwatch = checkIfShouldDisplayAsSwatch(
            dimensionTypeValue as DimensionTypes,
            context as ICoreContext<IDimensionsApp>,
            DimensionSwatchDisplayTypes.productCard);
        if (!shouldDisplayAsSwatch) {
            return null;
        }

        const siteContext = context as ICoreContext<IDimensionsApp>;
        const dimensionToPreSelectInProductCard = siteContext.app.config.dimensionToPreSelectInProductCard;
        const dimensionType = dimensionTypeValue as DimensionTypes;
        const swatches = item.Swatches?.map<ISwatchItem>(swatchItem => {
            return {
                itemId: `${item.RecordId ?? ''}-${dimensionTypeValue}-${swatchItem.SwatchValue ?? ''}`,
                value: swatchItem.SwatchValue ?? '',
                dimensionType,
                colorHexCode: swatchItem.SwatchColorHexCode,
                imageUrl: swatchItem.SwatchImageUrl,
                productImageUrls: swatchItem.ProductImageUrls,
                isDefault: swatchItem.IsDefault,
                swatchItemAriaLabel: swatchItemAriaLabel ? format(swatchItemAriaLabel, dimensionType) : '',
                isDisabled: enableStockCheck && dimensionAvailabilities?.find(
                    dimensionAvailability => dimensionAvailability.value === (swatchItem.SwatchValue ?? ''))?.isDisabled
            };
        }) ?? [];
        if (dimensionToPreSelectInProductCard !== DimensionTypes.none && ArrayExtensions.hasElements(swatches) &&
            !swatches.some(swatch => swatch.isDefault) && dimensionType === DimensionTypes.color) {
            swatches[0].isDefault = true;
        }
        return { recordId: item.RecordId, swatches };
    }));

    // Construct telemetry attribute to render
    const payLoad = getPayloadObject('click', telemetryContent!, '', product.RecordId.toString());

    const attribute = getTelemetryAttributes(telemetryContent!, payLoad);

    // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access -- -- Do not need type check for appsettings
    const isUnitOfMeasureEnabled = context.app.config && context.app.config.unitOfMeasureDisplayType === 'buyboxAndBrowse';

    /**
     * Gets the react node for product unit of measure display.
     * @param  unitOfMeasure - DefaultUnitOfMeasure property from product.
     * @returns The node representing markup for unit of measure component.
     */
    function renderProductUnitOfMeasure(unitOfMeasure?: string): JSX.Element | null {

        if (!unitOfMeasure) {
            return null;
        }
        return (
            <div className='msc-product__unit-of-measure'>
                <span>
                    {unitOfMeasure}
                </span>
            </div>
        );
    }

    /**
     * Gets the react node for product availability.
     * @param inventoryAvailabilityLabel - The product information.
     * @returns The node representing markup for product availability.
     */
    function renderProductAvailability(inventoryAvailabilityLabel: string | undefined): JSX.Element | null {
        if (!inventoryAvailabilityLabel || inventoryAvailabilityLabel === '') {
            return null;
        }

        return (
            <div className='msc-product__availability'>
                <span>
                    {inventoryAvailabilityLabel}
                </span>
            </div>
        );
    }

    /**
     * Gets the react node for product dimension as swatch.
     * @returns The node representing markup for unit of measure component.
     */
    function renderProductDimensions(): JSX.Element | null {
        if (!ArrayExtensions.hasElements(swatchItems)) {
            return null;
        }

        return (
            <div className='msc-product__dimensions'>
                {
                    swatchItems.map(item => {
                        return (
                            <ProductComponentSwatchComponent
                                key={item.recordId}
                                context={context}
                                onSelectDimension={updatePageAndImageUrl}
                                swatches={item.swatches}
                            />
                        );
                    })
                }
            </div>
        );
    }

    /**
     * Gets the react node for  product description.
     * @param  quickview - Quick view node.
     * @param  item - Product id to de displayed in quickview.
     * @returns The product quickview component.
     */
    function renderQuickView(quickview: React.ReactNode, item?: number): JSX.Element | undefined {
        if (quickview === null) {
            return undefined;
        }
        const selectedDimensions: ProductDimension[] = selectedSwatchItems.getValues().map<ProductDimension>(swatches => {
            return {
                DimensionTypeValue: convertDimensionTypeToProductDimensionType(swatches.dimensionType),
                DimensionValue: {
                    RecordId: 0,
                    Value: swatches.value
                }
            };
        });
        return React.cloneElement(quickview as React.ReactElement, { selectedProductId: item, selectedDimensions });
    }

    /**
     * Gets the aria label for rating.
     * @param  rating - Product rating.
     * @param  ratingAriaLabelText - Aria label format for rating.
     * @returns The product rating aria label string.
     */
    function getRatingAriaLabel(rating?: number, ratingAriaLabelText?: string): string {
        if (rating && ratingAriaLabelText) {
            const roundedRating = rating.toFixed(2);
            return format(ratingAriaLabelText || '', roundedRating, '5');
        }
        return '';
    }

    /**
     * Gets the aria label for review count.
     * @param  reviewCount - Product review count.
     * @param  ratingCountAriaLabelText - Aria label format for review.
     * @returns The product review count aria label string.
     */
    function getReviewAriaLabel(reviewCount?: number, ratingCountAriaLabelText?: string): string {
        if (reviewCount && ratingCountAriaLabelText) {
            return format(ratingCountAriaLabelText || '', reviewCount);
        }
        return '';
    }

    /**
     * Gets the aria label string for product that includes product name with its price and rating.
     * @param  name - Product name.
     * @param  price - Product price.
     * @param  rating - Product rating.
     * @param  ratingAriaLabelText - Rating aria label text.
     * @param  reviewCount - Product review count.
     * @param  ratingCountAriaLabelText - Number of ratings.
     * @returns The aria label string for the product card.
     */
    function renderLabel(
        name?: string,
        price?: string,
        rating?: number,
        ratingAriaLabelText?: string,
        reviewCount?: number,
        ratingCountAriaLabelText?: string): string {
        const reviewCountArialableText = getReviewAriaLabel(reviewCount, ratingCountAriaLabelText ?? '');
        return (
            `${name ?? ''} ${price ?? ''} ${getRatingAriaLabel(rating, ratingAriaLabelText)}${reviewCountArialableText ? ` ${reviewCountArialableText}` : ''}`
        );
    }

    /**
     * Gets the react component for product rating.
     * @param  productCardimageSettings - Module image settings for product card.
     * @param  gridSettings - Grid settings defined in theme.
     * @param  imageUrl - Image url.
     * @param fallbackImageUrl - Fallback url for imge.
     * @param  altText - Image Alt text.
     * @param  requestContext - Request context using the component.
     * @returns React component for product image.
     */
    function renderProductPlacementImage(
        productCardimageSettings?: IImageSettings, gridSettings?: IGridSettings,
        imageUrl?: string, fallbackImageUrl?: string, altText?: string, requestContext?: IRequestContext): JSX.Element | null {

        if (!imageUrl || !gridSettings || !productCardimageSettings) {
            return null;
        }
        const image: IImageData = {
            src: imageUrl,
            altText: altText ? altText : '',
            fallBackSrc: fallbackImageUrl
        };
        const imageProps: IImageProps = { gridSettings };
        imageProps.gridSettings = gridSettings;
        imageProps.imageSettings = productCardimageSettings;
        imageProps.imageSettings.cropFocalRegion = true;
        return (
            <Image
                {...image} {...imageProps} loadFailureBehavior='empty'
                requestContext={requestContext}
            />
        );
    }

    /**
     * Gets the react component for product rating.
     * @param  coreContext - Context of the module using the component.
     * @param  moduleTypeName - Module type name.
     * @param  moduleId - Module id using the component.
     * @param  basePrice - Product base price.
     * @param  adjustedPrice - Product adjusted price.
     * @param  maxVariantPrice - Product variant max price.
     * @param  minVariantPrice - Product variant min price.
     * @param  savingsPriceResourceText - Product price saving text.
     * @param  freePriceResourceText - Product price free text.
     * @param  originalPriceResourceText - Product price original text.
     * @param  currentPriceResourceText - Product price current text.
     * @returns React component for Product price.
     */
    function renderPrice(coreContext: ICoreContext, moduleTypeName: string, moduleId: string, basePrice?: number, adjustedPrice?: number,
        maxVariantPrice?: number, minVariantPrice?: number, savingsPriceResourceText?: string, freePriceResourceText?: string,
        originalPriceResourceText?: string, currentPriceResourceText?: string): JSX.Element | null {
        const price: ProductPrice = {
            BasePrice: basePrice,
            AdjustedPrice: adjustedPrice,
            CustomerContextualPrice: adjustedPrice,
            MaxVariantPrice: maxVariantPrice ? maxVariantPrice : adjustedPrice,
            MinVariantPrice: minVariantPrice ? minVariantPrice : adjustedPrice
        };

        return (
            <PriceComponent
                context={coreContext}
                id={moduleId}
                typeName={moduleTypeName}
                data={{ price }}
                savingsText={savingsPriceResourceText}
                freePriceText={freePriceResourceText}
                originalPriceText={originalPriceResourceText}
                currentPriceText={currentPriceResourceText}
                isPriceMinMaxEnabled={isPriceMinMaxEnabled}
                priceResources={priceResources}
            />
        );
    }

    /**
     * Gets the react node for product description.
     * @param  description - Product description.
     * @returns The product description component.
     */
    function renderDescription(description?: string): JSX.Element | null {
        return (<p className='msc-product__text'>
            {description}
        </p>);
    }

    /**
     * Gets the react component for product rating.
     * @param  coreContext - Context of the module using the component.
     * @param  moduleTypeName - Module type name.
     * @param  moduleId - Module id using the component.
     * @param  avgRating - Average rating.
     * @param  totalRatings - Total rating.
     * @param  ariaLabel - Aria label for rating.
     * @returns React component for Product rating.
     */
    function renderRating(coreContext: ICoreContext, moduleTypeName: string, moduleId: string, avgRating?: number,
        totalRatings?: number, ariaLabel?: string): JSX.Element | null {
        if (!avgRating) {
            return null;
        }

        const numberRatings = totalRatings?.toString() || undefined;
        const ratingAriaLabelText = getRatingAriaLabel(avgRating, ariaLabel);
        const ratingCountAriaLabelText = getReviewAriaLabel(Number(numberRatings), ratingCountAriaLabel);

        return (
            <RatingComponent
                context={coreContext}
                id={moduleId}
                typeName={moduleTypeName}
                avgRating={avgRating}
                ratingCount={numberRatings}
                readOnly
                ariaLabel={ratingAriaLabelText}
                ratingCountAriaLabel={ratingCountAriaLabelText}
                data={{}}
            />
        );
    }
    const _renderCartButton = (): JSX.Element => {
        const product1: SimpleProduct = {
            RecordId: data.product?.RecordId!,
            ItemId: data.product?.ItemId,
            Name: data.product?.Name,
            Description: data.product?.Description,
            ProductTypeValue: data.product?.RecordId!,
            DefaultUnitOfMeasure: data.product?.DefaultUnitOfMeasure,
            BasePrice: data.product?.BasePrice!,
            Price: data.product?.Price!,
            AdjustedPrice: data.product?.Price!,
            MasterProductId: data.product?.MasterProductId,
            PrimaryImageUrl: data.product?.PrimaryImageUrl,
            ExtensionProperties: data.product?.ExtensionProperties
        };
        const productDimensionAvailabilities =
            dimensionAvailabilities &&
            dimensionAvailabilities?.find(
                dimensionAvailability =>
                    ArrayExtensions.hasElements(dimensionAvailabilities) &&
                    dimensionAvailability.masterProductId &&
                    dimensionAvailability.masterProductId === product.MasterProductId
            );

        const ProductAvailableQuantity: ProductAvailableQuantity = {
            AvailableQuantity: 1
        };
        return (
            <Node className='ms-search-result-container__add-to-cart-container'>
                <AddToCartComponent
                    addToCartText='Add to cart'
                    outOfStockText='Out of stock'
                    hasAvailableProducts={productDimensionAvailabilities?.isDisabled}
                    productAvailability={ProductAvailableQuantity}
                    data={{ product: product1 }}
                    context={context}
                    id={id}
                    typeName={typeName}
                />
            </Node>
        );
    };
    return (
        <>
            <a
                href={productPageUrl} onClick={onTelemetryClick(telemetryContent!, payLoad, product.Name!)}
                aria-label={renderLabel(
                    product.Name,
                    context.cultureFormatter.formatCurrency(product.Price),
                    product.AverageRating, ratingAriaLabel,
                    product.TotalRatings, ratingCountAriaLabel)}
                className='msc-product' {...attribute}>
                <div className='msc-product__image'>
                    {renderProductPlacementImage(
                        imageSettings, context.request.gridSettings, productImageUrl,
                        product.PrimaryImageUrl, product.Name, context.actionContext.requestContext)}
                </div>
                <h4 className='msc-product__title'>
                    {product.Name}
                </h4>
            </a>
            {renderProductDimensions()}
            <div className='msc-product__details'>
                {renderPrice(context, typeName, id, product.BasePrice, product.Price,
                    product.MaxVariantPrice, product.MinVariantPrice, savingsText, freePriceText, originalPriceText, currentPriceText)}
                {isUnitOfMeasureEnabled && renderProductUnitOfMeasure(product.DefaultUnitOfMeasure)}
                {renderDescription(product.Description)}
                {!context.app.config.hideRating && renderRating(context, typeName, id, product.AverageRating, product.TotalRatings, ratingAriaLabel)}
                {renderProductAvailability(inventoryLabel)}
            </div>
            {_renderCartButton()}
            {quickViewButton && renderQuickView(quickViewButton, product.RecordId)}
        </>
    );
};

export const ProductComponent: React.FunctionComponent<IProductComponentProps> = msdyn365Commerce.createComponentOverride<IProductComponent>(
    'Product',
    { component: ProductCard, ...PriceComponentActions }
);


export default ProductComponent;

```
## 2. Build and test module

The sample can now be tested in a web browser using the ```yarn start``` command.

### 1.Test by using mock file

Please refer mock file under the **src/pageMocks/search-result.json**, [pageMock](src/pageMocks/search-result.json). 

Verify the changes by running the application with mock and theme (http://localhost:4000/page?mock=search-result&theme=fabrikam-extended)

## Third party Image and Video Usage restrictions

The software may include third party images and videos that are for personal use only and may not be copied except as provided by Microsoft within the demo websites.  You may install and use an unlimited number of copies of the demo websites., You may not publish, rent, lease, lend, or redistribute any images or videos without authorization from the rights holder, except and only to the extent that the applicable copyright law expressly permits doing so.
