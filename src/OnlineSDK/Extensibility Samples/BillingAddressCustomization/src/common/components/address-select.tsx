/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { Address, AddressPurpose } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import { INodeProps, ITelemetryContent } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { AddressFormat } from '../address-format';
import { IAddressResource } from '../address-module.data';
import AddressButtonComponent from './address-button';
import AddressInputComponent from './address-input';
import { AddressShow, IAddressShowProps } from './address-show';

export interface IAddressSelectInputProps {
    hasExternalSubmitGroup?: boolean;
    addressFormat: AddressFormat;
    addresses: Address[];
    addressPurposes: AddressPurpose[];
    resources: IAddressResource;
    selectedAddress: Address;
    telemetryContent?: ITelemetryContent;
    onAddressOptionChange(event: React.ChangeEvent<HTMLInputElement>): void;
    onAddAddress(): void;
    onSave(): void;
    onCancel(): void;
}

export interface IAddressSelectItem {
    key: number;
    SelectItem: INodeProps;
    input: React.ReactNode;
    showItems: IAddressShowProps;
}

export interface IAddressSelectProps {
    SelectAddress: INodeProps;
    addButton: React.ReactNode;
    items: IAddressSelectItem[];
    isShowSaveButton: boolean;
    saveButton: React.ReactNode;
    isShowCancelButton: boolean;
    cancelButton: React.ReactNode;
}

const getInput = (index: number, address: Address, props: IAddressSelectInputProps): React.ReactNode => {
    const { addresses, onAddressOptionChange, selectedAddress, resources } = props;

    const ichecked = address.RecordId === selectedAddress.RecordId;
    const additionalAttributes = {
        checked: ichecked,
        'aria-checked': ichecked,
        'aria-setsize': addresses.length,
        'aria-posinset': index + 1,
        'aria-label': resources.addressChangeCheckboxAriaLabel
    };

    return (
        <AddressInputComponent
            {...{
                className: 'msc-address-select',
                name: 'selectAddressOptions',
                type: 'radio',
                value: (address.RecordId || '').toString(),
                onChange: onAddressOptionChange,
                additionalAddributes: additionalAttributes,
                telemetryContent: props.telemetryContent
            }}
        />
    );
};

const getAddressSelectItems = (props: IAddressSelectInputProps): IAddressSelectItem[] => {
    const { addresses, addressFormat, addressPurposes } = props;

    return addresses.map((address, index) => {
        return {
            key: address.RecordId || 0,
            SelectItem: { className: 'msc-address-select__item' },
            input: getInput(index, address, props),
            showItems: AddressShow({
                addressFormat: addressFormat.getAddressFormat(address.ThreeLetterISORegionName || ''),
                address,
                addressPurposes
            })
        };
    });
};

export const AddressSelect = (props: IAddressSelectInputProps): IAddressSelectProps => {
    const { resources, onCancel, onSave, onAddAddress, hasExternalSubmitGroup } = props;

    return {
        SelectAddress: { className: 'msc-address-select' },
        addButton: (
            <AddressButtonComponent
                {...{
                    className: 'msc-address-select__button-add',
                    text: resources.addressAddButtonText,
                    ariaLabel: resources.addressAddButtonAriaLabel,
                    onClick: onAddAddress,
                    telemetryContent: props.telemetryContent
                }}
            />
        ),
        isShowSaveButton: !hasExternalSubmitGroup,
        saveButton: (
            <AddressButtonComponent
                {...{
                    className: 'msc-address-select__button-save',
                    text: resources.addressSaveButtonText,
                    ariaLabel: resources.addressSaveButtonAriaLabel,
                    onClick: onSave,
                    telemetryContent: props.telemetryContent
                }}
            />
        ),
        isShowCancelButton: !hasExternalSubmitGroup,
        cancelButton: (
            <AddressButtonComponent
                {...{
                    className: 'msc-address-select__button-cancel',
                    text: resources.addressCancelButtonText,
                    ariaLabel: resources.addressCancelButtonAriaLabel,
                    onClick: onCancel,
                    telemetryContent: props.telemetryContent
                }}
            />
        ),
        items: getAddressSelectItems(props)
    };
};
