/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { buildHydratedMockActionContext } from '@msdyn365-commerce/core';

import mockCountryRegions from './country-regions';
import mockUsStates from './us-states';

const countries = [...mockCountryRegions];
const states = [...mockUsStates];

const mockActionContext = buildHydratedMockActionContext({});

// @ts-expect-error -- unused action paramter.
jest.spyOn(mockActionContext, 'chainAction').mockImplementation((action, input) => {
    // eslint-disable-line consistent-return -- test file.
    // eslint-disable-next-line @typescript-eslint/no-unnecessary-condition -- checking if the input in test cases.
    if (input) {
        // @ts-expect-error -- input is of any type.
        const cacheObjectType: string = input._cacheObjectType; // eslint-disable-line @typescript-eslint/no-unsafe-assignment -- Any type for input.
        if (cacheObjectType === 'CountryRegionInfo') {
            return Promise.resolve(countries);
        }

        if (cacheObjectType === 'StateProvinceInfo') {
            return Promise.resolve(states);
        }
    }
});

export default mockActionContext;
