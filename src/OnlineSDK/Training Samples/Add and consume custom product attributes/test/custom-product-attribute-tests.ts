declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=pdp&theme=fabrikam-extended';
fixture('Custom Product Attribute').page(url).beforeEach(async testController => {
    await testController.maximizeWindow();
});;

test(`Validate Custom Product Attribute Text Value`, async t => {
    if(await Selector('.msc-product-custom-attribute').with({ visibilityCheck: true })()) {
        await t.hover(Selector('.msc-product-custom-attribute'), { speed: 0.4 });
    }
});
