declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'https://localhost:4000/page?mock=local-theme-page&theme=summer';
fixture('Default Page tests').page(url);

test('validate PDP render page', async (testController: TestController) => {
    const renderPageDiv = Selector('#renderPage');
    await testController.expect(renderPageDiv.exists).eql(true, 'Could not find default page container');
});

test('validate resource override for local summer theme AddToCart button text', async (testController: TestController) => {
    const addToCartButton = Selector('.msc-add-to-cart ');
    await testController
        .expect(addToCartButton.innerText)
        .eql('Local theme overriden resource', 'AddToCart button is not rendering on the page')
        .hover(addToCartButton, { speed: 0.4 });
});
