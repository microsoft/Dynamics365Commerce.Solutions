/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import * as Msdyn365 from '@msdyn365-commerce/core';
import { Address, AddressPurpose } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import { Heading as HeadingData } from '@msdyn365-commerce-modules/data-types';
import { Heading, INodeProps, ITelemetryContent } from '@msdyn365-commerce-modules/utilities';
import classname from 'classnames';
import * as React from 'react';

import { AddressFormat } from '../address-format';
import { AddressOperation, IAddressResource, IAddressResponse } from '../address-module.data';
import AddressButtonComponent from './address-button';
import AddressErrorMessageComponent from './address-error-message';
import AddressErrorTitleComponent from './address-error-title';
import { AddressShow, IAddressShowProps } from './address-show';

export interface IAddressListError {
    Error: INodeProps;
    isShowError: boolean;
    title: React.ReactNode;
    message: React.ReactNode;
}

export interface IAddressListItem {
    Item: INodeProps;
    key: number;
    error: IAddressListError;
    showItems: IAddressShowProps;
    isShowPrimaryButton: boolean;
    primaryButton: React.ReactNode;
    editButton: React.ReactNode;
    removeButton: React.ReactNode;
}

export interface IAddressList {
    List: INodeProps;
    isShowList: boolean;
    heading: React.ReactNode;
    items: IAddressListItem[];
}

export interface IAddressListProps {
    ListAddress: INodeProps;
    heading: React.ReactNode;
    isShowEmptyListMessage: boolean;
    emptyListMessage: React.ReactNode;
    addButton: React.ReactNode;
    primaryAddressList?: IAddressList;
    otherAddressList?: IAddressList;
    billingAddressList?: IAddressList;
}

export interface IAddressListInputProps {
    isUpdating?: boolean;
    selectedAddress?: Address;
    addressFormat: AddressFormat;
    addresses: Address[];
    addressPurposes: AddressPurpose[];
    heading?: HeadingData;
    currentOperation: AddressOperation;
    primaryAddressSectionHeading?: HeadingData;
    otherAddressSectionHeading?: HeadingData;
    billingAddressSectionHeading?: HeadingData;
    addressActionResponse?: IAddressResponse;
    resources: IAddressResource;
    contextRequest: Msdyn365.IRequestContext;
    telemetryContent?: ITelemetryContent;
    onAddAddress(): void;
    onEditAddress(address?: Address): void;
    onRemoveAddress(address?: Address): void;
    onUpdatePrimaryAddress(address: Address): void;
}

const getButtonAriaLabel = (ariaLabel: string, addressName?: string): string => {
    return ariaLabel.replace('{addressName}', addressName || '');
};

