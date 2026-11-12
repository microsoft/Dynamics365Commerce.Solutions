/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { IAddressAddItem, IAddressAddUpdateProps } from '../../common/components/custom-address-add';
import { IBusinessAccountAddressViewProps } from './custom-business-account-address';

/**
 * Address Add Update Component.
 * @param param0 - Root param.
 * @param param0.AddressForm - Address form.
 * @param param0.heading - Address Heading.
 * @param param0.items - IAddressAddItem[].
 * @param param0.hasError - Boolean.
 * @param param0.error - IAddressError.
 * @param param0.isShowSaveButton - Boolean.
 * @param param0.saveButton - Save button.
 * @param param0.isShowCancelButton - Boolean.
 * @param param0.cancelButton - Cancel button.
 * @returns - AddressForm Node.
 */
const AddressAddUpdate: React.FC<IAddressAddUpdateProps> = ({
    AddressForm,
    heading,
    items,
    hasError,
    error,
    isShowSaveButton,
    saveButton,
    isShowCancelButton,
    cancelButton
}) => {
    return (
        <Node {...AddressForm}>
            {heading}
            {items.map((item: IAddressAddItem) => {
                const { AddressItem, key, label, alert, input } = item;
                return (
                    <Node {...AddressItem} key={key}>
                        {label}
                        {hasError && alert}
                        {input}
                    </Node>
                );
            })}
            {hasError && (
                <Node {...error.AddressError}>
                    {error.title}
                    {error.message}
                </Node>
            )}
            {isShowSaveButton && saveButton}
            {isShowCancelButton && cancelButton}
        </Node>
    );
};

/**
 * Business Account Address View Component.
 * @param props - Props.
 * @returns - BusinessAccountAddress Module.
 */
const BusinessAccountAddressView: React.FC<IBusinessAccountAddressViewProps> = props => {
    const { BusinessAccountAddress, showAddOrUpdateAddress } = props;

    return (
        <Module {...BusinessAccountAddress}>
            <AddressAddUpdate {...showAddOrUpdateAddress} />
        </Module>
    );
};

export default BusinessAccountAddressView;
