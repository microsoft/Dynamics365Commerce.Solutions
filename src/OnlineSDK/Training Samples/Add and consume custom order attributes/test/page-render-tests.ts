declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=order-history&mockUser=true&theme=fabrikam-extended';
fixture('validate order history page').page(url).beforeEach(async testController => {
    await testController.maximizeWindow();
});

test('validate render order history', async (testController: TestController) => {
    const orderHistory = Selector('.ms-order-history');
    if(await Selector(orderHistory).with({ visibilityCheck: true })()){
    await testController
    .expect(orderHistory.exists)
    .ok('sales order attribute not found')
    .hover(orderHistory, { speed: 0.4 });
    }
});

test('validate render attribute value', async (testController: TestController) => {
    const attribute = Selector('.salesline_inventory');
    if(await Selector(attribute).with({ visibilityCheck: true })()){
    await testController
    .expect(attribute.exists)
    .ok('sales order attribute not found')
    .hover(attribute, { speed: 0.4 });
    }
});
