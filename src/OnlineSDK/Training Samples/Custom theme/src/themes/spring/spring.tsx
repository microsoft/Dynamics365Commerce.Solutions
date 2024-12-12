/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as React from 'react';

/**
 * CoreComponent component
 * @extends {React.PureComponent<IScriptInjectorProps>}
 */

class Spring extends React.PureComponent {
    constructor(props: {}) {
        super(props);
    }

    public render(): JSX.Element | null {
        return <script />;
    }
}

export default Spring;
