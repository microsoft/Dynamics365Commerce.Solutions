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

test('Validate super-content-block module properties', async (testController: TestController) => {
    for (const defN of testController.fixtureCtx.resJSON) {
        const name = defN.name;
        if (name === 'super-content-block') {
            await testController
                .expect(defN.friendlyName)
                .ok('super-content-block is missing friendlyName property');
            await testController
                .expect(defN.type)
                .ok('super-content-block is missing type property');
            await testController    
                .expect(defN.description)
                .ok('super-content-block is missing description property');
            await testController  
                .expect(defN.config)
                .ok('super-content-block is missing config property');
            await testController
                .expect(defN.config.heading)
                .ok('super-content-block config is missing heading property');
            await testController               
                .expect(defN.config.paragraph)
                .ok('super-content-block config is missing paragraph property');
            await testController               
                .expect(defN.config.image)
                .ok('super-content-block config is missing image property');
            await testController              
                .expect(defN.config.links)
                .ok('super-content-block config is missing links property');
            await testController             
                .expect(defN.config.additionalContent)
                .ok('super-content-block config is missing additionalContent property');
            await testController                           
                .expect(defN.config.actionableRegion)
                .ok('super-content-block config is missing actionableRegion property');
            await testController                                           
                .expect(defN.config.imageLink)
                .ok('super-content-block config is missing imageLink property');
            await testController                                         
                .expect(defN.config.imageAriaLabel)
                .ok('super-content-block config is missing imageAriaLabel property');
            await testController                                           
                .expect(defN.config.className)
                .ok('super-content-block config is missing className property');
        }
    }
});
