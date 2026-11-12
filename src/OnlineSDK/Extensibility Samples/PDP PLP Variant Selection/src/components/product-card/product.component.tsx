/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { IProductsDimensionsAvailabilities } from '@msdyn365-commerce/commerce-entities';
import { generateImageUrl, getProductPageUrlSync } from '@msdyn365-commerce-modules/retail-actions';
import { format, ITelemetryContent } from '@msdyn365-commerce-modules/utilities';
import { PriceComponent, RatingComponent, IPriceComponentResources } from '@msdyn365-commerce/components';
import MsDyn365, {
    IComponent,
    IComponentProps,
    ICoreContext,
    IGridSettings,
    IImageData,
    IImageSettings,
    Image
} from '@msdyn365-commerce/core';
import { ProductPrice, ProductSearchResult } from '@msdyn365-commerce/retail-proxy';
import { AttributeValue } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import classNames from 'classnames';
import React from 'react';

export interface IProductComponentProps extends IComponentProps<{ product?: ProductSearchResult }> {
    className?: string;
    imageSettings?: IImageSettings;
    savingsText?: string;
    freePriceText?: string;
    originalPriceText?: string;
    currentPriceText?: string;
    inStoreOnlyText?: string;
    enableInStoreOnly?: boolean;
    ratingAriaLabel?: string;
    allowBack?: boolean;
    imgConfig: string;
    includeProductNavWithDimIdConfig: boolean;
    telemetryContent?: ITelemetryContent;
    quickViewButton?: React.ReactNode;
    inventoryLabel?: string;
    isPriceMinMaxEnabled?: boolean;
    priceResources?: IPriceComponentResources;
    dimensionAvailabilities?: IProductsDimensionsAvailabilities[];
    swatchItemAriaLabel?: string;
}

export interface IProductComponent extends IComponent<IProductComponentProps> {}

export interface IColorSwatchItem {
    hex: string;
    id: string;
    name: string;
    url: string;
}

interface IProductCardState {
    selectedItemId: string;
    imageUrl: string;
    productId: number | undefined;
    autoSelectedItemId: string;
    productUrlWithDimid: string;
}

/**
 *
 * BuyboxDimensions component
 * @extends {React.PureComponent<IProductComponentProps>}
 */

/**
 *
 */
export default class ProductCard extends React.PureComponent<IProductComponentProps, IProductCardState> {
    private productUrl: string;
    constructor(props: IProductComponentProps) {
        super(props);
        this.productUrl = '';
        this.state = {
            selectedItemId: '',
            imageUrl: '',
            productId: undefined,
            autoSelectedItemId: '',
            productUrlWithDimid: ''
        };
    }

    // tslint:disable-next-line:max-func-body-length cyclomatic-complexity

