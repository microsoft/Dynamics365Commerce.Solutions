# Promo code Affiliation- Overview
The Promo Code component is currently available in the cart module. User can apply any valid promo code and get the benefits. When user apply the promo code, an api call happens which first verify the validity of promo code in terms of expiry, category and delicacy. If it’s a valid promo code, it will be applied on cart.

Promo-code affiliation extends the above default behaviour, that performs below functions:
- End user can land to website with promo code query param.
- Emprt Cart: Apply the promo code from URL to the empty cart.
- Cart with products: Apply promo-code automatically on the cart page.

## License
License is listed in the [LICENSE](https://github.com/microsoft/Dynamics365Commerce.Solutions/tree/release/9.29/src/CommerceOnboarding/Module%20Extensions/Promo%20Code%20Affiliation) file.

## Starter kit license
License for starter kit is listed in the [LICENSE](./starter-pack/LICENSE) file of starter pack.

## Prerequisites
SSK Module code need to be deployed and running.
Reference [link](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/setup-dev-environment)

# Promo code Affiliation - Sample code & documentation
Documentation on how to use/apply the samples can be found [here] (https://github.com/microsoft/Dynamics365Commerce.Solutions/tree/release/9.29/src/CommerceOnboarding/Module%20Extensions/Promo%20Code%20Affiliation)

# Note: Sample code is tested with below package version.
- Store starter kit: 10.0.16 (i.e. @msdyn365-commerce-modules/starter-pack": "~9.26.0)
- Retail-Proxy: 10.0.16 (i.e. "@msdyn365-commerce/retail-proxy": "~9.26.0")

# Additional References
- Extend a theme to add module extensions. [here](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/theme-module-extensions)
- Extend a theme from a base theme. [here](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/extend-theme)
- Clone a module library module. [here](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/clone-starter-module)
