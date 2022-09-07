/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { ITelemetryContent } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import AddressInputComponent from './address-input';

export interface IAddressBillingCheckbox {
    isChecked: boolean;
    value: string;
    ariaLabel: string;
    text: string;
    telemetryContent?: ITelemetryContent;
    onChange(event: React.ChangeEvent<HTMLInputElement>): void;
}

export const AddressBillingCheckbox: React.FC<IAddressBillingCheckbox> = ({
    isChecked,
    value,
    ariaLabel,
    text,
    telemetryContent,
    onChange
}) => {
    const additionalAttributes = {
        checked: isChecked,
        'aria-checked': isChecked
    };

    return (
        <label className='ms-checkout-billing-address__shipping-address-label'>
            <AddressInputComponent
                {...{
                    type: 'checkbox',
                    className: 'ms-checkout-billing-address',
                    value,
                    onChange,
                    additionalAddributes: additionalAttributes,
                    telemetryContent
                }}
            />
            <span className='ms-checkout-billing-address__shipping-address-checkbox-text'>{text}</span>
        </label>
    );
};
export default AddressBillingCheckbox;
