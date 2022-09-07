/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import { IProductRefinerHierarchy } from '@msdyn365-commerce/commerce-entities';
import { ICoreContext, ITelemetry } from '@msdyn365-commerce/core';
import { ProductRefinerValue } from '@msdyn365-commerce/retail-proxy';
import { ITelemetryContent } from '@msdyn365-commerce-modules/utilities';

export interface IChoiceSummaryClickNotification {
    itemClicked: HTMLElement;
    choiceClicked: ProductRefinerValue | undefined;
    clearAll: boolean;
    nextItemToFocus: HTMLElement | undefined;
}

export interface IChoiceSummaryProps {
    /**
     * Telemetry from  module.
     */
    telemetry: ITelemetry;

    /**
     * Custom classNames for the component
     */
    classNames: string;
    clearAllText: string;
    label?: string;
    choiceFormat?: string;
    choiceRangeValueFormat: string;
    choiceAriaLabel?: string;
    closeAriaLabel?: string;
    selectedChoices: ProductRefinerValue[];
    refinerHierarchy: IProductRefinerHierarchy[];

    /**
     * The telemetry content
     */
    telemetryContent?: ITelemetryContent;
    context?: ICoreContext;

    /**
     * Function called onClick of choice summary children
     */
    onChoiceClicked?(notification: IChoiceSummaryClickNotification): void;

    /**
     * Function to build URL including choice
     */
    urlBuilder(selectedRefiner: ProductRefinerValue, isClearAll: boolean): string;
}
