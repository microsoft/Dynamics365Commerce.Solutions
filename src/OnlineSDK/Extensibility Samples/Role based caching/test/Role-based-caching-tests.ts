declare var test: TestFn;
import { Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=default-page';
fixture('Role based caching tests')
    .page(url)
    .beforeEach(async testController => {
        await testController.maximizeWindow();
    });

test('validate category page', async (testController: TestController) => {
    const renderPageDiv = Selector('.ms-search-result-container');
    await testController.expect(renderPageDiv.exists).eql(true, 'category page open');
});

test('validate category page for cutomer type anonymous', async (testController: TestController) => {
    const refinerChoice = Selector('.ms-choice-summary-by-category');
    const refinerChoice1 = refinerChoice.withAttribute('id', 'Brown_Color_0');
    await testController
        .hover(refinerChoice, { speed: 0.4 })
        .expect(refinerChoice.visible)
        .ok();
    await testController.expect(refinerChoice.innerText).eql('Brown', 'Could not find refiner type');
});
