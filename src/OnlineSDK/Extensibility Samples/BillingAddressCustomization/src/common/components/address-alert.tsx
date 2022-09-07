/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import * as React from 'react';

export interface IAdressAlert {
    message?: string;
}

const AdressAlert: React.FC<IAdressAlert> = ({ message }) => (
    <span className='msc-address-form__alert' role='alert' aria-live='assertive'>
        {message && <>{message}</>}
    </span>
);

export default AdressAlert;
