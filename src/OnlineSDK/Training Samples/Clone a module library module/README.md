# Dynamics 365 Commerce - online SDK samples
## License
License is listed in the [LICENSE](./LICENSE) file.

# Sample - Clone a module library module

## Overview
The Microsoft Dynamics 365 Commerce online software development kit (SDK) includes a set of module library modules that can be used on an e-Commerce site. Although these modules can't be modified directly, they can be cloned into new modules and then updated.

You can also create module extension views. In this way, you can provide alternative layout views without having to clone a module. We recommend that you avoid cloning if you can, because clones will be copies of module library modules and won't receive any automatic service updates that the module library modules get. For more information, see [Theming overview](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/theming).

## Doc links
* [Module overview](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/modules-overview)
* [Clone a module library module](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/clone-starter-module)

## Detailed Steps

### 1. Use CLI command to clone the content-block module library module

Use the CLI command: ```yarn msdyn365 clone content-block super-content-block``` to create a module called **super-content-block**. The new module will be created under the **\src\modules** directory.

### 2.  Build and test module

The module can now be built and tested in a web browser using the ```yarn start``` command.  You may see some linting errors appear with some of the module library modules, these can be fixed by running the ```yarn lint:fix``` command.  

Once the code finishes building and the Node server is running, you can see the new module **super-content-block** listed using the following URL **https://localhost:4000/_sdk/allmodules** or go directly to the new module using **https://localhost:4000/modules?type=super-content-block**.  You can also apply the module library **Fabrikam** theme by using the following URL: **https://localhost:4000/modules?type=super-content-block&theme=fabrikam**.

Any part of the cloned module code can now be modified as needed and will not affect the original module.  Since this module is now a copy of the original module, future updates to the original module library module will not be picked up by the cloned module and manual diff will be required to keep it consistent if desired.
