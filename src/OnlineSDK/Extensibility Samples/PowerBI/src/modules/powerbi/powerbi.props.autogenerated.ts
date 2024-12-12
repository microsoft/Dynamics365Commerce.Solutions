/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IPowerbi contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export const enum reportView {
    single = 'single',
    list = 'list'
}

export interface IPowerbiConfig extends Msdyn365.IModuleConfig {
    enableAuthFlow?: boolean;
    reportHeading?: IReportHeadingData;
    scope?: string;
    clientId?: string;
    workspaceId?: string;
    reportId?: string;
    reportView?: reportView;
    role?: string;
}

export interface IPowerbiResources {
    resourceKey: string;
}

export const enum ReportHeadingTag {
    h1 = 'h1',
    h2 = 'h2',
    h3 = 'h3',
    h4 = 'h4',
    h5 = 'h5',
    h6 = 'h6'
}

export interface IReportHeadingData {
    text: string;
    tag?: ReportHeadingTag;
}

export interface IPowerbiProps<T> extends Msdyn365.IModule<T> {
    resources: IPowerbiResources;
    config: IPowerbiConfig;
}
