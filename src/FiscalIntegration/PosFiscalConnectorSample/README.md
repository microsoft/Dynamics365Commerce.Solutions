# Overview
This solution contains the POS fiscal connector sample for the [fiscal integration framework](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/fiscal-integration-for-retail-channel) which enables POS to directly communicate with a fiscal device or service without a hardware station. The sample currently supports the [EFSTA EFR](https://public.efsta.net/efr/) fiscal service.

# Using the sample
You can download the sample as zip and open it in Visual Studio (VS 2017). After opening in VS 2017, build the solution. After successful build, output installer packages will be created.

If you want to use the sample in the legacy SDK, please read [Using the sample in legacy SDK](./using-in-legacy-sdk.md).

# Configuration
## Configure Commerce channels
In order for POS to use the new fiscal connector, create a fiscal connector technical profile in the Commerce headquarters with the following settings:

- Connector type = Local.
- Connector location = Register.

Add it as a fiscal service to the required POS functionality profile.

For more details on how to configure fiscal integration, please refer to [Set up the fiscal integration for Commerce channels](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/setting-up-fiscal-integration-for-retail-channel).

## Configure HTTPS and TLS
In order to use POS fiscal connector, the fiscal device or service must be configured to use HTTPS.
* For information about how to set up HTTPS in EFR, please refer to 'EFR Reference' (http://public.efsta.net/efr/).

If extension to be used with Cloud POS and/or Store Commerce Android/iOS additional EFSTA service configuration is required to avoid blocking by CORS policy:
EFSTA service configuration -> Profile -> EFR Control -> Attributes:
HttpServer_AllowOrigin=https://scu****************-cpos.su.retail.dynamics.com

Please ensure that the client devices which run Cloud POS or Modern POS use TLS 1.2 and later. TLS 1.0 and 1.1 must be disabled.

# YAML and pipeline
The extension installers of this sample can be published via Azure pipelines using the [PosFiscalConnector build-pipeline.yml](../../../Pipeline/YAML_Files/PosFiscalConnector%20build-pipeline.yml) YAML file. For more information, please see [Key concepts for new Azure Pipelines users](https://docs.microsoft.com/azure/devops/pipelines/get-started/key-pipelines-concepts?view=azure-devops).

# License
Please review the license terms in the [license file](./slt-dynamics365commerce-posfiscalconnectorsample_en-us.md).
