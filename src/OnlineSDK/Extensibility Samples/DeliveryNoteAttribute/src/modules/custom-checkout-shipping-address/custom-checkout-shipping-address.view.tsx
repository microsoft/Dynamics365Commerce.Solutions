/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { IAddressAddItem, IAddressAddUpdateProps } from '@msdyn365-commerce-modules/address';
import { IAddressSelectItem, IAddressSelectProps } from '@msdyn365-commerce-modules/address';
import { IAddressShowItem, IAddressShowProps } from '@msdyn365-commerce-modules/address';
import { ICheckoutShippingAddressViewProps } from './custom-checkout-shipping-address';

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
            {items.map((item: IAddressShowItem) => {
                return <>{item.description}</>;
            })}
        </Node>
    );
};

/**
 * Address Select Component.
 * @param param0 - Root param.
 * @param param0.SelectAddress - Select address.
 * @param param0.addButton - Add button.
 * @param param0.items - IAddressSelectItem[].
 * @param param0.isShowSaveButton - Boolean.
 * @param param0.saveButton - Save button.
 * @param param0.isShowCancelButton - Boolean.
 * @param param0.cancelButton - Cancel button.
 * @returns - SelectAddress Node.
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
            {items.map((item: IAddressSelectItem) => {
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
 * @param param0.heading - Address heading.
 * @param param0.items - IAddressAddItem[].
 * @param param0.hasError - Boolean.
 * @param param0.error - Error message.
 * @param param0.isShowSaveButton - Boolean.
 * @param param0.saveButton - Save button.
 * @param param0.isShowCancelButton - Boolean.
 * @param param0.cancelButton - Cancel button.
 * @returns Address add update component node.
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
 * Checkout Shipping Address View Component.
 * @param props - Props.
 * @returns - CheckoutShippingAddress Module.
 */
const CheckoutShippingAddressView: React.FC<ICheckoutShippingAddressViewProps> = props => {
    const {
        CheckoutShippingAddress,
        viewState,
        showAddress,
        showAddressSelect,
        showAddOrUpdateAddress,
        cartLineImages,
        deliveryNotes
    } = props;

    return (
        <Module {...CheckoutShippingAddress}>
            {cartLineImages}
            {viewState.isShowAddress && <AddressShow {...showAddress} />}
            {viewState.isShowAddresList && <AddressSelect {...showAddressSelect} />}
            {viewState.isShowAddOrUpdateAddress && <AddressAddUpdate {...showAddOrUpdateAddress} />}
            {deliveryNotes}
        </Module>
    );
};

export default CheckoutShippingAddressView;
