# Dynamics 365 Commerce - online SDK samples
## License
License is listed in the [LICENSE](./LICENSE) file.

# Sample 17- Batching a data action
In this samples you will learn how to write a batched data action and the scenario where you can use batch a data action.

## Overview
Often you'll have an application that requires many calls to the same application programming interface (API) during the load of a single page. An example is a product feature page that showcases information about many products instead of just one product. 

In a typical approach, multiple calls are made to the data action to get products. However, because this approach uses many individual HTTP requests to get the product information, it might not be efficient. To solve this issue, the data action architecture supports batchable data actions. For a batched data action, the data action framework automatically groups the requests together instead of making individual calls. That's why this approach helps minimize the number of HTTP requests that are required and helps maximize performance.

Note: A batched data action can be used to call retail-proxy api or any third party api
## Doc links
https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/batch-data-actions


## Detailed Steps
In this sample we will be calling a free third party API called OpenWeatherMaps API, through which we will be getting the current weather information for multiple cities and displaying the weather information through our module.

### 1. Use CLI command to create new get-locations data actions:
Use the CLI to create get-locations data action: ```yarn msdyn365 add-data-action get-locations```. The new data action will be created under the **\src\actions** directory.


In this data action, we return the locations for which we want to get the weather conditions for, please copy the below code in the file **\src\actions\get-locations.action.ts**

```typescript
/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as Msdyn365 from '@msdyn365-commerce/core';
import { ILocation } from './get-current-weather-conditions.action';

const locations: ILocation[] = [
    { id: '2643743', name: 'London, UK' },
    { id: '5856195', name: 'Honolulu, HI' },
    { id: '2147714', name: 'Sydney, AU' },
    { id: '5809844', name: 'Seattle, WA' }
];

/**
 * input for get-favorites
 */
export class GetLocationsInput implements Msdyn365.IActionInput {
    public getCacheKey = () => 'Default';
    public getCacheObjectType = () => 'FAVORITE-Locations';
    public dataCacheType = (): Msdyn365.CacheType => 'request';
}

const createLocationsInput = (): Msdyn365.IActionInput => {
    return new GetLocationsInput();
};

async function action(): Promise<ILocation[]> {
    return locations;
}

export default Msdyn365.createObservableDataAction({
    action: <Msdyn365.IAction<ILocation[]>>action,
    id: 'get-favorites-location',
    input: createLocationsInput
});

```

### 2. Use CLI command to create new get-current-weather-conditions data actions:
=> Use the CLI to create get-current-weather-conditions data action: ```yarn msdyn365 add-data-action get-current-weather-conditions```. The new data action will be created under the **\src\actions** directory.

We will mark get-current-weather-conditions as batched actions, as in this data action we will call the thirdparty OpenWeatherMap API only once to get the weather information for our favorite cities

Please copy the below code in the file **\src\actions\get-current-weather-conditions.action.ts**

```typescript
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

    await new Promise(resolve => {
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
```

## Things to note in this action:
1) The input to the data action is an array ```input: OpenWeatherApiInput[]```
2) We get the id's from the input[] and pass it in the OpenWeatherMaps URL all at once, so that only one call is made to OpenWeatherMaps API. In order to batch a data-action, if you are calling a third party api, the third party api should also support taking multiple inputs.
3) Lastly, note that in the function ```Msdyn365.createObservableDataAction()```, the isBatched flag is set to true ```isBatched: true```

### 3. Use CLI command to create new get-favorite-location-weather data actions:
=> Use the CLI to create get-favorite-location-weather data action: ```yarn msdyn365 add-data-action get-favorite-location-weather```. The new module will be created under the **\src\modules** directory.

In this data action we will be calling our batched data action ```get-current-weather-conditions```. Please copy the below code in file **\src\actions\get-favorite-location-weather.action.ts**

