/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import {
    Address,
    AddressPurpose,
    CountryRegionInfo,
    StateProvinceInfo
} from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import { get, set } from 'mobx';

import { AddressItemType, IAddressDropdownsData, IAddressItem, IAddressValidationRule } from './address-format.data';
import { AddressMetaData } from './address-meta-data';

/**
 *
 * Retail Service Address format parser.
 */
export class AddressFormat {
    private readonly countryRegionsInfo?: CountryRegionInfo[];

    private readonly addressPurposes?: AddressPurpose[];

    private readonly addressMetaData: AddressMetaData;

    constructor(countryRegionsInfo: CountryRegionInfo[], addressMetaData: AddressMetaData, addressPurposes: AddressPurpose[]) {
        this.countryRegionsInfo = countryRegionsInfo;
        this.addressMetaData = addressMetaData;
        this.addressPurposes = addressPurposes;
    }

    public getAddressFormat(countryRegionId: string): IAddressItem[] {
        const countryRegionInfo = this._getCountryRegionInfo(countryRegionId);
        if (countryRegionInfo) {
            return this._getAddressDisplayFormat(countryRegionInfo);
        }
        return [];
    }

    public getCountryFormat(): { key?: string; value?: string }[] {
        return (this.countryRegionsInfo || []).map(countryRegion => {
            return {
                key: countryRegion.CountryRegionId,
                value: countryRegion.ShortName
            };
        });
    }

    public getStateFormat(stateProvinceInfo?: StateProvinceInfo[]): { key?: string; value?: string }[] {
        return (stateProvinceInfo || []).map(state => {
            return {
                key: state.StateId,
                value: state.StateName
            };
        });
    }

    public getAddressTypeFormat(): { key?: number; value?: string }[] {
        return (this.addressPurposes || []).map(addressPurpose => {
            return {
                key: addressPurpose.AddressType,
                value: addressPurpose.Name
            };
        });
    }

    public getPrefilledAddressDropdownData = (
        stateDefaultSelectionText: string,
        stateProvinceInfo?: StateProvinceInfo[]
    ): IAddressDropdownsData => {
        const defaultStateText = {
            key: '',
            value: stateDefaultSelectionText
        };
        const dropdownData: IAddressDropdownsData = {};

        dropdownData[AddressItemType[AddressItemType.ThreeLetterISORegionName]] = this.getCountryFormat();
        dropdownData[AddressItemType[AddressItemType.State]] = this.getStateFormat(stateProvinceInfo);
        dropdownData[AddressItemType[AddressItemType.State]].unshift(defaultStateText);
        dropdownData[AddressItemType[AddressItemType.AddressTypeValue]] = this.getAddressTypeFormat();

        return dropdownData;
    };

    public getTwoLetterISORegionName = (countryRegionId: string): string | undefined => {
        const countryRegionInfo = this._getCountryRegionInfo(countryRegionId);

        return countryRegionInfo!.ISOCode;
    };

    public validateAddressFormat = (
        address: Address,
        validationError: Address,
        countryRegionId: string,
        propertyName?: string
    ): boolean => {
        let isValid: boolean = true;
        let validationtor;
        const addressFormat = this.getAddressFormat(address.ThreeLetterISORegionName || countryRegionId);

        addressFormat.forEach(addressFormatItem => {
            if (!propertyName || (propertyName && addressFormatItem.name === propertyName)) {
                validationtor = this._inputValidation(addressFormatItem, validationError, address);
                if (validationtor !== undefined) {
                    isValid = validationtor;
                }
            }
        });

        return isValid;
    };

    public getTranformedAddress = (result: Microsoft.Maps.ISuggestionResult, stateProvinceInfo?: StateProvinceInfo[]): Address => {
        const address: Address = {};

        // Zip Code
        if (result.address.postalCode !== undefined) {
            address.ZipCode = result.address.postalCode;
        } else {
            address.ZipCode = '';
        }

        // State
        if (stateProvinceInfo) {
            const selectedState = stateProvinceInfo.find(state => state.StateName === result.address.adminDistrict);
            if (!selectedState) {
                address.State = '';
                address.StateName = result.address.adminDistrict;
            } else {
                address.State = selectedState.StateId;
                address.StateName = selectedState.StateName;
            }
        }

        // Street
        if (result.address.addressLine !== undefined) {
            address.Street = result.address.addressLine;
        } else {
            address.Street = ' ';
        }

        // City
        if (result.address.locality !== undefined) {
            address.City = result.address.locality;
        } else {
            address.City = '';
        }

        // District
        if (result.address.district !== undefined) {
            address.DistrictName = result.address.district;
            address.CountyName = result.address.district;
        } else {
            address.DistrictName = '';
        }

        // Formatted Address
        address.FullAddress = result.address.formattedAddress;

        return address;
    };

    private _inputValidation(addressFormatItem: IAddressItem, validationError: Address, address: Address): boolean | undefined {
        set(validationError, { [addressFormatItem.name]: null });
        for (const validationRule of addressFormatItem.validationRules || []) {
            if (!this._validateRegEx(address, addressFormatItem.name, validationRule)) {
                set(validationError, { [addressFormatItem.name]: validationRule.message });
                return false;
            }
        }
        return undefined;
    }

    private readonly _validateRegEx = (address: Address, propertyName: string, validationRule: IAddressValidationRule): boolean => {
        if (validationRule.regEx && validationRule.regEx.length > 0) {
            const regex = new RegExp(validationRule.regEx);
            return regex.test(get(address, propertyName) || '');
        }
        return true;
    };

    private _getCountryRegionInfo(countryRegionId: string): CountryRegionInfo | undefined {
        return (this.countryRegionsInfo || []).find(countryRegion => {
            return (countryRegion.CountryRegionId || '').toLowerCase() === countryRegionId.toLowerCase();
        });
    }

    private _getAddressDisplayFormat(countryRegionInfo: CountryRegionInfo): IAddressItem[] {
        const addressDisplayItem: IAddressItem[] = [];

        if (countryRegionInfo && countryRegionInfo.AddressFormatLines) {
            const AddressTypeItem = this._extendAddressDisplayFormat(AddressItemType.AddressTypeValue, true);
            if (AddressTypeItem) {
                addressDisplayItem.push(AddressTypeItem);
            }

            const nameDisplayItem = this._extendAddressDisplayFormat(AddressItemType.Name, true);
            if (nameDisplayItem) {
                addressDisplayItem.push(nameDisplayItem);
            }

            countryRegionInfo.AddressFormatLines.forEach(formatLine => {
                if (formatLine.AddressComponentNameValue) {
                    const addressItem = this.addressMetaData.getItemFormat(formatLine.AddressComponentNameValue);
                    if (addressItem) {
                        addressItem.isNewLine = formatLine.NewLine || false;
                        addressDisplayItem.push(addressItem);
                    }
                }
            });

            const phoneDisplayItem = this._extendAddressDisplayFormat(AddressItemType.Phone, false);
            if (phoneDisplayItem) {
                addressDisplayItem.push(phoneDisplayItem);
            }

            const isPrimaryDisplayItem = this._extendAddressDisplayFormat(AddressItemType.IsPrimary, false);
            if (isPrimaryDisplayItem) {
                addressDisplayItem.push(isPrimaryDisplayItem);
            }
        }

        return addressDisplayItem;
    }

    private _extendAddressDisplayFormat(type: AddressItemType, isNewLine: boolean): IAddressItem | undefined {
        const addressItem = this.addressMetaData.getItemFormat(type);
        if (addressItem) {
            addressItem.isNewLine = isNewLine;
        }
        return addressItem;
    }
}
