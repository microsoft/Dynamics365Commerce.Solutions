/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { IAddressList, IAddressListItem, IAddressListProps } from '@msdyn365-commerce-modules/address';
import { IAddressShowItem, IAddressShowProps } from '@msdyn365-commerce-modules/address';
import { IAddressAddItem, IAddressAddUpdateProps } from '../../common/components/custom-address-add';
import { IAccountManagementAddressViewProps } from './custom-account-management-address';

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
 * Address List Component.
 * @param param0 - Root param.
 * @param param0.List - Address list.
 * @param param0.isShowList - Boolean.
 * @param param0.heading - Address heading.
 * @param param0.items - IAddressListItem[].
 * @returns - AddressListComponent Node.
 */
const AddressList: React.FC<IAddressList> = ({ List, isShowList, heading, items }) => {
    if (!isShowList) {
        return null;
    }

    return (
        <Node {...List}>
            {heading}
            {items.map((item: IAddressListItem) => {
                const { Item, key, error, showItems, isShowPrimaryButton, primaryButton, editButton, removeButton } = item;
                const { Error, isShowError, title, message } = error;
                return (
                    <Node {...Item} key={key}>
                        <AddressShow {...showItems} />
                        {isShowError && (
                            <Node {...Error}>
                                {title}
                                {message}
                            </Node>
                        )}
                        {isShowPrimaryButton && primaryButton}
                        {editButton}
                        {removeButton}
                    </Node>
                );
            })}
        </Node>
    );
};

/**
 * Address Lists Component.
 * @param param0 - Root param.
 * @param param0.ListAddress - List address.
 * @param param0.heading - List heading.
 * @param param0.isShowEmptyListMessage - Boolean.
 * @param param0.emptyListMessage - Empty list message.
 * @param param0.addButton - Add button.
 * @param param0.primaryAddressList - Primary address.
 * @param param0.otherAddressList - Other address.
 * @returns Address Lists Component Node.
 */
const AddressLists: React.FC<IAddressListProps> = ({
    ListAddress,
    heading,
    isShowEmptyListMessage,
    emptyListMessage,
    addButton,
    primaryAddressList,
    otherAddressList
}) => {
    return (
        <Node {...ListAddress}>
            {heading}
            {isShowEmptyListMessage && emptyListMessage}
            {addButton}
            <AddressList {...primaryAddressList} />
            <AddressList {...otherAddressList} />
        </Node>
    );
};

/**
 * AddressAddUpdateComponent.
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
 * Account Management Address View Component.
 * @param props - Props.
 * @returns - AccountAddressManagement Module.
 */
const AccountManagementAddressView: React.FC<IAccountManagementAddressViewProps> = props => {
    const {
        AccountAddressManagement,
        infoMessageBar,
        viewState,
        showAddressList,
        showAddOrUpdateAddress,
        screenReaderNotification
    } = props;

    return (
        <Module {...AccountAddressManagement}>
            {infoMessageBar}
            {viewState.isShowAddresList && <AddressLists {...showAddressList} />}
            {viewState.isShowAddOrUpdateAddress && <AddressAddUpdate {...showAddOrUpdateAddress} />}
            {screenReaderNotification}
        </Module>
    );
};

export default AccountManagementAddressView;