```typescript
/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as Msdyn365 from '@msdyn365-commerce/core';
import { ISampleCurrentConditionsConfig } from '../modules/sample-current-conditions/sample-current-conditions.props.autogenerated';
import getFavorites, { GetLocationsInput } from './get-locations.action';
import getCurrentWeatherConditions, { IWeatherConditions, OpenWeatherApiInput} from './get-current-weather-conditions.action';

export class GetFavoriteLocationWeatherInput implements Msdyn365.IActionInput {
    public readonly apiKey: string;
    public readonly units?: string;
    public readonly language?: string;

    public constructor(apiKey: string, language?: string, units?: string) {
        this.apiKey = apiKey;
        this.language = language;
        this.units = units;
    }

    public getCacheKey = () => 'Default';
    public getCacheObjectType = () => 'FAVORITE-LOCATION-WEATHER';
    public dataCacheType = (): Msdyn365.CacheType => 'none';
}

const createFavoriteLocationWeatherInput = (args: Msdyn365.ICreateActionContext<ISampleCurrentConditionsConfig>): Msdyn365.IActionInput => {
    return new GetFavoriteLocationWeatherInput((args.config && args.config.apiKey) || '', args.requestContext.locale);
};

async function action(input: GetFavoriteLocationWeatherInput, ctx: Msdyn365.IActionContext): Promise<IWeatherConditions[]> {
    const favorites = await getFavorites(new GetLocationsInput(), ctx);
    if (favorites && favorites.length) {
        const weatherApiInput = favorites.map(fave => new OpenWeatherApiInput(input.apiKey, fave, input.language, input.units));
        return getCurrentWeatherConditions(weatherApiInput, ctx);
    }

    return [];
}

export default Msdyn365.createObservableDataAction({
    id: 'get-favorite-location-weather',
    action: <Msdyn365.IAction<IWeatherConditions[]>>action,
    input: createFavoriteLocationWeatherInput
});
```

Things to note in the data action:
1) We construct an input array ```weatherApiInput``` of ```OpenWeatherApiInput[]``` type
2) We call ```getCurrentWeatherConditions``` data-action and pass the variable ```weatherApiInput``` of ```OpenWeatherApiInput[]``` as an input parameter

### 4. Use CLI command to create a sample-current-conditions module:
Use the CLI command: ```yarn msdyn365 add-module sample-current-conditions``` to create a module called **sample-current-conditions**. The new module will be created under the **\src\modules** directory.

### 5. Add configuration properties into the module definition file

Copy the following json into the **\src\modules\ sample-current-conditions\ sample-current-conditions.json** file to create a set of configuration properties.

```typescript
{
    "$type": "contentModule",
    "friendlyName": "Current Conditions",
    "name": "sample-current-conditions",
    "description": "Module that demonstrates a batch action",
    "categories": ["samples"],
    "dataActions": {
        "favorite_locations":{
            "path": "../../actions/get-locations.action",
            "runOn": "client"
        },
        "forecast":{
            "path": "../../actions/get-favorite-location-weather.action",
            "runOn": "client"
        }
    },
    "resources": {
        "cardHeader": {
            "value": "Current conditions at your favorite locations"
        }
    },
    "config": {
        "apiKey": {
            "friendlyName": "Weather API Key",
            "description": "API Key for weather",
            "type": "string",
            "scope": "siteOnly",
            "group": "Weather module Properties"            
        }
    }
}
```

### 6. Update Sample-current-conditions.data.ts file:

Copy the following json into the **\src\modules\ sample-current-conditions\sample-current-conditions.data.ts** file to 

```typescript
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
```

### 7. Update the module.tsx file:
Copy the following code into the sample-current-conditions.tsx file to setup the module to render the current weather location:

