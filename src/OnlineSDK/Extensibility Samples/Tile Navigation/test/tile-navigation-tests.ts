declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'https://localhost:4000/?mock=tile-navigation&theme=fabrikam-extended';

fixture('validate product page').page(url).beforeEach(async testController => {
    await testController.maximizeWindow();
});

test('validate tile navigation category name is rendered on the page', async (testController: TestController) => {
    const tileNavigationTitle = Selector('.tile-category-name').with({ visibilityCheck: true })();
    await testController
    .expect(tileNavigationTitle.exists)
    .ok('Incorrect category name rendered')
    .hover(tileNavigationTitle, { speed: 0.1 });
});