const getAddressList = (
    className: string,
    addresses: Address[],
    showPrimaryButton: boolean,
    props: IAddressListInputProps,
    heading?: HeadingData
): IAddressList => {
    const {
        resources,
        isUpdating,
        addressActionResponse,
        selectedAddress,
        addressFormat,
        onEditAddress,
        onRemoveAddress,
        onUpdatePrimaryAddress,
        addressPurposes,
        telemetryContent
    } = props;
    const { errorTitle = null, errorMessage = null } = addressActionResponse || {};

    return {
        List: { className },
        isShowList: addresses.length > 0,
        heading: heading && <Heading className={`${className}-heading`} {...heading} />,
        items: addresses.map(
            (address: Address): IAddressListItem => {
                const isSelectedAddress = selectedAddress && selectedAddress.RecordId === address.RecordId;
                const isShowError = isSelectedAddress && errorTitle && errorMessage;
                return {
                    Item: { className: `${className}-list` },
                    key: address.RecordId || 0,
                    showItems: AddressShow({
                        addressFormat: addressFormat.getAddressFormat(address.ThreeLetterISORegionName || ''),
                        address,
                        addressPurposes
                    }),
                    error: {
                        Error: { className: `${className}-error` },
                        isShowError: !!isShowError,
                        title: <AddressErrorTitleComponent {...{ title: errorTitle || '' }} />,
                        message: <AddressErrorMessageComponent {...{ message: errorMessage || '' }} />
                    },
                    isShowPrimaryButton: showPrimaryButton,

                    primaryButton: (
                        <AddressButtonComponent
                            {...{
                                className: classname('msc-address-list__button-primary msc-btn', {
                                    'msc-address-list__button-updating': isUpdating && isSelectedAddress
                                }),
                                disabled: isUpdating,
                                text: resources.addressPrimaryButtonText,
                                ariaLabel: getButtonAriaLabel(resources.addressPrimaryButtonAriaLabel, address.Name),
                                telemetryContent,
                                onClick: () => {
                                    onUpdatePrimaryAddress(address);
                                }
                            }}
                        />
                    ),
                    editButton: (
                        <AddressButtonComponent
                            {...{
                                className: 'msc-address-list__button-edit',
                                disabled: isUpdating,
                                text: resources.addressEditButtonText,
                                ariaLabel: getButtonAriaLabel(resources.addressEditButtonAriaLabel, address.Name),
                                telemetryContent,
                                onClick: () => {
                                    onEditAddress(address);
                                },
                                role: 'link'
                            }}
                        />
                    ),
                    removeButton: (
                        <AddressButtonComponent
                            {...{
                                className: 'msc-address-list__button-remove',
                                disabled: isUpdating,
                                text: resources.addressRemoveButtonText,
                                ariaLabel: getButtonAriaLabel(resources.addressRemoveButtonAriaLabel, address.Name),
                                telemetryContent,
                                onClick: () => {
                                    onRemoveAddress(address);
                                },
                                role: 'link'
                            }}
                        />
                    )
                };
            }
        )
    };
};

const getPrimaryAddressList = (props: IAddressListInputProps): IAddressList => {
    const { addresses, primaryAddressSectionHeading } = props;
    const primaryAddresses = addresses.filter((address: Address) => address.IsPrimary && address.AddressTypeValue !== 1);
    return getAddressList('msc-address-list__primary', primaryAddresses, false, props, primaryAddressSectionHeading);
};

const getOtherAddressList = (props: IAddressListInputProps): IAddressList => {
    const { addresses, otherAddressSectionHeading } = props;
    const otherAddresses = addresses.filter((address: Address) => !address.IsPrimary && address.AddressTypeValue !== 1);
    return getAddressList('msc-address-list__primary', otherAddresses, true, props, otherAddressSectionHeading);
};

const getBillingAddressessList = (props: IAddressListInputProps): IAddressList => {
    const { addresses, billingAddressSectionHeading } = props;
    const billingAddresses = addresses.filter((address: Address) => address.AddressTypeValue === 1);
    return getAddressList('msc-address-list__primary', billingAddresses, false, props, billingAddressSectionHeading);
};

export const AddressList = (props: IAddressListInputProps): IAddressListProps => {
    const { addresses, heading, resources, onAddAddress, telemetryContent, currentOperation } = props;
    const handleLineItemHeadingChange = (event: Msdyn365.ContentEditableEvent) => {
        heading!.text = event.target.value;
    };
    const headingComponent = heading && heading.text && (
        <Msdyn365.Text
            className='msc-address-list__heading'
            tag={heading.headingTag || 'h2'}
            text={heading.text}
            editProps={{ onEdit: handleLineItemHeadingChange, requestContext: props.contextRequest }}
        />
    );

    return {
        ListAddress: { className: 'ms-address-list' },
        heading: headingComponent,
        isShowEmptyListMessage: addresses.length === 0,
        emptyListMessage: <p className='msc-address-list__add-empty'>{resources.addressEmptyListAddressMessage}</p>,
        addButton: (
            <AddressButtonComponent
                {...{
                    className: 'msc-address-list__button-add msc-btn',
                    text: resources.addressAddButtonText,
                    ariaLabel: resources.addressAddButtonAriaLabel,
                    telemetryContent,
                    onClick: onAddAddress
                }}
            />
        ),
        primaryAddressList: currentOperation === AddressOperation.List ? getPrimaryAddressList(props) : undefined,
        otherAddressList: currentOperation === AddressOperation.List ? getOtherAddressList(props) : undefined,
        billingAddressList: currentOperation === AddressOperation.BillingList ? getBillingAddressessList(props) : undefined
    };
};
