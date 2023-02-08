/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import {
    CacheType,
    createObservableDataAction,
    IAction,
    IActionContext,
    IActionInput,
    IAny,
    ICommerceApiSettings,
    ICreateActionContext,
    IGeneric
} from '@msdyn365-commerce/core';
import { SortColumn } from '@msdyn365-commerce/retail-proxy';
import { getSearchConfigurationAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/StoreOperationsDataActions.g';
import { buildCacheKey } from '@msdyn365-commerce-modules/retail-actions';

// TODO: import from commerce entities when available
// eslint-disable-next-line @typescript-eslint/naming-convention
interface SearchConfiguration {
    ProductSortColumns?: SortColumn[];
}

// eslint-disable-next-line @typescript-eslint/naming-convention
export interface MappedSearchConfiguration {
    key: string;
    sortColumn: SortColumn;
}

export const sortOptions = {
    sortByOptionRelevanceDesc: 'Ranking-Desc',
    sortByOptionRatingAsc: 'AverageRating-Asc',
    sortByOptionRatingDesc: 'AverageRating-Desc',
    sortByOptionNameAsc: 'Name-Asc',
    sortByOptionNameDesc: 'Name-Desc',
    sortByOptionPriceAsc: 'Price-Asc',
    sortByOptionPriceDesc: 'Price-Desc',
    sortByOptionBestSelling: 'BestSelling',
    sortByOptionNewScore: 'NewScore',
    sortByOptionTrendingScore: 'TrendingScore'
};

/**
 * ActionInput class for the get-mapped-search-configuration Data Action.
 */
export class MappedSearchInput implements IActionInput {
    private readonly apiSettings: ICommerceApiSettings;

    constructor(apiSettings: ICommerceApiSettings) {
        this.apiSettings = apiSettings;
    }

    public getCacheKey = () => buildCacheKey('MappedSearchInput', this.apiSettings);

    public getCacheObjectType = () => 'MappedSearchInput';

    public dataCacheType = (): CacheType => 'request';
}

/**
 * Get a mapped search configuration that is easy to use for sorting.
 * @param input
 * @param ctx
 */
export async function getMappedSearchConfiguration(input: MappedSearchInput, ctx: IActionContext): Promise<MappedSearchConfiguration[]> {
    return getSearchConfigurationAsync({ callerContext: ctx }).then((searchConfiguration: SearchConfiguration) => {
        return <MappedSearchConfiguration[]>(searchConfiguration.ProductSortColumns || [])
            .map<MappedSearchConfiguration | undefined>((sortColumn: SortColumn) => {
                let key = '';
                if (sortColumn.ColumnName) {
                    switch (sortColumn.ColumnName.toUpperCase()) {
                        case 'NAME':
                            key = sortColumn.IsDescending ? sortOptions.sortByOptionNameDesc : sortOptions.sortByOptionNameAsc;
                            break;
                        case 'PRICE':
                            key = sortColumn.IsDescending ? sortOptions.sortByOptionPriceDesc : sortOptions.sortByOptionPriceAsc;
                            break;
                        case 'AVERAGERATING':
                            key = sortColumn.IsDescending ? sortOptions.sortByOptionRatingDesc : sortOptions.sortByOptionRatingAsc;
                            break;
                        case 'RANKING':
                            key = sortOptions.sortByOptionRelevanceDesc;
                            break;
                        case 'BESTSELLINGSCORE':
                            key = sortOptions.sortByOptionBestSelling;
                            break;
                        case 'NEWSCORE':
                            key = sortOptions.sortByOptionNewScore;
                            break;
                        case 'TRENDINGSCORE':
                            key = sortOptions.sortByOptionTrendingScore;
                            break;
                        default:
                    }
                } else {
                    ctx.telemetry.warning('[get-mapped-search-configuration] unknown search option');
                }
                if (key) {
                    return {
                        key,
                        sortColumn
                    };
                }
                return undefined;
            })
            .filter(Boolean);
    });
}

export const createInput = (inputData: ICreateActionContext<IGeneric<IAny>>): IActionInput => {
    return new MappedSearchInput(inputData.requestContext.apiSettings);
};

export const getMappedSearchConfigurationDataAction = createObservableDataAction({
    id: '@msdyn365-commerce-modules/search-result-container/get-mapped-search-configuration',
    action: <IAction<MappedSearchConfiguration[]>>getMappedSearchConfiguration,
    input: createInput
});

export default getMappedSearchConfigurationDataAction;
