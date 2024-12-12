/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { getPayloadObject, getTelemetryAttributes, ITelemetryContent } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

export interface IAdressCheckbox {
    id?: string;
    name?: string;
    className: string;
    type: string;
    isChecked: boolean;
    autoFocus?: boolean;
    additionalAddributes?: object;
    telemetryContent?: ITelemetryContent;
    onChange(event: React.ChangeEvent<HTMLInputElement>): void;
}

const AdressCheckbox: React.FC<IAdressCheckbox> = ({
    id,
    name,
    className,
    type,
    isChecked,
    additionalAddributes,
    telemetryContent,
    autoFocus,
    onChange
}) => {
    const payLoad = getPayloadObject('click', telemetryContent!, name!);
    const attributes = getTelemetryAttributes(telemetryContent!, payLoad);
    return (
        <input
            name={name}
            id={id}
            className={`${className}__input ${className}__input-${type}`}
            type='checkbox'
            autoFocus={autoFocus}
            aria-checked={isChecked}
            onChange={onChange}
            checked={isChecked}
            {...(additionalAddributes || {})}
            {...attributes}
        />
    );
};

export default AdressCheckbox;
