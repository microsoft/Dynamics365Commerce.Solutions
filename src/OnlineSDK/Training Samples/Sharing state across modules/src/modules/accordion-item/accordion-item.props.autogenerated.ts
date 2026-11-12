/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IAccordionItem moduleDefinition Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';
import * as React from 'react';

export interface IAccordionItemConfig {
    accordionItemTitle: string;
    isExpandedOnInitialLoad?: boolean;
}

export interface IAccordionItemProps<T> extends Msdyn365.IModule<T> {
    config: IAccordionItemConfig;
    slots: {
        accordionItemContent: React.ReactNode[];
    };
}
