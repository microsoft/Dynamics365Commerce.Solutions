/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import MsDyn365, {
    CacheType,
    createObservableDataAction,
    IAction,
    IActionContext,
    IActionInput,
    ICreateActionContext
} from '@msdyn365-commerce/core';
import {
    AsyncResult,
    ChannelInventoryConfiguration,
    ProductRefinerValue,
    ProductSearchCriteria,
    ProductSearchResult
} from '@msdyn365-commerce/retail-proxy';
import { searchByCriteriaAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/ProductsDataActions.g';
import { getInventoryConfigurationAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/StoreOperationsDataActions.g';
import { ArrayExtensions, generateProductImageUrl, InventoryLevels, ObjectExtensions } from '@msdyn365-commerce-modules/retail-actions';

import { ListPageStateInput } from './list-page-state';
import { BaseCollectionInput, createBaseCollectionInput } from './base-collection.action';
import { compressRefiners } from '../utils/url-utils';

/**
 * GetFullProductsByCollection Action Input.
 */
export class CustomGetFullProductsByCollectionInput extends BaseCollectionInput implements IActionInput {
    /**
     * The cache object type.
     * @returns The cache object type.
     */
    public getCacheObjectType = (): string => 'CustomFullProductSearchResult';

    /**
     * The data cache type.
     * @returns The data cache type.
     */
    public dataCacheType = (): CacheType => {
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
 * @param customerType - Customer type.
 * @returns IActionInput - The action input.
 */
export const customInput = (
    args: ICreateActionContext<{ itemsPerPage: number; includedAttributes: boolean | undefined }>,
    customerType?: string
): IActionInput => {
    const input = createBaseCollectionInput(args, CustomGetFullProductsByCollectionInput, customerType);

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
    metadataCount: number | undefined,
    channelInventoryConfiguration: ChannelInventoryConfiguration
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
export async function customAction(
    input: CustomGetFullProductsByCollectionInput,
    context: IActionContext
): Promise<IFullProductsSearchResultsWithCount> {
    let promise: AsyncResult<ProductSearchResult[]>;
    let searchProductId;
    const searchCriteriaInput: ProductSearchCriteria = {};
    searchCriteriaInput.Context = { ChannelId: context.requestContext.apiSettings.channelId, CatalogId: input.catalogId };
    searchCriteriaInput.Refinement = input.refiners;
    searchCriteriaInput.IncludeAttributes = input.includeAttributes;

    // change the below refiners as per requirement
    const anonymousRefiner: ProductRefinerValue = {
        RefinerRecordId: 11290923669,
        RightValueBoundString: 'Picton Blu',
        LeftValueBoundString: 'Picton Blu',
        RefinerSourceValue: 1,
        Count: 3,
        DataTypeValue: 5,
        ExtensionProperties: [],
        LeftValueBoundLocalizedString: '',
        RightValueBoundLocalizedString: '',
        RowNumber: 0,
        SwatchColorHexCode: '#36A1F2',
        SwatchImageUrl: '',
        UnitText: '',
        // @ts-ignore
        '@odata.type': '#Microsoft.Dynamics.Commerce.Runtime.DataModel.ProductRefinerValue'
    };

    const user1Refiner: ProductRefinerValue = {
        RefinerRecordId: 11290923669,
        RightValueBoundString: 'Grape',
        LeftValueBoundString: 'Grape',
        RefinerSourceValue: 1,
        Count: 1,
        DataTypeValue: 5,
        ExtensionProperties: [],
        LeftValueBoundLocalizedString: '',
        RightValueBoundLocalizedString: '',
        RowNumber: 0,
        SwatchColorHexCode: '#6f2da8',
        SwatchImageUrl: '',
        UnitText: '',
        // @ts-ignore
        '@odata.type': '#Microsoft.Dynamics.Commerce.Runtime.DataModel.ProductRefinerValue'
    };

    const user2Refiner: ProductRefinerValue = {
        RefinerRecordId: 11290923669,
        RightValueBoundString: 'Mantis',
        LeftValueBoundString: 'Mantis',
        RefinerSourceValue: 1,
        Count: 1,
        DataTypeValue: 5,
        ExtensionProperties: [],
        LeftValueBoundLocalizedString: '',
        RightValueBoundLocalizedString: '',
        RowNumber: 0,
        SwatchColorHexCode: '#74c365',
        SwatchImageUrl: '',
        UnitText: '',
        // @ts-ignore
        '@odata.type': '#Microsoft.Dynamics.Commerce.Runtime.DataModel.ProductRefinerValue'
    };
    const channelInventoryConfiguration = await getInventoryConfigurationAsync({ callerContext: context });
    const isUserAuthenticated = context.requestContext.user.isAuthenticated;
    if (isUserAuthenticated) {
        if (input.customerType === 'Temple') {
            if (
                !searchCriteriaInput.Refinement.find(
                    refiner =>
                        refiner.RefinerRecordId === user1Refiner.RefinerRecordId &&
                        refiner.LeftValueBoundString === user1Refiner.LeftValueBoundString
                )
            ) {
                searchCriteriaInput.Refinement.push(user1Refiner);
            }
        } else if (input.customerType === 'Endowment') {
            if (
                !searchCriteriaInput.Refinement.find(
                    refiner =>
                        refiner.RefinerRecordId === user2Refiner.RefinerRecordId &&
                        refiner.LeftValueBoundString === user2Refiner.LeftValueBoundString
                )
            ) {
                searchCriteriaInput.Refinement.push(user2Refiner);
            }
        }
    } else {
        if (
            !searchCriteriaInput.Refinement.find(
                refiner =>
                    refiner.RefinerRecordId === anonymousRefiner.RefinerRecordId &&
                    refiner.LeftValueBoundString === anonymousRefiner.LeftValueBoundString
            )
        ) {
            searchCriteriaInput.Refinement.push(anonymousRefiner);
        }
    }

    if (searchCriteriaInput && searchCriteriaInput.Refinement) {
        const compressRefiner = compressRefiners(searchCriteriaInput.Refinement);
        context.requestContext.query!['refiners'] = JSON.stringify(compressRefiner);
    }

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

    return returnProducts(productSearchResults, context, promise.metadata.count, channelInventoryConfiguration);
}

export const customDataAction = createObservableDataAction({
    id: '@msdyn365-commerce-modules/search-result-container/custom-get-full-products-by-collection',
    action: <IAction<IFullProductsSearchResultsWithCount>>customAction,
    input: customInput
});

export default customDataAction;
