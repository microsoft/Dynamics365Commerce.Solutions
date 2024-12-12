/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { ITelemetry } from '@msdyn365-commerce/core';

/**
 * RefineItem properties that are common for all items.
 */
export interface IRefineItemCommonProps {
    /**
     * Telemetry from RefineMenu module.
     */
    telemetry: ITelemetry;

    /**
     * Locale used for formatting.
     */
    locale: string;

    /**
     * Placeholder text to use for range input max.
     */
    placeholderTextMax: string;

    /**
     * Label text to use for range input max.
     */
    minLabel: string;

    /**
     * Label text to use for range input max.
     */
    maxLabel: string;

    /**
     * Format of friendly string for aria label around input range.
     */
    rangeNameFormat: string;

    /**
     * Error message to display when entered value is not numeric.
     */
    validationErrorNaN: string;

    /**
     * Error message to display when entered value results in invalid input range.
     */
    validationErrorRange: string;
}
