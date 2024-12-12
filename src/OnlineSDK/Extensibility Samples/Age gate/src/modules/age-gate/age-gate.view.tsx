/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { IAgeGateViewProps } from './age-gate';

/**
 * View component.
 * @param props - The view properties.
 * @returns - Age gate view.
 */
export const ageGateView: React.FC<IAgeGateViewProps> = props => {
    const { moduleProps, headerContainerProps, headerProps, bodyContainerProps, bodyContent, consentButton } = props;

    return (
        <Module {...moduleProps}>
            <Node {...headerContainerProps}>{headerProps}</Node>
            <Node {...bodyContainerProps}>
                {bodyContent}
                {consentButton}
            </Node>
        </Module>
    );
};

export default ageGateView;
