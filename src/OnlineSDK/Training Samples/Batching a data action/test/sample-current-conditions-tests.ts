declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'https://localhost:4000/modules?type=sample-current-conditions&theme=fall';
fixture('Default Page tests').page(url);

const getWindowLocation = ClientFunction(() => window.location);

test('validate renderPage div render', async (testController: TestController) => {
    const renderPageDiv = Selector('#renderPage');
    await testController.expect(renderPageDiv.exists).eql(true, 'Could not find default page container');
});

test('validate weather information', async (testController: TestController) => {
    const whetherHeaderInfoDiv = Selector('.sample-current-conditions .card-header');
    await testController.wait(5000);
    await testController.expect(whetherHeaderInfoDiv.exists).eql(true, 'Could not find sample current conditions container');
    await testController.expect(whetherHeaderInfoDiv.innerText).contains('Current conditions at your favorite locations', 'Could not find header container text');

    const whetherBodyInfoDiv = Selector('.sample-current-conditions .card-body .justify-content-center').child();
    await testController.expect(whetherBodyInfoDiv.nth(0).find('.card-header').innerText).contains('London', 'Could not find location');
    await testController.expect(whetherBodyInfoDiv.nth(1).find('.card-header').innerText).contains('Honolulu', 'Could not find location');
    await testController.expect(whetherBodyInfoDiv.nth(2).find('.card-header').innerText).contains('Sydney', 'Could not find location');
    await testController.expect(whetherBodyInfoDiv.nth(3).find('.card-header').innerText).contains('Seattle', 'Could not find location');

    const whetherImageInfoDiv = Selector('.sample-current-conditions .card-body .justify-content-center').child().find('img');
    await testController.expect(whetherImageInfoDiv.exists).eql(true, 'Could not find whether info image');

    const whetherInfoDiv = Selector('.sample-current-conditions .card-body .justify-content-center').child().find('.card-title');
    await testController.expect(whetherInfoDiv.exists).eql(true, 'Could not find whether info');

    const whetherTempInfoDiv = Selector('.sample-current-conditions .card-body .justify-content-center').child().find('.card-text');
    await testController.expect(whetherTempInfoDiv.exists).eql(true, 'Could not find whether temperature info');
});