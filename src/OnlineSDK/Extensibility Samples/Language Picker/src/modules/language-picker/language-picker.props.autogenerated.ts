/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * ILanguagePicker contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface ILanguagePickerConfig extends Msdyn365.IModuleConfig {
    languages?: ILanguagesData[];
    linkRedirectFlag?: boolean;
    linkBaseUrl?: string;
}

export interface ILanguagePickerResources {
    selectLocaleText: string;
}

export interface ILanguagesData {
    name?: string;
    code: string;
    imageUrl?: string;
    linkUrl?: string;
}

export interface ILanguagePickerProps<T> extends Msdyn365.IModule<T> {
    resources: ILanguagePickerResources;
    config: ILanguagePickerConfig;
}
