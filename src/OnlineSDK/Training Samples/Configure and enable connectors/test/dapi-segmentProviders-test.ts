/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import fetch from 'node-fetch';
import 'testcafe';
import * as https from 'https';

fixture`Segmentation DAPI Tests`.before(async ctx => {
    const agent = new https.Agent({
        rejectUnauthorized: false
    });
    const res = await fetch('https://localhost:4000/_sdk/dapi/segmentproviders', {headers: {}, agent});
    ctx.res = res;
    const resBuffer = await res.buffer();
    const resJSON = JSON.parse(resBuffer.toString());
    ctx.resJSON = resJSON;
});
test('got valid dapi response', async (testController: TestController) => {
    await testController.expect(testController.fixtureCtx.res).ok('Invalid DAPI response');
});

test('validate all segment provider dapi responses', async (testController: TestController) => {
    for (const defN of testController.fixtureCtx.resJSON) {
        await testController
            .expect(defN.id)
            .notEql(undefined, `missing id property for module ${defN.id}`)
            .expect(defN.name)
            .notEql(undefined, `missing name property for module ${defN.name}`);
    }
});

test('validate all segment provider dapi responses', async (testController: TestController) => {
        await testController
            .expect(testController.fixtureCtx.resJSON[0].id)
            .eql('3pgeoLocationTest', `missing id property for module ${testController.fixtureCtx.resJSON[0].id}`)
            .expect(testController.fixtureCtx.resJSON[0].name)
            .eql('geoLocationTest', `missing name property for module ${testController.fixtureCtx.resJSON[0].name}`)
            .expect(testController.fixtureCtx.resJSON[0].segmentations.length).notEql(0, 'Missing segmentation list for geoLocationTest connector')
});
