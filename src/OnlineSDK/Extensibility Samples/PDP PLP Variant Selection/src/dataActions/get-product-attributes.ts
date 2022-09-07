import {
    CacheType,
    createObservableDataAction,
    IAction,
    IActionContext,
    IActionInput,
    IAny,
    ICreateActionContext,
    IGeneric
} from '@msdyn365-commerce/core';
import { AttributeValue, SimpleProduct } from '@msdyn365-commerce/retail-proxy';

import { getSelectedProductIdFromActionInput, getSelectedVariant, SelectedVariantInput } from '@msdyn365-commerce-modules/retail-actions';
import { getAttributeValuesAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/ProductsDataActions.g';

/**
 * Chains getting the AttributesForProduct
 */
export class AttributesForProductInput implements IActionInput {
    public productId: number;
    public channelId: number;
    public catalogId: number;
    public selectedProduct: SimpleProduct | undefined;

    constructor(productId: number, channelId: number, selectedProduct?: SimpleProduct, catalogId?: number) {
        this.productId = productId;
        this.channelId = channelId;
        this.selectedProduct = selectedProduct;
        this.catalogId = catalogId || 0;
    }

    public getCacheKey = () => `AttributesForSelectedVariant`;
    public getCacheObjectType = () => 'ProductAttributeValue';
    public dataCacheType = (): CacheType => 'none';
}

export const createAttributesForProductInput = (inputData: ICreateActionContext<IGeneric<IAny>>): AttributesForProductInput => {
    const productId = getSelectedProductIdFromActionInput(inputData);

    if (productId) {
        return new AttributesForProductInput(+productId, +inputData.requestContext.apiSettings.channelId);
    } else {
        throw new Error('Unable to create AttributesForSelectedVariantInput, no productId found on module config or query');
    }
};

/**
 * Calls the Retail API a get a list of dimension values for a given product.
 */
export async function getProductAttributesAction(input: AttributesForProductInput, ctx: IActionContext): Promise<AttributeValue[]> {
    // @ts-ignore: Promise vs. ObservablePromise typing conflict
    return Promise.resolve()
        .then(() => {
            const activeProduct: SimpleProduct | undefined = input.selectedProduct;

            if (!activeProduct) {
                const selectedVariantInput = new SelectedVariantInput(input.productId, input.channelId);

                return getSelectedVariant(selectedVariantInput, ctx);
            }

            return activeProduct;
        })
        .then<AttributeValue[] | null>((productResult: SimpleProduct | null) => {
            const activeProduct: SimpleProduct | undefined = <SimpleProduct | undefined>productResult;

            if (activeProduct) {
                return getAttributeValuesAsync(
                    { callerContext: ctx, queryResultSettings: {} },
                    activeProduct.RecordId,
                    input.channelId,
                    input.catalogId
                );
            }

            return null;
        })
        .then((attributeValues: AttributeValue[] | null) => {
            const resultArray = <AttributeValue[]>attributeValues;

            if (resultArray) {
                return resultArray;
            }

            return [];
        })
        .catch((error: Error) => {
            ctx.trace(error.message);
            throw new Error('[getAttributesForSelectedVariantAction]Error executing action');
        });
}

export default createObservableDataAction({
    id: 'getProductAttributes',
    action: <IAction<AttributeValue[]>>getProductAttributesAction,
    input: createAttributesForProductInput
});
