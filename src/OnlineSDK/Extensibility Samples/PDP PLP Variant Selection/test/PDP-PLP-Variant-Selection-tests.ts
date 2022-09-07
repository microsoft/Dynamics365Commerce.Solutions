declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=default-page&theme=fabrikam-extended';
fixture('validate search page').page(url).beforeEach(async testController => {
    await testController.maximizeWindow();
});
const getWindowLocation = ClientFunction(() => window.location);
test('validate PDP PLP varient selection', async (testController: TestController) => {
    console.log('url: ', await getWindowLocation());   
       const colorButton = Selector(".Color");
       await testController.click(colorButton);
       const refinerChoice = Selector('.ms-refine-submenu-item').find("[data-exp-event-id='" + "Category.custom-search-result-container.Brown.click" + "']");
       await testController.click(refinerChoice);
       await testController.navigateTo('https://localhost:4000/page/?mock=pdp&theme=fabrikam-extended&dimid=Brown');
       const renderProductBadge = Selector('.ms-search-result-container__Products .ms-product-search-result__item');
       renderProductBadge?.find('.custom-swatches').child(0);
});