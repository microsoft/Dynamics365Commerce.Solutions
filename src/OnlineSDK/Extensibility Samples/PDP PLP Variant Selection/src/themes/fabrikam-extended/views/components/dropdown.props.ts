/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

export interface IDropdownProps {
    /**
     * @friendlyName dropdownId
     * @description Id for dropdown component
     */
    dropdownId: string;

    /**
     * @friendlyName dropdownName
     * @description Name of the dropdown component
     */
    dropdownName: string;

    /**
     * @friendlyName dropdownList
     * @description List of items in dropdown
     */
    dropdownList: IDropdownItemProps[];

    /**
     * @friendlyName dropdownToggleName
     * @description Name to use for the toggle when nothing is selected
     */
    dropdownToggleName: string;

    /**
     * @friendlyName onChange
     * @description Callback that gets fired when when a selected dimension changes
     */
    onChange?(notification: IDropdownOnSelectionChangeNotification): Promise<void>;
}

export interface IDropdownItemProps {
    value: string;
    id: string;
}

export interface IDropdownOnSelectionChangeNotification {
    dropdownId: string;
    selectId: string;
    selectedValue: string;
}
