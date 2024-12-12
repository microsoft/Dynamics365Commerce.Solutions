/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */
import { AsyncResult, Category } from '@msdyn365-commerce/retail-proxy';

export interface IApiTestData {
    categories: AsyncResult<Category[]>;
    actionResponse: { text: string };
}
