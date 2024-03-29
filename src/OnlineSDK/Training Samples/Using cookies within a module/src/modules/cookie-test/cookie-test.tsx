/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as React from 'react';

import { ICookieTestData } from './cookie-test.data';
import { ICookieTestProps } from './cookie-test.props.autogenerated';

export interface ICookieTestViewProps extends ICookieTestProps<ICookieTestData> {}

/**
 *
 * CookieTest component
 * @extends {React.PureComponent<ICookieTestProps<ICookieTestData>>}
 */
class CookieTest extends React.PureComponent<ICookieTestProps<ICookieTestData>> {
    public render(): JSX.Element | null {
        return this.props.renderView(this.props);
    }
}

export default CookieTest;
