/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import { setPopUpState } from '../pop-up';
import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { ISubscribeViewProps } from './subscribe';

/**
 * Override form action and button action below.
 * @param event - The form event.
 */
const onSubmit = (props: ISubscribeViewProps) => (event: React.FormEvent<HTMLFormElement>): void => {
    event.preventDefault();
    console.log('Subscibe module');
    const {
        context: {
            actionContext,
            app: {
                config: { popUpCookieName }
            },
            request: { cookies }
        }
    } = props;

    const popUpState = { isOpen: false };
    const cookieAge: number = 172_800;
    const cookieName = (popUpCookieName as string) || 'msdyn365_popUp_';
    cookies.set(cookieName, 'Subscribed', { maxAge: cookieAge });
    actionContext.update(setPopUpState(popUpState), popUpState);
};

/**
 * View component.
 * @param props - The view properties.
 * @returns - Returns nothing.
 */
export const subscribeView: React.FC<ISubscribeViewProps> = props => {
    const { subscribe, subscribeContainer, heading, text, subscribeForm, emailInput, submitButton } = props;
    return (
        <Module {...subscribe}>
            <Node {...subscribeContainer} className={subscribeContainer.className}>
                {heading}
                {text}
                <Node {...subscribeForm} onSubmit={onSubmit(props)} className={subscribeForm?.className ?? ''}>
                    <Node {...emailInput} className={emailInput?.className ?? ''} />
                    <Node {...submitButton} className={submitButton?.className ?? ''}>
                        {props.submitButtonLabelText}
                    </Node>
                </Node>
            </Node>
        </Module>
    );
};

export default subscribeView;
