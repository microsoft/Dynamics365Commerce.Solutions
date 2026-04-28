/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/**
 * Interface for Suggestion object inside store-selector.
 */
export interface ISuggestion {
    formattedSuggestion: string;
    title: string;
    subtitle: string;
    /** The location north or south of the equator from +90 to -90 */
    latitude?: number; // optional for Bing Maps
    /** The location east or west of the prime meridian +180 to -180 */
    longitude?: number; // optional for Bing Maps
}

/**
 * Interface for different map-clients in store-selector.
 */
export interface IMapClient {
    initialize(isMapAPILoaded: boolean): void;
    getSuggestions(query: string): Promise<ISuggestion[]>;
}
