# Overview

This folder contains the source code and configuration files of the sample of integration of Dynamics 365 Commerce with a fiscal service for Austria, the Czech Republic, and Germany. The sample extends the [fiscal integration functionality](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/fiscal-integration-for-retail-channel). It's based on the EFR (Electronic Fiscal Register) solution from [EFSTA](https://www.efsta.eu/) and enables communication with the EFR service via the HTTPS protocol. The EFR service should be hosted on either the Retail Hardware station or a separate computer that can be connected to from the Hardware station.

# Getting started

Please review the documentation for the fiscal service integration sample for:
- [Austria](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/emea-aut-fi-sample).
- [Czech Republic](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/emea-cze-fi-sample).
- [Germany](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/emea-deu-fi-sample).

# YAML and pipeline

The extension installers of this fiscal integration sample can be published via Azure pipelines using the [EFR build-pipeline.yml](../../../Pipeline/YAML_Files/EFR%20build-pipeline.yml) YAML file. For more information, please see [Key concepts for new Azure Pipelines users](https://docs.microsoft.com/azure/devops/pipelines/get-started/key-pipelines-concepts?view=azure-devops).