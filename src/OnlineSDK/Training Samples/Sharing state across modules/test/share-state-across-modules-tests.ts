declare var test: TestFn;
import { Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=share-state-across-modules';
fixture('Share state across modules tests')
    .page(url)
    .beforeEach(async testController => {
        await testController.maximizeWindow();
    });

test('validate share state across modules ExpandAll button', async (testController: TestController) => {
    const openAllButton = Selector('.ms-accordion_ExpandAll');
    await testController.expect(openAllButton.exists).eql(true, 'openAll button found');
    await testController
        .hover(openAllButton)
        .expect(openAllButton.visible)
        .ok()
        .click(Selector(openAllButton), { speed: 0.4 });
    const drawer = Selector('.ms-accordion-item-section .drawer__button.msc-btn.btn-block').withAttribute('aria-expanded', 'true');
    await testController
        .hover(drawer)
        .expect(drawer.exists)
        .ok();
});
test('validate share state across modules CollapseAll button', async (testController: TestController) => {
    const closeAllButton = Selector('.ms-accordion_CollapseAll');
    await testController.expect(closeAllButton.exists).eql(true, 'openAll button found');
    await testController
        .hover(closeAllButton)
        .expect(closeAllButton.visible)
        .ok()
        .click(Selector(closeAllButton), { speed: 0.4 });
    const drawer = Selector('.ms-accordion-item-section .drawer__button.msc-btn.btn-block').withAttribute('aria-expanded', 'false');
    await testController
        .hover(drawer)
        .expect(drawer.exists)
        .ok();
});
