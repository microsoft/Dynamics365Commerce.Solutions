# Overview

This folder contains the source code and configuration files of the sample of integration of Dynamics 365 Commerce with a fiscal printer for Italy. The sample extends the [fiscal integration functionality](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/fiscal-integration-for-retail-channel) so that it works with [Epson FP-90III Series](https://www.epson.it/products/sd/pos-printer/epson-fp-90iii-series) printers from Epson, and it enables communication with a fiscal printer in the web server mode via the EpsonFPMate web-service using Fiscal ePOS-Print API.

# Getting started

Please review the [documentation](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/emea-ita-fpi-sample) for the fiscal printer integration sample for Italy.

# YAML and pipeline

The extension installers of this fiscal integration sample can be published via Azure pipelines using the [EpsonFP90III build-pipeline.yml](../../../Pipeline/YAML_Files/EpsonFP90III%20build-pipeline.yml) YAML file. For more information, please see [Key concepts for new Azure Pipelines users](https://docs.microsoft.com/azure/devops/pipelines/get-started/key-pipelines-concepts?view=azure-devops).