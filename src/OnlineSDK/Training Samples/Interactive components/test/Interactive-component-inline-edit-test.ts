import { Selector, test } from 'testcafe';

const url = 'https://localhost:4000/modules?type=product-feature&theme=spring&editorial=true';
fixture('authoring inline edit tests').page(url);

test('validate text inline edit functionality', async (testController: TestController) => {
    const primaryAreaContainer = Selector('main .primaryRegion');
    const headingTextElement = primaryAreaContainer.find('.ms-editable-field');

    await testController.expect(headingTextElement.getAttribute('contenteditable')).eql('true', 'Heading text field is not editable');
});