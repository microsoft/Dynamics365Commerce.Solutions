> [!WARNING]
> Because of limitations of the [new independent packaging and extension model](https://docs.microsoft.com/dynamics365/commerce/dev-itpro/build-pipeline), it can't currently be used for this fiscal integration sample. You must use the [previous version](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/emea-swe-fi-sample) of the fiscal device integration sample for Sweden that is available in the Retail SDK on a developer virtual machine (VM) in Microsoft Dynamics Lifecycle Services (LCS).

# Overview

This folder contains the source code and configuration files of the sample of integration of Dynamics 365 Commerce with a fiscal device (control unit) for Sweden. The sample extends the [fiscal integration functionality](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/fiscal-integration-for-retail-channel). It's assumed that a control unit is physically connected to a Hardware station that the POS is paired with. As an example, this sample uses the application programming interface (API) of the [CleanCash Type A](https://www.retailinnovation.se/produkter) control unit by Retail Innovation HTT AB. Version 1.1.4 of the CleanCash API is used.

# Getting started

Please review the [documentation](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/emea-swe-fi-sample) for the fiscal device integration sample for Sweden.

# YAML and pipeline

The extension installers of this fiscal integration sample can be published via Azure pipelines using the [CleanCash build-pipeline.yml](../../../Pipeline/YAML_Files/CleanCash%20build-pipeline.yml) YAML file. For more information, please see [Key concepts for new Azure Pipelines users](https://docs.microsoft.com/azure/devops/pipelines/get-started/key-pipelines-concepts?view=azure-devops).