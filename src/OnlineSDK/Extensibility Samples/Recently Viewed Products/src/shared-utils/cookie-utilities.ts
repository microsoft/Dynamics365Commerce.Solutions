import { ICookieValue } from '@msdyn365-commerce/core';

// tslint:disable-next-line: no-any
export const GetCookie = (cookie: any, cookieName: string): string | undefined => {
    // tslint:disable-next-line: prefer-type-cast
    const marketCookie = cookie.get(cookieName) as ICookieValue<string> | undefined;

    if (marketCookie && marketCookie.value && marketCookie.value.length > 0) {
        return marketCookie.value;
    } else {
        return undefined;
    }
};

// tslint:disable-next-line: no-any
export const SetCookie = (cookies: any, value: string, cookieName: string, exp: number): void => {
    if (value) {
        cookies.set(cookieName, value, { path: '/', maxAge: exp });
    }
};
