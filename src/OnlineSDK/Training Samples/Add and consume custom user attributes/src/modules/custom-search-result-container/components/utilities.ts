/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { ITelemetry } from '@msdyn365-commerce/core';
import { AttributeDataType, ProductRefinerValue, RefinerType } from '@msdyn365-commerce/retail-proxy';

import { IRefineItemToggleNotification } from './refine-item-toggle-notification';

/**
 * Types of product refiner values.
 * @deprecated This will be removed soon. Please, use `AttributeDataType` from `@msdyn365-commerce/retail-proxy` instead.
 * @example
 * ```
 * import { AttributeDataType } from `@msdyn365-commerce/retail-proxy`;
 * ```
 */
export enum ProductRefinerValueDataTypeValue {
    /**
     * Range slider is used for selections like price.
     * @deprecated This will be removed soon. Please, use `AttributeDataType` from `@msdyn365-commerce/retail-proxy` instead.
     * @example
     * ```
     * import { AttributeDataType } from `@msdyn365-commerce/retail-proxy`;
     * ```
     */
    Range = 1,

    /**
     * Range input is a different way to specify ranges and can be expressed with input boxes
     * as well as a set of discrete single-select type values.
     * @deprecated This will be removed soon. Please, use `AttributeDataType` from `@msdyn365-commerce/retail-proxy` instead.
     * @example
     * ```
     * import { AttributeDataType } from `@msdyn365-commerce/retail-proxy`;
     * ```
     */
    RangeInput = 4,

    /**
     * This is a discrete list item, either multi-select or single-select.
     * @deprecated This will be removed soon. Please, use `AttributeDataType` from `@msdyn365-commerce/retail-proxy` instead.
     * @example
     * ```
     * import { AttributeDataType } from `@msdyn365-commerce/retail-proxy`;
     * ```
     */
    List = 5,

    /**
     * Boolean types allows only single-select.
     * @deprecated This will be removed soon. Please, use `AttributeDataType` from `@msdyn365-commerce/retail-proxy` instead.
     * @example
     * ```
     * import { AttributeDataType } from `@msdyn365-commerce/retail-proxy`;
     * ```
     */
    Boolean = 6
}

/**
 * Types of product refiners.
 * @deprecated This will be removed soon. Please, use `RefinerType` from `@msdyn365-commerce/retail-proxy` instead.
 * @example
 * ```
 * import { RefinerType } from `@msdyn365-commerce/retail-proxy`;
 * ```
 */
export enum ProductRefinerTypeValue {
    /**
     * Refiner values are single-select.
     * @deprecated This will be removed soon. Please, use `RefinerType` from `@msdyn365-commerce/retail-proxy` instead.
     * @example
     * ```
     * import { RefinerType } from `@msdyn365-commerce/retail-proxy`;
     * ```
     */
    Single = 0,

    /**
     * Refiner values are multi-select.
     * @deprecated This will be removed soon. Please, use `RefinerType` from `@msdyn365-commerce/retail-proxy` instead.
     * @example
     * ```
     * import { RefinerType } from `@msdyn365-commerce/retail-proxy`;
     * ```
     */
    Multi = 1
}

/**
 * ProductRefinerSource enum type.
 * @deprecated This will be removed soon. Please, use `ProductRefinerSource` from `@msdyn365-commerce/retail-proxy` instead.
 * @example
 * ```
 * import { ProductRefinerSource } from `@msdyn365-commerce/retail-proxy`;
 * ```
 */
export enum ProductRefinerSource {
    /**
     * The None member.
     * @deprecated This will be removed soon. Please, use `ProductRefinerSource` from `@msdyn365-commerce/retail-proxy` instead.
     * @example
     * ```
     * import { ProductRefinerSource } from `@msdyn365-commerce/retail-proxy`;
     * ```
     */
    None = 0,

    /**
     * The Attribute member.
     * @deprecated This will be removed soon. Please, use `ProductRefinerSource` from `@msdyn365-commerce/retail-proxy` instead.
     * @example
     * ```
     * import { ProductRefinerSource } from `@msdyn365-commerce/retail-proxy`;
     * ```
     */
    Attribute = 1,

    /**
     * The Category member.
     * @deprecated This will be removed soon. Please, use `ProductRefinerSource` from `@msdyn365-commerce/retail-proxy` instead.
     * @example
     * ```
     * import { ProductRefinerSource } from `@msdyn365-commerce/retail-proxy`;
     * ```
     */
    Category = 2,

    /**
     * The Price member.
     * @deprecated This will be removed soon. Please, use `ProductRefinerSource` from `@msdyn365-commerce/retail-proxy` instead.
     * @example
     * ```
     * import { ProductRefinerSource } from `@msdyn365-commerce/retail-proxy`;
     * ```
     */
    Price = 3,

    /**
     * The Rating member.
     * @deprecated This will be removed soon. Please, use `ProductRefinerSource` from `@msdyn365-commerce/retail-proxy` instead.
     * @example
     * ```
     * import { ProductRefinerSource } from `@msdyn365-commerce/retail-proxy`;
     * ```
     */
    Rating = 4
}

/**
 * Checks if the data type value corresponds to a slider.
 * @param dataTypeValue
 */
export function isRangeType(dataTypeValue: number | undefined): boolean {
    return (
        dataTypeValue === AttributeDataType.Currency ||
        dataTypeValue === AttributeDataType.Decimal ||
        dataTypeValue === AttributeDataType.Integer
    );
}

