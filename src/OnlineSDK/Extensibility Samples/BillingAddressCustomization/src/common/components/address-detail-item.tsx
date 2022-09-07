/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import classname from 'classnames';
import * as React from 'react';

export interface IAddressDetailItem {
    isNewLine: boolean;
    isShowLabel: boolean;
    labelText: string;
    name: string;
    value: string;
}

const AddressDetailItem: React.FC<IAddressDetailItem> = ({ isNewLine, isShowLabel, labelText, name, value }) => {
    const className = `msc-address-detail__item msc-address-detail__item-${name.toLowerCase()}`;
    const isEmpty = !(value && value.length > 0);
    const mainClass = isShowLabel
        ? `msc-address-detail__item msc-address-detail__item-address-detail_${labelText}`
        : classname(className, { 'msc-address-detail__item-empty': isEmpty, 'msc-address-detail__item-newline': isNewLine });
    return (
        <span className={classname(mainClass, { 'msc-address-detail__main-item-empty': isEmpty })}>
            {isShowLabel && (
                <>
                    <span className={`${className}-label`}>{labelText}</span>
                    <span
                        className={classname(className, {
                            'msc-address-detail__item-empty': isEmpty,
                            'msc-address-detail__item-newline': isNewLine
                        })}
                        aria-hidden
                    />
                </>
            )}
            {value}
        </span>
    );
};

export default AddressDetailItem;
