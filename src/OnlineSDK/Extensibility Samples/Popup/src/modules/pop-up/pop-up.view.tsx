/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { IPopUpViewProps } from './pop-up';

/**
 * View component.
 * @param props - The view properties.
 * @returns - Pop up view.
 */
export const popUpView: React.FC<IPopUpViewProps> = props => {
    const { moduleProps, headerContainerProps, headerProps, bodyContainerProps, bodyContent } = props;

    return (
        <Module {...moduleProps}>
            <Node {...headerContainerProps}>{headerProps}</Node>
            <Node {...bodyContainerProps}>{bodyContent}</Node>
        </Module>
    );
};

export default popUpView;
