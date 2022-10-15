/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import { IProductRefinerHierarchy } from '@msdyn365-commerce/commerce-entities';
import { CacheType, createObservableDataAction, IAction, IActionContext, IActionInput } from '@msdyn365-commerce/core';
import { buildCacheKey } from '@msdyn365-commerce-modules/retail-actions';

import { BaseCollectionInput } from './base-collection-action';
import { getProductRefinerHierarchy } from './get-product-refiner-hierarchy';

/**
 * InventoryInStockRefinerValue Input action.
 */
export class InventoryInStockRefinerValueInput extends BaseCollectionInput implements IActionInput {
    public inventoryProductAttributeRecordId: number;

    public constructor(input: BaseCollectionInput, inventoryProductAttributeRecordId: number = 0) {
        super(
            input.pageType,
            input.apiSettings,
            input.queryResultSettings,

            // Parameter - refiners
            [],

            // Parameter - category
            undefined,

            // Parameter - searchText
            '',

            // Parameter - includeAttributes
            false,

            // Parameter - isUpdateRefinerPanel
            false,

            // Parameter - locale
            undefined,
            input.catalogId
        );

        this.inventoryProductAttributeRecordId = inventoryProductAttributeRecordId;
    }

    /**
     * GetCacheKey.
     * @returns - Returns string.
     */
    public getCacheKey = (): string => buildCacheKey('RefinerList', this.apiSettings);

    /**
     * GetCacheObjectType.
     * @returns - Returns string.
     */
    public getCacheObjectType = (): string => 'RefinerList';

    /**
     * DataCacheType.
     * @returns - CacheType string.
     */
    public dataCacheType = (): CacheType => 'application';
}

/**
 * Action method returns inventory in stock refiners.
 * @param input - InventoryInStockRefinerValueInput.
 * @param context - IActionContext.
 * @returns - Promise<IProductRefinerHierarchy | null>.
 */
async function action(input: InventoryInStockRefinerValueInput, context: IActionContext): Promise<IProductRefinerHierarchy | null> {
    const refiners = await getProductRefinerHierarchy(
        {
            Context: {
                ChannelId: input.apiSettings.channelId,
                CatalogId: input.catalogId
            }
        },
        input.queryResultSettings,
        context
    );

    return refiners.find(refiner => refiner.RecordId === input.inventoryProductAttributeRecordId) ?? null;
}

/**
 * Action.
 * @param id - Id.
 * @param action - Action.
 * @returns - Results.
 */
export const actionDataAction = createObservableDataAction({
    id: '@msdyn365-commerce-modules/search-result-container/get-inventory-refiners',
    action: action as IAction<IProductRefinerHierarchy>
});

export default actionDataAction;