```typescript
/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as Msdyn365 from '@msdyn365-commerce/core';
import * as React from 'react';
import { comparer, reaction } from 'mobx';
import { observer } from 'mobx-react';
import getCurrentWeatherConditions, {IWeatherConditions, OpenWeatherApiInput} from '../../actions/get-current-weather-conditions.action';
import { ISampleCurrentConditionsData } from './sample-current-conditions.data';
import { ISampleCurrentConditionsProps } from './sample-current-conditions.props.autogenerated';

interface IState {
    units: string;
    updated: Date;
    client: boolean;
}

export interface ISampleCurrentConditionsViewProps extends ISampleCurrentConditionsProps<ISampleCurrentConditionsData> {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    state: any;
    formatter: Msdyn365.ICultureInfoFormatter;
    onClick(): Promise<void>;   
    _renderForecast(data: IWeatherConditions, index: number): JSX.Element;
}

/**
 *
 * SampleWeatherCurrentConditions component
 * @extends {React.PureComponent<ISampleCurrentConditionsProps<ISampleCurrentConditionsData>>}
 */
@observer
class SampleCurrentConditions extends React.Component<ISampleCurrentConditionsProps<ISampleCurrentConditionsData>, IState> {
    private readonly formatter: Msdyn365.ICultureInfoFormatter;
    constructor(props: ISampleCurrentConditionsProps<ISampleCurrentConditionsData>) {
        super(props);
        this.state = {
            client: false,
            units: 'F',
            updated: new Date()
        };
        this.toggleUnit.bind(this);
        this._renderForecast.bind(this);
        this.formatter = (this.props.context && this.props.context.cultureFormatter) || new Msdyn365.CultureInfoFormatter('en-us', 'USD');
    }

    public async componentDidMount(): Promise<void> {
        reaction(
            () => this.props.data.favorite_locations.result && this.props.data.favorite_locations.result.map(f => f.id),
            () => {
                console.log('Detected a change');
                return this._refreshData();
            },
            {
                equals: comparer.structural
            }
        );
        this.setState({ client: true });
    }

    public render(): JSX.Element | null {
        const data = this.props.data;

        if (data.forecast.status === 'LOADING' || !this.state.client) {
            return this._renderLoading();
        }

        if (!data || data.forecast.error || data.favorite_locations.error) {
            return this._renderError();
        }

        const viewProps: ISampleCurrentConditionsViewProps = {
            ...this.props,
            onClick: this.toggleUnit.bind(this),
            _renderForecast: this._renderForecast.bind(this),
            formatter: this.formatter,
            state: this.state
        };
        return this.props.renderView(viewProps);
    }

    private async _refreshData(): Promise<void> {
        const { config, context, data } = this.props;
        if (config && config.apiKey && context && data.favorite_locations.result) {
            const apiKey = config.apiKey || '';

            const inputs = data.favorite_locations.result.map(f => new OpenWeatherApiInput(apiKey, f, context.request.locale, this.state.units));

            // Calling current weather conditions data action here, Notice, the input passed is an array[]
            void getCurrentWeatherConditions(inputs, context.actionContext).then(foo => {
                this.props.data.forecast.result = foo;
            });

            this.setState({});
        } else {
            console.log('Reaction triggered, but no result available');
        }
    }

    private _renderError(): JSX.Element {
        return (
            <div className='card'>
                <div className='card-header'>Error!</div>
            </div>
        );
    }

    private _renderLoading(): JSX.Element {
        return (
            <div className='spinner-border text-primary' role='status'>
                <span className='sr-only'>Loading...</span>
            </div>
        );
    }

    private _renderForecast(data: IWeatherConditions, index: number): JSX.Element {
        if (!data || !data.weather || !data.weather.length) {
            return (
                <div className='col-lg-3 col-md-6 col-sm-8' key={`cc-${index}`}>
                    No data available
                </div>
            );
        }

        const icon = data.weather[0].icon;
        const caption = data.weather[0].main;
        const date = new Date(data.dt * 1000);
        return (
            <div className='col-lg-3 col-md-6 col-sm-8' key={`cc-${index}`}>
                <div className='card mb-3 mx-5 mx-sm-0'>
                    <div className='card-header'>{data.name}</div>
                    <div className='text-center'>
                        <img
                            src={`//openweathermap.org/themes/openweathermap/assets/vendor/owm/img/widgets/${icon}.png?w=75&m=6`}
                            alt={caption}
                            width='50'
                        />
                        <h4 className='card-title'>{caption}</h4>
                        <h5 className='card-text'>{`${data.main.temp} ${this.state.units}`}</h5>
                        <div className='font-size-small'>
                            <p>
                                {this.formatter.formatDate(date, { weekday: 'short', month: 'short', day: '2-digit' })}
                                &nbsp;
                                {this.formatter.formatTime(date, { hour: 'numeric', minute: 'numeric', second: 'numeric' })}
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    private toggleUnit = async () => {
        this.setState({ units: this.state.units === 'F' ? 'C' : 'F' }, async () => {
            await this._refreshData();
        });
    };
}

