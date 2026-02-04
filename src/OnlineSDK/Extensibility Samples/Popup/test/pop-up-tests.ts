declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'https://localhost:4000/?mock=popup&theme=fabrikam-extended';
fixture('Validate Pop Up Module on page tests').page(url);

const getWindowLocation = ClientFunction(() => window.location);

test('validate renderPage div render', async (testController: TestController) => {
    console.log('url: ', await getWindowLocation());
    const renderPageDiv = Selector('#renderPage');
    await testController.expect(renderPageDiv.exists).eql(true, 'Could not find default page container');
});

test('validate subscribe module rendered inside pop up module on the page', async (testController: TestController) => {
    const subscribeButton = Selector('.ms-pop-up__modal-body .ms-subscribe__details__form__submit');
    await testController
            .expect(subscribeButton.exists) 
            .eql(true, 'Could not find Subscribe Sign Me Up button on Pop Up');
    await testController.hover(subscribeButton).expect(subscribeButton.visible).ok();
});

test('validate pop up title is rendered on the page', async (testController: TestController) => {
    const popupTitle = Selector('.ms-pop-up .ms-pop-up__modal-title');
    await testController
            .expect(popupTitle.exists) 
            .eql(true, 'Could not find pop up title on the page');
     await testController
            .expect(popupTitle.innerText) 
            .eql('Subscribe', 'Incorrect title rendered');
});
