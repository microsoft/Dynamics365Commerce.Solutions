/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { CacheType, getCatalogIdForSdk, IActionContext, IActionInput, ICommerceApiSettings } from '@msdyn365-commerce/core-internal';
import { SimpleProduct, ProductSearchResult } from '@msdyn365-commerce/retail-proxy';
import { generateImageUrl } from '@msdyn365-commerce-modules/retail-actions';

/**
 * Links coming from OneRF will have the following structure
 */
export interface IServerLink {
    type: string;
    productId?: number;
    categoryId?: number;
    destinationUrl?: string;
    error?: string;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    [x: string]: any;
}

/**
 * Used by the URL builder to construct the formatted
 * category or product link
 */
export interface IPageTypeContext {
    category?: {
        Slug: string;
        RecordId: number;
    };
    product?: {
        Name: string;
        RecordId: number;
    };
    // eslint-disable-next-line @typescript-eslint/no-explicit-any -- needed because type pageTypeContext in url builder allows additional properties
    [x: string]: any;
}

/**
 * Input Class for ProductLink related actions
 */
export class ProductLinkInput implements IActionInput {
    public categoryId: number;
    public productId: number | undefined;
    public channelId: number;
    public catalogId?: number;
    // private apiSettings: ICommerceApiSettings;
    private locale: string;

    constructor(apiSettings: ICommerceApiSettings, categoryId: number, productId?: number, catalogId?: number | null, locale?: string) {
        this.categoryId = categoryId;
        this.productId = productId;
        this.channelId = apiSettings.channelId;
        // this.apiSettings = apiSettings;
        this.catalogId = catalogId || 0;
        this.locale = locale || '';
    }

    public getCacheKey = () => `${this.catalogId}-${this.categoryId}-${this.productId}-${this.locale}`;
    public getCacheObjectType = () => 'ProductLink';
    public dataCacheType = (): CacheType => 'application';
}

/**
 * Input Class for CategoryLink related actions
 */
export class CategoryLinkInput implements IActionInput {
    public categoryId: number;
    public channelId: number;
    public catalogId?: number;
    private apiSettings: ICommerceApiSettings;
    private locale: string;

    constructor(apiSettings: ICommerceApiSettings, categoryId: number, catalogId?: number | null, locale?: string) {
        this.categoryId = categoryId;
        this.channelId = apiSettings.channelId;
        this.apiSettings = apiSettings;
        this.catalogId = catalogId || 0;
        this.locale = locale || '';
    }

    public getCacheKey = () => `${(this.apiSettings, this.catalogId)}-${this.categoryId}-${this.locale}`;
    public getCacheObjectType = () => 'CategoryLink';
    public dataCacheType = (): CacheType => 'application';
}

/**
 * ProductList metadata passed from CMS used to hydrate product list
 */
export interface IProductListMetaData {
    listType: string;
    productIds?: number[];
    categoryIds?: number[];
    searchText?: string;
    includePageContext?: boolean;
    includeDescendentCategoryChildren?: boolean;
    includeCart?: boolean;
    personalization?: boolean;
    recommendationListId?: string;
    relationshipId?: string;
    pageSize?: number;
}

// Enum Representing all supported ProductList types
export enum ProductListType {
    algo,
    editorial,
    productByCategory,
    productBySearch,
    recommendation,
    relatedProducts,
    recentlyViewedProducts
}

/**
 * Input Class for ProductList related actions
 */
export class ProductListInput implements IActionInput {
    public listMetadata: IProductListMetaData;
    public channelId: number;
    public catalogId: number;
    private _cacheType: CacheType;
    private _cacheObjectType: string;
    private _cacheKey: string;

    constructor(listMetadata: IProductListMetaData, actionContext: IActionContext) {
        const requestContext = actionContext.requestContext;
        const catalogIdNumber = getCatalogIdForSdk(requestContext, 0);

        this.listMetadata = listMetadata;
        this.catalogId = catalogIdNumber ?? 0;
        this.channelId = requestContext.apiSettings.channelId;
        this._cacheObjectType = `ProductList-${listMetadata.listType}`;

        const query = requestContext.query || {};
        const skipToken = query.skip;
        const context = JSON.stringify(requestContext.urlTokens);
        const locale = actionContext.requestContext.locale || 'unknown';
        const baseCacheKey = `${this.channelId}-${this.catalogId}-${locale}-${JSON.stringify(listMetadata)}-${skipToken || ''}`;

        switch (ProductListType[listMetadata.listType]) {
            case ProductListType.productBySearch:
                // application cache if not reading from context
                this._cacheType = listMetadata.searchText ? 'application' : 'request';
                this._cacheKey = listMetadata.searchText ? baseCacheKey : `${baseCacheKey}-${query.q || ''}`;
                break;
            case ProductListType.productByCategory:
                this._cacheType = 'application';
                this._cacheKey = listMetadata.categoryIds && listMetadata.categoryIds.length ? baseCacheKey : `${baseCacheKey}-${context}`;
                break;
            case ProductListType.relatedProducts:
                this._cacheType = 'application';
                this._cacheKey = listMetadata.productIds && listMetadata.productIds.length ? baseCacheKey : `${baseCacheKey}-${context}`;
                break;
            case ProductListType.editorial:
            case ProductListType.algo:
            case ProductListType.recommendation:
            default:
                this._cacheType = listMetadata.includeCart || listMetadata.personalization ? 'request' : 'application';
                this._cacheKey = `${baseCacheKey}-${context}`;
        }
    }

    public getCacheKey = () => this._cacheKey;
    public getCacheObjectType = () => this._cacheObjectType;
    public dataCacheType = (): CacheType => this._cacheType;
}

export const getProductImageUrls = (
    products: SimpleProduct[] | ProductSearchResult[],
    apiSettings: ICommerceApiSettings
): SimpleProduct[] | ProductSearchResult[] => {
    const productsWithImageUrls: SimpleProduct[] | ProductSearchResult[] = [];

    products &&
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        products.forEach((product: any) => {
            if (product && product.RecordId) {
                product.PrimaryImageUrl = generateImageUrl(product.PrimaryImageUrl, apiSettings);
                productsWithImageUrls.push(product);
            }
        });

    return productsWithImageUrls;
};
