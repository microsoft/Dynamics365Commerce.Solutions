/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as React from 'react';
import { ITileNavigationViewProps } from './tile-navigation';

const TileNavigationView: React.FC<ITileNavigationViewProps> = props => {
    const { tiles } = props;
    return <div className='row'>{tiles}</div>;
};

export default TileNavigationView;
