declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=default-page&theme=fabrikam-extended';
fixture('Default Page tests').page(url);

const getWindowLocation = ClientFunction(() => window.location);

test('validate renderPage div render', async (testController: TestController) => {
    console.log('url: ', await getWindowLocation());
    const renderPageDiv = Selector('#renderPage');
    await testController.expect(renderPageDiv.exists).eql(true, 'Could not find default page container');
});

test('validate title is rendered on the page', async (testController: TestController) => {
    const pageTitle = Selector('.ms-age-gate .ms-content-block__title');
    await testController
            .expect(pageTitle.exists) 
            .eql(true, 'Could not find title on the page');
     await testController
            .expect(pageTitle.innerText) 
            .eql('Age Verification', 'Incorrect title rendered');
});

test('validate pageText is rendered on the page', async (testController: TestController) => {
    const pageText = Selector('.ms-age-gate .ms-content-block__text');
    await testController
            .expect(pageText.exists) 
            .eql(true, 'Could not find page text on the page');
    await testController
            .expect(pageText.innerText) 
            .eql('Are you 21 years of Age or older?', 'Incorrect page text rendered on the page');
});