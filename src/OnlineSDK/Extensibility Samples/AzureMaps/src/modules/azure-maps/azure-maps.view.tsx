/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { IHeader, IAzureMapViewProps } from './azure-maps';

/**
 * Render the heading.
 * @param param0 -- Heading props.
 * @param param0.headerProps -- Heading Props.
 * @param param0.heading -- Heading Node.
 * @returns -- Returns the node.
 */
const AzureMapHeader: React.FC<IHeader> = ({ headerProps, heading }) => <Node {...headerProps}>{heading}</Node>;

/**
 * Renders the Azure map view props.
 * @param param0 -- Azure Map view props.
 * @param param0.ModuleProps -- Module props.
 * @param param0.Header -- Header props.
 * @param param0.MapProps -- Map props.
 * @param param0.Map -- The Azure map instance.
 * @returns -- Returns the Azure map module.
 */
const AzureMapView: React.FC<IAzureMapViewProps> = ({ ModuleProps, Header, MapProps }) => {
    return ModuleProps ? (
        <Module {...ModuleProps}>
            {Header && <AzureMapHeader {...Header} />}
            {MapProps && <Node {...MapProps} />}
        </Module>
    ) : null;
};

export default AzureMapView;
