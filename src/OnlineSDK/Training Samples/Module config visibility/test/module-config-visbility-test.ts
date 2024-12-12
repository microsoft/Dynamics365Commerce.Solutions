/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { test } from 'testcafe';
import fetch from 'node-fetch';

fixture`MOdule config visiblity schema test`.before(async ctx => {
    // @ts-ignore
    process.env['NODE_TLS_REJECT_UNAUTHORIZED'] = 0;
    const res = await fetch('https://localhost:4000/_sdk/dapi/modules');
    ctx.res = res;
    const resBuffer = await res.buffer();
    const resJSON = JSON.parse(resBuffer.toString());
    ctx.resJSON = resJSON;
});

test('check dependency schema for config-visibility module', async (testController: TestController) => {
    for (const defN of testController.fixtureCtx.resJSON) {
        const name = defN.name;
        if (name === 'config-visibility') {
            const moduleConfig = defN.config;
            const dependentSchemas = defN.dependentSchemas;
            await testController
                .expect(moduleConfig['productTitle'])
                .ok('Module config property productTitle is missing');
            await testController
                .expect(moduleConfig['layout'])
                .ok('Module config property layout is missing');
            await testController
                .expect(dependentSchemas['productTitle'])
                .ok('dependentSchemas is mssing productTitle selector');
            await testController
                .expect(dependentSchemas['layout'])
                .ok('dependentSchemas is mssing layout selector');
            await testController
                .expect(dependentSchemas['productTitle']['properties']['subTitle'])
                .ok('dependentSchemas layout selector is missing  subTitle config');
            await testController
                .expect(dependentSchemas['productTitle']['required'][0]).eql('productTitle', 'dependentSchemas layout selector required property is empty');
            await testController
                .expect(dependentSchemas['layout']['oneOf'].length).eql(3, 'dependentSchemas layout selector is missing oneoF properties');
            await testController
                .expect(dependentSchemas['layout']['oneOf'][0].properties.layout.enum.plainTextOnly).ok("Missing plainTextOnly enum in Dependent schema");
                await testController
                .expect(dependentSchemas['layout']['oneOf'][0].properties.featureText).ok("Missing featureText config in Dependent schema");
                await testController
                .expect(dependentSchemas['layout']['oneOf'][1].properties.layout.enum.richTextOnly).ok("Missing richTextOnly enum in Dependent schema");
                await testController
                .expect(dependentSchemas['layout']['oneOf'][1].properties.featureRichText).ok("Missing featureRichText config in Dependent schema");
                await testController
                .expect(dependentSchemas['layout']['oneOf'][2].properties.layout.enum.richTextWithImage).ok("Missing richTextWithImage config in Dependent schema");
                await testController
                .expect(dependentSchemas['layout']['oneOf'][2].properties.featureRichText).ok("Missing featureRichText config in Dependent schema");
                await testController
                .expect(dependentSchemas['layout']['oneOf'][2].properties.featureImage).ok("Missing featureImage config in Dependent schema");
                await testController
                .expect(dependentSchemas['layout']['oneOf'][2].properties.imageAlignment).ok("Missing imageAlignment config in Dependent schema");
        }
    }
});


