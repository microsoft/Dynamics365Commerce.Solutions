# PDP-PLP Variant Selection - Overview
The sample code provides code customizations required to achieve below features:
1.	Product List Page: Render color swatch for each product card that has variant products.
2.	Product List Page: an option to select a variant color, once user selects the color swatch in PLP and click on the product we capture that variant DIM ID as query string parameter and pass it to along the PDP URL.
3.	Product Description Page: Check for DIM ID in query string parameter if it exists, make that variant selected as part of the initial load and load associated variant product images.

## License
License is listed in the [LICENSE](https://github.com/microsoft/Dynamics365Commerce.Solutions/tree/release/9.29/src/CommerceOnboarding/Module%20Extensions/PDP%20PLP%20Variant%20Selection) file.

## Starter kit license
License for starter kit is listed in the [LICENSE](./starter-pack/LICENSE) file of starter pack.

## Prerequisites
SSK Module code need to be deployed and running.
Reference [link](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/setup-dev-environment)

# Sample code & documentation
Documentation on how to use/apply the samples can be found [here] (https://github.com/microsoft/Dynamics365Commerce.Solutions/tree/release/9.29/src/CommerceOnboarding/Module%20Extensions/PDP%20PLP%20Variant%20Selection)

# Note: Sample code is tested with below package version.
- Store starter kit: 10.0.10 (i.e. @msdyn365-commerce-modules/starter-pack": "9.20.27)
- Retail-Proxy: 10.0.10 (i.e. "@msdyn365-commerce/retail-proxy": "9.20.6")

# Additional References
- Extend a theme to add module extensions. [here](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/theme-module-extensions)
- Extend a theme from a base theme. [here](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/extend-theme)
- Clone a module library module. [here](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/clone-starter-module)
