/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import {
    AddressItemDisplayType,
    AddressItemType,
    AddressValidationRuleType,
    IAddressItem,
    IAddressValidationRule
} from './address-format.data';
import { IAddressResource } from './address-module.data';

/**
 * Address meta data.
 */
export class AddressMetaData {
    private readonly metaData: IAddressItem[] = [];

    private readonly resources: IAddressResource;

    private readonly requiredFieldRegEx: string = '\\S';

    private readonly resourcesPrefix: string = 'address';

    private readonly maxLength: number = 64;

    private readonly requiredFields: AddressItemType[];

    private readonly excludedTypes: AddressItemType[];

    constructor(resources: IAddressResource, excluded?: AddressItemType[], required?: AddressItemType[]) {
        this.resources = resources || {};
        this.excludedTypes = excluded || [];
        this.requiredFields =
            required !== undefined
                ? required
                : [
                      AddressItemType.Name,
                      AddressItemType.ZipCode,
                      AddressItemType.City,
                      AddressItemType.State,
                      AddressItemType.ThreeLetterISORegionName,
                      AddressItemType.Street
                  ];
        this._init();
    }

    public getItemFormat(id: number): IAddressItem | undefined {
        return this.metaData.find(item => {
            return item.type === id;
        });
    }

    private _init(): void {
        this._addItem(AddressItemType.Name, AddressItemDisplayType.Input);
        this._addItem(AddressItemType.Phone, AddressItemDisplayType.Input);
        this._addItem(AddressItemType.ZipCode, AddressItemDisplayType.Input);
        this._addItem(AddressItemType.City, AddressItemDisplayType.Input);
        this._addItem(AddressItemType.County, AddressItemDisplayType.Input);
        this._addItem(AddressItemType.State, AddressItemDisplayType.Dropdown);
        this._addItem(AddressItemType.ThreeLetterISORegionName, AddressItemDisplayType.Dropdown);
        this._addItem(AddressItemType.Street, AddressItemDisplayType.Input);
        this._addItem(AddressItemType.DistrictName, AddressItemDisplayType.Input);
        this._addItem(AddressItemType.StreetNumber, AddressItemDisplayType.Input);
        this._addItem(AddressItemType.BuildingCompliment, AddressItemDisplayType.Input);
        this._addItem(AddressItemType.Postbox, AddressItemDisplayType.Input);
        this._addItem(AddressItemType.House_RU, AddressItemDisplayType.Input);
        this._addItem(AddressItemType.Flat_RU, AddressItemDisplayType.Input);
        this._addItem(AddressItemType.CountryOKSMCode_RU, AddressItemDisplayType.Input);
        this._addItem(AddressItemType.AddressTypeValue, AddressItemDisplayType.Dropdown);
        this._addItem(AddressItemType.IsPrimary, AddressItemDisplayType.Checkbox);
    }

    private _addItem(type: AddressItemType, displayType: AddressItemDisplayType): void {
        if (this.excludedTypes.includes(type)) {
            return;
        }

        const nameKey = AddressItemType[type].replace('_', '');

        // Find out if currentItem is required field
        const validationRules = this._validationRules(type, nameKey);
        let isRequired = false;
        validationRules.forEach(rule => {
            if (rule.type === AddressValidationRuleType.Required) {
                isRequired = true;
            }
        });

        // If no optional string is provided in resource file, use the original label text
        const optionalString =
            this.resources[`${this.resourcesPrefix}${nameKey}LabelOptional`] || this.resources[`${this.resourcesPrefix}${nameKey}Label`];
        const item: IAddressItem = {
            type,
            displayType,
            name: AddressItemType[type],
            label: isRequired ? this.resources[`${this.resourcesPrefix}${nameKey}Label`] : optionalString,
            maxLength: this.maxLength,
            validationRules,
            isNewLine: true
        };

        this.metaData.push(item);
    }

    private _validationRules(type: AddressItemType, name: string): IAddressValidationRule[] {
        const validationRules: IAddressValidationRule[] = [];

        for (const ruleType of Object.keys(AddressValidationRuleType)) {
            const key = `${this.resourcesPrefix}${name}${ruleType}`;
            const message = this.resources[`${key}ErrorMessage`];
            switch (ruleType) {
                case AddressValidationRuleType.Required: {
                    if (this.requiredFields.find((itemType: AddressItemType) => itemType === type)) {
                        validationRules.push(this._validationRule(ruleType, this.requiredFieldRegEx, message));
                    }
                    break;
                }
                default:
            }
        }
        return validationRules;
    }

    private _validationRule(type: AddressValidationRuleType, regEx: string, message: string): IAddressValidationRule {
        return {
            type,
            regEx,
            message
        };
    }
}
