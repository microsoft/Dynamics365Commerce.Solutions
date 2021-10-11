/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable @typescript-eslint/no-explicit-any */
import { IExpLogger } from '@msdyn365-commerce/telemetry-internal';

/**
 * A basic implementation of the ExperimentationProvider interface used for testing
 */
class ExpTestConnectorListener implements IExpLogger {
    private userId: string = '';
    public initializeClientSide(config: any, userId: string): boolean {
        this.userId = userId;
        console.log(`Listener initialize called with config ${JSON.stringify(config)} and userId ${userId}`);
        return true;
    }
    public logEvent(eventName: string, payload: any, attributes?: any): void {
        console.log(`tracking events with event type - ${eventName}, payload - ${payload},  user id - ${this.userId}`);
    }
}

const connectorListener = new ExpTestConnectorListener();
export default connectorListener;
