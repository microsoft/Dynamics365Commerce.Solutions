/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface IWeatherConditions {
    name: string;
    id: string;
    dt: number;
    weather: [
        {
            id: string;
            main: string;
            description: string;
            icon: string;
        }
    ];
    main: {
        temp: number;
        temp_min: number;
        temp_max: number;
    };
}

export interface IWeatherConditionsList {
    list: IWeatherConditions[];
    cnt: number;
}

export interface ILocation {
    id: string;
    name: string;
}

/**
 * Input for the getWeather call
 */
export class OpenWeatherApiInput implements Msdyn365.IActionInput {
    public readonly location: ILocation;
    public readonly apiKey: string;
    public readonly units: string;
    private readonly language: string;

    public constructor(apiKey: string, location: ILocation, language?: string, units?: string) {
        this.location = location;
        this.apiKey = apiKey;
        this.language = language || 'en';
        this.units = units === 'c' || units === 'C' ? 'metric' : 'imperial';
    }

    public getCacheObjectType = () => 'open-weather-map-object';
    public getCacheKey = () => `Weather-${this.language}-${this.units}-${this.location.id}`;
    public dataCacheType = (): Msdyn365.CacheType => 'application';
}

async function action(input: OpenWeatherApiInput[], ctx: Msdyn365.IActionContext): Promise<IWeatherConditions[]> {
    if (!input || !input.length || !input[0].apiKey) {
        ctx.trace('Invalid API key, returning empty array');
        return [];
    }

    const first = input[0];
    const ids = input.map(i => i.location.id).join();
    const url = `https://api.openweathermap.org/data/2.5/group?units=${first.units}&lang=en&appid=${first.apiKey}&id=${ids}`;

    await new Promise<void>(resolve => {
        setTimeout(() => {
            resolve();
        }, 1000);
    });

    if (typeof window === 'undefined') {
        const response = await Msdyn365.sendRequest<IWeatherConditionsList>(url);
        return response.data.list;
    } else {
        const response = await fetch(url);
        if (response && response.ok) {
            const data = <IWeatherConditionsList>await response.json();
            return data.list;
        }
    }

    return [];
}

export default Msdyn365.createObservableDataAction({
    action: <Msdyn365.IAction<IWeatherConditions[]>>action,
    id: 'get-current-weather-conditions',
    isBatched: true
});
