/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';
import { IAddressSelectItem, IAddressSelectProps } from '../../common/components/address-select';
import { IAddressAddItem, IAddressAddUpdateProps } from '../../common/components/address-add';
import { IAddressShowItem, IAddressShowProps } from '../../common/components/address-show';
import { ICheckoutBillingAddressViewProps } from './custom-checkout-billing-address';

/**
 * Address show component.
 * @param param0 - Root param.
 * @param param0.AddressDetail - Address detail.
 * @param param0.items - IAddressShowItem[].
 * @returns - Address Node.
 */
const AddressShow: React.FC<IAddressShowProps> = ({ AddressDetail, items }) => {
    return (
        <Node {...AddressDetail}>
            {items &&
                items.map((item: IAddressShowItem) => {
                    return <>{item.description}</>;
                })}
        </Node>
    );
};

/**
 * Address show component.
 * @param param0 - Root param.
 * @param param0.SelectAddress - Select Address.
 * @param param0.addButton - addButton.
 * @param param0.items - items.
 * @param param0.isShowSaveButton - isShowSaveButton.
 * @param param0.saveButton - saveButton.
 * @param param0.isShowCancelButton - isShowCancelButton.
 * @param param0.cancelButton - cancelButton.
 * @returns - AddressSelect Node.
 */
const AddressSelect: React.FC<IAddressSelectProps> = ({
    SelectAddress,
    addButton,
    items,
    isShowSaveButton,
    saveButton,
    isShowCancelButton,
    cancelButton
}) => {
    return (
        <Node {...SelectAddress}>
            {addButton}
            {items &&
                items.map((item: IAddressSelectItem) => {
                    const SelectItem = item.SelectItem;
                    return (
                        <Node {...SelectItem} key={item.key}>
                            {item.input}
                            <AddressShow {...item.showItems} />
                        </Node>
                    );
                })}
            {isShowSaveButton && saveButton}
            {isShowCancelButton && cancelButton}
        </Node>
    );
};

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
            {items &&
                items.map((item: IAddressAddItem) => {
                    const { AddressItem, key, label, alert, input } = item;
                    return (
                        <Node {...AddressItem} key={key}>
                            {label}
                            {alert}
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
 * Checkout Billing Address View Component.
 * @param props - Props.
 * @returns - CheckoutBillingAddress Module.
 */
const CheckoutBillingAddressView: React.FC<ICheckoutBillingAddressViewProps> = props => {
    const {
        CheckoutBillingAddress,
        showAddressSelect,
        viewState,
        heading,
        sameAsShippingCheckbox,
        showAddress,
        showAddOrUpdateAddress
    } = props;

    return (
        <Module {...CheckoutBillingAddress}>
            {heading}
            {viewState.isShowSameAsShippingCheckbox && sameAsShippingCheckbox}
            {viewState.isShowAddress && <AddressShow {...showAddress} />}
            {viewState.isShowAddresList && <AddressSelect {...showAddressSelect} />}
            {viewState.isShowAddOrUpdateAddress && <AddressAddUpdate {...showAddOrUpdateAddress} />}
        </Module>
    );
};

export default CheckoutBillingAddressView;
