declare var test: TestFn;

import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=cart-route';
fixture('Cart route test')
    .page(url)
    .beforeEach(async testController => {
        await testController.maximizeWindow();
    });

const getWindowLocation = ClientFunction(() => window.location);

// Check that cart icon link is same as in settings
test('validate cart route', async (testController: TestController) => {
    const cartIcon = Selector('.ms-cart-icon')
        .find('a')
        .withAttribute('href', '/cart');
    await testController.expect(cartIcon.exists).eql(true, 'Could not find cart route');
});
