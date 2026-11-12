# Using the sample in legacy SDK
If you want to use the POS fiscal connector in the legacy SDK, follow these steps.

1. Clean the solution if it was previously built:
    ```
    C:\Commerce-Samples-EndToEndSolutions\src\FiscalIntegration\PosFiscalConnectorSample> msbuild /t:Clean
    ```
1. Copy `Pos.Extension` folder to the POS extensions folder in the legacy SDK, e.g.:
    ```
    C:\RetailSDK\src\POS\Extensions
    ```

1. Rename the copied folder from `Pos.Extension` to `PosFiscalConnector`.
1. Remove the following folders and files from the `PosFiscalConnector` folder:
    * `bin`
    * `DataService`
    * `devDependencies`
    * `Libraries`
    * `obj`
    * `Contoso.PosFiscalConnectorSample.Pos.csproj`
    * `RetailServerEdmxModel.g.xml`
    * `tsconfig.json`
1. Open `CloudPos.sln` or `ModernPos.sln`.
    1. In `Pos.Extensions` project include `PosFiscalConnector` folder.
    1. Open `extensions.json` and add `PosFiscalConnector` extension.
1. Build the SDK.

For more information about POS samples, please refer to [Run the POS samples](https://docs.microsoft.com/en-us/dynamics365/commerce/dev-itpro/pos-run-samples).
