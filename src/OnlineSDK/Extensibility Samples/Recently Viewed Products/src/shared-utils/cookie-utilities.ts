/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { ICookieValue } from '@msdyn365-commerce/core';

/* eslint-disable  @typescript-eslint/no-explicit-any */
export const GetCookie = (cookie: any, cookieName: string): string | undefined => {
    // tslint:disable-next-line: prefer-type-cast
    const marketCookie = cookie.get(cookieName) as ICookieValue<string> | undefined;

    if (marketCookie && marketCookie.value && marketCookie.value.length > 0) {
        return marketCookie.value;
    } else {
        return undefined;
    }
};

/* eslint-disable  @typescript-eslint/no-explicit-any */
export const SetCookie = (cookies: any, value: string, cookieName: string, exp: number): void => {
    if (value) {
        cookies.set(cookieName, value, { path: '/', maxAge: exp });
    }
};
