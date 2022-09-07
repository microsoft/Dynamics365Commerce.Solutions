/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { Address, AddressPurpose } from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import { INodeProps } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { AddressItemType, IAddressItem } from '../address-format.data';
import AddressDetailItemComponent from './address-detail-item';

export interface IAddressShowItem {
    key: string;
    description: React.ReactNode;
}

export interface IAddressShowProps {
    AddressDetail: INodeProps;
    items: IAddressShowItem[];
}

export interface IAddressShowInputProps {
    addressFormat: IAddressItem[];
    address: Address;
    addressPurposes: AddressPurpose[];
}

export const AddressShow = (props: IAddressShowInputProps): IAddressShowProps => {
    const { addressFormat, addressPurposes, address } = props;

    return {
        AddressDetail: { className: 'msc-address-detail' },
        items: addressFormat.map(item => {
            if (item.name === AddressItemType[AddressItemType.AddressTypeValue]) {
                const addressType = addressPurposes.find(
                    addressTypeValue => addressTypeValue.AddressType.toString() === (address[item.name] || '').toString()
                );
                if (addressType) {
                    const value = addressType.Name || '';
                    return {
                        key: item.name,
                        description: (
                            <AddressDetailItemComponent
                                {...{
                                    isNewLine: item.isNewLine,
                                    isShowLabel: item.type === AddressItemType.Phone,
                                    labelText: item.label,
                                    name: item.name,
                                    value
                                }}
                            />
                        )
                    };
                }
            }
            return {
                key: item.name,
                description: (
                    <AddressDetailItemComponent
                        {...{
                            isNewLine: item.isNewLine,
                            isShowLabel: item.type === AddressItemType.Phone,
                            labelText: item.label,
                            name: item.name,
                            value: (address[item.name] || '').toString()
                        }}
                    />
                )
            };
        })
    };
};
