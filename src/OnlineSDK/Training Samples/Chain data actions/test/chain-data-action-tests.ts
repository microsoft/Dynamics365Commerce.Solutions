declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'https://localhost:4000/page?mock=chain-data';
fixture('Default Page tests').page(url);

test('validate PDP render page', async (testController: TestController) => {
    const renderPageDiv = Selector('#renderPage');
    await testController.expect(renderPageDiv.exists).eql(true, 'Could not find default page container');
});

test('validate chained basePriceValue', async (testController: TestController) => {
    const basePriceValue = Selector('.chain_basePrice');
    await testController
        .expect(basePriceValue.exists)
        .ok('Chained basePriceValue rendered')
        .hover(basePriceValue, { speed: 0.4 });
});

test('validate chained variantPriceValue', async (testController: TestController) => {
    const chainedVariantPrice = Selector('.chain_variantPrice');
    await testController
        .expect(chainedVariantPrice.exists)
        .ok('Chained chainedVariantPrice rendered')
        .hover(chainedVariantPrice, { speed: 0.4 });
});
