/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as React from 'react';
import { IGiftCardPersonalizationViewProps } from './gift-card-personalization';

export default (props: IGiftCardPersonalizationViewProps) => {
    return <div className='row ms-giftCardPersonalization'>{props.personalizeOptions}</div>;
};
