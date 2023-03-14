declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=pdp&theme=fabrikam-extended';

fixture('validate checkout page')
    .page(url)
    .beforeEach(async testController => {
        await testController.maximizeWindow();
    });

test('validate delivery notes', async (testController: TestController) => {
    //add to cart
    const cookieAccept = Selector('.ms-cookie-compliance__accept-button');
    await testController.expect(cookieAccept.exists).eql(true, 'cookie accept button found');
    await testController
        .hover(cookieAccept)
        .expect(cookieAccept.visible)
        .ok()
        .click(Selector(cookieAccept), { speed: 0.4 });

    const pdpPage = Selector('.ms-buybox__content');
    await testController.expect(pdpPage.exists).eql(true, 'pdp page found');
    const sizeSelector = Selector('.msc-swatch-container .msc-swatch-container__item-22565421956');
    await testController
        .hover(sizeSelector)
        .expect(sizeSelector.visible)
        .ok()
        .click(Selector(sizeSelector), { speed: 0.4 })
        .wait(5000);

    await testController.hover('.msc-add-to-cart', { speed: 0.4 });
    const addToCartButton = Selector('.msc-add-to-cart');
    await Selector(addToCartButton).with({ visibilityCheck: true })();
    await testController
        .hover(addToCartButton)
        .expect(addToCartButton.visible)
        .ok()
        .click(Selector(addToCartButton), { speed: 0.4 });

    //cart page
    await testController.navigateTo('http://localhost:4000/page/?mock=cart&theme=fabrikam-extended');
    const cartPage = Selector('.msc-cart__btn-guestcheckout');
    await testController
        .hover(cartPage)
        .expect(cartPage.visible)
        .ok()

    //checkout page
    await testController.navigateTo('http://localhost:4000/page/?mock=checkout&theme=fabrikam-extended');
    const deliveryNotesInput = Selector('.deliveryNotes-textarea');
    await testController
        .expect(deliveryNotesInput.exists)
        .ok('delivery notes not found')
        .hover(deliveryNotesInput, { speed: 0.4 });
});
