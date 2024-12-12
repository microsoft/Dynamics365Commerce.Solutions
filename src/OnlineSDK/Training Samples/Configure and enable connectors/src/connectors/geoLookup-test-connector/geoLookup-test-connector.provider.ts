/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { IGeoLocation, IGeoLookupProvider, IRequestContext } from '@msdyn365-commerce/core';
/**
 * A basic implementation of the ExperimentationProvider interface used for testing
 */
class GeoLookupTestConnector implements IGeoLookupProvider {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    public initialize(config: any): Promise<boolean> {
        console.log(`GeoLookup Test Connector called with config: ${JSON.stringify(config)}`);
        return Promise.resolve(true);
    }

    public getGeoInformation(ip: string, requestContext: IRequestContext, userId?: string): Promise<IGeoLocation> {
        console.log('GeoLookup Test Connector get geo information with ip, baseurl -', ip, requestContext?.apiSettings?.baseUrl, userId);
        let userid = 'not Added';
        if (userId) {
            userid = userId;
        }
        const geoLocation: IGeoLocation = {
            country: 'test-country',
            city: 'test-city',
            user: userid
        };
        return Promise.resolve(geoLocation);
    }
}

const connector = new GeoLookupTestConnector();
export default connector;
