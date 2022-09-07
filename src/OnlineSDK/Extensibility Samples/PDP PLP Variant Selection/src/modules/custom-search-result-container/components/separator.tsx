/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import * as React from 'react';

interface ISeparator {
    separator: string;
}

export const Separator: React.FC<ISeparator> = ({ separator }) => <span> {separator} </span>;
