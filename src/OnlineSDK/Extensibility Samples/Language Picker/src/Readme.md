# Dynamics 365 Commerce - online extensibility samples

## License
License is listed in the [LICENSE](./LICENSE) file.

# Sample - PLP Infinite scroll

## Overview
This sample will add an infinite scroll ability to the product category/search result page to replace the default paging.

![Overview](docs/image1.png)

## Starter kit license
License for starter kit is listed in the [LICENSE](./module-library/LICENSE) .

## Prerequisites
Follow the instructions mentioned in [document](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/setup-dev-environment) to set up the development environment.

### Procedure to create custom theme
Follow the instructions mentioned in [document](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/create-theme) to create the custom theme

Create a theme folder with name fabrikam-extended.

## Detailed Steps

### 1. Add custom-search-result-container Module
In this step we will add the new module like product collection module which will read the product ids from cookie and render the products as product collection on product description page.Below is the CLI for adding the new module is Custom-recently-viewed-products and follow below steps to modify the code.
**yarn msdyn365 add-module custom-recently-viewed-products**

