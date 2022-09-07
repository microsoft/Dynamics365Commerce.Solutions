/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

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
import { getAddressPurposesAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/StoreOperationsDataActions.g';
import { AddressPurpose } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import { buildCacheKey } from '@msdyn365-commerce-modules/retail-actions';

/**
 * Input class for get Address Purposes data action.
 */
export class GetAddressPurposesInput implements IActionInput {
    private readonly apiSettings: ICommerceApiSettings;

    constructor(apiSettings: ICommerceApiSettings) {
        this.apiSettings = apiSettings;
    }

    public getCacheKey = () => buildCacheKey('AddressPurpose', this.apiSettings);

    public getCacheObjectType = () => 'AddressPurpose';

    public dataCacheType = (): CacheType => 'request';
}

/**
 * Creates the input required to make the retail api call.
 * @param inputData
 */
const createInput = (inputData: ICreateActionContext<IGeneric<IAny>>): GetAddressPurposesInput => {
    return new GetAddressPurposesInput(inputData.requestContext.apiSettings);
};

export async function getAddressPurposesAction(input: GetAddressPurposesInput, ctx: IActionContext): Promise<AddressPurpose[]> {
    return getAddressPurposesAsync({ callerContext: ctx, queryResultSettings: {} });
}

export const getAddressPurposesActionDataAction = createObservableDataAction({
    id: '@msdyn365-commerce-modules/address/get-address-purposes',
    action: <IAction<AddressPurpose[]>>getAddressPurposesAction,
    input: createInput
});

export default getAddressPurposesActionDataAction;
