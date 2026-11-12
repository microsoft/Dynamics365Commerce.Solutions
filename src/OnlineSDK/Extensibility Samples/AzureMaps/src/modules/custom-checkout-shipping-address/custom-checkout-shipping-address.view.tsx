/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { INodeProps, Module, Node } from '@msdyn365-commerce-modules/utilities';
import React from 'react';

import { IAddressSelectItem, IAddressSelectProps } from '@msdyn365-commerce-modules/address';
import { IAddressShowItem, IAddressShowProps } from '@msdyn365-commerce-modules/address';
import { IShipMultipleAddressesViewProps } from '@msdyn365-commerce-modules/address';
import { IShipMultipleAddressesLineViewProps } from '@msdyn365-commerce-modules/address';
import { ICheckoutShippingAddressViewProps } from './custom-checkout-shipping-address';
import { IAddressAddItem, IAddressAddUpdateProps } from '../../common/components/custom-address-add';

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
 * @param param0.shipMultipleAddressesButton - Ship to multiple addresses button.
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
    shipMultipleAddressesButton,
    addButton,
    items,
    isShowSaveButton,
    saveButton,
    isShowCancelButton,
    cancelButton
}) => {
    return (
        <Node {...SelectAddress}>
            <Node className='msc-address-select__address-button-container'>
                {addButton}
                {shipMultipleAddressesButton}
            </Node>
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
 * Address Ship Multiple Component.
 * @param param0 - Root param.
 * @param param0.ShipMultipleAddress - Ship multiple address form.
 * @param param0.shipSingleAddressButton - Ship single address button.
 * @param param0.clearSelectionButton - Clear selection button.
 * @param param0.addressDropdown - Address dropdown button.
 * @param param0.ProductsTable - Products table component.
 * @param param0.ProductsTableHeading - Products table heading component.
 * @param param0.ProductsTableRow - Products table row component.
 * @param param0.shipMultipleAddressLines - Ship multiple address lines.
 * @param param0.hasError - Boolean.
 * @param param0.error - Error message.
 * @returns Address ship multiple component node.
 */
const AddressShipMultiple: React.FC<IShipMultipleAddressesViewProps> = ({
    ShipMultipleAddress,
    shipSingleAddressButton,
    clearSelectionButton,
    addressDropdown,
    ProductsTable,
    ProductsTableHeading,
    ProductsTableRow,
    shipMultipleAddressLines,
    isShowSaveButton,
    saveButton,
    isShowCancelButton,
    cancelButton,
    isMobile
}) => {
    if (ShipMultipleAddress && isMobile !== undefined) {
        if (!isMobile) {
            return (
                <Node {...ShipMultipleAddress}>
                    <Node className='msc-address-select__ship-single-button-container'>{shipSingleAddressButton}</Node>
                    <Node className='msc-address-select__heading-button-container'>
                        <Node className='msc-address-select__clear-button-wrapper'>{clearSelectionButton}</Node>

                        <Node className='msc-address-select__dropdown-button-wrapper'>{addressDropdown}</Node>
                    </Node>
                    {ProductsTable && shipMultipleAddressLines && shipMultipleAddressLines.length > 0 && (
                        <Node {...ProductsTable}>
                            {ProductsTableHeading}
                            <tbody>
                                {shipMultipleAddressLines &&
                                    shipMultipleAddressLines.map((line: IShipMultipleAddressesLineViewProps) => {
                                        return _renderShipMultipleAddressLine(line, isMobile, ProductsTableRow);
                                    })}
                            </tbody>
                        </Node>
                    )}
                    {isShowSaveButton && saveButton}
                    {isShowCancelButton && cancelButton}
                </Node>
            );
        } else {
            return (
                <Node {...ShipMultipleAddress}>
                    <Node className='msc-address-select__ship-single-button-container'>{shipSingleAddressButton}</Node>
                    <Node className='msc-address-select__dropdown-button-wrapper'>{addressDropdown}</Node>
                    {ProductsTable && shipMultipleAddressLines && shipMultipleAddressLines.length > 0 && (
                        <Node {...ProductsTable}>
                            {ProductsTableHeading}
                            <tbody>
                                {shipMultipleAddressLines &&
                                    shipMultipleAddressLines.map((line: IShipMultipleAddressesLineViewProps) => {
                                        return _renderShipMultipleAddressLine(line, isMobile, ProductsTableRow);
                                    })}
                            </tbody>
                        </Node>
                    )}
                    {isShowSaveButton && saveButton}
                    {isShowCancelButton && cancelButton}
                </Node>
            );
        }
    } else {
        return null;
    }
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

const _renderShipMultipleAddressLine = (
    shipMultipleAddressesLine: IShipMultipleAddressesLineViewProps,
    isMobile: boolean,
    ProductsTableRow?: INodeProps
): JSX.Element | null => {
    if (!shipMultipleAddressesLine) {
        return null;
    }

    const {
        selectLine,
        productImage,
        productNumber,
        productInfo,
        productPrice,
        productQuantity,
        productAddress
    } = shipMultipleAddressesLine;

    if (!isMobile) {
        return (
            <Node className={`${ProductsTableRow!.className}`} {...ProductsTableRow}>
                <td style={{ width: '10%' }}>{selectLine}</td>
                <td style={{ width: '25%' }}>
                    <div style={{ display: 'inline-block' }}>{productImage}</div>
                    <div style={{ display: 'inline-block', verticalAlign: 'top', marginTop: '25px' }}>{productNumber}</div>
                </td>
                <td style={{ width: '15%' }}>{productInfo}</td>
                <td style={{ width: '12%', textAlign: 'right' }}>{productPrice}</td>
                <td style={{ width: '15%', textAlign: 'center' }}>{productQuantity}</td>
                <td style={{ width: '100%' }}>{productAddress}</td>
            </Node>
        );
    } else {
        return (
            <Node className={`${ProductsTableRow!.className}`} {...ProductsTableRow}>
                <tr>
                    <td style={{ position: 'relative', top: '10px', verticalAlign: 'top' }}>{selectLine}</td>
                    <td style={{ position: 'relative', top: '10px' }}>
                        {productImage}
                        {productInfo}
                        {`QTY: ${productQuantity}`}
                    </td>
                    <td className='msc-address-select__product-price'>{productPrice}</td>
                </tr>
                <tr>
                    <td colSpan={3}>{productAddress}</td>
                </tr>
            </Node>
        );
    }
};

/**
 * Checkout Shipping Address View Component.
 * @param props - Props.
 * @returns - CheckoutShippingAddress Module.
 */
const CheckoutShippingAddressView: React.FC<ICheckoutShippingAddressViewProps> = props => {
    const {
        CheckoutShippingAddress,
        checkoutErrorRef,
        alert,
        viewState,
        showAddress,
        showAddressSelect,
        showAddOrUpdateAddress,
        showShipMultipleAddresses,
        cartLineImages
    } = props;

    return (
        <Module {...CheckoutShippingAddress} ref={checkoutErrorRef}>
            {alert}
            {!viewState.isShowShipMultipleAddress && cartLineImages}
            {viewState.isShowAddress && <AddressShow {...showAddress} />}
            {viewState.isShowAddresList && <AddressSelect {...showAddressSelect} />}
            {viewState.isShowAddOrUpdateAddress && <AddressAddUpdate {...showAddOrUpdateAddress} />}
            {viewState.isShowShipMultipleAddress && (
                <AddressShipMultiple {...(showShipMultipleAddresses as IShipMultipleAddressesViewProps)} />
            )}
        </Module>
    );
};

export default CheckoutShippingAddressView;
