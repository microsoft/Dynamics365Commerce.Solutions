/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { HtmlHeadInclude } from '@msdyn365-commerce/core-internal';
import * as React from 'react';
import { IMyScriptInjectorViewProps } from './my-script-injector';

export default (props: IMyScriptInjectorViewProps) => {
    return (
        <HtmlHeadInclude>
            <script
                data-load-point='headStart'
                // eslint-disable-next-line react/no-danger
                dangerouslySetInnerHTML={{ __html: props.config.inlineScript }}
                defer={props.config.defer}
                async={props.config.async}
                crossOrigin={props.config.crossorigin}
            />
            <p>This is a custom script injector module</p>
        </HtmlHeadInclude>
    );
};
