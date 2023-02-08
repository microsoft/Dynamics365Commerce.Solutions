/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import {
    CacheType,
    createObservableDataAction,
    getCatalogId,
    IAction,
    IActionContext,
    IActionInput,
    IAny,
    ICreateActionContext,
    IGeneric
} from '@msdyn365-commerce/core';
import { getCartFromCustomer } from '@msdyn365-commerce/global-state';
import { getActivePricesAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/ProductsDataActions.g';
import {
    AffiliationLoyaltyTier,
    ProductPrice,
    ProjectionDomain,
    SimpleProduct
} from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';

import { getSelectedProductIdFromActionInput, getSelectedVariant, SelectedVariantInput } from '@msdyn365-commerce-modules/retail-actions';
// eslint-disable-next-line no-duplicate-imports
import { ArrayExtensions, QueryResultSettingsProxy } from '@msdyn365-commerce-modules/retail-actions';

/**
 * Input class for the GetVariantPrice Data Action.
 */
export class PriceForSelectedVariantInput implements IActionInput {
    public productId: number;

    public channelId: number;

    public selectedProduct: SimpleProduct | undefined;

    public customerId?: string;

    public constructor(productId: number, channelId: number, selectedProduct?: SimpleProduct, customerId?: string) {
        this.productId = productId;
        this.channelId = channelId;
        this.selectedProduct = selectedProduct;
        this.customerId = customerId ?? '';
    }

    public getCacheKey = (): string => 'PriceForSelectedVariant';

    public getCacheObjectType = (): string => 'Price';

    public dataCacheType = (): CacheType => 'none';
}

/**
 * The createInput method for the GetVariantPriceDataAction.
 * @param inputData - The input data for the createInput method.
 * @returns The PriceForSelectedVariantInput.
 */
export const createGetVariantPriceInput = (inputData: ICreateActionContext<IGeneric<IAny>>): PriceForSelectedVariantInput => {
    const productId = getSelectedProductIdFromActionInput(inputData);

    if (productId) {
        return new PriceForSelectedVariantInput(+productId, +inputData.requestContext.apiSettings.channelId, undefined);
    }
    throw new Error('Unable to create PriceForSelectedVariantInput, no productId found on module config or query');
};

/**
 * The Action Method for the GetVariantPrice Data Action
 * Pulls the currently selected variant from the cache using the getSelectedVariant data action, and gets it's current contextual price
 * via the getActivePrice RetailServer API.
 * @param input - The input.
 * @param ctx - The ctx.
 * @returns The Promise<ProductPrice | null>.
 */
export async function getVariantPriceAction(input: PriceForSelectedVariantInput, ctx: IActionContext): Promise<ProductPrice | null> {
    let affiliations: AffiliationLoyaltyTier[] | undefined = [];
    if (ctx.requestContext.user.isAuthenticated) {
        const cart = await getCartFromCustomer(ctx);
        affiliations = cart?.AffiliationLines;
    }
    return Promise.resolve()
        .then(() => {
            const activeProduct: SimpleProduct | undefined = input.selectedProduct;

            if (!activeProduct) {
                const selectedVariantInput = new SelectedVariantInput(
                    input.productId,
                    input.channelId,
                    undefined,
                    undefined,
                    ctx.requestContext
                );

                return getSelectedVariant(selectedVariantInput, ctx);
            }

            return activeProduct;
        })
        .then<ProductPrice | null>(async (productResult: SimpleProduct | null) => {
            const catalogId = getCatalogId(ctx.requestContext);
            const projectDomain: ProjectionDomain = {
                ChannelId: +ctx.requestContext.apiSettings.channelId,
                CatalogId: catalogId
            };

            const activeProduct: SimpleProduct | undefined = productResult as SimpleProduct | undefined;
            if (activeProduct) {
                return getActivePricesAsync(
                    { callerContext: ctx, queryResultSettings: QueryResultSettingsProxy.getPagingFromInputDataOrDefaultValue(ctx) },
                    projectDomain,
                    [activeProduct.RecordId],
                    new Date(),
                    input.customerId,
                    affiliations,
                    true
                ).then((productPrices: ProductPrice[]) => {
                    if (!ArrayExtensions.hasElements(productPrices)) {
                        throw new Error('[getVariantPriceAction]Invalid response recieved from getActivePricesAsync');
                    }
                    return productPrices[0];
                });
            }

            return null;
        })
        .catch((error: Error) => {
            ctx.trace(error.message);
            ctx.telemetry.exception(error);
            ctx.telemetry.debug('[getVariantPriceAction]Error executing action');
            throw new Error('[getVariantPriceAction]Error executing action');
        });
}

export const getVariantPriceActionDataAction = createObservableDataAction({
    id: '@msdyn365-commerce-modules/retail-actions/get-price-for-selected-variant',
    action: getVariantPriceAction as IAction<ProductPrice | null>,
    input: createGetVariantPriceInput
});

export default getVariantPriceActionDataAction;
