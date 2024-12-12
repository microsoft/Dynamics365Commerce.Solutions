declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'https://localhost:4000/page?mock=default-page&theme=fabrikam-extended';
fixture('Default Page tests').page(url);

const getWindowLocation = ClientFunction(() => window.location);
const scroll = ClientFunction(function() {
    window.scrollBy(0, 2000);
});

test(`Validate PLP infinite scroll`, async t => {
    await scroll();
    if(await Selector('.ms-back-to-top').with({ visibilityCheck: true })()) {
        await t.hover(Selector('.ms-back-to-top'));
        await t.click(Selector('.ms-back-to-top'));
    }
});


