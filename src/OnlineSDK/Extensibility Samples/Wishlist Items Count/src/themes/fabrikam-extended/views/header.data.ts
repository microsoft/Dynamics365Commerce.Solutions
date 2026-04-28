/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { IStoreSelectorStateManager } from '@msdyn365-commerce-modules/bopis-utilities';
import { ICartState } from '@msdyn365-commerce/global-state';
import { AsyncResult, CommerceList, Customer } from '@msdyn365-commerce/retail-proxy';

export interface IHeaderData {
    cart: AsyncResult<ICartState>;
    accountInformation: AsyncResult<Customer>;
    storeSelectorStateManager: AsyncResult<IStoreSelectorStateManager>;
    wishlists?: AsyncResult<CommerceList[]>;
}
