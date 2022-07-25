declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=pdp&theme=fabrikam-extended';
fixture('Show promo code on cart page tests').page(url).beforeEach(async testController => {
    await testController.maximizeWindow();
});;

const getWindowLocation = ClientFunction(() => window.location);

test('validate promo code on cart page', async (testController: TestController) => {
    const pdpVariant = Selector('.msc-swatch-container__item');
    await testController.expect(pdpVariant.exists).eql(true, 'variant found');
     await testController.hover(pdpVariant).expect(pdpVariant.visible).ok();
     await testController.hover(pdpVariant).expect(pdpVariant.visible).ok().click(
        Selector(pdpVariant),
        { speed: 0.4 }
    ).wait(5000);
    await testController.hover('.msc-add-to-cart', { speed: 0.4 });
    const addToCartButton = Selector('.msc-add-to-cart');
    await Selector(addToCartButton).with({ visibilityCheck: true })();
    await testController.hover(addToCartButton).expect(addToCartButton.visible).ok().click(
        Selector(addToCartButton),
        { speed: 0.4 }
    );

    await testController.navigateTo('https://localhost:4000/page/?mock=cart&theme=fabrikam-extended&promocode=WEEKLYAD');
    if(await Selector('.msc-promo-code__line-value').with({ visibilityCheck: true })()) {
        await testController.hover('.msc-promo-code__line-value', { speed: 0.4 })
    }
});

