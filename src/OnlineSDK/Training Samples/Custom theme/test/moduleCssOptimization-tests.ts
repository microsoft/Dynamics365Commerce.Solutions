declare var test: TestFn;
import { Selector, t, RequestLogger, ClientFunction } from 'testcafe';

// Fail if there is any error on client
export default async function checkErrors(): Promise<void> {
    const { error } = await t.getBrowserConsoleMessages();
    await t.expect(error[0]).notOk();
}

const url = `https://localhost:4000/page?mock=optimized`;
const preloadScriptUrl = `https://localhost:4000/page?mock=optimized&preloadscripts=true`;

const FontFilelogger = RequestLogger(/msdyn365-assets/, {
    logRequestHeaders: true,
    logRequestBody: true,
    logResponseHeaders: true,
    logResponseBody: true
});

const staticCssLogger = RequestLogger(/module-css-styles/, {
    logRequestHeaders: true,
    logRequestBody: true,
    logResponseHeaders: true,
    logResponseBody: true
});

const getModuleList = ClientFunction(() => {
    const data = window[`___initialData___`][`_moduleList`];
    return data;
});

fixture('Validate Inline Module css chunks for ModuleCssOptimization feature')
    .page(url)
    .requestHooks(FontFilelogger);

test('validate renderPage div render', async (testController: TestController) => {
    const renderPageDiv = Selector('#renderPage');
    await testController.expect(renderPageDiv.exists).eql(true, 'Could not find default page container');
});

test('validate correct inline module css chunks are injected', async (testController: TestController) => {
    const styleTagCount = await Selector('style').count;
    await testController.expect(styleTagCount).notEql(0, 'No style tag found on page');

    for (let i = 0; i < styleTagCount; i++) {
        const styleCss = await Selector('style').nth(i).innerText;
        const assetUrlRelativePathRegex = new RegExp(/(?<=[:|,|]url\()([.|\/].*?)(?=msdyn365-assets)/, 'gm');
        const relativePathInModuleCss = styleCss.match(assetUrlRelativePathRegex);
        const getUrlsRegex = new RegExp(/(?<=url\()(.*?)(?=\))/, 'gm');
        var urlAssets = styleCss.match(getUrlsRegex);

        // Inline css should not contain {msdyn365CommerceCdnUrl} mock variable
        await testController
            .expect(styleCss)
            .notContains(
                '{msdyn365CommerceCdnUrl}',
                'Inline css contains mock cdn variable `{msdyn365CommerceCdnUrl}`. Incorrect regex replace, please check regex when replacing relative path from module css during server start up.'
            );

        // Inline css should not contain relative path
        await testController.expect(relativePathInModuleCss).eql(null, 'Inline css contains relative path in Url');

        if (urlAssets) {
            for (let i = 0; i < urlAssets.length; i++) {
                if (urlAssets[i].includes('msdyn365-assets')) {
                    // Inline css should alwyas contain cdn url
                    await testController
                        .expect(urlAssets[i])
                        .contains('_scnr/00000-00000-00000-00000-00000/msdyn365-assets/', 'Inline css contains relative path in Url');
                }
            }
        }
    }

    // Test to check if font files assets are successfully downloaded
    FontFilelogger?.requests.map(async r => {
        if (r.request.url.includes('/_scnr/00000-00000-00000-00000-00000/msdyn365-assets/')) {
            await testController.expect(r.response.statusCode).eql(200, 'Font file download for module css failed');
        }
    });
});

fixture('Validate static module css chunks for ModuleCssOptimization feature')
    .page(preloadScriptUrl)
    .requestHooks(staticCssLogger);

test('validate renderPage div render', async (testController: TestController) => {
    const renderPageDiv = Selector('#renderPage');
    await testController.expect(renderPageDiv.exists).eql(true, 'Could not find default page container');
});

test('validate static module css chunks are injected on the page', async (testController: TestController) => {
    const htmlLinkTagsCount = await Selector('link').count;
    const moduleListClient = await getModuleList();
    const moduleListforCss = new Set();
    const staticCsspath = '_scnr/00000-00000-00000-00000-00000/static/css/module-css-styles';
    const staticModuleCssLinkList = new Set();

    moduleListClient?.forEach(module => {
        const moduleName = module?.typeName || '';
        if (moduleName !== '') {
            moduleListforCss.add(moduleName);
        }
    });

    for (let i = 0; i < htmlLinkTagsCount; i++) {
        const linkTag = await Selector('link').nth(i);
        const href = await linkTag.getAttribute('href');
        if (href.includes(staticCsspath)) {
            staticModuleCssLinkList.add(staticCsspath);
        }
    }

    // Test to check in preloadScript scenario, we inject module static CSS on the page
    await testController
        .expect(staticModuleCssLinkList.size)
        .notEql(0, 'Page does not contain any module static CSS chunks on the page in service worker/preload script scenraio');

    // Test to check in preloadScript scenario, static css chunks are loaded
    staticCssLogger?.requests.map(async r => {
        if (r.request.url.includes(staticCsspath)) {
            await testController.expect(r.response.statusCode).eql(200, 'Static CSS chunk download for module css failed.');
        }
    });
});

fixture('Validate font files are downloading sucessfully in preload script scenario for ModuleCssOptimization feature')
    .page(preloadScriptUrl)
    .requestHooks(FontFilelogger);

test('Validate font files are downloading sucessfully when preload script/service worker', async (testController: TestController) => {
    // Test to check if font files assets are successfully downloaded
    FontFilelogger?.requests.map(async r => {
        if (r.request.url.includes('/_scnr/00000-00000-00000-00000-00000/msdyn365-assets/')) {
            await testController.expect(r.response.statusCode).eql(200, 'Font file download for module css failed');
        }
    });
});
