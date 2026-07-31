/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { getPayloadObject, getTelemetryAttributes, ITelemetryContent } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

export interface IAdressInput {
    id?: string;
    name?: string;
    className: string;
    type: string;
    value: string;
    maxLength?: number;
    autoFocus?: boolean;
    additionalAddributes?: object;
    telemetryContent?: ITelemetryContent;
    onChange(event: React.ChangeEvent<HTMLInputElement>): void;
}

const AdressInput: React.FC<IAdressInput> = ({
    id,
    name,
    className,
    type,
    value,
    maxLength,
    additionalAddributes,
    autoFocus,
    telemetryContent,
    onChange
}) => {
    const payLoad = getPayloadObject('click', telemetryContent!, name!);
    const attributes = getTelemetryAttributes(telemetryContent!, payLoad);
    return (
        <input
            name={name}
            id={id}
            className={`${className}__input ${className}__input-${type}`}
            type={type}
            autoFocus={autoFocus}
            value={value}
            maxLength={maxLength}
            {...(additionalAddributes || {})}
            {...attributes}
            onChange={onChange}
        />
    );
};

export default AdressInput;
