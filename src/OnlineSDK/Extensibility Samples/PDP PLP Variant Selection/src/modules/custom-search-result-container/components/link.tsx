/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import * as React from 'react';

interface ILink {
    className?: string;
    text?: string;
    href?: string;
    ariaLabel?: string;
}

export const Link: React.FC<ILink> = ({ text, className, href, ariaLabel }) => (
    <a className={className} href={href} aria-label={ariaLabel}>
        {' '}
        {text}{' '}
    </a>
);
