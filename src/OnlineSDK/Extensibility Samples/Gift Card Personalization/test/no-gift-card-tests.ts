declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=no-gift-card-pdp';

fixture('validate checkout page')
    .page(url)
    .beforeEach(async testController => {
        await testController.maximizeWindow();
    });

test('validate no gift card personalization if cart does not have gift card', async (testController: TestController) => {
    //add to cart
    const cookieAccept = Selector('.ms-cookie-compliance__accept-button');
    await testController.expect(cookieAccept.exists).eql(true, 'cookie accept button found');
    await testController
        .hover(cookieAccept)
        .expect(cookieAccept.visible)
        .ok()
        .click(Selector(cookieAccept), { speed: 0.8 })
        .wait(5000);

        const pdpPage = Selector('.ms-buybox__content');
        await testController.expect(pdpPage.exists).eql(true, 'pdp page found');
        const sizeSelector = Selector('.msc-swatch-container .msc-swatch-container__item-22565421234');
        await testController
            .hover(sizeSelector)
            .expect(sizeSelector.visible)
            .ok()
            .click(Selector(sizeSelector), { speed: 0.4 })
            .wait(5000);

    const colorSelector = Selector('.msc-swatch-container .msc-swatch-container__item-22565421190');
    await testController
        .hover(colorSelector)
        .expect(colorSelector.visible)
        .ok()
        .click(Selector(colorSelector), { speed: 0.4 })
        .wait(5000);

    const addToCartButton = Selector('.msc-add-to-cart');
    await Selector(addToCartButton).with({ visibilityCheck: true })();
    await testController
        .hover(addToCartButton)
        .expect(addToCartButton.visible)
        .ok()
        .click(Selector(addToCartButton), { speed: 0.4 })
        .wait(5000);
    //cart page
    await testController.navigateTo('http://localhost:4000/page?mock=cart');
    const cartPage = Selector('.msc-order-summary__checkout__action .msc-cart__btn-guestcheckout');
    await testController
        .hover(cartPage)
        .expect(cartPage.visible)
        .ok()
        .wait(5000);

    //checkout page
    await testController.navigateTo('http://localhost:4000/page/?mock=checkout&theme=summer');
    const personalizaGiftCard = Selector('.ms-giftCardPersonalization .hide-personalization');
    await testController
        .expect(personalizaGiftCard.exists)
        .ok('gift card personalization is hidden');
});
