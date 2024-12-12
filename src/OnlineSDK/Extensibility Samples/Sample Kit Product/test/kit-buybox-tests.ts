declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=pdp-kit&theme=fabrikam-extended';
fixture('Sample Kit Product tests').page(url).beforeEach(async testController => {
    await testController.maximizeWindow();
});;

const getWindowLocation = ClientFunction(() => window.location);

test(`Validate PLP Kit Buybox Products Description`, async t => {
    if(await Selector('.ms-buybox__kit-product-description').with({ visibilityCheck: true })()) {
        await t.hover(Selector('.ms-buybox__kit-product-description'), { speed: 0.4 });
        await t.hover(Selector('ul li.ms-buybox__kit-product-description__item'), { speed: 0.4 });
    }
});
