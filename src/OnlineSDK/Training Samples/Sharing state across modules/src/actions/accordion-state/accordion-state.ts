/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { createObservableDataAction, IAction, IActionInput } from '@msdyn365-commerce/core';
import { GenericInput, getGenericAction } from '@msdyn365-commerce-modules/retail-actions';

export interface IAccordionExpandedState {
    isAllExpanded?: boolean;
}

export function createAccordionStateInput(result: IAccordionExpandedState): GenericInput<IAccordionExpandedState> {
    return new GenericInput<IAccordionExpandedState>('accordionExpandedState', result, 'IAccordionExpandedState');
}

const createAccordionStateInputInternal = (): IActionInput => {
    return createAccordionStateInput({});
};

export const getGenericActionDataAction = createObservableDataAction({
    action: <IAction<IAccordionExpandedState>>getGenericAction,
    input: createAccordionStateInputInternal
});

export default getGenericActionDataAction;
