declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'https://localhost:4000/modules?type=product-feature&theme=summer';
fixture('Default Page tests').page(url)

test('validate renderPage div render', async (testController: TestController) => {
    const renderPageDiv = Selector('#renderPage');
    await testController.expect(renderPageDiv.exists).eql(true, 'Could not find default page container');
});

test('validate custom module title', async (testController: TestController) => {
    const pageDiv = Selector('.product-feature-title');
    await testController
        .expect(pageDiv.exists)
        .ok('custom module title is rendered correctly')
        .hover(pageDiv, { speed: 0.4 })
});

test('validate if price is rendered before product information', async (testController: TestController) => {
    const pageDiv = Selector('.container').child('p').nth(0);
    await testController
    .expect(pageDiv.innerText).eql('$129', 'price is not rendered before product information').hover(pageDiv, {speed: 0.4})
});

test('validate sub title rendered correctly', async (testController: TestController) => {
    const pageDiv = Selector('p').withText('This is a product sub title');
    await testController
        .expect(pageDiv.exists)
        .ok('sub title is rendered correctly')
        .hover(pageDiv, { speed: 0.4 })
});

test('validate the text color for extend theme from base theme', async (testController: TestController) => {
    const primaryAreaContainer = Selector('main .align-items-center');
    const productTitle = primaryAreaContainer.find('.product-feature-title');
    await testController
        .expect(productTitle.getStyleProperty('color')).eql('rgb(255, 0, 0)')
});