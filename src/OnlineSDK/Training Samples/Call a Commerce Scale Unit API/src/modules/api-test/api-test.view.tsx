/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as React from 'react';
import { IApiTestViewProps } from './api-test';

export default (props: IApiTestViewProps) => {
    const { categories } = props;

    return (
        <div className='row'>
            <h2>Config Value: {props.config.showText}</h2>
            <h2>Resource Value: {props.resources.resourceKey}</h2>
            <div className='row'>
                <p>{categories}</p>
            </div>
        </div>
    );
};
