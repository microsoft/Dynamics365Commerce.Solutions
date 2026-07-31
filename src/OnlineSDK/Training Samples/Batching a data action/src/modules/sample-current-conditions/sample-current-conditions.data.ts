/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { AsyncResult } from '@msdyn365-commerce/retail-proxy';
import { ILocation, IWeatherConditions } from '../../actions/get-current-weather-conditions.action';

export interface ISampleCurrentConditionsData {
    favorite_locations: AsyncResult<ILocation[]>;
    forecast: AsyncResult<IWeatherConditions[]>;
}
