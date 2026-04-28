/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { IHistogramItemViewProps, IRatingsHistogramViewProps } from '@msdyn365-commerce-modules/ratings-reviews';
import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

const RatingsHistogramView: React.FC<IRatingsHistogramViewProps> = props => {
    const { heading, histogramItems, histogramProps, moduleProps } = props;
    return (
        <Module {...moduleProps}>
            {heading}
            <Node {...histogramProps}>
                {histogramItems.map(item => {
                    return histogramItem(item);
                })}
            </Node>
        </Module>
    );
};

const histogramItem = (props: IHistogramItemViewProps) => {
    return (
        <Node {...props.buttonProps}>
            {props.starLabel}
            {props.bar}
            {props.percentage}
        </Node>
    );
};

export default RatingsHistogramView;
