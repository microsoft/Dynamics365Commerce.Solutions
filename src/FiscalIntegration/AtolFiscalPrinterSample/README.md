# Overview

This folder contains the source code and configuration files of the sample of integration of Dynamics 365 Commerce with a fiscal printer (online cash register) for Russia. The sample extends the [fiscal integration functionality](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/fiscal-integration-for-retail-channel) and supports the application programming interface (API) of fiscal printers from [ATOL](http://integration.atol.ru/). It enables communication with a fiscal printer that is connected via a communication (COM) port by using a native software driver.

# License

Please review the license terms in the [slt-dynamics365commerce-atolsample_en-us.md](./slt-dynamics365commerce-atolsample_en-us.md) file.

# Getting started

Please review the [documentation](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/rus-fpi-sample) for the fiscal printer integration sample for Russia.

# YAML and pipeline

> [!WARNING]
> Because of limitations of the [new independent packaging and extension model](https://docs.microsoft.com/dynamics365/commerce/dev-itpro/build-pipeline), it can't currently be used for this fiscal integration sample. You must use the previous version of the Retail SDK on a developer virtual machine (VM) in Microsoft Dynamics Lifecycle Services (LCS). For more information, see the fiscal integration sample [documentation](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/rus-fpi-sample#configure-channel-components).

The extension installers of this fiscal integration sample can be published via Azure pipelines using the [Atol build-pipeline.yml](../../../Pipeline/YAML_Files/Atol%20build-pipeline.yml) YAML file. For more information, please see [Key concepts for new Azure Pipelines users](https://docs.microsoft.com/azure/devops/pipelines/get-started/key-pipelines-concepts?view=azure-devops).