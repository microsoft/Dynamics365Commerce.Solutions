/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import * as React from 'react';

export interface IAdressErrorMessage {
    message: string;
}

const AdressErrorMessage: React.FC<IAdressErrorMessage> = ({ message }) => <p className='msc-address-form__error-message'>{message}</p>;

export default AdressErrorMessage;
