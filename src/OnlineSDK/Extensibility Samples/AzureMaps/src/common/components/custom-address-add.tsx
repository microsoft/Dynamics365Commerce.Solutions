/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */

import { Address, ChannelConfiguration } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
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
    IAddressValidationRule,
    AddressType,
    IAddressResource,
    IAddressResponse,
    AddressLabelFunctionComponent,
    AddressErrorTitleFunctionComponent,
    AddressErrorMessageFunctionComponent,
    AddressDropdownFunctionComponent,
    AddressAlertFunctionComponent,
    AddressButtonFunctionComponent
} from '@msdyn365-commerce-modules/address';
import AddressCheckboxFunctionComponent from './custom-address-checkbox';
import AddressInputFunctionComponent from './custom-address-input';

interface ISearchResultAddress {
    streetNumber?: string;
    streetName?: string;
    municipality?: string;
    countrySecondarySubdivision?: string;
    countrySubdivision?: string;
    postalCode?: string;
    extendedPostalCode?: string;
    countryCode?: string;
    country?: string;
    countryCodeISO3?: string;
    freeformAddress?: string;
    countrySubdivisionName?: string;
    localName?: string;
    countrySubdivisionCode?: string;
}

interface ISuggestion {
    formattedSuggestion: string;
    title: string;
    subtitle: string;
    address: ISearchResultAddress;
}
export interface IAddressAddInputProps {
    isUpdating?: boolean;
    hasError?: boolean;
    hasExternalSubmitGroup?: boolean;
    addressType: AddressType;
    addressFormat: IAddressItem[];
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
    shouldAutoFocus?: boolean;
    channel?: ChannelConfiguration;
    locale?: string;
    countryRegionId?: string;

    onInputChange(event: React.ChangeEvent<HTMLInputElement>): void;
    onDropdownChange(event: React.ChangeEvent<HTMLSelectElement>): void;
    onSave?(): void;
    onCancel?(): void;
    onAzureMapsSuggestionSelected?(result: ISuggestion): Promise<void>;
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
    channel?: ChannelConfiguration;
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
        addressType,
        dropdownDisplayData,
        defaultCountryRegionId,
        defaultAddressType,
        validationError = {},
        onInputChange,
        onDropdownChange,
        shouldAutoFocus,
        onAzureMapsSuggestionSelected,
        channel,
        locale,
        countryRegionId
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
                <AddressInputFunctionComponent
                    {...{
                        id: elementId,
                        name: addressFormatItem.name,
                        className: 'msc-address-form',
                        type: 'text',
                        autoFocus: shouldAutoFocus ?? index === 0,
                        value: selectedAddress[addressFormatItem.name],
                        maxLength: addressFormatItem.maxLength,
                        onChange: onInputChange,
                        additionalAddributes: getRequriedAttribute(addressFormatItem.validationRules),
                        onAzureMapsSuggestionSelected: onAzureMapsSuggestionSelected,
                        channel,
                        locale,
                        countryRegionId
                    }}
                />
            );
        } else if (addressFormatItem.displayType === AddressItemDisplayType.Checkbox) {
            input = (
                <AddressCheckboxFunctionComponent
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
                <AddressDropdownFunctionComponent
                    {...{
                        id: elementId,
                        name: addressFormatItem.name,
                        className: 'msc-address-form',
                        value: selectedValue,
                        displayData,
                        onChange: onDropdownChange,
                        additionalAddributes: getRequriedAttribute(addressFormatItem.validationRules)
                    }}
                />
            );
        }

        return {
            key: addressFormatItem.name,
            AddressItem: { className, id: `${elementId}_container` },
            label: <AddressLabelFunctionComponent {...{ id: elementId, text: addressFormatItem.label }} />,
            alert: <AddressAlertFunctionComponent {...{ message: errorMessage }} />,
            input
        };
    });
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
        telemetryContent,
        channel
    } = props;
    const heading = selectedAddress.RecordId ? editAddressHeading : addAddressHeading;

    return {
        AddressForm: { className: 'msc-address-form' },
        heading: heading && <Heading className='msc-address-form__heading' {...heading} />,
        items: getAddessItems(selectedAddress, props),
        isShowSaveButton: !hasExternalSubmitGroup,
        saveButton: onSave && (
            <AddressButtonFunctionComponent
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
            <AddressButtonFunctionComponent
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
                <AddressErrorTitleFunctionComponent {...{ title: addressActionResponse.errorTitle || '' }} />
            ),
            message: addressActionResponse && addressActionResponse.errorMessage && (
                <AddressErrorMessageFunctionComponent {...{ message: addressActionResponse.errorMessage || '' }} />
            )
        },
        channel
    };
};
