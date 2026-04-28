/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as React from 'react';
import { IConfigVisibilityViewProps } from './config-visibility';

export default (props: IConfigVisibilityViewProps) => {
    return (
        <div className='row'>
            <h2>Product Title: {props.config.productTitle}</h2>
            <h2>Product Sub Title: {props.config.productTitle}</h2>
        </div>
    );
};
