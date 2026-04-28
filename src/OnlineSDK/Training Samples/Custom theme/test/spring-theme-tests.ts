declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'https://localhost:4000/modules?type=product-feature&theme=spring';
fixture('Default Page tests').page(url)

test('validate renderPage div render', async (testController: TestController) => {
    const renderPageDiv = Selector('#renderPage');
    await testController.expect(renderPageDiv.exists).eql(true, 'Could not find default page container');
});

test('validate custom module title', async (testController: TestController) => {
    const pageDiv = Selector('.product-feature-title');
    await testController
        .expect(pageDiv.exists)
        .ok('product feature title is rendered correctly')
        .hover(pageDiv, { speed: 0.4 })
});

test('validate the text color', async (testController: TestController) => {
    const primaryAreaContainer = Selector('main .align-items-center');
    const productTitle = primaryAreaContainer.find('.product-feature-title');
    await testController
        .expect(productTitle.getStyleProperty('color')).eql('rgb(255, 0, 0)')
});
