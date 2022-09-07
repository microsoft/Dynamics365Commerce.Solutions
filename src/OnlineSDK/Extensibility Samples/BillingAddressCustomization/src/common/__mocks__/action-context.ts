/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { buildHydratedMockActionContext } from '@msdyn365-commerce/core';

import mockCountryRegions from './country-regions';
import mockUsStates from './us-states';

const countries = [...mockCountryRegions];
const states = [...mockUsStates];

const mockActionContext = buildHydratedMockActionContext({});

// @ts-expect-error
jest.spyOn(mockActionContext, 'chainAction').mockImplementation((action, input) => {
    if (input) {
        // @ts-expect-error
        const cacheObjectType: string = input._cacheObjectType;
        if (cacheObjectType === 'CountryRegionInfo') {
            return Promise.resolve(countries);
        }

        if (cacheObjectType === 'StateProvinceInfo') {
            return Promise.resolve(states);
        }
    }
});

export default mockActionContext;
