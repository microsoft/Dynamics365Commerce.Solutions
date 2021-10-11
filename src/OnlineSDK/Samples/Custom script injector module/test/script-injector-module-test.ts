declare var test: TestFn;
import { ClientFunction, Selector, t } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = 'https://localhost:4000/modules?type=my-script-injector';
fixture('Script injector module test').page(url);

const getWindowLocation = ClientFunction(() => window.location);

test('validate script is included as part of head in the script injector module', async (testController: TestController) => {
    const scriptInjectorScriptTag = Selector('.data-ms-head');
    await testController.expect(scriptInjectorScriptTag.exists).eql(true, 'Could not find default page container');
    await testController.expect(scriptInjectorScriptTag.innerText).eql(`window.ga=window.ga||function(){(ga.q=ga.q||[]).push(arguments)};ga.l=+new Date;ga('create', 'UA-XXXXX-Y', 'auto');ga('send', 'pageview')`, 'Could not find default page container');
});