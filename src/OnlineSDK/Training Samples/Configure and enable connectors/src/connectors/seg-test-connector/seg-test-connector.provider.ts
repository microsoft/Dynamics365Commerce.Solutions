/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { ISegmentationProvider, ISementations } from '@msdyn365-commerce/core';
/**
 * A basic implementation of the ExperimentationProvider interface used for testing
 */
class SegmentationTestConnector implements ISegmentationProvider {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    public initialize(config: any): Promise<boolean> {
        console.log(`Segmentation Test Connector called with config: ${JSON.stringify(config)}`);
        return Promise.resolve(true);
    }

    public async getSegmentations(userId: string, segmentationIds: string[]): Promise<ISementations> {
        console.log(`Segmentation  Test Connector will resolve segments - ${segmentationIds} for ${userId}`);
        const testResult: ISementations = {};
        if (segmentationIds) {
            segmentationIds.forEach(segmentId => {
                testResult[segmentId] = `${segmentId}'s value`;
            });
        }

        return Promise.resolve(testResult);
    }
}

const connector = new SegmentationTestConnector();
export default connector;
