declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=default-page';
fixture('Default Page tests').page(url);

const getWindowLocation = ClientFunction(() => window.location);

test('validate renderPage div render', async (testController: TestController) => {
    console.log('url: ', await getWindowLocation());
    const renderPageDiv = Selector('#renderPage');
    await testController.expect(renderPageDiv.exists).eql(true, 'Could not find default page container');
});

test('validate title is rendered on the page', async (testController: TestController) => {
    const pageTitle = Selector('main .ms-content-block__title');
    await testController
            .expect(pageTitle.exists) 
            .eql(true, 'Could not find title on the page');
     await testController
            .expect(pageTitle.innerText) 
            .eql('Heading', 'Incorrect title rendered');
});

test('validate pageText is rendered on the page', async (testController: TestController) => {
    const pageText = Selector('main .ms-content-block__text');
    await testController
            .expect(pageText.exists) 
            .eql(true, 'Could not find page text on the page');
    await testController
            .expect(pageText.innerText) 
            .eql('Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.', 'Incorrect page text rendered on the page');
});