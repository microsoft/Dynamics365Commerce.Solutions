/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import React, { useState } from 'react';
import { ILanguagePickerViewProps } from './language-picker';

export default (props: ILanguagePickerViewProps) => {
    const languages = props.config.languages;
    const [show, setShow] = useState(false);
    const setShowTrue = () => setShow(true);
    const setShowFalse = () => setShow(false);
    let baseUrl = props.config.linkBaseUrl;
    const currentUrl = props.context.request.url.requestUrl.pathname;
    let currentLocale = '';
    if (!baseUrl?.endsWith('/')) {
        baseUrl = `${baseUrl}/`;
    }
    const options = languages!.map((item, i) => {
        const index = currentUrl.indexOf(`/${item.code}/`);
        let startIndex = 0;
        let localeCodeLen = 0;
        if (index > -1) {
            startIndex = index + 1;
            localeCodeLen = item.code.length;
            currentLocale = currentUrl.substring(startIndex, startIndex + localeCodeLen);
        }

        return (
            // tslint:disable-next-line: max-line-length
            <li
                className={`ms-language-picker__list__item  ${item.code === currentLocale && item.code !== '' ? 'selected' : ''}`}
                key={item.name}
                role='menuitem'
                id={item.code}
                onClick={props.onChange}
                style={{ display: `${show ? 'block' : 'none'}` }}
            >
                <img className={`ms-language-picker__list__item__img`} src={item.imageUrl} alt={`imageUrl of ${item.code}`} /> {item.name}
            </li>
        );
    });
    options.unshift(
        <button className='ms-language-picker__button'>
            Select your language
            <span className='ms-language-picker__span' />
        </button>
    );
    return (
        <div className='ms-language-picker'>
            <ul className='ms-language-picker__list' onMouseEnter={setShowTrue} onMouseLeave={setShowFalse}>
                {options}
            </ul>
        </div>
    );
};
