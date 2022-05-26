/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { buildHydratedMockActionContext } from '@msdyn365-commerce/core';
import { IActionContext, ICommerceApiSettings, IRequestContext } from '@msdyn365-commerce/core-internal';
import { CacheType, IActionInput, IQueryResultSettings, ProductRefinerValue } from '@msdyn365-commerce/retail-proxy';
import { actionDataAction, GetFullProductsByCollectionInput } from '../get-full-products-by-collection.action';

describe('Role based caching test cases', () => {
    const mockRequestContextJson: any = {
        url: {
            serverPageUrl: 'https://localhost:3000'
        },
        locale: 'en-us',
        apiSettings: {
            baseUrl: '',
            baseImageUrl: '',
            channelId: 0,
            oun: '',
            ratingsReviewsEndpoint: ''
        },
        user: {
            token: 'Dummy token',
            isAuthenticated: true
        },
        urlTokens: {
            locale: 'en-us',
            categories: ['fashion-sunglasses']
        }
    };

    const anonymousRefiner: ProductRefinerValue[] = [
        {
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
            //@ts-ignore
            '@odata.type': '#Microsoft.Dynamics.Commerce.Runtime.DataModel.ProductRefinerValue'
        }
    ];

    const user1Refiner: ProductRefinerValue[] = [
        {
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
            //@ts-ignore
            '@odata.type': '#Microsoft.Dynamics.Commerce.Runtime.DataModel.ProductRefinerValue'
        }
    ];

    const user2Refiner: ProductRefinerValue[] = [
        {
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
            //@ts-ignore
            '@odata.type': '#Microsoft.Dynamics.Commerce.Runtime.DataModel.ProductRefinerValue'
        }
    ];

    it('Should return valid instance', () => {
        const instance = actionDataAction({} as IActionInput, {} as IActionContext);
        expect(instance).toBeDefined();
    });

    it('If no role has been set, all the products will be listed without any filter being applied - customer type : Anonymous', () => {
        const actionInput: GetFullProductsByCollectionInput = {
            getCacheKey: () => 'FullProductByCollection',
            getCacheObjectType: (): string => 'FullProductSearchResult',
            dataCacheType: (): CacheType => {
                return 'none';
            },
            pageType: 'Category',
            customerType: 'Anonymous',
            category: 0,
            searchText: '',
            refiners: anonymousRefiner,
            queryResultSettings: {} as IQueryResultSettings,
            apiSettings: {} as ICommerceApiSettings,
            includeAttributes: true
        };
        let actionContext: IActionContext;
        actionContext = buildHydratedMockActionContext({ requestContext: mockRequestContextJson as IRequestContext });
        actionContext.requestContext.user.isAuthenticated = false;
        const instance = actionDataAction(actionInput, actionContext);
        expect(instance).toBeDefined();
    });
    it('When role based caching is implemented as per this sample, products will be filtered on the page load depending on the role that is set - customer type : Temple', () => {
        const actionInput: GetFullProductsByCollectionInput = {
            getCacheKey: () => 'FullProductByCollection',
            getCacheObjectType: (): string => 'FullProductSearchResult',
            dataCacheType: (): CacheType => {
                return 'none';
            },
            pageType: 'Category',
            customerType: 'Temple',
            category: 0,
            searchText: '',
            refiners: user1Refiner,
            queryResultSettings: {} as IQueryResultSettings,
            apiSettings: {} as ICommerceApiSettings,
            includeAttributes: true
        };
        let actionContext: IActionContext;
        actionContext = buildHydratedMockActionContext({ requestContext: mockRequestContextJson as IRequestContext });
        actionContext.requestContext.user.isAuthenticated = true;
        const instance = actionDataAction(actionInput, actionContext);
        expect(instance).toBeDefined();
    });

    it('When role based caching is implemented as per this sample, products will be filtered on the page load depending on the role that is set - customer type : Endowment', () => {
        const actionInput: GetFullProductsByCollectionInput = {
            getCacheKey: () => 'FullProductByCollection',
            getCacheObjectType: (): string => 'FullProductSearchResult',
            dataCacheType: (): CacheType => {
                return 'none';
            },
            pageType: 'Category',
            customerType: 'Endowment',
            category: 0,
            searchText: '',
            refiners: user2Refiner,
            queryResultSettings: {} as IQueryResultSettings,
            apiSettings: {} as ICommerceApiSettings,
            includeAttributes: true
        };
        let actionContext: IActionContext;
        actionContext = buildHydratedMockActionContext({ requestContext: mockRequestContextJson as IRequestContext });
        actionContext.requestContext.user.isAuthenticated = true;
        const instance = actionDataAction(actionInput, actionContext);
        expect(instance).toBeDefined();
    });
});
