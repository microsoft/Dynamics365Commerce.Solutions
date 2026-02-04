/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { IActionContext, ICreateActionContext } from '@msdyn365-commerce/core-internal';
import { createListPageInput, listPageStateAction, ListPageStateInput } from '../list-page-state';

describe('list page state test cases', () => {
    it('createListPageInput', () => {
        const instance = createListPageInput({} as ICreateActionContext);
        expect(instance).toBeDefined();
    });

    it('listPageStateAction', () => {
        const instance = listPageStateAction({} as ListPageStateInput, {} as IActionContext);
        expect(instance).toBeDefined();
    });
});
