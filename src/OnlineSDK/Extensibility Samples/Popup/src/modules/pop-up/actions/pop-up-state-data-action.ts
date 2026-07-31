/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { createObservableDataAction, IAction, IActionInput } from '@msdyn365-commerce/core';
import { GenericInput, getGenericAction } from '@msdyn365-commerce-modules/retail-actions';

import { IPopUpState } from './pop-up-state';

/**
 * This method builds the input for the createPopUpStateInput call.
 * @param result - Popup open state.
 * @returns - New GenericInput.
 */
export function setPopUpState(result: IPopUpState): GenericInput<IPopUpState> {
    return new GenericInput<IPopUpState>('popUpState', result, 'IPopUpState');
}

/**
 * This method returns the input for the createPopUpOpenStateInput call.
 * @returns - New Popup state input.
 */
const createPopUpStateInput = (): IActionInput => {
    return setPopUpState({});
};

/**
 * Generic data action.
 */
export const getGenericActionDataAction = createObservableDataAction({
    action: getGenericAction as IAction<IPopUpState>,
    input: createPopUpStateInput
});

export default getGenericActionDataAction;
