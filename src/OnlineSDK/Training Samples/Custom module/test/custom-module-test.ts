/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { Selector, test } from 'testcafe';
import fetch from 'node-fetch';

fixture('Custom module test').page('https://localhost:4000/modules?type=product-feature&theme=spring');
test('validate renderPage div render', async (testController: TestController) => {
    const renderPageDiv = Selector('#renderPage');
    await testController.expect(renderPageDiv.exists).eql(true, 'Could not find default page container');
});

test('validate image is rendering on the page', async (testController: TestController) => {
    const primaryAreaContainer = Selector('main .default-container');
    const imageTag = primaryAreaContainer.find('.img-fluid');
    await testController.expect(imageTag.exists).eql(true, 'Image not rendered on the page');
});

test('validate product heading is rendering on the page', async (testController: TestController) => {
    const primaryAreaContainer = Selector('main .primaryRegion');
    const productTitle = primaryAreaContainer.find('#product_Title_1');
    await testController.expect(productTitle.innerText).eql('Retro Horn Rimmed Keyhole Nose Bridge Round Sunglasses', 'Product title is not rendering on the page');
});

fixture`DAPI preview API Test`.before(async ctx => {
    // @ts-ignore
    process.env['NODE_TLS_REJECT_UNAUTHORIZED'] = 0;
    const modulePreviewRes = await fetch('https://localhost:4000/_sdk/dapi/previews?module=product-feature');
    ctx.modulePreviewRes = modulePreviewRes;
    const modulePreviewResBuffer = await modulePreviewRes.buffer();
    const modulePreviewResJSON = JSON.parse(modulePreviewResBuffer.toString());
    ctx.modulePreviewResJSON = modulePreviewResJSON;
});

test('check preview data schema for product-feature module', async (testController: TestController) => {
    const previewDatas = { ...testController.fixtureCtx.modulePreviewResJSON };
    await testController.expect(Object.keys(previewDatas)).contains('product-feature');
    Object.keys(previewDatas).forEach(async key => {
        const previewData = previewDatas[key];
        await testController.expect(previewData.typeName).ok(`typename missing for ${key}`);
        await testController.expect(previewData.id).ok(`id missing for ${key}`);
        await testController.expect(previewData.id).eql('product-feature', 'Incorrect product id');
        await testController.expect(previewData.config).ok(`Config missing for ${key}`);
        await testController.expect(previewData.config.imageAlignment).eql('left', 'Incorrect value for imageAlignment config');
        await testController.expect(previewData.config.productTitle).eql('Sample product title', 'Incorrect value for Sample product title config');
        await testController.expect(previewData.config.productDetails).eql('Sample product title details.', 'Incorrect value for productDetails config');
        await testController.expect(previewData.config.buttonText).eql('Buy Now"', 'Incorrect value for buttonText config');
        await testController.expect(previewData.data).ok(`data missing for ${key}`);
    });
});


