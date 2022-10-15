/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import * as React from 'react';

interface ITitle {
    className: string;
    text: string;
}

export const Title: React.FC<ITitle> = ({ text, className }) => <span className={className}> {text} </span>;
