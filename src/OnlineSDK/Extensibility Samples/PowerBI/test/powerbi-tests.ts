declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'https://localhost:4000/?mock=powerbi-test&theme=fabrikam-extended';
fixture('Default Page tests').page(url)

test('validate power bi test', async (testController: TestController) => {
    const pageDiv = Selector('.ms-carousel');
    await testController
        .expect(pageDiv.exists)
        .ok('div is not rendered on page')
        .hover(pageDiv, { speed: 0.4 })
});
