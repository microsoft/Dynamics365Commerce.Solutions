/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

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
import { getCountryRegionsByLanguageIdAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/StoreOperationsDataActions.g';
import { CountryRegionInfo } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import { buildCacheKey } from '@msdyn365-commerce-modules/retail-actions';

/**
 * Input class for get country regions data action.
 */
export class GetCountryRegionsInput implements IActionInput {
    public locale: string;

    private readonly apiSettings: ICommerceApiSettings;

    constructor(locale: string, apiSettings: ICommerceApiSettings) {
        this.locale = locale;
        this.apiSettings = apiSettings;
    }

    public getCacheKey = () => buildCacheKey(`CountryRegionInfo-${this.locale}`, this.apiSettings);

    public getCacheObjectType = () => 'CountryRegionInfo';

    public dataCacheType = (): CacheType => 'request';
}

/**
 * Creates the input required to make the retail api call.
 * @param inputData
 */
const createInput = (inputData: ICreateActionContext<IGeneric<IAny>>): GetCountryRegionsInput => {
    if (!(inputData.requestContext && inputData.requestContext.locale)) {
        throw new Error('Unable to create country regions input.  Locale is not provided.');
    }

    return new GetCountryRegionsInput(inputData.requestContext.locale, inputData.requestContext.apiSettings);
};

export async function getCountryRegionsAction(input: GetCountryRegionsInput, ctx: IActionContext): Promise<CountryRegionInfo[]> {
    return getCountryRegionsByLanguageIdAsync({ callerContext: ctx, queryResultSettings: {} }, input.locale);
}

export const getCountryRegionsActionDataAction = createObservableDataAction({
    action: <IAction<CountryRegionInfo[]>>getCountryRegionsAction,
    input: createInput
});

export default getCountryRegionsActionDataAction;
