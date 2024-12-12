declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}
const setCookie = ClientFunction(value => {
    document.cookie = value;
});

const url = 'http://localhost:4000/page?mock=prod1';
fixture('override hydrator actions tests').page(url);
const scroll = ClientFunction(function() {
    window.scrollBy(0, 1500);
});
const getWindowLocation = ClientFunction(() => window.location);

test('validate hydrated data actions override to product names', async (testController: TestController) => {
    const acceptCookieConsentBtn = Selector('.ms-cookie-compliance__accept-button');
    await testController
        .hover(acceptCookieConsentBtn)
        .expect(acceptCookieConsentBtn.visible)
        .ok()
        .click(Selector(acceptCookieConsentBtn), { speed: 0.4 });
    await testController.wait(8000);
    const getCookie = ClientFunction(() => document.cookie);
    await testController.navigateTo('https://localhost:4000/page/?mock=prod2');
    await testController.wait(8000);
    const cookieValue = 'msdyn365___recently_viewed_products=68719493168';
    setCookie(cookieValue);
    await testController.navigateTo('https://localhost:4000/page/?mock=override-hydrator-actions');
    await testController.wait(13000);
    await scroll();
    await t.wait(3000);

    const categoryProdCollection = Selector('.col-12 .ms-product-collection');
    await testController
        .expect(categoryProdCollection.nth(0).find('.ms-product-collection__item .msc-product__title').innerText)
        .contains('Category', 'Could not find location');
    await testController
        .expect(categoryProdCollection.nth(1).find('.ms-product-collection__item .msc-product__title').innerText)
        .contains('Related', 'Could not find location')
        .wait(5000);
    // setCookie(cookieValue);
    await testController
        .expect(categoryProdCollection.nth(2).find('.ms-product-collection__item .msc-product__title').innerText)
        .contains('Recent', 'Could not find location');
});