    // eslint-disable-next-line complexity
    public render(): JSX.Element | null {
        const {
            data,
            context,
            imageSettings,
            savingsText,
            freePriceText,
            originalPriceText,
            currentPriceText,
            inStoreOnlyText,
            enableInStoreOnly,
            ratingAriaLabel,
            allowBack,
            typeName,
            id
        } = this.props;
        const product = data.product;
        if (!product) {
            return null;
        }

        let productUrl = getProductPageUrlSync(product.Name || '', product.RecordId, context && context.actionContext, undefined);
        if (allowBack) {
            productUrl = this.updateProductUrl(productUrl, context);
        }
        this.productUrl = productUrl;

        // Construct telemetry attribute to render
        const attribute =
            context &&
            context.telemetry &&
            context.telemetry.setTelemetryAttribute &&
            context.telemetry.setTelemetryAttribute(product.RecordId.toString(), {
                pid: product.RecordId,
                pname: product.Name,
                mname: id
            });
        const colorHexMappingRes: AttributeValue[] =
            (product.AttributeValues && product.AttributeValues.filter(x => x.Name?.toLocaleLowerCase() === 'colorhexmapping')) || [];
        const colorHexMapping =
            colorHexMappingRes &&
            colorHexMappingRes[0] &&
            colorHexMappingRes[0].TextValue &&
            this._getParsedJson(colorHexMappingRes[0].TextValue);
        const colorImageVariantMappingRes: AttributeValue[] =
            (product.AttributeValues && product.AttributeValues.filter(x => x.Name?.toLocaleLowerCase() === 'colorimagevariantmapping')) ||
            [];
        const colorImageVariantMapping =
            colorImageVariantMappingRes &&
            colorImageVariantMappingRes[0] &&
            colorImageVariantMappingRes[0].TextValue &&
            this._getParsedJson(colorImageVariantMappingRes[0].TextValue);
        const colorImgMapping = [];

        // converting object keys to lower case
        /* eslint-disable  @typescript-eslint/no-explicit-any */
        let colorHexMappingObj: any[] = [];
        if (colorHexMapping && colorHexMapping.length > 0) {
            // tslint:disable-next-line:no-any
            colorHexMappingObj = colorHexMapping.map((item: any) => {
                return (
                    // tslint:disable-next-line:ban-comma-operator
                    // eslint-disable-next-line no-sequences
                    Object.keys(item).reduce((c, k) => ((c[k.toLowerCase()] = item[k]), c), {})
                );
            });
        }

        // converting object keys to lower case
        /* eslint-disable  @typescript-eslint/no-explicit-any */
        let colorImageVariantMappingObj: any[] = [];
        if (colorImageVariantMapping && colorImageVariantMapping.length > 0) {
            // tslint:disable-next-line:no-any
            colorImageVariantMappingObj = colorImageVariantMapping.map((item: any, index: number) => {
                return (
                    // tslint:disable-next-line:ban-comma-operator
                    // eslint-disable-next-line no-sequences
                    Object.keys(item).reduce((c, k) => ((c[k.toLowerCase()] = item[k]), c), {})
                );
            });
        }

        // combining colorHexMappingObj and colorImageVariantMappingObj into single object
        if (colorHexMappingObj && colorHexMappingObj.length) {
            for (let i = 0; i < colorHexMappingObj.length; i++) {
                colorImgMapping.push({
                    ...colorHexMappingObj[i],
                    // use 'id' as lowercase as we are converting the keys to lowercase
                    // tslint:disable-next-line:no-any
                    // eslint-disable-next-line @typescript-eslint/no-explicit-any
                    ...colorImageVariantMappingObj.find((item: any) => item.id === colorHexMappingObj[i].id)
                });
            }
        }

        let imgURL = '';
        let hasSale = false;

        if (product.BasePrice && product.Price) {
            hasSale = product.BasePrice > product.Price;
        }

        // use 'url' as lowercase as we are converting the keys to lowercase
        if (
            colorImgMapping &&
            colorImgMapping.length > 0 &&
            colorImgMapping[0].hasOwnProperty('url') &&
            colorImgMapping[0].hasOwnProperty('id')
        ) {
            // @ts-ignore making this.props.context.request as type IRequestContext as this.props.context interface doesnot have requestContext
            // tslint:disable-next-line:prefer-template restrict-plus-operands
            imgURL =
                generateImageUrl(
                    `Products/${colorImgMapping[0]?.url.toString().replace(/ /g, '')}`,
                    this.props.context.request.apiSettings
                ) || 'undefined';
            this.setState({ autoSelectedItemId: colorImgMapping[0].id });
            this.setProductUrl();
        }
        return (
            <div
                aria-label={this.renderLabel(
                    product.Name,
                    context.cultureFormatter.formatCurrency(product.Price),
                    product.AverageRating,
                    ratingAriaLabel
                )}
                className={classNames('msc-product', hasSale ? 'has-sale' : '')}
                {...attribute}
            >
                <a href={this.state.productUrlWithDimid || productUrl}>
                    <div onClick={this.productNavigation(productUrl)} role='link' className='msc-product__image'>
                        {this.renderProductPlacementImage(
                            imageSettings,
                            context.request.gridSettings,
                            imgURL || product.PrimaryImageUrl,
                            product.Name,
                            product.RecordId
                        )}
                    </div>
                </a>
                {this.renderColorSwatch(product.RecordId, colorImgMapping)}
                <a href={this.state.productUrlWithDimid || productUrl}>
                    <div onClick={this.productNavigation(productUrl)} role='link' className='msc-product__details'>
                        <h4 className='msc-product__title'>{product.Name}</h4>
                        {this.renderPrice(
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
                        {this.renderDescription(product.Description)}
                        {enableInStoreOnly &&
                            // IsInStoreOnlyFromProductAttributes(product.AttributeValues, this.props) &&
                            this.renderInStoreOnly(inStoreOnlyText)}
                        {!context.app.config.hideRating &&
                            this.renderRating(context, typeName, id, product.AverageRating, product.TotalRatings, ratingAriaLabel)}
                    </div>
                </a>
            </div>
        );
    }

    /* eslint-disable  @typescript-eslint/no-explicit-any */
    private productNavigation = (productUrl?: string) => (e: any): any => {
        let productUrlDimid = productUrl;
        if (this.props.includeProductNavWithDimIdConfig) {
            const qsp = 'dimid';
            if (this.state.selectedItemId !== '') {
                productUrlDimid = `${productUrl}${productUrl && productUrl.indexOf('?') === -1 ? '?' : '&'}${qsp}=${
                    this.state.selectedItemId
                }`;
            } else {
                productUrlDimid = `${productUrl}${productUrl && productUrl.indexOf('?') === -1 ? '?' : '&'}${qsp}=${
                    this.state.autoSelectedItemId
                }`;
            }
        }
        MsDyn365.isBrowser && window.location.assign(productUrlDimid || '');
    };

    private setProductUrl = (): void => {
        let productUrlDimid = this.productUrl;
        if (productUrlDimid && this.props.includeProductNavWithDimIdConfig) {
            const qsp = 'dimid';
            if (this.state.selectedItemId !== '') {
                productUrlDimid = `${productUrlDimid}${productUrlDimid && productUrlDimid.indexOf('?') === -1 ? '?' : '&'}${qsp}=${
                    this.state.selectedItemId
                }`;
            } else {
                productUrlDimid = `${productUrlDimid}${productUrlDimid && productUrlDimid.indexOf('?') === -1 ? '?' : '&'}${qsp}=${
                    this.state.autoSelectedItemId
                }`;
            }
        }
        productUrlDimid && this.setState({ productUrlWithDimid: productUrlDimid });
    };

    // tslint:disable-next-line:no-any
    private renderColorSwatch = (productId: number, colorSwatchItems?: any): JSX.Element | null => {
        if (colorSwatchItems) {
            let bgColor: React.CSSProperties = {};
            return (
                <ul className={`custom-swatches`}>
                    {colorSwatchItems
                        ? // tslint:disable-next-line:no-any
                          colorSwatchItems.map((item: any, index: number) => {
                              if (index > 9) {
                                  return;
                              }
                              // use 'hex' as lowercase as we are converting the keys to lowercase
                              bgColor = { backgroundColor: item.hex };
                              let isActive = false;
                              if (this.state.selectedItemId !== '') {
                                  isActive = item.id === this.state.selectedItemId;
                              } else {
                                  isActive = item.id === this.state.autoSelectedItemId;
                              }
                              return (
                                  <li
                                      key={index}
                                      className={isActive ? `color-swatch selected` : `color-swatch`}
                                      onClick={this.generateOnClick(item, productId)}
                                      role='button'
                                  >
                                      <button style={bgColor || {}} />
                                  </li>
                              );
                          })
                        : false}
                </ul>
            );
        } else {
            return <div className='color-swatch-empty' />;
        }
    };

    private renderInStoreOnly = (label: string | undefined): JSX.Element | null => {
        if (label) {
            return <div className='in-store-only'>{label}</div>;
        }
        return null;
    };

    // tslint:disable-next-line:no-any
    private generateOnClick = (item: IColorSwatchItem, productId: number) => (e: any): any => {
        Object.keys(item)
            .sort()
            .forEach(key => {
                item[key] = item[key];
            });
        if (item && item.hasOwnProperty('url')) {
            // @ts-ignore making this.props.context.request as type IRequestContext as this.props.context interface doesnot have requestContext
            // tslint:disable-next-line:prefer-template
            const imgURL = generateImageUrl(`Products/${item.url}`, this.props.context.request.apiSettings) || 'undefined';
            this.setState({ selectedItemId: item.id, imageUrl: imgURL, productId: productId });
        } else {
            if (item && Object.values(item)[2]) {
                const imageName: string = Object.values(item)[2].trim();
                const imgURL = this.props.imgConfig + imageName || 'undefined';
                this.setState({ selectedItemId: item.id, imageUrl: imgURL, productId: productId });
            }
        }
        this.setProductUrl();
    };

    private renderLabel = (name?: string, price?: string, rating?: number, ratingAriaLabel?: string): string => {
        name = name || '';
        price = price || '';
        return `${name} ${price} ${this.getRatingAriaLabel(rating, ratingAriaLabel)}`;
    };

    private renderDescription = (description?: string): JSX.Element | null => {
        return <p className='msc-product__text'>{description}</p>;
    };

    private getRatingAriaLabel = (rating?: number, ratingAriaLabel?: string): string => {
        if (rating && ratingAriaLabel) {
            const roundedRating = rating.toFixed(2);
            return format(ratingAriaLabel || '', roundedRating, '5');
        }
        return '';
    };

    private updateProductUrl = (productUrl: string, context: ICoreContext): string => {
        const srcUrl = new URL(productUrl, context.request.apiSettings.baseUrl);
        const queryString = `back=true`;
        if (srcUrl.search) {
            srcUrl.search += `&${queryString}`;
        } else {
            srcUrl.search += queryString;
        }

        const updatedUrl = new URL(srcUrl.href);
        return updatedUrl.pathname + srcUrl.search;
    };

    private renderRating = (
        context: ICoreContext,
        typeName: string,
        id: string,
        avgRating?: number,
        totalRatings?: number,
        ariaLabel?: string
    ): JSX.Element | null => {
        if (!avgRating) {
            return null;
        }

        const numRatings = (totalRatings && totalRatings.toString()) || undefined;
        const ratingAriaLabel = this.getRatingAriaLabel(avgRating, ariaLabel);

        return (
            <RatingComponent
                context={context}
                id={id}
                typeName={typeName}
                avgRating={avgRating}
                ratingCount={numRatings}
                readOnly={true}
                ariaLabel={ratingAriaLabel}
                data={{}}
            />
        );
    };

    private renderPrice = (
        context: ICoreContext,
        typeName: string,
        id: string,
        basePrice?: number,
        adjustedPrice?: number,
        savingsText?: string,
        freePriceText?: string,
        originalPriceText?: string,
        currentPriceText?: string
    ): JSX.Element | null => {
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
                data={{ price: price }}
                savingsText={savingsText}
                freePriceText={freePriceText}
                originalPriceText={originalPriceText}
            />
        );
    };

    private renderProductPlacementImage = (
        imageSettings?: IImageSettings,
        gridSettings?: IGridSettings,
        imageUrl?: string,
        altText?: string,
        productId?: number
    ): JSX.Element | null => {
        if (!imageUrl || !gridSettings || !imageSettings) {
            return null;
        }
        let imgSrc = '';
        if (this.state.productId !== undefined && productId === this.state.productId) {
            imgSrc = this.state.imageUrl !== '' ? this.state.imageUrl : imageUrl;
        } else {
            imgSrc = imageUrl;
        }
        const img: IImageData = {
            src: imgSrc,
            altText: altText ? altText : ''
        };
        const imageProps = {
            gridSettings: gridSettings,
            imageSettings: imageSettings
        };
        return (
            <div className={'msc-empty_image-placeholder'}>
                <Image {...img} {...imageProps} loadFailureBehavior='empty' />
            </div>
        );
    };

    // tslint:disable-next-line: no-any
    private _getParsedJson = (text: string): any => {
        try {
            return JSON.parse(text);
        } catch (error) {
            this.props.context.telemetry.warning('Unable to parse text');
            this.props.context.telemetry.error(error);
            return '';
        }
    };
}
