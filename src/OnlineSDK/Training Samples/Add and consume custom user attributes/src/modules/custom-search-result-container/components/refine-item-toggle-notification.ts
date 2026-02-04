/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { IProductRefinerHierarchy } from '@msdyn365-commerce/commerce-entities';
import { ProductRefinerValue } from '@msdyn365-commerce/retail-proxy';

/**
 * When a refine item is toggled (selected/deselected for discrete items, or min/max changed for slider items)
 * this a notification is raised of this type.
 */
export interface IRefineItemToggleNotification {
    /**
     * Refiner that is the parent of this refiner value item.
     */
    parentProductRefinerHierarchy: IProductRefinerHierarchy;

    /**
     * Refiner value item.
     */
    productRefinerValue: ProductRefinerValue;

    /**
     * For discrete items, indicates whether the item is being selected (true) or deselected (false).
     */
    isSelecting: boolean;

    /**
     * For selectable ranges, this is the selected start of the range.
     */
    rangeStart?: number;

    /**
     * For selectable ranges, this is the selected end of the range.
     */
    rangeEnd?: number;
}
