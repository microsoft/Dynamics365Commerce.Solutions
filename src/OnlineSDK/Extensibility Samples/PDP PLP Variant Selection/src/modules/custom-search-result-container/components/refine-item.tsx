/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { IProductRefinerHierarchy } from '@msdyn365-commerce/commerce-entities';
import { RatingComponent, SwatchComponent } from '@msdyn365-commerce/components';
import { IAny, ICoreContext, IGeneric } from '@msdyn365-commerce/core';
import { ProductRefinerSource, ProductRefinerValue, RefinerType } from '@msdyn365-commerce/retail-proxy';
import { checkIfShouldDisplayAsSwatch, DimensionTypes, IDimensionsApp, StringExtensions } from '@msdyn365-commerce-modules/retail-actions';
import { format, getPayloadObject, getTelemetryAttributes, IPayLoad, ITelemetryContent } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { IRefineItemCommonProps } from './refine-item.props.common';
import { IRefineItemToggleNotification } from './refine-item-toggle-notification';

/**
 * RefineItem properties.
 */
export interface IRefineItemProps {
    parentProductRefinerHierarchy: IProductRefinerHierarchy;
    productRefinerValue: ProductRefinerValue;
    selectedRefinementCriterion: ProductRefinerValue | undefined;
    selectedRefinerValues?: ProductRefinerValue[];
    refineItemCommonProps: IRefineItemCommonProps;
    isDisabled: boolean;
    context: ICoreContext<IGeneric<IAny>>;
    moduleId: string;
    moduleTypeName: string;
    index?: number;
    productCountAriaLabel?: string;
    singleProductCountAriaLabel?: string;
    isMobileView?: boolean;

    /**
     * The telemetry content
     */
    telemetryContent?: ITelemetryContent;
    onToggle(notfication: Readonly<IRefineItemToggleNotification>): void;
    urlBuilder(refiner: IRefineItemToggleNotification): string;
}

/**
 * Refine item state.
 */
export interface IRefineItemState extends React.ComponentState {
    isChecked: boolean;
    renderingError?: object;
}

/**
 * Single-select and multi-select refine item component (controlled by RefineSubmenu).
 */
export default class RefineItem extends React.Component<IRefineItemProps, IRefineItemState> {
    private readonly anchorType: React.RefObject<HTMLAnchorElement>;

    private readonly payLoad: IPayLoad;

    public constructor(props: IRefineItemProps) {
        super(props);
        this._onClick = this._onClick.bind(this);
        this.state = {
            isChecked: !!this.props.selectedRefinementCriterion
        };
        this.anchorType = React.createRef();
        this.payLoad = getPayloadObject('click', this.props.telemetryContent!, '');
    }

    public shouldComponentUpdate(nextProps: IRefineItemProps, nextState: IRefineItemState): boolean {
        if (this.state === nextState && this.props === nextProps) {
            return false;
        }
        return true;
    }

    public render(): JSX.Element | undefined {
        const {
            isDisabled,
            refineItemCommonProps,
            parentProductRefinerHierarchy,
            productRefinerValue,
            selectedRefinementCriterion,
            productCountAriaLabel,
            singleProductCountAriaLabel,
            index,
            children,
            onToggle,
            context,
            telemetryContent,
            ...attrs
        } = this.props;
        if (!productRefinerValue) {
            refineItemCommonProps.telemetry.error('[refine-item] Cannot render refineItem without productRefinerValue');
            return undefined;
        }
        if (!productRefinerValue.LeftValueBoundString) {
            refineItemCommonProps.telemetry.warning(
                `[refine-item]  RefineItem without LeftValueBoundString: ${JSON.stringify(productRefinerValue)}`
            );
        }
        const isSingleSelect = parentProductRefinerHierarchy.RefinerTypeValue === RefinerType.SingleSelect;
        let itemTypeClassName = isSingleSelect ? 'single-select' : 'multi-select';
        itemTypeClassName = `ms-refine-submenu-item ${itemTypeClassName}`;
        const inputType = isSingleSelect ? 'radio' : 'checkbox';
        const isChecked = !!selectedRefinementCriterion;
        itemTypeClassName = isChecked ? `${itemTypeClassName}-checked` : itemTypeClassName;
        if (parentProductRefinerHierarchy.SourceValue === ProductRefinerSource.Rating) {
            return this._renderRating(
                productRefinerValue,
                parentProductRefinerHierarchy,
                isChecked,
                context,
                index,
                telemetryContent,
                productCountAriaLabel,
                singleProductCountAriaLabel
            );
        }
        this.payLoad.contentAction.etext = productRefinerValue.LeftValueBoundLocalizedString || productRefinerValue.LeftValueBoundString;
        const attribute = getTelemetryAttributes(telemetryContent!, this.payLoad);

        return (
            <li className='ms-refine-submenu-item' id={`${parentProductRefinerHierarchy.KeyName!}_${index}`}>
                <a
                    key={selectedRefinementCriterion ? 'true' : 'false'}
                    ref={this.anchorType}
                    href={this._getRefinerUrl()}
                    tabIndex={0}
                    onClick={this._onClick}
                    className={itemTypeClassName}
                    role={inputType}
                    aria-checked={!!selectedRefinementCriterion}
                    {...attribute}
                    {...attrs}
                >
                    {this._renderSwatch()}
                    <span className='ms-refine-submenu-item__label'>
                        {productRefinerValue.LeftValueBoundLocalizedString || productRefinerValue.LeftValueBoundString}
                        {productRefinerValue.Count !== undefined && ` (${productRefinerValue.Count})`}
                    </span>
                </a>
            </li>
        );
    }

