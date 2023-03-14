/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { test } from 'testcafe';
import fetch from 'node-fetch';

fixture(`DAPI Experiments tests`).before(async ctx => {
     // @ts-ignore
     process.env['NODE_TLS_REJECT_UNAUTHORIZED'] = 0;
    const res = await fetch('https://localhost:4000/_sdk/dapi/experiments');
    ctx.res = res;
    const resBuffer = await res.buffer();
    const experimentsJSON = JSON.parse(resBuffer.toString());
    ctx.experimentsJSON = experimentsJSON;
});

test('got valid experiments list response', async (testController: TestController) => {
    await testController.expect(testController.fixtureCtx.res).ok('Invalid DAPI response for experiments');
});

test('validate experiments response', async (testController: TestController) => {
    /**
     * If this test is failing please ensure that exp-test-connector-test-2 is the selected experimentation provider in
     * connector.settings.json and please verify that the connector exists in demo-partner/src/connectors
     *
     */
    const experimentsJSON = testController.fixtureCtx.experimentsJSON;
    await testController
        .expect(experimentsJSON.name).eql('exp-test-connector')
        .expect(experimentsJSON.experiments.length).notEql(0, 'Error: Missing list of experiments')
        .expect(experimentsJSON.experiments[0].id)
        .eql('expid1')
        .expect(experimentsJSON.experiments[0].friendlyName)
        .eql(
            'experiment-1',
            'Experiment friendly name does not match friendly name defined in exp-test-connector. Check src/connectors'
        )
        .expect(experimentsJSON.experiments[0].description)
        .eql(
            'This is a exp-test-connector experiment',
            'Experiment description does not match description defined in exp-test-connector. Check src/connectors'
        )
        .expect(experimentsJSON.experiments[0].type)
        .eql('A/B Test', 'Experiment type does not match type defined in exp-test-connector. Check src/connectors')
        .expect(experimentsJSON.experiments[0].status)
        .eql(
            'Running',
            'Experiment status does not match status defined in exp-test-connector. Check src/connectors'
        )
        .expect(experimentsJSON.experiments[0].createdDate)
        .notEql(undefined, 'Missing createdDate for the experiment')
        .expect(experimentsJSON.experiments[0].lastModifiedDate)
        .notEql(undefined, 'Missing lastModifiedDate for the experiment')
        .expect(experimentsJSON.experiments[0].lastModifiedBy)
        .notEql(undefined, 'Missing lastModifiedBy for experiments')
        .expect(experimentsJSON.experiments[0].variations[0])
        .notEql(undefined, 'Missing list of variations defined for experiment')
        .expect(experimentsJSON.experiments[0].variations[0].id)
        .eql('var1', 'Variant id does not match status defined in exp-test-connector. Check src/connectors')
        .expect(experimentsJSON.experiments[0].variations[0].status)
        .eql('Active', 'Variant status does not match status defined in exp-test-connector. Check src/connectors')
        .expect(experimentsJSON.experiments[0].variations[0].weight)
        .eql('0.4', 'Variant weight does not match weight defined in exp-test-connector. Check src/connectors')
        .expect(experimentsJSON.experiments[0].variations[0].friendlyName)
        .eql(
            'variant-1',
            'Variant friendlyName does not match friendlyName defined in exp-test-connector. Check src/connectors'
        )
        .expect(experimentsJSON.experiments[0].variations[1].id)
        .eql('var2', 'Variant id does not match status defined in exp-test-connector. Check src/connectors')
        .expect(experimentsJSON.experiments[0].variations[1].status)
        .eql('Active', 'Variant status does not match status defined in exp-test-connector. Check src/connectors')
        .expect(experimentsJSON.experiments[0].variations[1].weight)
        .eql('0.4', 'Variant weight does not match weight defined in exp-test-connector. Check src/connectors')
        .expect(experimentsJSON.experiments[0].variations[1].friendlyName)
        .eql(
            'variant-2',
            'Variant friendlyName does not match friendlyName defined in exp-test-connector. Check src/connectors'
        );
});