/**
 * Find the refinement criterion associated with this product refiner value.
 * @param productRefinerValue Product refiner value to match.
 * @param refinementCriteria Selected refinement criteria.
 */
export function findMatchingRefinementCriterion(
    productRefinerValue: ProductRefinerValue,
    refinementCriteria: ProductRefinerValue[]
): ProductRefinerValue | undefined {
    // If the value is a range, then match only on data type value; otherwise match on item string
    return refinementCriteria.find((refinementCriterion: ProductRefinerValue) =>
        isMatchingRefinementCriterion(productRefinerValue, refinementCriterion)
    );
}

/**
 * Find the refinement criterion associated with this product refiner value.
 * @param productRefinerValue Product refiner value to match.
 * @param refinementCriteria Selected refinement criteria.
 * @param refinementCriterion
 */
export function isMatchingRefinementCriterion(productRefinerValue: ProductRefinerValue, refinementCriterion: ProductRefinerValue): boolean {
    // If the value is a range, then match only on data type value; otherwise match on item string
    return (
        refinementCriterion.RefinerRecordId === productRefinerValue.RefinerRecordId &&
        refinementCriterion.RefinerSourceValue === productRefinerValue.RefinerSourceValue &&
        refinementCriterion.DataTypeValue === productRefinerValue.DataTypeValue &&
        (isRangeType(refinementCriterion.DataTypeValue) ||
            refinementCriterion.LeftValueBoundString === productRefinerValue.LeftValueBoundString)
    );
}

export function getUpdatedRefinementCriteria(
    itemToggleNotification: IRefineItemToggleNotification,
    currentRefinementCriteria: ProductRefinerValue[]
): ProductRefinerValue[] {
    const updatedRefinementCriteria: ProductRefinerValue[] = [];
    let toggledItemFound = false;
    currentRefinementCriteria.forEach((selectedCriterion: ProductRefinerValue) => {
        if (isMatchingRefinementCriterion(itemToggleNotification.productRefinerValue, selectedCriterion)) {
            toggledItemFound = true;
            if (itemToggleNotification.isSelecting) {
                const next = {
                    ...selectedCriterion,
                    LeftValueBoundString:
                        (itemToggleNotification.rangeStart !== undefined && `${itemToggleNotification.rangeStart}`) ||
                        selectedCriterion.LeftValueBoundString,
                    RightValueBoundString:
                        (itemToggleNotification.rangeEnd !== undefined && `${itemToggleNotification.rangeEnd}`) ||
                        selectedCriterion.RightValueBoundString
                };
                updatedRefinementCriteria.push(next);
            } // Else the item is being de-selected, so omit it from the refinement criteria
        } else {
            // Keep existing criterion because it is not in the item toggle notification
            updatedRefinementCriteria.push(selectedCriterion);
        }
    });

    if (!toggledItemFound) {
        const next = {
            ...itemToggleNotification.productRefinerValue,
            LeftValueBoundString:
                (itemToggleNotification.rangeStart !== undefined && `${itemToggleNotification.rangeStart}`) ||
                itemToggleNotification.productRefinerValue.LeftValueBoundString,
            RightValueBoundString:
                (itemToggleNotification.rangeEnd !== undefined && `${itemToggleNotification.rangeEnd}`) ||
                itemToggleNotification.productRefinerValue.RightValueBoundString
        };
        updatedRefinementCriteria.push(next);

        // If single select, then deselect any others in the parent refiner group
        if (
            (itemToggleNotification.productRefinerValue.DataTypeValue === AttributeDataType.Text ||
                itemToggleNotification.productRefinerValue.DataTypeValue === AttributeDataType.TrueFalse) &&
            itemToggleNotification.parentProductRefinerHierarchy.RefinerTypeValue === RefinerType.SingleSelect
        ) {
            itemToggleNotification.parentProductRefinerHierarchy.Values.forEach((child: ProductRefinerValue) => {
                if (child.RefinerRecordId === next.RefinerRecordId && child.LeftValueBoundString === next.LeftValueBoundString) {
                    // Do nothing
                } else {
                    const matchingIndex = updatedRefinementCriteria.findIndex((criterion: ProductRefinerValue) =>
                        isMatchingRefinementCriterion(child, criterion)
                    );
                    if (matchingIndex > -1) {
                        updatedRefinementCriteria.splice(matchingIndex, 1);
                    }
                }
            });
        }
    }

    return updatedRefinementCriteria;
}

export function formatPrice(
    amount: string | undefined,
    currency: string | undefined,
    locale: string | undefined,
    telemetry: ITelemetry
): string {
    if (!amount || !currency) {
        telemetry.trace(`[refine-menu.utilities.formatPrice] could not format price for ${amount} ${currency}`);
        return amount || '';
    }
    const priceAmount = (amount && Number(amount)) || 0;
    let result: string;

    try {
        result = new Intl.NumberFormat(locale, {
            style: 'currency',
            currencyDisplay: 'symbol',
            currency,
            minimumFractionDigits: 0
        }).format(priceAmount);
    } catch (error) {
        result = `${priceAmount} ${currency}`;
        telemetry.warning(`[refine-menu.utilities.formatPrice] Failed to format price for ${result}: ${error}`);
    }

    return result;
}
