declare var test: TestFn;

import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=hide-rating-true';
fixture('Hide rating true test')
    .page(url)
    .beforeEach(async testController => {
        await testController.maximizeWindow();
    });

const getWindowLocation = ClientFunction(() => window.location);

// Rating should not render due to hideRating being set to true
test('validate rating is not rendered', async (testController: TestController) => {
    const rating = Selector('main .msc-rating__range');
    await testController.expect(rating.exists).eql(false, 'hideRating is enabled and rating should not render');
});
