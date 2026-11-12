/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as React from 'react';
import { useState } from 'react';

import { ICookieTestViewProps } from './cookie-test';

export default (props: ICookieTestViewProps) => {
    const [favColor, setfavColor] = useState(' ');

    const setCookieValue = () => {
        // set a cookie representing the users favorite color but only is consent is given
        if (props.context.request.cookies.isConsentGiven()) {
            props.context.request.cookies.set<string>('favoriteColor', favColor);
        }
        alert(`set cookie to ${favColor}`);
    };

    const getCookieValue = () => {
        // get the value of the users favorite cookie
        const favColor = props.context.request.cookies.get<string>('favoriteColor');
        alert(`favColor = ${favColor.value}`);
    };

    return (
        <div className='row'>
            <input type='text' onChange={event => setfavColor(event.target.value)} placeholder='Enter your favorite color' />
            <br></br>
            <button onClick={setCookieValue}>Set cookie</button>
            <br></br>
            <button onClick={getCookieValue}>Get cookie</button>
        </div>
    );
};
