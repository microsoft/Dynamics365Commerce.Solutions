/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { Address } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import { Heading as HeadingData } from '@msdyn365-commerce-modules/data-types';
import { Heading, INodeProps, ITelemetryContent } from '@msdyn365-commerce-modules/utilities';
import classname from 'classnames';
import { get } from 'mobx';
import * as React from 'react';

import {
    AddressItemDisplayType,
    AddressItemType,
    AddressValidationRuleType,
    IAddressDropdownsData,
    IAddressItem,
    IAddressValidationRule
} from '../address-format.data';
import { AddressOperation, AddressType, IAddressResource, IAddressResponse } from '../address-module.data';
import AddressAlertComponent from './address-alert';
import AddressButtonComponent from './address-button';
import AdressCheckboxComponent from './address-checkbox';
import AdressDropdownComponent from './address-dropdown';
import AddressErrorMessageComponent from './address-error-message';
import AddressErrorTitleComponent from './address-error-title';
import AddressInputComponent from './address-input';
import AddressLabelComponent from './address-label';

export interface IAddressAddInputProps {
    isUpdating?: boolean;
    hasError?: boolean;
    hasExternalSubmitGroup?: boolean;
    addressType: AddressType;
    addressFormat: IAddressItem[];
    currentOperation: AddressOperation;
    isCheckoutBillingAddress?: boolean;
    defaultCountryRegionId: string;
    defaultAddressType: number;
    selectedAddress?: Address;
    dropdownDisplayData: IAddressDropdownsData;
    resources: IAddressResource;
    addressActionResponse?: IAddressResponse;
    addAddressHeading?: HeadingData;
    editAddressHeading?: HeadingData;
    validationError?: object;
    telemetryContent?: ITelemetryContent;
    onInputChange(event: React.ChangeEvent<HTMLInputElement>): void;
    onDropdownChange(event: React.ChangeEvent<HTMLSelectElement>): void;
    onSave?(): void;
    onCancel?(): void;
}

export interface IAddressError {
    AddressError: INodeProps;
    title: React.ReactNode;
    message: React.ReactNode;
}

export interface IAddressAddItem {
    key: string;
    AddressItem: INodeProps;
    label: React.ReactNode;
    alert: React.ReactNode;
    input: React.ReactNode;
}

export interface IAddressAddUpdateProps {
    AddressForm: INodeProps;
    heading: React.ReactNode;
    items: IAddressAddItem[];
    hasError: boolean;
    error: IAddressError;
    isShowSaveButton: boolean;
    saveButton: React.ReactNode;
    isShowCancelButton: boolean;
    cancelButton: React.ReactNode;
}

const getRequriedAttribute = (validationRules?: IAddressValidationRule[]): object => {
    const requriedRule = (validationRules || []).find(validationRule => {
        return validationRule.type === AddressValidationRuleType.Required;
    });

    return requriedRule ? { 'aria-required': true } : {};
};

