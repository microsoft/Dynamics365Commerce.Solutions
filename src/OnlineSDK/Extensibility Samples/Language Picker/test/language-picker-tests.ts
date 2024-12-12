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

test('validate language picker', async (testController: TestController) => {
    console.log('url: ', await getWindowLocation());
    const languagePickerDiv = Selector('.ms-language-picker');
    await Selector(languagePickerDiv).with({ visibilityCheck: true })();
    await testController.hover(languagePickerDiv).expect(languagePickerDiv.visible).ok();
    await testController.hover(languagePickerDiv,{ speed: 0.4 })
});
