/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import MsDyn365, {
    CacheType,
    createObservableDataAction,
    IAction,
    IActionContext,
    IActionInput,
    ICreateActionContext
} from '@msdyn365-commerce/core';
import { AsyncResult, ChannelInventoryConfiguration, ProductSearchCriteria, ProductSearchResult } from '@msdyn365-commerce/retail-proxy';
import { searchByCriteriaAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/ProductsDataActions.g';
import { getInventoryConfigurationAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/StoreOperationsDataActions.g';
import { ArrayExtensions, generateProductImageUrl, InventoryLevels, ObjectExtensions } from '@msdyn365-commerce-modules/retail-actions';

import { ListPageStateInput } from '../list-page-state';
import { BaseCollectionInput, createBaseCollectionInput } from './base-collection-action';

/**
 * GetFullProductsByCollection Action Input.
 */
export class GetFullProductsByCollectionInput extends BaseCollectionInput implements IActionInput {
    /**
     * The cache object type.
     * @returns The cache object type.
     */
    public getCacheObjectType = (): string => 'FullProductSearchResult';

    /**
     * The data cache type.
     * @returns The data cache type.
     */
    public dataCacheType = (): CacheType => {
        if (
            this.pageType !== 'Category' ||
            (this.refiners && this.refiners.length > 0) ||
            (this.queryResultSettings &&
                this.queryResultSettings.Sorting &&
                this.queryResultSettings.Sorting.Columns &&
                this.queryResultSettings.Sorting.Columns.length > 0)
        ) {
            return 'request';
        }
        return 'application';
    };
}

/**
 * This setting defines inventory filtering options.
 */
export enum ProductListInventoryFilteringOptions {
    /**
     * Filter out all products out of stock.
     */
    HideOOS = 'hideOOS',

    /**
     * Sort products by availability, OOS goes last.
     */
    SortOOS = 'sortOOS',

    /**
     * No filtering selected.
     */
    Default = 'default'
}

/**
 * The full product search result with count interface.
 */
export interface IFullProductsSearchResultsWithCount {
    products: ProductSearchResult[];
    count: number;
    channelInventoryConfigurationId?: number;
}

/**
 * CreateInput function which creates and actionInput used to fetch products for a list page.
 * @param args - The API arguments.
 * @returns IActionInput - The action input.
 */
const createInput = (args: ICreateActionContext<{ itemsPerPage: number; includedAttributes: boolean | undefined }>): IActionInput => {
    const input = createBaseCollectionInput(args, GetFullProductsByCollectionInput);

    // Set Top
    if (input.queryResultSettings.Paging && args.config) {
        input.queryResultSettings.Paging.Top = args.config.itemsPerPage || 1;
    }

    // Set Skip
    if (input.queryResultSettings.Paging && args.requestContext.query && args.requestContext.query.skip) {
        input.queryResultSettings.Paging.Skip = +args.requestContext.query.skip;
    }

    input.queryResultSettings.count = true;

    return input;
};

/**
 * Finds whether a product is out of stock based on the attribute.
 * @param  product - The product.
 * @param  channelInventoryConfiguration - The channel configuration.
 * @returns True if is OOS, false otherwise.
 */
function isOutOfStock(product: ProductSearchResult, channelInventoryConfiguration: ChannelInventoryConfiguration): boolean {
    if (!ArrayExtensions.hasElements(product.AttributeValues)) {
        // If no attributes then assume is in stock
        return false;
    }
    for (const attribute of product.AttributeValues) {
        if (
            attribute.RecordId === channelInventoryConfiguration.InventoryProductAttributeRecordId &&
            attribute.TextValue === channelInventoryConfiguration.InventoryOutOfStockAttributeValueText
        ) {
            // The matching recordId indicates that is the inventory attribute
            // if the text value are the same then is out of stock.
            return true;
        }
    }

    return false;
}

/**
 * Returns only in stock products.
 * @param  products - The product.
 * @param  channelInventoryConfiguration - The channel configuration.
 * @returns Filtered product search result.
 */
function filterOutOfStockProducts(
    products: ProductSearchResult[],
    channelInventoryConfiguration: ChannelInventoryConfiguration
): ProductSearchResult[] {
    if (!ArrayExtensions.hasElements(products)) {
        return [];
    }
    const filteredProducts: ProductSearchResult[] = [];

    for (const product of products) {
        if (!isOutOfStock(product, channelInventoryConfiguration)) {
            filteredProducts.push(product);
        }
    }
    return filteredProducts;
}

/**
 * Returns sorted products based on availability.
 * @param  products - The product.
 * @param  channelInventoryConfiguration - The channel configuration.
 * @returns Sorted product search result.
 */
function sortsProductsBasedOnAvailability(
    products: ProductSearchResult[],
    channelInventoryConfiguration: ChannelInventoryConfiguration
): ProductSearchResult[] {
    if (!ArrayExtensions.hasElements(products)) {
        return [];
    }
    const inStockProducts: ProductSearchResult[] = [];
    const outOfStockProducts: ProductSearchResult[] = [];

    for (const product of products) {
        if (isOutOfStock(product, channelInventoryConfiguration)) {
            outOfStockProducts.push(product);
        } else {
            inStockProducts.push(product);
        }
    }
    return inStockProducts.concat(outOfStockProducts);
}

/**
 * Returns list of products based on inventory information.
 * @param  products - The products.
 * @param  channelInventoryConfiguration - The channel configuration.
 * @param  context - The context.
 * @returns List of product based on the inventory information.
 */
export function returnProductsBasedOnInventoryInformation(
    products: ProductSearchResult[],
    channelInventoryConfiguration: ChannelInventoryConfiguration,
    context: IActionContext
): ProductSearchResult[] {
    if (!ArrayExtensions.hasElements(products) || ObjectExtensions.isNullOrUndefined(channelInventoryConfiguration)) {
        return [];
    }
    switch (context.requestContext.app.config.productListInventoryDisplay) {
        case ProductListInventoryFilteringOptions.HideOOS:
            return filterOutOfStockProducts(products, channelInventoryConfiguration);
        case ProductListInventoryFilteringOptions.SortOOS:
            return sortsProductsBasedOnAvailability(products, channelInventoryConfiguration);
        default:
            return products;
    }
}

/**
 * Returns list of products based on inventory information.
 * @param  productSearchResults - The products.
 * @param  context - The context.
 * @param  metadataCount - The metadata count.
 * @returns List of product based on the inventory information.
 */
export async function returnProducts(
    productSearchResults: ProductSearchResult[],
    context: IActionContext,
    metadataCount: number | undefined
): Promise<IFullProductsSearchResultsWithCount> {
    const defaultProductCount: number = 0;

    const productSearchResultsWithImages = productSearchResults.map(productSearchResult => {
        const newImageUrl = generateProductImageUrl(productSearchResult, context.requestContext.apiSettings);

        if (newImageUrl) {
            productSearchResult.PrimaryImageUrl = newImageUrl;
        }

        return productSearchResult;
    });

    // If inventory level is threshold or inventory check is disabled then return the list of products without the inventory configuration
    // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access -- read config file.
    if (
        context.requestContext.app.config.inventoryLevel === InventoryLevels.threshold ||
        context.requestContext.app.config.enableStockCheck === false
    ) {
        return {
            products: productSearchResultsWithImages,
            count: metadataCount ?? defaultProductCount
        };
    }
    const channelInventoryConfiguration = await getInventoryConfigurationAsync({ callerContext: context });

    const mappedProducts = productSearchResultsWithImages.map(productSearchResult => {
        if (ArrayExtensions.hasElements(productSearchResult.AttributeValues)) {
            for (const element of productSearchResult.AttributeValues) {
                // eslint-disable-next-line @typescript-eslint/no-unnecessary-condition -- need check.
                if (
                    channelInventoryConfiguration &&
                    element.RecordId !== undefined &&
                    element.RecordId === channelInventoryConfiguration.InventoryProductAttributeRecordId &&
                    // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access -- read config file.
                    context.requestContext.app.config.inventoryRanges !== 'all' &&
                    element.TextValue !== channelInventoryConfiguration.InventoryOutOfStockAttributeValueText
                ) {
                    // If same RecordId then it means that is the Inventory attribute
                    // Based on the inventory range (and filtering options), the inventory label will be displayed
                    // If Inventory range is 'All' then in stock and out of stock labels are shown, else only OOS
                    // if the text value is different that the channelInventoryConfiguration.InventoryOutOfStockAttributeValueText then is in stock
                    element.TextValue = '';
                }
            }
        }

        return productSearchResult;
    });
    const productsBasedOnInventory = returnProductsBasedOnInventoryInformation(mappedProducts, channelInventoryConfiguration, context);
    return {
        products: productsBasedOnInventory,
        count: metadataCount ?? defaultProductCount,
        // eslint-disable-next-line @typescript-eslint/no-unnecessary-condition -- check config.
        channelInventoryConfigurationId: channelInventoryConfiguration
            ? channelInventoryConfiguration.InventoryProductAttributeRecordId
            : undefined
    };
}

/**
 * Action function to fetch products for a list page.
 * @param input - The input.
 * @param context - The context.
 * @returns IFullProductsSearchResultsWithCount - The full product search result with count.
 */
async function action(input: GetFullProductsByCollectionInput, context: IActionContext): Promise<IFullProductsSearchResultsWithCount> {
    let promise: AsyncResult<ProductSearchResult[]>;
    let searchProductId;
    const searchCriteriaInput: ProductSearchCriteria = {};
    searchCriteriaInput.Context = { ChannelId: context.requestContext.apiSettings.channelId, CatalogId: input.catalogId };
    searchCriteriaInput.Refinement = input.refiners;
    searchCriteriaInput.IncludeAttributes = input.includeAttributes;
    searchCriteriaInput.SkipVariantExpansion = true;
    const defaultNumber: number = 0;

    if (input.pageType === 'Category' || (context.requestContext.query && context.requestContext.query.categoryId)) {
        if (input.category) {
            searchCriteriaInput.CategoryIds = [input.category || defaultNumber];
            promise = searchByCriteriaAsync(
                {
                    callerContext: context,
                    queryResultSettings: input.queryResultSettings
                },
                searchCriteriaInput
            );
        } else {
            throw new Error('[GetFullProductsForCollection]Category Page Detected, but no global categoryId found');
        }
    } else if (input.searchText && context.requestContext.query && context.requestContext.query.q) {
        searchCriteriaInput.SearchCondition = input.searchText;
        promise = searchByCriteriaAsync(
            {
                callerContext: context,
                queryResultSettings: input.queryResultSettings
            },
            searchCriteriaInput
        );
    } else if (
        input.searchText &&
        context.requestContext.query &&
        context.requestContext.query.productId &&
        context.requestContext.query.recommendation
    ) {
        const searchObject = JSON.parse(input.searchText);
        searchProductId = Number(searchObject.ProductId);
        if (Number.isNaN(searchProductId)) {
            throw new Error('Failed to cast search product id into a number.');
        } else if (!searchObject.Recommendation) {
            throw new Error('Failed to retrieve the Recommendation.');
        } else {
            searchCriteriaInput.RecommendationListId = searchObject.Recommendation;
            searchCriteriaInput.Ids = [searchProductId || defaultNumber];
            promise = searchByCriteriaAsync(
                {
                    callerContext: context,
                    queryResultSettings: input.queryResultSettings
                },
                searchCriteriaInput
            );
        }
    } else {
        throw new Error('[GetFullProductsForCollection]Search Page Detected, but no q= or productId= query parameter found');
    }

    const productSearchResults = await promise;

    // Update ListPageState For SSR
    if (!MsDyn365.isBrowser) {
        context.update(new ListPageStateInput(), {
            totalProductCount: promise.metadata.count ?? defaultNumber,
            activeFilters: input.refiners
        });
    }

    return returnProducts(productSearchResults, context, promise.metadata.count);
}

export const actionDataAction = createObservableDataAction({
    id: '@msdyn365-commerce-modules/search-result-container/get-full-products-by-collection',
    action: <IAction<IFullProductsSearchResultsWithCount>>action,
    input: createInput
});

export default actionDataAction;
