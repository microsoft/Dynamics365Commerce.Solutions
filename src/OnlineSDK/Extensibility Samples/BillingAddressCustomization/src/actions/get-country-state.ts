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
import { CountryRegionInfo, StateProvinceInfo } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import { buildCacheKey } from '@msdyn365-commerce-modules/retail-actions';

import { getCountryRegionsAction, GetCountryRegionsInput } from './get-country-regions';
import { getStateProvinceAction, GetStateProvincesInput } from './get-state-provinces';

/**
 * Input class for get country regions data action.
 */
export class GetCountryStateInput implements IActionInput {
    public locale: string;

    public apiSettings: ICommerceApiSettings;

    constructor(locale: string, apiSettings: ICommerceApiSettings) {
        this.locale = locale;
        this.apiSettings = apiSettings;
    }

    public getCacheKey = () => buildCacheKey(`CountryStateInfo-${this.locale}`, this.apiSettings);

    public getCacheObjectType = () => 'StateProvinceInfo';

    public dataCacheType = (): CacheType => 'request';
}

/**
 * Creates the input required to make the retail api call.
 * @param inputData
 */
const createInput = (inputData: ICreateActionContext<IGeneric<IAny>>): GetCountryStateInput => {
    const { locale, apiSettings } = inputData.requestContext;
    if (!(inputData.requestContext && locale)) {
        throw new Error('Unable to create country regions input.  Locale is not provided.');
    }
    return new GetCountryStateInput(locale, apiSettings);
};

export async function getCountryStateAction(input: GetCountryStateInput, ctx: IActionContext): Promise<StateProvinceInfo[]> {
    const countryInput = new GetCountryRegionsInput(input.locale, input.apiSettings);
    const countryRegionsInfo = await getCountryRegionsAction(countryInput, ctx);

    const channelConfiguration = ctx.requestContext.channel;

    const marketISOCode = (channelConfiguration && channelConfiguration.ChannelCountryRegionISOCode) || 'US';
    const currentCountryRegion = countryRegionsInfo.find(countryRegion => (countryRegion.ISOCode || '') === marketISOCode);
    const countryRegionId = (currentCountryRegion && currentCountryRegion.CountryRegionId) || 'USA';

    const stateInput = new GetStateProvincesInput(countryRegionId, input.apiSettings);
    return getStateProvinceAction(stateInput, ctx);
}

export const getCountryStateActionDataAction = createObservableDataAction({
    id: '@msdyn365-commerce-modules/address/get-country-state',
    action: <IAction<CountryRegionInfo[]>>getCountryStateAction,
    input: createInput
});

export default getCountryStateActionDataAction;
