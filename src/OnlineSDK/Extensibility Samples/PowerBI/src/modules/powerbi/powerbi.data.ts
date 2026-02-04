/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { AsyncResult } from '@msdyn365-commerce/retail-proxy';
import { IPowerBi } from '../../themes/fabrikam-extended/data-actions/DataServiceEntities.g';

export interface IPowerbiData {
    powerBIDetails: AsyncResult<IPowerBi>;
}
