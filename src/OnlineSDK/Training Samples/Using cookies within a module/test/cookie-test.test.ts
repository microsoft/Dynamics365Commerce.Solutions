declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'https://localhost:4000/page?mock=cookie-page';
fixture('Use cookie within a module test').page(url).beforeEach(async testController => {
    await testController.maximizeWindow();
});

const getWindowLocation = ClientFunction(() => window.location);

test('validate set and get cookie test', async (testController: TestController) => {
    const acceptCookieConsentBtn = Selector('.ms-cookie-compliance__accept-button');
    await testController.hover(acceptCookieConsentBtn).expect(acceptCookieConsentBtn.visible).ok().click(
        Selector(acceptCookieConsentBtn),
        { speed: 0.4 }
    );
    const inputBox = Selector('input').withAttribute('placeholder', 'Enter your favorite color');
    await testController.click(inputBox).typeText(inputBox, 'red', {speed: 0.4});
    const setCookieButton = Selector('button').withText('Set cookie');
    await testController.hover(setCookieButton).expect(setCookieButton.visible).ok().setNativeDialogHandler(() => true).click(
       Selector(setCookieButton),
       { speed: 0.4 }
   );

   const setCookieDialogBoxHistory = await t.getNativeDialogHistory();
   await t
        .expect(setCookieDialogBoxHistory[0].type).eql('alert')
        .expect(setCookieDialogBoxHistory[0].text).eql('set cookie to red');

    const getCookieButton = Selector('button').withText('Get cookie');
    await testController.hover(getCookieButton).expect(getCookieButton.visible).ok().setNativeDialogHandler(() => true).click(
        Selector(getCookieButton),
        { speed: 0.4 }
    );

    const getCookieDialogBoxHistory = await t.getNativeDialogHistory();
    await t
        .expect(getCookieDialogBoxHistory[0].type).eql('alert')
        .expect(getCookieDialogBoxHistory[0].text).eql('favColor = red');
});