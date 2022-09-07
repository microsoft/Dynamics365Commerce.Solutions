/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import * as React from 'react';

export interface IAdressDropdown {
    id: string;
    name: string;
    className: string;
    value: string | number;
    additionalAddributes?: object;
    displayData: { key?: string | number; value?: string }[];
    onChange(event: React.ChangeEvent<HTMLSelectElement>): void;
    disabled: boolean;
}

const getDropdownItem = (key?: string | number, value?: string, selectedValue?: string | number): React.ReactNode => {
    let isSelected: boolean;
    if (typeof key === 'number') {
        isSelected = key === selectedValue;
    } else {
        isSelected = typeof selectedValue === 'string' && (key || '').toLowerCase() === (selectedValue || '').toLowerCase();
    }

    return (
        <option key={key} value={key} aria-selected={isSelected}>
            {value}
        </option>
    );
};

const AdressDropdown: React.FC<IAdressDropdown> = ({
    id,
    name,
    className,
    value,
    additionalAddributes,
    displayData,
    onChange,
    disabled
}) => (
    <select
        id={id}
        className={`${className}__dropdown`}
        name={name}
        value={value}
        onChange={onChange}
        disabled={disabled}
        {...(additionalAddributes || {})}
    >
        {displayData && displayData.map(item => getDropdownItem(item.key, item.value, value))}
    </select>
);

export default AdressDropdown;
