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

/**
 * Address checkbox.
 * @param props - Configuration of the functional component.
 * @returns React functional component.
 */
export const AddressCheckboxFunctionComponent: React.FC<IAdressCheckbox> = (props: IAdressCheckbox) => {
    const {
        id: itemId,
        name,
        className,
        type,
        isChecked,
        additionalAddributes,
        telemetryContent,
        autoFocus: shouldBeAutoFocused,
        onChange
    } = props;

    const payLoad = getPayloadObject('click', telemetryContent!, name!);
    const attributes = getTelemetryAttributes(telemetryContent!, payLoad);
    return (
        <input
            name={name}
            id={itemId}
            className={`${className}__input ${className}__input-${type}`}
            type='checkbox'
            autoFocus={shouldBeAutoFocused}
            aria-checked={isChecked}
            onChange={onChange}
            checked={isChecked}
            {...(additionalAddributes || {})}
            {...attributes}
        />
    );
};

export default AddressCheckboxFunctionComponent;
