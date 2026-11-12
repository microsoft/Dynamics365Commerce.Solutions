/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { AsyncResult } from '@msdyn365-commerce/core';

import { IPopUpState } from './actions';

/**
 * Pop up data.
 */
export interface IPopUpData {
    popUpState: AsyncResult<IPopUpState>;
}
