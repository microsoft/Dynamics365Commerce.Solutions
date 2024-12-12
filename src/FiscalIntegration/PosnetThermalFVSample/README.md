# Overview

This folder contains the source code and configuration files of the sample of integration of Dynamics 365 Commerce with a fiscal printer for Poland. The sample extends the [fiscal integration functionality](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/fiscal-integration-for-retail-channel) and supports the POSNET THERMAL HD 2.02 protocol for fiscal printers from [Posnet Polska S.A.](https://www.posnet.com.pl/).
The sample enables communication with a fiscal printer that is connected via a COM port by using a native software driver.

# Getting started

Please review the [documentation](https://docs.microsoft.com/en-us/dynamics365/commerce/localizations/emea-pol-fpi-sample) for the fiscal printer integration sample for Poland.

# YAML and pipeline

The extension installers of this fiscal integration sample can be published via Azure pipelines using the [Posnet build-pipeline.yml](../../../Pipeline/YAML_Files/Posnet%20build-pipeline.yml) YAML file. For more information, please see [Key concepts for new Azure Pipelines users](https://docs.microsoft.com/azure/devops/pipelines/get-started/key-pipelines-concepts?view=azure-devops).

# Include external content/assembly for HWS extension

External content/assembly files (.lng, .dll etc) needs to be placed inside \src\FiscalIntegration\PosnetThermalFVSample\Dependencies folder, then build the solution to include those files in the extension installer