/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import * as React from 'react';

export interface IAddressBillingHeading {
    text: string;
}

export const AddressBillingHeading: React.FC<IAddressBillingHeading> = ({ text }) => (
    <p className='ms-checkout-billing-address__heading'>{text}</p>
);

export default IAddressBillingHeading;
