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
const getWindowLocation = ClientFunction(() => window.location);

fixture('Third-party API tests').page('https://localhost:4000/modules?type=product-feature&theme=fall');
test('validate renderPage div render', async (testController: TestController) => {
    console.log('url: ', await getWindowLocation());
    const renderPageDiv = Selector('#renderPage');
    await testController.expect(renderPageDiv.exists).eql(true, 'Could not find default page container');
});
test('Validate response from third party api is rendered on the page', async (testController: TestController)=> {
    const paragraphText = Selector('#apiText');
    await testController.expect(paragraphText.exists).eql(true, 'Could not find p tag');
    await testController.expect(paragraphText.innerText).notEql('', 'API text is not empy');
});