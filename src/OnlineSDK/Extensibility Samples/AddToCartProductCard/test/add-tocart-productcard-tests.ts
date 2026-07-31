declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=search-result&theme=fabrikam-extended';
fixture('Show Add to cart button tests').page(url).beforeEach(async testController => {
    await testController.maximizeWindow();
});;

const getWindowLocation = ClientFunction(() => window.location);

test('validate category page', async (testController: TestController) => {
    const renderPageDiv = Selector('.ms-search-result-container');
    await testController.expect(renderPageDiv.exists).eql(true, 'category page open');
});

test('validate product Add to cart button visibilty', async (testController: TestController) => {
    const renderPageDiv = Selector('.ms-search-result-container__Products .ms-product-search-result__item');
    await testController.expect(renderPageDiv.exists).eql(true, 'product list found');
    const renderProduct = Selector('.ms-product-search-result__item').child(0);
    const addCartButton = Selector('.ms-search-result-container__Products .ms-product-search-result__item').child().find('.msc-add-to-cart');
    await testController.hover(renderProduct,{ speed: 0.4 }).expect(renderProduct.visible).ok();
     await testController.hover(addCartButton,{ speed: 0.4 }).expect(addCartButton.visible).ok();
     await testController.hover(addCartButton).expect(addCartButton.visible).ok().click(
        Selector(addCartButton),
        { speed: 0.4 }
    );
    await testController.hover('.msc-cart-icon', { speed: 0.4 });
});

