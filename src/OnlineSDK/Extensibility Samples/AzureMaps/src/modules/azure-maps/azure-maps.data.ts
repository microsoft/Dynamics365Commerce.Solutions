
/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { AsyncResult } from '@msdyn365-commerce/retail-proxy';
import { IStoreSelectorStateManager } from '@msdyn365-commerce-modules/bopis-utilities';
import { IDistributorSelectorStateManager } from '@msdyn365-commerce-modules/distributor-utilities';

export interface IAzureMapsData {
    actionResponse?: { text: string };
    distributorSelectorStateManager?: AsyncResult<IDistributorSelectorStateManager>;
    storeSelectorStateManager?: AsyncResult<IStoreSelectorStateManager>;
}
