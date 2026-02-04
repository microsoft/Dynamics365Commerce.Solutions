/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as Msdyn365 from '@msdyn365-commerce/core';
import { ILocation } from './get-current-weather-conditions.action';

const locations: ILocation[] = [
    { id: '2643743', name: 'London, UK' },
    { id: '5856195', name: 'Honolulu, HI' },
    { id: '2147714', name: 'Sydney, AU' },
    { id: '5809844', name: 'Seattle, WA' }
];

/**
 * input for get-favorites
 */
export class GetLocationsInput implements Msdyn365.IActionInput {
    public getCacheKey = () => 'Default';
    public getCacheObjectType = () => 'FAVORITE-Locations';
    public dataCacheType = (): Msdyn365.CacheType => 'request';
}

const createLocationsInput = (): Msdyn365.IActionInput => {
    return new GetLocationsInput();
};

async function action(): Promise<ILocation[]> {
    return locations;
}

export default Msdyn365.createObservableDataAction({
    action: <Msdyn365.IAction<ILocation[]>>action,
    id: 'get-favorites-location',
    input: createLocationsInput
});
