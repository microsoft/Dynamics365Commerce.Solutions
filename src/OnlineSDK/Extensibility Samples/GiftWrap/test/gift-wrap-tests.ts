declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=pdp&theme=fabrikam-extended';
fixture('validate product page').page(url).beforeEach(async testController => {
    await testController.maximizeWindow();
});

test('validate gift wrap', async (testController: TestController) => {
    const renderBadge = Selector('.ms-buybox__is-gift-wrap-checkbox');
    await testController
    .expect(renderBadge.exists)
    .ok('gift wrap checkbox not found')
    .hover(renderBadge, { speed: 0.4 });
});