const getAddessItems = (selectedAddress: Address, props: IAddressAddInputProps): IAddressAddItem[] => {
    const {
        addressFormat,
        currentOperation,
        isCheckoutBillingAddress,
        addressType,
        dropdownDisplayData,
        defaultCountryRegionId,
        defaultAddressType,
        validationError = {},
        onInputChange,
        onDropdownChange
    } = props;

    return addressFormat.map((addressFormatItem, index) => {
        const elementId = `${addressType.toLowerCase()}_address${addressFormatItem.name.toLowerCase()}`;
        const errorMessage = get(validationError, addressFormatItem.name);
        const className = classname('msc-address-form__item', `msc-address-form__item-${addressFormatItem.name.toLowerCase()}`, {
            'msc-address-form__item-newline': addressFormatItem.isNewLine,
            'address-form__item-invalid': errorMessage
        });
        let input;

        if (addressFormatItem.displayType === AddressItemDisplayType.Input) {
            input = (
                <AddressInputComponent
                    {...{
                        id: elementId,
                        name: addressFormatItem.name,
                        className: 'msc-address-form',
                        type: 'text',
                        autoFocus: index === 0,
                        value: selectedAddress[addressFormatItem.name],
                        maxLength: addressFormatItem.maxLength,
                        onChange: onInputChange,
                        additionalAddributes: getRequriedAttribute(addressFormatItem.validationRules)
                    }}
                />
            );
        } else if (addressFormatItem.displayType === AddressItemDisplayType.Checkbox) {
            input = (
                <AdressCheckboxComponent
                    {...{
                        id: elementId,
                        name: addressFormatItem.name,
                        className: 'msc-address-form',
                        type: 'checkbox',
                        autoFocus: index === 0,
                        isChecked: selectedAddress[addressFormatItem.name],
                        onChange: onInputChange,
                        additionalAddributes: getRequriedAttribute(addressFormatItem.validationRules)
                    }}
                />
            );
        } else {
            const displayData = dropdownDisplayData[addressFormatItem.name];
            let selectedValue = selectedAddress[addressFormatItem.name];

            if (addressFormatItem.type === AddressItemType.ThreeLetterISORegionName) {
                selectedValue = selectedValue || defaultCountryRegionId;
            }

            if (addressFormatItem.type === AddressItemType.AddressTypeValue) {
                selectedValue = selectedValue || defaultAddressType;
            }

            input = (
                <AdressDropdownComponent
                    {...{
                        id: elementId,
                        name: addressFormatItem.name,
                        className: 'msc-address-form',
                        value: selectedValue,
                        displayData,
                        onChange: onDropdownChange,
                        disabled: isDropDownDisable(isCheckoutBillingAddress, currentOperation, elementId) ? true : false,
                        additionalAddributes: getRequriedAttribute(addressFormatItem.validationRules)
                    }}
                />
            );
        }

        return {
            key: addressFormatItem.name,
            AddressItem: { className, id: `${elementId}_container` },
            label: <AddressLabelComponent {...{ id: elementId, text: addressFormatItem.label }} />,
            alert: <AddressAlertComponent {...{ message: errorMessage }} />,
            input
        };
    });
};

export const isDropDownDisable = (
    isCheckoutBillingAddress: boolean | undefined,
    currentOperation: AddressOperation,
    elementId: string
): boolean => {
    let isDisable: boolean = false;
    if (currentOperation === AddressOperation.BillingList && elementId === 'shipping_addressaddresstypevalue') {
        isDisable = true;
    }
    if (isCheckoutBillingAddress && elementId === 'billing_addressaddresstypevalue') {
        isDisable = true;
    }
    return isDisable;
};

export const AddressAddUpdate = (props: IAddressAddInputProps): IAddressAddUpdateProps => {
    const {
        editAddressHeading,
        addAddressHeading,
        selectedAddress = {},
        resources,
        hasError,
        onCancel,
        onSave,
        hasExternalSubmitGroup,
        isUpdating,
        addressActionResponse,
        telemetryContent
    } = props;
    const heading = selectedAddress.RecordId ? editAddressHeading : addAddressHeading;

    return {
        AddressForm: { className: 'msc-address-form' },
        heading: heading && <Heading className='msc-address-form__heading' {...heading} />,
        items: getAddessItems(selectedAddress, props),
        isShowSaveButton: !hasExternalSubmitGroup,
        saveButton: onSave && (
            <AddressButtonComponent
                {...{
                    className: classname('msc-address-form__button-save msc-btn', { 'msc-address-form__button-updating': isUpdating }),
                    text: resources.addressSaveButtonText,
                    ariaLabel: resources.addressSaveButtonAriaLabel,
                    disabled: isUpdating,
                    onClick: onSave,
                    telemetryContent
                }}
            />
        ),
        isShowCancelButton: !hasExternalSubmitGroup,
        cancelButton: onCancel && (
            <AddressButtonComponent
                {...{
                    className: 'msc-address-form__button-cancel msc-btn',
                    text: resources.addressCancelButtonText,
                    ariaLabel: resources.addressCancelButtonAriaLabel,
                    onClick: onCancel,
                    telemetryContent
                }}
            />
        ),
        hasError: hasError || false,
        error: {
            AddressError: { className: 'msc-address-form__error' },
            title: addressActionResponse && addressActionResponse.errorTitle && (
                <AddressErrorTitleComponent {...{ title: addressActionResponse.errorTitle || '' }} />
            ),
            message: addressActionResponse && addressActionResponse.errorMessage && (
                <AddressErrorMessageComponent {...{ message: addressActionResponse.errorMessage || '' }} />
            )
        }
    };
};