    private _renderSwatch(): JSX.Element | null {
        const productRefinerValue = this.props.productRefinerValue;
        // eslint-disable-next-line @typescript-eslint/prefer-nullish-coalescing -- Replace with actual value for empty string.
        const text = productRefinerValue.LeftValueBoundLocalizedString || productRefinerValue.LeftValueBoundString || '';

        const dimensionType = (this.props.parentProductRefinerHierarchy.KeyName?.toLocaleLowerCase() ?? '') as DimensionTypes;
        const hasColor = !StringExtensions.isNullOrWhitespace(productRefinerValue.SwatchColorHexCode);
        const hasImage = !StringExtensions.isNullOrWhitespace(productRefinerValue.SwatchImageUrl);
        const hasValidSwatch = hasColor || hasImage;
        const shouldDisplayAsSwatch =
            hasValidSwatch && checkIfShouldDisplayAsSwatch(dimensionType, this.props.context as ICoreContext<IDimensionsApp>);
        const swatchItems = shouldDisplayAsSwatch
            ? [
                  {
                      itemId: `${productRefinerValue.RefinerRecordId ?? ''}-${dimensionType}-${text}`,
                      value: text,
                      dimensionType,
                      colorHexCode: productRefinerValue.SwatchColorHexCode,
                      imageUrl: productRefinerValue.SwatchImageUrl
                  }
              ]
            : undefined;

        if (!swatchItems) {
            return null;
        }

        return (
            <SwatchComponent
                className='ms-refine-submenu-item__swatch'
                apiSettings={this.props.context.request.apiSettings}
                list={swatchItems}
                isSelectionEnabled={false}
            />
        );
    }

    private _getRefinerUrl(): string {
        const { urlBuilder, parentProductRefinerHierarchy, productRefinerValue, selectedRefinementCriterion } = this.props;

        if (productRefinerValue) {
            return urlBuilder({
                parentProductRefinerHierarchy,
                productRefinerValue,
                isSelecting: !selectedRefinementCriterion
            });
        }

        return '';
    }

    private readonly _onClick = (e: React.MouseEvent<HTMLAnchorElement | HTMLInputElement>): void => {
        e.preventDefault();

        const { parentProductRefinerHierarchy, productRefinerValue, selectedRefinementCriterion } = this.props;
        if (productRefinerValue) {
            this.props.onToggle({
                parentProductRefinerHierarchy,
                productRefinerValue,
                isSelecting: !selectedRefinementCriterion
            });

            setTimeout(() => {
                this.anchorType.current && this.anchorType.current.focus();
            }, 0);
        }
    };

    private _renderRating(
        productRefinerValue: ProductRefinerValue,
        parentProductRefinerHierarchy: IProductRefinerHierarchy,
        isChecked: boolean,
        context: ICoreContext,
        index?: number,
        telemetryContent?: ITelemetryContent,
        ratingUserCountAriaLabel?: string,
        ratingSingleUserCountAriaLabel?: string
    ): JSX.Element | undefined {
        if (productRefinerValue.LeftValueBoundString) {
            this.payLoad.contentAction.etext = productRefinerValue.LeftValueBoundLocalizedString;
            const attribute = getTelemetryAttributes(telemetryContent!, this.payLoad);
            const refinerRating = Number.parseInt(productRefinerValue.LeftValueBoundString, 10);
            const ratingRefiner = this.props.selectedRefinerValues?.find(
                value => value.DataTypeValue === productRefinerValue.DataTypeValue
            );
            const selectedRating = Number.parseInt(ratingRefiner?.LeftValueBoundString || '0', 10);
            const defaultChkProductCount = 1;
            let ratingComponentClass: string = 'ms-rating-component';
            if (refinerRating > selectedRating) {
                ratingComponentClass = `${ratingComponentClass}__up`;
            } else if (refinerRating < selectedRating) {
                ratingComponentClass = `${ratingComponentClass}__down`;
            } else {
                ratingComponentClass = `${ratingComponentClass}__current`;
            }
            const productCountAriaLabelValue =
                productRefinerValue.Count !== undefined && productRefinerValue.Count === defaultChkProductCount
                    ? ratingSingleUserCountAriaLabel && format(ratingSingleUserCountAriaLabel, productRefinerValue.Count)
                    : ratingUserCountAriaLabel && format(ratingUserCountAriaLabel, productRefinerValue.Count);
            return (
                <li
                    className='ms-refine-submenu-item'
                    role={!this.props.isMobileView ? 'presentation' : undefined}
                    id={`${parentProductRefinerHierarchy.KeyName!}_${index!}`}
                >
                    <a
                        href={this._getRefinerUrl()}
                        role='radio'
                        aria-checked={isChecked}
                        aria-label={`${parentProductRefinerHierarchy.KeyName!}_${productRefinerValue.LeftValueBoundLocalizedString!}
                        ${productCountAriaLabelValue!}`}
                        onClick={this._onClick}
                        {...attribute}
                    >
                        <RatingComponent
                            className={ratingComponentClass}
                            avgRating={refinerRating}
                            ratingCount={productRefinerValue.LeftValueBoundLocalizedString ?? productRefinerValue.LeftValueBoundString}
                            hideCount={false}
                            readOnly
                            ariaLabel=''
                            context={context}
                            id={this.props.moduleId}
                            typeName={this.props.moduleTypeName}
                            data={{}}
                        />
                        <span className='refine-submenu-item__rating' aria-hidden={this.props.isMobileView ? 'true' : undefined}>
                            {productRefinerValue.Count !== undefined && `(${productRefinerValue.Count})`}
                        </span>
                    </a>
                </li>
            );
        }
        return undefined;
    }
}
