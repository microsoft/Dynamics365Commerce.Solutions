# Pop Up - Overview
This is a generic sample code for implementing a pop-up module. Pop-up module can be added anywhere in the page, and it will create a modal dialog and attach it to the DOM. Other modules could be exposed inside this module to make them appear inside the pop-up. It can be set to pop up at the page load or anytime during the session. It can be associated with a cookie to determine whether certain action has taken place.

Pop Up module can be integrated for the following scenario:
1. Show pop up in the initial page load every time the page loads.
2. Show pop up in the initial page load only if the associated cookie is absent or it's value is falsy.
3. Show pop up during the session based on any user action.

Please refer documentation for setting up the appropriate scenario and other CMS (i.e Content Management System) configurations.

 ## License
License is listed in the [LICENSE](https://github.com/microsoft/Dynamics365Commerce.Solutions/tree/release/9.29/src/CommerceOnboarding/Module%20Extensions/Pop%20Up) file.

## Starter kit license
License for starter kit is listed in the [LICENSE](./starter-pack/LICENSE) file of starter pack.

## Prerequisites
SSK Module code need to be deployed and running.
Reference [link](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/setup-dev-environment)

# Pop Up - Sample code & documentation
Documentation on how to use/apply the samples can be found [here](https://github.com/microsoft/Dynamics365Commerce.Solutions/tree/release/9.29/src/CommerceOnboarding/Module%20Extensions/Pop%20Up)

# Note: Sample code is tested with below package version.
- Store starter kit: "@msdyn365-commerce-modules/starter-pack": "9.32.15-preview.0
- Retail-Proxy: "@msdyn365-commerce/retail-proxy": "9.32.4"
- SDK version: 1.33.13

# Additional References
- Extend a theme to add module extensions. [here](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/theme-module-extensions)
- Extend a theme from a base theme. [here](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/extend-theme)
- Clone a module library module. [here](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/clone-starter-module)

