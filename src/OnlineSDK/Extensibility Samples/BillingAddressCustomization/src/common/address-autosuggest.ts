/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { ITelemetry } from '@msdyn365-commerce/core';
import { IStoreSelectorStateManager } from '@msdyn365-commerce-modules/bopis-utilities';

import { IAutoSuggestOptions } from './address-autosuggest.data';

/**
 *
 * Auto Suggest.
 */
export class AutoSuggest {
    public readonly excludedAddressFields: string[] = ['ThreeLetterISORegionName', 'Name', 'AddressTypeValue', 'Phone'];

    private readonly telemetry: ITelemetry;

    private readonly countryCode?: string;

    private readonly bingMapsApiKey?: string;

    private readonly defaultLanguageId?: string;

    private autoSuggestManager: Microsoft.Maps.AutosuggestManager | undefined;

    private autoSuggestOptions: IAutoSuggestOptions;

    constructor(
        telemetry: ITelemetry,
        autoSuggestOptions: IAutoSuggestOptions,
        BingMapsApiKey?: string,
        countryCode?: string,
        defaultLanguageId?: string
    ) {
        this.telemetry = telemetry;
        this.autoSuggestOptions = autoSuggestOptions;
        this.bingMapsApiKey = BingMapsApiKey;
        this.countryCode = countryCode;
        this.defaultLanguageId = defaultLanguageId;
    }

    public attachAutoSuggest = (
        inputId: string,
        containerId: string,
        selectedSuggestionCallback: (result: Microsoft.Maps.ISuggestionResult) => void
    ): void => {
        if (!this.autoSuggestManager) {
            Microsoft.Maps.loadModule('Microsoft.Maps.AutoSuggest', {
                callback: () => {
                    const options = { ...this.autoSuggestOptions, countryCode: this.countryCode };
                    this.autoSuggestManager = new Microsoft.Maps.AutosuggestManager(options);
                    this.autoSuggestManager.attachAutosuggest(inputId, containerId, selectedSuggestionCallback);
                    document.querySelector(inputId)?.setAttribute('role', 'combobox');
                    setTimeout(() => {
                        document.querySelector(inputId)?.setAttribute('type', 'text');
                    }, 0);
                },
                errorCallback: () => {
                    if (this.telemetry) {
                        this.telemetry.debug('Unable to attach map auto suggest.');
                    }
                },
                credentials: this.bingMapsApiKey
            });
        }
    };

    public changeAutoSuggestionCountryCode = (countryCode?: string): void => {
        if (this.autoSuggestManager) {
            this.autoSuggestOptions.countryCode = countryCode;
            this.autoSuggestManager.setOptions(this.autoSuggestOptions);
        }
    };

    public disposeAutoSuggest = (): void => {
        this.autoSuggestManager?.detachAutosuggest();
        this.autoSuggestManager?.dispose();
        this.autoSuggestManager = undefined;
    };

    // eslint-disable-next-line @typescript-eslint/naming-convention
    public _loadMapAPI = async (storeSelectorStateManager: IStoreSelectorStateManager): Promise<void> => {
        await storeSelectorStateManager.loadMapApi({
            key: this.bingMapsApiKey,
            lang: this.defaultLanguageId,
            market: this.countryCode
        });
    };
}
