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
