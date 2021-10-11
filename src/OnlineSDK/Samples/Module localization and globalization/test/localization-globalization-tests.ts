/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url1 = 'https://localhost:4000/page?mock=myLocTestPage&theme=spring&locale=en-us';
fixture('Localization and Globalization test - en-us locale').page(url1);

test('validate rendered module strings are localized in en-us locale', async (testController: TestController) => {
    const primaryAreaContainer = Selector('main .container');
    const buyNowButton = primaryAreaContainer.find('#button_1');

    await testController.expect(buyNowButton.innerText).eql('Buy Now English US', 'Localized string is not in en-us locale');
});

test('validate currenty is localized in en-us locale', async (testController: TestController) => {
    const primaryAreaContainer = Selector('main .container');
    const priceText = primaryAreaContainer.find('#price_1');
    await testController.expect(priceText.innerText).eql('$129.00', 'Incorrect en-us currency locale');
});

const url2 = 'https://localhost:4000/page?mock=myLocTestPage&theme=spring&locale=fr-fr';
fixture('Globalization and localization test - fr-fr locale').page(url2);
test('validate rendered module strings are localized in fr-fr locale', async (testController: TestController) => {
    const primaryAreaContainer = Selector('main .container');
    const buyNowButton = primaryAreaContainer.find('#button_1');

    await testController.expect(buyNowButton.innerText).eql('Acheter Maintenant', 'Localized string is not in fr-fr locale');
});

test('validate currenty is localized in fr-fr locale', async (testController: TestController) => {
    const primaryAreaContainer = Selector('main .container');
    const priceText = primaryAreaContainer.find('#price_1');
    await testController.expect(priceText.innerText).eql('129,00\xa0€', 'Incorrect fr-fr currency locale');
});