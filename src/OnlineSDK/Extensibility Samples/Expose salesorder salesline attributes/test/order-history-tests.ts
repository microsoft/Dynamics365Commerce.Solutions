declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'http://localhost:4000/page?mock=order-history&mockUser=true&theme=summer';
fixture('validate order history page').page(url).beforeEach(async testController => {
    await testController.maximizeWindow();
});

test('validate render inventoryId', async (testController: TestController) => {
    const inventoryId = Selector('.salesline_inventory');
    if(await Selector(inventoryId).with({ visibilityCheck: true })()){
    await testController
    .expect(inventoryId.exists)
    .ok('sales order attribute not found')
    .hover(inventoryId, { speed: 0.4 });
    }
});

test('validate render salesTax groupId', async (testController: TestController) => {
    const grpId = Selector('.salesline_groupId');
    if(await Selector(grpId).with({ visibilityCheck: true })()){
    await testController
    .expect(grpId.exists)
    .ok('sales order attribute not found')
    .hover(grpId, { speed: 0.4 });
    }
});