export default SampleCurrentConditions;
```
Notice on that in the ```_refreshData()``` function we call we call ```getCurrentWeatherConditions``` data-action and pass the variable ```weatherApiInput``` of ```OpenWeatherApiInput[]``` as an input parameter.

### 7. Add module view code
The module view code is responsible for generating the module's HTML. Add the below to the module view file **\src\modules\sample-current-conditions\sample-current-conditions.view.tsx**.

```typescript
/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as React from 'react';
import { IWeatherConditions } from '../../actions/get-current-weather-conditions.action';
import { ISampleCurrentConditionsViewProps } from './sample-current-conditions';

export default (props: ISampleCurrentConditionsViewProps) => {
    return (
        <div className='sample-current-conditions card'>
            <div className='card-header'>
                {props.resources.cardHeader}
                <button onClick={props.onClick} role='button' className='btn btn-outline-primary float-right'>
                    {props.state.units}
                </button>
            </div>
            <div className='card-body'>
                <div className='container'>
                    <div className='row justify-content-center'>
                        {props.data.forecast.result &&
                            props.data.forecast.result.map((d: IWeatherConditions, i: number) => props._renderForecast(d, i))}
                    </div>
                </div>
            </div>
        </div>
    );
};
```
### 8. Add module mock for testing module locally:
The openWeatherMaps API requeries and API access key, to access the weather information. Please update the module mock in  **\src\modules\sample-current-conditions\mocks\sample-current-conditions.json**. with the below code: 
```json
{
  "id": "current_conditions__0",
  "config": {
    "apiKey": "20d86600c995ba23fad03e1a291fa9b9"
  },
  "data": {
      "foo": "bar"
  },
  "typeName": "sample-current-conditions"
}
```

### 9. Create a custom theme

The module view code above leverages the open-source JavaScript **Bootstrap** library to create two distinct columns of equal width.  Bootstrap will provide a responsive design, so that shrinking the window size will eventually drop it to a single column without any additional work.  To support Bootstrap, we need to specify a theme when rendering the module.  Run the below command to create a new theme called **fall** that will be created in the \src\themes\ directory.

```yarn msdyn365 add-theme fall```

We will not make any changes to the theme, however if you open the theme’s \styles\fall.theme.scss file you will see that it imports the bootstrap library.  

### 9. Build and test module
The module and theme can now be built and tested in a web browser using the ```yarn start``` command to build and start the Node server. Once the Node server is ready, from within a local browser you can view the module and theme with the following URL https://localhost:4000/modules?type=sample-current-conditions&theme=fall to see the current weather for different cities

![Module with theme](docs/Image1.png)


With this example, we successfully batch a data action by making only 1 HTTPS call to a thirdparty API. Following the similar example, you can call a retail-proxy action through your custom data-action and make it a batched action.

Note: This sample is just for example purpose for developers to understand how they can use batching a data action feature to reduce HTTPS calls and improve performance of thier application.




