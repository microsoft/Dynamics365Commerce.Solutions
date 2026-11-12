declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'https://localhost:4000/page?mock=wishlistcount&theme=fabrikam-extended';

fixture('validate renderPage div render').page(url).beforeEach(async testController => {
    await testController.maximizeWindow();
});
test('validate wishlist Count module rendered inside header module on the page', async (testController: TestController) => {
    const wishlistCountText = Selector('.msc-wishlist-icon__textcount');
    await testController
    .expect(wishlistCountText.exists)
    .ok('Could not find wishlist count text on the page')
    .hover(wishlistCountText, { speed: 0.4 });
});

