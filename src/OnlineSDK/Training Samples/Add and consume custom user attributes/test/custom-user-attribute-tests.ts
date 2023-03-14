declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=plp&mockUser=true&theme=fabrikam-extended';
fixture('Custom User Attribute').page(url).beforeEach(async testController => {
    await testController.maximizeWindow();
});

test(`Validate Custom User Attribute`, async testController => {
    const pageBreadCrumb = Selector('.ms-breadcrumb');
    await testController
    .expect(pageBreadCrumb.exists).ok('PLP page is not rendered')
    .hover(pageBreadCrumb, { speed: 0.4 });
});
