declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=search-result&theme=fabrikam-extended';
fixture('validate search page').page(url).beforeEach(async testController => {
    await testController.maximizeWindow();
});

test('validate product badge', async (testController: TestController) => {
       const renderProductBadge = Selector('.ms-search-result-container__Products .ms-product-search-result__item').find('.msc-product-badges').child(0);
       await testController
       .expect(renderProductBadge.exists)
       .ok('badge not found')
       .hover(renderProductBadge, { speed: 0.4 })
});