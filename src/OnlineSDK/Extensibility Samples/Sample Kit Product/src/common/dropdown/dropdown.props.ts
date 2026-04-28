/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { ICommerceApiSettings } from '@msdyn365-commerce/core-internal';
import { ITelemetryContent } from '@msdyn365-commerce-modules/utilities';

/**
 * Specifies the type of selection.
 */
export enum SelectionType {
    dropdown = 0,
    swatch = 1
}

/**
 * Copyright (c) 2018 Microsoft Corporation.
 */
export interface IDropdownProps {
    /**
     * Specifies the type of selection. Dropdown by default.
     */
    selectionType?: SelectionType;

    /**
     * @friendlyName dropdownId
     * @description Id for dropdown component.
     */
    dropdownId: string;

    /**
     * @friendlyName dropdownName
     * @description Name of the dropdown component.
     */
    dropdownName: string;

    /**
     * @friendlyName dropdownList
     * @description List of items in dropdown.
     */
    dropdownList: IDropdownItemProps[];

    /**
     * @friendlyName dropdownToggleName
     * @description Name to use for the toggle when nothing is selected.
     */
    dropdownToggleName: string;

    /**
     * @telemetryContent telemetryContent
     * @description Telemetry content.
     */
    telemetryContent?: ITelemetryContent;

    apiSettings?: ICommerceApiSettings;

    /**
     * @friendlyName onChange
     * @description Callback that gets fired when when a selected dimension changes.
     */
    onChange?(notification: IDropdownOnSelectionChangeNotification): Promise<void>;
}

export interface IDropdownItemProps {
    value: string;
    id: string;
    colorHexCode?: string;
    imageUrl?: string;
    isDisabled?: boolean;
    isDefault?: boolean;
}

export interface IDropdownOnSelectionChangeNotification {
    dropdownId: string;
    selectId: string;
    selectedValue: string;
}
