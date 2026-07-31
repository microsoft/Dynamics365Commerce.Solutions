/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import { AsyncResult } from '@msdyn365-commerce/retail-proxy';
import { IStoreSelectorStateManager } from '@msdyn365-commerce-modules/bopis-utilities';
import { IFullOrgUnitAvailability } from '@msdyn365-commerce-modules/retail-actions';

export interface ICustomStoreSelectorData {
    storeSelectorStateManager: AsyncResult<IStoreSelectorStateManager>;
    availabilitiesWithHours: AsyncResult<IFullOrgUnitAvailability[]>;
    storeLocations: AsyncResult<IFullOrgUnitAvailability[]>;
}
