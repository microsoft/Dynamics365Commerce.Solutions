declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=buybox';
fixture('Default Page tests').page(url);

const getWindowLocation = ClientFunction(() => window.location);

test('validate renderPage div render', async (testController: TestController) => {
    const renderPageDiv = Selector('#renderPage');
    await testController.expect(renderPageDiv.exists).eql(true, 'Could not find default page container');
});

test('validate data action override to add root category of product in PDP breadcrumb', async (testController: TestController) => {
    const breadcrumbDiv = Selector('.ms-breadcrumb_list .ms-breadcrumb_item .ms-breadcrumb_link').child('span');
    await testController
        .expect(breadcrumbDiv.nth(-2).innerText)
        .contains('Custom path', 'Could not find default category')
        .hover(breadcrumbDiv.nth(-2), { speed: 0.4 });
});
