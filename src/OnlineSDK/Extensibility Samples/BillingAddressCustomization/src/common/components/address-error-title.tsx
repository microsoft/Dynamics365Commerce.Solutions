/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as React from 'react';

export interface IAdressErrorTitle {
    title: string;
}

const AdressErrorTitle: React.FC<IAdressErrorTitle> = ({ title }) => <p className='msc-address-form__error-title'>{title}</p>;

export default AdressErrorTitle;
