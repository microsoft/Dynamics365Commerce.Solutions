/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { AsyncResult } from '@msdyn365-commerce/retail-proxy';

import { IAccordionExpandedState } from '../../actions/accordion-state/accordion-state';

export interface IAccordionData {
    accordionExpandedState: AsyncResult<IAccordionExpandedState>;
}
