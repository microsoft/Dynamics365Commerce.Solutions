/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as React from 'react';

export interface IAdressLabel {
    id: string;
    text: string;
}

const AdressLabel: React.FC<IAdressLabel> = ({ id, text }) => (
    <label className='msc-address-form__label' htmlFor={id}>
        {text}
    </label>
);

export default AdressLabel;
