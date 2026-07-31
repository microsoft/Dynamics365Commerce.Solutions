declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=Product1&theme=fabrikam-extended';
fixture('Default Page tests').page(url);

const getWindowLocation = ClientFunction(() => window.location);
const scroll = ClientFunction(function() {
    window.scrollBy(0,1500);
});
const setCookie = ClientFunction((value) => {
    document.cookie = value;
});
test('validate recently viewed products', async (testController: TestController) => {
    console.log('url: ', await getWindowLocation());
    await testController.wait(8000);
    const getCookie = ClientFunction(() => document.cookie);
    await testController.navigateTo('https://localhost:4000/page/?mock=product2&theme=fabrikam-extended');
    await testController.wait(8000);
    const cookieValue = "msdyn365___recently_viewed_products=68719497523";
    setCookie(cookieValue);
    await testController.navigateTo('https://localhost:4000/page/?mock=default-page&theme=fabrikam-extended');
    await testController.wait(13000);
    await scroll();
    await t.wait(3000);
});
