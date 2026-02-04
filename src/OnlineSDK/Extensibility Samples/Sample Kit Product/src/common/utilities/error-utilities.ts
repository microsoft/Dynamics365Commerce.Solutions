/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { IAddToCartFailureResult } from '@msdyn365-commerce/components';
import { IAny, ICoreContext, IGeneric } from '@msdyn365-commerce/core';
import { ICartState } from '@msdyn365-commerce/global-state';
import { OrgUnitLocation, ProductAvailableQuantity, ProductDimension, SimpleProduct } from '@msdyn365-commerce/retail-proxy';
import { ArrayExtensions, StringExtensions } from '@msdyn365-commerce-modules/retail-actions';

import { IBuyboxCommonResources } from '../buyboxInterface';

/**
 * Get the quantity limit.
 * @param context - The context.
 * @param productAvailability - The product availability.
 * @returns The quantity limit.
 */
export function getQuantityLimit(context: ICoreContext<IGeneric<IAny>>, productAvailability: ProductAvailableQuantity | undefined): number {
    // Get the quantity limit for the product
    // eslint-disable-next-line @typescript-eslint/no-unsafe-assignment, @typescript-eslint/no-unsafe-member-access -- need read from config.
    const maxQuantityForCartLineItem: number | undefined = context.app.config.maxQuantityForCartLineItem;
    const availableQty = productAvailability?.AvailableQuantity;
    const defaultQuantityLimit = 10;

    let quantityLimit: number;

    if (maxQuantityForCartLineItem === undefined && availableQty === undefined) {
        quantityLimit = defaultQuantityLimit;
    } else if (availableQty === undefined) {
        quantityLimit = maxQuantityForCartLineItem ?? defaultQuantityLimit;
    } else if (maxQuantityForCartLineItem === undefined) {
        quantityLimit = availableQty;
    } else {
        quantityLimit = Math.min(maxQuantityForCartLineItem, availableQty);
    }

    return quantityLimit;
}

/**
 * Get the element index.
 * @param cart - The cart.
 * @param product - The product.
 * @param location - The location.
 * @returns The index of the element.
 */
export function getElementIndex(
    cart: ICartState | undefined,
    product: SimpleProduct | undefined,
    location: OrgUnitLocation | undefined
): number {
    // Get the quantity of the product in cart
    let elementFoundAt: number = -1;

    const productIdToFind = product?.RecordId;

    for (let index = 0; index < cart!.cart.CartLines!.length; index++) {
        if (
            cart!.cart.CartLines![index].ProductId === productIdToFind &&
            (cart!.cart.CartLines![index].FulfillmentStoreId || '') === (location?.OrgUnitNumber || '')
        ) {
            elementFoundAt = index;

            break;
        }
    }

    return elementFoundAt;
}

export function getQuantityError(stockLeft: number | undefined, resources: IBuyboxCommonResources): string | undefined {
    if (!stockLeft || stockLeft <= 0) {
        return resources.errorMessageOutOfStock;
    } else if (stockLeft === 1) {
        return resources.errorMessageOutOfRangeOneLeft;
    }
    return resources.errorMessageOutOfRangeFormat.replace('{numLeft}', stockLeft.toString());
}

export function getGenericError(
    result: IAddToCartFailureResult,
    cart: ICartState | undefined,
    resources: IBuyboxCommonResources,
    context: ICoreContext<IGeneric<IAny>>,
    product: SimpleProduct | undefined,
    productAvailability: ProductAvailableQuantity | undefined,
    location: OrgUnitLocation | undefined
): string | undefined {
    if (result.failureReason === 'EMPTYINPUT') {
        return resources.addedToCartFailureMessage;
    } else if (result.failureReason === 'CARTACTIONFAILED') {
        if (result.cartActionResult && result.cartActionResult.substatus === 'MAXQUANTITY') {
            // Get the quantity of the product in cart
            const elementFoundAt: number = getElementIndex(cart, product, location);

            let currentQuantity: number = 0;
            if (elementFoundAt !== -1) {
                const cartLineToUpdate = { ...cart!.cart.CartLines![elementFoundAt] };
                currentQuantity = cartLineToUpdate.Quantity ?? 0;
            }

            // Get the quantity limit for the product
            const quantityLimit = getQuantityLimit(context, productAvailability);
            return resources.maxQuantityLimitText
                .replace('{curQuantity}', currentQuantity.toString())
                .replace('{maxQuantity}', quantityLimit.toString());
        } else if (
            result.cartActionResult &&
            (result.cartActionResult.substatus === 'QUANTITYLIMITS' || result.cartActionResult.substatus === 'INVOICEINCART')
        ) {
            return result.cartActionResult.errorDetails?.LocalizedMessage;
        }

        if (result.cartActionResult?.validationResults && ArrayExtensions.hasElements(result.cartActionResult.validationResults)) {
            return result.cartActionResult.validationResults
                .map(item => {
                    return StringExtensions.isNullOrEmpty(item.LocalizedMessage)
                        ? item.ErrorContext ?? item.ErrorResourceId
                        : item.LocalizedMessage;
                })
                .toString();
        }

        return resources.addedToCartFailureMessage;
    }

    return undefined;
}

export function getConfigureErrors(
    result: ProductDimension[] | undefined,
    resources: IBuyboxCommonResources,
    isGiftCard?: boolean
): { [configureId: string]: string | undefined } {
    if (!result) {
        return {};
    }

    const dimensions: { [configureId: string]: string | undefined } = {};

    for (const dimension of result) {
        dimensions[dimension.DimensionTypeValue] = getDimensionErrorString(dimension.DimensionTypeValue, resources, isGiftCard);
    }

    return dimensions;
}

export function getDimensionErrorString(dimensionTypeValue: number, resources: IBuyboxCommonResources, isGiftCard?: boolean): string {
    switch (dimensionTypeValue) {
        case 1: // ProductDimensionType.Color
            return resources.productDimensionTypeColorErrorMessage;
        case 2: // ProductDimensionType.Configuration
            return resources.productDimensionTypeConfigurationErrorMessage;
        case 3: // ProductDimensionType.Size
            return resources.productDimensionTypeSizeErrorMessage;
        case 4: // ProductDimensionType.Style
            return isGiftCard ? resources.productDimensionTypeAmountErrorMessage : resources.productDimensionTypeStyleErrorMessage;
        default:
            return '';
    }
}
