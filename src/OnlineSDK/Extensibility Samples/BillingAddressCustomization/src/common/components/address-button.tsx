/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { getPayloadObject, getTelemetryAttributes, ITelemetryContent } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

export interface IAddressButton {
    className: string;
    text: string;
    ariaLabel: string;
    disabled?: boolean;
    telemetryContent?: ITelemetryContent;
    role?: string;
    onClick(event: React.MouseEvent<HTMLElement>): void;
}

const AddressButton: React.FC<IAddressButton> = ({ className, text, ariaLabel, disabled, telemetryContent, onClick, role }) => {
    const payLoad = getPayloadObject('click', telemetryContent!, text);
    const attributes = getTelemetryAttributes(telemetryContent!, payLoad);
    return (
        <button className={className} aria-label={ariaLabel} disabled={disabled} role={role} onClick={onClick} {...attributes}>
            {text}
        </button>
    );
};

export const ShippingAddressButton: React.FC<IAddressButton> = ({
    className,
    text,
    ariaLabel,
    disabled,
    telemetryContent,
    onClick,
    role
}) => {
    const payLoad = getPayloadObject('click', telemetryContent!, text);
    const attributes = getTelemetryAttributes(telemetryContent!, payLoad);
    return (
        <button className={className} aria-label={ariaLabel} disabled={disabled} role={role} onClick={onClick} {...attributes}>
            {text}
        </button>
    );
};

export const BillingAddressButton: React.FC<IAddressButton> = ({
    className,
    text,
    ariaLabel,
    disabled,
    telemetryContent,
    onClick,
    role
}) => {
    const payLoad = getPayloadObject('click', telemetryContent!, text);
    const attributes = getTelemetryAttributes(telemetryContent!, payLoad);
    return (
        <button className={className} aria-label={ariaLabel} disabled={disabled} role={role} onClick={onClick} {...attributes}>
            {text}
        </button>
    );
};

export default AddressButton;
