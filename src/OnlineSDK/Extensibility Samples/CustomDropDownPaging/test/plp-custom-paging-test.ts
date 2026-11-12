declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=PLP&theme=fabrikam-extended';
fixture('validate plp page').page(url).beforeEach(async testController => {
    await testController.maximizeWindow();
});

test('validate custom dropdown paging', async (testController: TestController) => {
    const categoryDropdown = Selector('#categoryPageSizeFilterByDropdown');
    await testController
    .expect(categoryDropdown.exists)
    .ok('paging dropdown not found')
    .hover(categoryDropdown, { speed: 0.4 });
});