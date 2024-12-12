/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as React from 'react';
import { IReadAppSettingsViewProps } from './read-app-settings';

export default (props: IReadAppSettingsViewProps) => {
    // Displays the cart route as plain text from the app settings.
    return (
        <div className='ms-text-block fontsize__large'>
            <div className='row'>Cart route from app settings: {props.context.app.routes.cart.destinationUrl}</div>
        </div>
    );
};
