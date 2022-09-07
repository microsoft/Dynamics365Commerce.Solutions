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

const getWindowLocation = ClientFunction(() => window.location);

test('validate billing address form', async (testController: TestController) => {
    //add to cart
    const cookieAccept = Selector('.ms-cookie-compliance__accept-button');
    await testController.expect(cookieAccept.exists).eql(true, 'cookie accept button found');
    await testController
        .hover(cookieAccept)
        .expect(cookieAccept.visible)
        .ok()
        .click(Selector(cookieAccept), { speed: 0.4 });

    const stayOnthisBtn = Selector('.ms-country-picker__current-site');
    await testController.expect(stayOnthisBtn.exists).eql(true, 'Stay on this button found');
    await testController
        .hover(stayOnthisBtn)
        .expect(stayOnthisBtn.visible)
        .ok()
        .click(Selector(stayOnthisBtn), { speed: 0.4 });

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

    //checkout page
    await testController.navigateTo('http://localhost:4000/page/?mock=checkout&theme=fabrikam-extended');

    //shipping form
    const checkoutSelectedSection = '.ms-checkout__guided-form';

    const shipping_addressstate = Selector('#shipping_addressstate');
    const dropdownStateOption = shipping_addressstate.find('option');

    const saveButton = Selector('.ms-checkout__guided-card-btn-save');

    await testController
        .hover(Selector('#shipping_addressname'), { speed: 0.4 })
        .typeText(Selector('#shipping_addressname'), 'testName')
        .hover(Selector('#shipping_addressstreet'))
        .typeText(Selector('#shipping_addressstreet'), 'testStreet')
        .hover(Selector('#shipping_addresscity'))
        .typeText(Selector('#shipping_addresscity'), 'testCity')
        .click(Selector(shipping_addressstate), { speed: 0.4 })
        .click(dropdownStateOption.nth(1), { speed: 0.4 })
        .hover(Selector('#shipping_addresszipcode'))
        .typeText(Selector('#shipping_addresszipcode'), '12345');

    await Selector(checkoutSelectedSection)
        .child(1)
        .find('.ms-checkout__guided-card-btn-save')
        .with({ visibilityCheck: true })();

    await testController
        .hover(
            Selector(checkoutSelectedSection)
                .child(1)
                .find('.ms-checkout__guided-card-btn-save')
        )
        .click(
            Selector(checkoutSelectedSection)
                .child(1)
                .find('.ms-checkout__guided-card-btn-save')
        );

    //Billing form
    const sameAsShippingChk = '.ms-checkout-billing-address__input-checkbox';
    const billing_addressname = '#billing_addressaddresstypevalue';

    await testController.click(Selector(sameAsShippingChk)).hover(Selector(billing_addressname));
});
