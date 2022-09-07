/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { IActionInput, ICoreContext, IObservableAction, ITelemetry } from '@msdyn365-commerce/core';
import {
    Address,
    AddressPurpose,
    CountryRegionInfo,
    StateProvinceInfo
} from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';
import {
    addAddress,
    AddressManagementInput,
    ArrayExtensions,
    updateAddress,
    updatePrimaryAddress
} from '@msdyn365-commerce-modules/retail-actions';
import { getAddressPurposesAction, GetAddressPurposesInput } from '../actions/get-address-purposes';
import { getStateProvinceAction, GetStateProvincesInput } from '../actions/get-state-provinces';
import { IAddressResource, IAddressResponse } from './address-module.data';

/**
 *
 * Address common.
 */
export class AddressCommon {
    private readonly context: ICoreContext;

    private readonly resources: IAddressResource;

    private readonly telemetry: ITelemetry;

    constructor(context: ICoreContext, resources: IAddressResource, telemetry: ITelemetry) {
        this.context = context;
        this.resources = resources;
        this.telemetry = telemetry;
    }

    public getDefaultCountryRegionId = (countryRegionId: string, countryRegions: CountryRegionInfo[], market?: string): string => {
        const marketISOCode = market || 'US';
        const currentCountryRegion = countryRegions.find(countryRegion => (countryRegion.ISOCode || '') === marketISOCode);
        return (currentCountryRegion && currentCountryRegion.CountryRegionId) || countryRegionId;
    };

    public parseRetailException = (resources: IAddressResource): IAddressResponse => {
        return {
            errorTitle: resources.addressErrorMessageTitle,
            errorMessage: resources.addressGenericErrorMessage
        };
    };

    public isAuthenticatedFlow = (): boolean => {
        return this.context.request.user.isAuthenticated;
    };

    public getStateProvinces = async (countryRegionId: string): Promise<StateProvinceInfo[]> => {
        let response: StateProvinceInfo[] = [];
        if (this.context && this.context.actionContext) {
            try {
                const input = new GetStateProvincesInput(countryRegionId, this.context.request.apiSettings);
                response = await getStateProvinceAction(input, this.context.actionContext);
            } catch (error) {
                if (this.telemetry) {
                    this.telemetry.error(`Error encountered ${error}`);
                    this.telemetry.debug('Unable to get state provinces');
                }
            }
        }
        return Promise.resolve(response);
    };

    // For any address, check for common requried field else treat it as invalid\empty address.
    public isEmpty = (address: Address): boolean => {
        if (address && address.ThreeLetterISORegionName && (address.State || address.City)) {
            return false;
        }

        return true;
    };

    public addCustomerAddress = async (address: Address): Promise<IAddressResponse> => {
        await this.updateLogisticsLocationRoleRecordId(address);
        return this.submitCustomerAddress(addAddress, address);
    };

    public updateCustomerAddress = async (address: Address): Promise<IAddressResponse> => {
        await this.updateLogisticsLocationRoleRecordId(address);
        return this.submitCustomerAddress(updateAddress, address);
    };

    public updateCustomerPrimaryAddress = async (address: Address): Promise<IAddressResponse> => {
        return this.submitCustomerAddress(updatePrimaryAddress, address);
    };

    private readonly submitCustomerAddress = async (
        addressAction: IObservableAction<Address[]>,
        address: Address
    ): Promise<IAddressResponse> => {
        address.AddressTypeValue = address.AddressTypeValue || 7; // 7 sets it to HOME by default

        const input = new AddressManagementInput(address);
        return this.execAddressAction(addressAction, input, address);
    };

    private readonly execAddressAction = async (
        addressAction: IObservableAction<Address[]>,
        input: IActionInput | IActionInput[],
        address: Address
    ): Promise<IAddressResponse> => {
        let response: IAddressResponse = {};

        if (this.context && this.context.actionContext) {
            try {
                const addresses = await addressAction(input, this.context.actionContext);
                if (addresses.length > 0) {
                    response.address = address.RecordId ? address : addresses[addresses.length - 1];
                } else {
                    response.address = address;
                }
                response.customerAddresses = addresses;
            } catch (error) {
                if (this.telemetry) {
                    this.telemetry.error(`Error encountered ${error}`);
                    this.telemetry.debug('Unable to exec address action');
                }
                response = this.parseRetailException(this.resources);
            }
        }
        return Promise.resolve(response);
    };

    /**
     * Function to get the address purposes and update the LogisticsLocationRoleRecordId to update the address type.
     * @param address - Address object.
     * @returns Returns void.
     */
    private readonly updateLogisticsLocationRoleRecordId = async (address: Address): Promise<void> => {
        let addressPurposes: AddressPurpose[] = [];
        try {
            const input: GetAddressPurposesInput = new GetAddressPurposesInput(this.context.request.apiSettings);
            addressPurposes = await getAddressPurposesAction(input, this.context.actionContext);
        } catch (error) {
            // eslint-disable-next-line @typescript-eslint/restrict-template-expressions -- Supparsing the any type for error.
            this.telemetry.error(`Error encountered ${error}`);
            this.telemetry.debug('Unable to get address purposes');
        }
        if (ArrayExtensions.hasElements(addressPurposes)) {
            const addressPurpose: AddressPurpose | undefined = addressPurposes.find(
                purpose => purpose.AddressType === address.AddressTypeValue
            );
            if (addressPurpose) {
                address.LogisticsLocationRoleRecordId = addressPurpose.RecordId;
            }
        }
    };
}
