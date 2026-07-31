declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=show-category-products&skip=5&theme=fabrikam-extended';

fixture('validate plp page')
    .page(url)
    .beforeEach(async testController => {
        await testController.maximizeWindow();
    });
const getWindowLocation = ClientFunction(() => window.location.href);
test('validate pagination on show-category-products page', async (testController: TestController) => {
    const paginationDiv = Selector('.ms-search-result-container__Products');
    const products = paginationDiv.child().find('.ms-product-search-result__item');
    const productsCount = paginationDiv.child().find('.ms-product-search-result__item').count;
    await testController.expect(paginationDiv.exists).eql(true, 'pagination found');
    await testController.expect(productsCount).eql(5);

        var pageUrl = await getWindowLocation();
        await t.expect(pageUrl).contains('skip=5', 'skip count not available');
});
