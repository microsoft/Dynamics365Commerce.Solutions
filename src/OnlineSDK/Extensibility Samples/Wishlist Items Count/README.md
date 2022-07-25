# Dynamics 365 Commerce - online extensibility samples

## License
License is listed in the [LICENSE](./LICENSE) file.

# Sample - Wishlist Items Count

## Overview
In this sample, you will learn how to implement and display a wishlist items count on the wishlist icon for a logged on e-commerce customer.

![Overview](docs/Image1.png)

## Starter kit license
License for starter kit is listed in the [LICENSE](./module-library/LICENSE) .

## Prerequisites
Follow the instructions mentioned in [document](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/setup-dev-environment) to set up the development environment.

### Procedure to create custom theme
Follow the instructions mentioned in [document](https://docs.microsoft.com/en-us/dynamics365/commerce/e-commerce-extensibility/create-theme) to create the custom theme,in this sample, we'll assume a custom theme has been cloned from the fabrikam theme named "fabrikam-extended".

## Detailed Steps to implement Wishlist items count 

### 1. Extend definition file for header
Create a new file named **header.definition.ext.json** under **src\themes\fabrikam-extended\definition-extensions** folder and paste the code below in it

```typescript
{
    "$type": "definitionExtension",
    "dataActions": {
        "wishlists": {
            "path": "@msdyn365-commerce-modules/retail-actions/dist/lib/get-wishlist-by-customer-id",
            "runOn": "server"
        }
    }
}
```

### 2. Create Wishlist Icon Component
Override wishlisticon.component.tsx component using this command **yarn msdyn365 add-component-override fabrikam-extended wishlisticon**.Open the new wishlisticon.component.tsx under **src/themes/fabrikam-extended/views/components** and replace existing code with below code to display Wishlist count button with text.

```typescript
/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { getUrlSync, IComponent, IComponentProps, msdyn365Commerce } from '@msdyn365-commerce/core';
import { Button, getPayloadObject, getTelemetryAttributes,
    ITelemetryContent, onTelemetryClick, UncontrolledTooltip } from '@msdyn365-commerce-modules/utilities';
import classname from 'classnames';
import * as React from 'react';

export interface IWishlistIconComponentProps extends IComponentProps<{}> {
    className?: string;
    wishlistTooltipText: string;
    showButtonTooltip?: boolean;
    telemetryContent?: ITelemetryContent;
    wishlistCountLabel?: string;
    wishlistCount?: number;
    isDispayWishlistCount?: boolean;
}

export interface IWishlistIconComponent extends IComponent<IWishlistIconComponentProps> {
}

const WishlistIconComponentActions = {};

/**
 * WishlistIcon component.
 * @param props
 * @extends {React.PureComponent<IWishlistIconProps>}
 */
const WishlistIcon: React.FC<IWishlistIconComponentProps> = (props: IWishlistIconComponentProps) => {
    const wishlistIconRef: React.RefObject<HTMLButtonElement> = React.createRef();
    const text = props.wishlistTooltipText;
    const showButtonIconTooltip = props.showButtonTooltip;
    const wishlistUrl = getUrlSync('wishlist', props.context.actionContext);
    const signInUrl = `${props.context.request.user.signInUrl}?ru=${wishlistUrl}`;
    const url = props.context.request.user.isAuthenticated ? wishlistUrl : signInUrl;
    const showTooltip = showButtonIconTooltip !== undefined ? showButtonIconTooltip : true;
    // const shouldShowCount = props.isDispayWishlistCount !== undefined ? props.isDispayWishlistCount : false;
    // const wishlistCountlbl = props.wishlistCountLabel !== undefined ? props.wishlistCountLabel : '';
    const wishlistItemCount = props.wishlistCount !== undefined ? props.wishlistCount : '';
    // const countLabel = format(wishlistCountlbl, wishlistItemCount);

    // Construct telemetry attribute to render
    const payLoad = getPayloadObject('click', props.telemetryContent!, text, '');
    const attributes = getTelemetryAttributes(props.telemetryContent!, payLoad);
    const formattedWishlistCount = props.className === 'ms-header__wishlist-mobile' ? `${text} `+`(${wishlistItemCount})` : `(${wishlistItemCount})` ;

    return (
        <>
            <Button
                className={classname('msc-wishlist-icon', props.className)}
                href={url}
                aria-label={text}
                innerRef={wishlistIconRef}
                {...attributes}
                onClick={onTelemetryClick(props.telemetryContent!, payLoad, text)}
            >
                { <span className='msc-wishlist-icon__textcount'>
                    {formattedWishlistCount}
                </span> }
            </Button>
            { showTooltip && <UncontrolledTooltip trigger='hover focus' target={wishlistIconRef}>
                {text}
            </UncontrolledTooltip>}
        </>
    );
};

// @ts-expect-error
export const WishListIconComponent: React.FunctionComponent<IWishlistIconComponentProps> = msdyn365Commerce.createComponentOverride<IWishlistIconComponent>(
    'WishListIcon',
    { component: WishlistIcon, ...WishlistIconComponentActions }
);


export default WishListIconComponent;
```

### 3. Create data file
Create a new file named **header.data.ts** under **src\themes\fabrikam-extended\views** folder and paste the code below in it

```typescript
/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { IStoreSelectorStateManager } from '@msdyn365-commerce-modules/bopis-utilities';
import { ICartState } from '@msdyn365-commerce/global-state';
import { AsyncResult, CommerceList, Customer } from '@msdyn365-commerce/retail-proxy';

export interface IHeaderData {
    cart: AsyncResult<ICartState>;
    accountInformation: AsyncResult<Customer>;
    storeSelectorStateManager: AsyncResult<IStoreSelectorStateManager>;
    wishlists?: AsyncResult<CommerceList[]>;
}

```

### 4. Extend view file for header
Create a new file named **header.view.tsx** under **src\themes\fabrikam-extended\views\header.view.tsx** folder and paste the code below in it

```typescript
/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { getTelemetryObject, Module, Node } from '@msdyn365-commerce-modules/utilities';
import { ArrayExtensions } from '@msdyn365-commerce-modules/retail-actions';
import * as React from 'react';

import { IHeaderViewProps } from '@msdyn365-commerce-modules/header/src/modules/header/./header';
import { IHeaderProps as IHeaderExtensionProps } from '../definition-extensions/header.ext.props.autogenerated';
import { WishListIconComponent } from './components/wishlisticon.component';
import { IHeaderData } from './header.data';

const headerView: React.FC<IHeaderViewProps> = props => {
    const {
        HeaderTag,
        HeaderContainer,
        HeaderTopBarContainer,
        MobileMenuContainer,
        MobileMenuBodyContainer,
        MobileMenuLinksContainer,
        Divider
    } = props;
    return (
        <Module {...HeaderTag}>
            <Node {...HeaderContainer}>
                <Node {...HeaderTopBarContainer}>
                    {props.navIcon}
                    {props.logo}
                    {_renderReactFragment(props.search)}
                    {props.preferredStore}
                    {_renderDesktopAccountBlock(props)}
                    {_renderWishListIconDesktop(props)}
                    <Node {...Divider} />
                    {props.cartIcon}
                    {_renderReactFragment(props.siteOptions)}
                </Node>
                <Node {...MobileMenuContainer}>
                    <Node {...MobileMenuBodyContainer}>
                        {props.MobileMenuHeader}
                        {_renderReactFragment(props.menuBar)}
                        <Node {...MobileMenuLinksContainer}>
                            {props.accountLinks ? props.accountLinks.map(link => link) : false}
                            {props.siteOptions}
                            {_renderWishListIconMobile(props)}
                            {props.signInLink}
                            {props.signOutLink}
                        </Node>
                    </Node>
                </Node>
                {_renderReactFragment(props.menuBar)}
            </Node>
        </Module>
    );
};

function _renderDesktopAccountBlock(props: IHeaderViewProps): JSX.Element | null {
    const {
        AccountInfoDropdownParentContainer,
        AccountInfoDropdownPopoverConentContainer,
        accountInfoDropdownButton,
        signOutLink,
        signInLink,
        accountLinks
    } = props;

    if (AccountInfoDropdownParentContainer) {
        if (AccountInfoDropdownPopoverConentContainer) {
            return (
                <Node {...AccountInfoDropdownParentContainer}>
                    {accountInfoDropdownButton}
                    <Node {...AccountInfoDropdownPopoverConentContainer}>
                        {accountLinks ? accountLinks.map(link => link) : false}
                        {signOutLink}
                    </Node>
                </Node>
            );
        } else if (signInLink) {
            return <Node {...AccountInfoDropdownParentContainer}>{signInLink}</Node>;
        }
    }
    props.context.telemetry.error('Header content is empty, module wont render.');
    return null;
}

function _renderReactFragment(items: React.ReactNode[]): JSX.Element | null {
    return (
        <>
            {items && items.length > 0
                ? items.map((slot: React.ReactNode, index: number) => {
                      return <React.Fragment key={index}>{slot}</React.Fragment>;
                  })
                : null}
        </>
    );
}

function _renderWishListIconDesktop(props: IHeaderViewProps & IHeaderExtensionProps<IHeaderData>): JSX.Element | null {
    const disableTooltip = props.context.app.config.disableTooltip || false;
    const wishlistTooltipText = props.resources.wishlistTooltipText;
    const context = props.context;
    const id = props.id;
    const typeName = props.typeName;
    const telemetryContent = getTelemetryObject(props.context.request.telemetryPageName!, props.friendlyName, props.telemetry);
    const wishlists = props.data.wishlists?.result;
    const wishlistCount = ((wishlists && ArrayExtensions.hasElements(wishlists) && wishlists[0].CommerceListLines) || []).length;
    return (
        <WishListIconComponent
            className='ms-header__wishlist-desktop'
            showButtonTooltip={!disableTooltip}
            wishlistTooltipText={wishlistTooltipText}
            context={context}
            id={id}
            typeName={typeName}
            telemetryContent={telemetryContent}
            data={props.data}
            wishlistCount={wishlistCount}
        />
    );
}

function _renderWishListIconMobile(props: IHeaderViewProps & IHeaderExtensionProps<IHeaderData>): JSX.Element | null {
    const disableTooltip = props.context.app.config.disableTooltip || false;
    const wishlistTooltipText = props.resources.wishlistTooltipText;
    const context = props.context;
    const id = props.id;
    const typeName = props.typeName;
    const telemetryContent = getTelemetryObject(props.context.request.telemetryPageName!, props.friendlyName, props.telemetry);
    const wishlists = props.data.wishlists?.result;
    const wishlistCount = ((wishlists && ArrayExtensions.hasElements(wishlists) && wishlists[0].CommerceListLines) || []).length;
    return (
        <WishListIconComponent
            className='ms-header__wishlist-mobile'
            showButtonTooltip={!disableTooltip}
            wishlistTooltipText={wishlistTooltipText}
            context={context}
            id={id}
            typeName={typeName}
            telemetryContent={telemetryContent}
            data={props.data}
            wishlistCount={wishlistCount}
        />
    );
}

export default headerView;
```

## Build and test module

The sample can now be tested in a web browser using the ```yarn start``` command.

Create a sample mock with name wishlistcount.json in the **src/pageMocks** and replace the mock file content with below content and run the application with mock by applying theme(https://localhost:4000/page?mock=wishlistcount&theme=fabrikam-extended)

```json
   
{
  "exception": null,
  "pageRoot": {
    "typeName": "core-root",
    "id": "core-root_qcgewk3",
    "friendlyName": "Core root 1",
    "config": {},
    "modules": {
      "body": [
        {
          "typeName": "default-page",
          "id": "default-page",
          "friendlyName": "Default Page",
          "config": {
            "pageCacheTTL": 3600.0,
            "pageCacheTTR": 900.0,
            "skipToMainText": "Skip to main content",
            "shouldCachePage": true,
            "pageTheme": "fabrikam"
          },
          "modules": {
            "header": [
              {
                "typeName": "default-container",
                "id": "IDNMAwGxa_c26y77k",
                "friendlyName": "Default container",
                "config": {
                  "heading": {},
                  "layout": "stacked",
                  "containerType": "fluid",
                  "clientRender": false,
                  "childrenWidth": "four"
                },
                "modules": {
                  "content": [
                    {
                      "typeName": "cookie-compliance",
                      "id": "IDNMAwGxa_c26y77k-cookie-compliance",
                      "friendlyName": "Cookie compliance",
                      "config": {
                        "content": "<p>This site uses cookies for analytics and personalized content. Accept to continue.&nbsp;</p>",
                        "actionLinks": [
                          {
                            "linkText": "Learn more",
                            "linkUrl": {
                              "destinationUrl": "https://go.microsoft.com/fwlink/?linkid=845480",
                              "type": "externalLink"
                            },
                            "ariaLabel": "Learn more about cookie compliance"
                          }
                        ],
                        "clientRender": false
                      }
                    },
                    {
                      "typeName": "promo-banner",
                      "id": "IDNMAwGxa_c26y77k-promo-banner",
                      "friendlyName": "Promo banner",
                      "platform": {
                        "layout": "mediumWidth"
                      },
                      "config": {
                        "msdyn365__moduleLayout": "mediumWidth",
                        "bannerMessages": [
                          {
                            "text": "Shop for Thanksgiving SALE!!",
                            "links": []
                          },
                          {
                            "text": "Men's shoes on sale!",
                            "links": [
                              {
                                "linkText": "Shop now",
                                "linkUrl": {
                                  "type": "categoryLink",
                                  "categoryId": 68719478053
                                }
                              }
                            ]
                          }
                        ],
                        "autoplay": false,
                        "dismissEnabled": false,
                        "hideFlipper": true,
                        "className": "alignment__center"
                      }
                    },
                    {
                      "typeName": "notifications-list",
                      "id": "IDNMAwGxa_c26y77k-notifications-list__0",
                      "friendlyName": "Notifications",
                      "config": {
                        "feed": "global",
                        "maxNotificationsCount": 5.0,
                        "minTopOffset": 20.0,
                        "maxTopOffset": 100.0,
                        "className": "msc-global-notifications-list-container"
                      }
                    },
                    {
                      "typeName": "header",
                      "id": "IDNMAwGxa_c26y77k-header",
                      "friendlyName": "Header",
                      "config": {
                        "logoImage": {
                          "$type": "image",
                          "_id": "MCsCx",
                          "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCsCy?ver=18c6",
                          "altText": "Fabrikam",
                          "location": {},
                          "copyright": "",
                          "binaryHash": "zsDVVIqCDbkGaw7eITavoA==",
                          "title": "Fabrikam",
                          "imageSettings": {
                            "quality": 80,
                            "disableLazyLoad": true,
                            "lazyload": true,
                            "viewports": {
                              "xs": {
                                "w": 132,
                                "h": 28,
                                "q": "w=132&h=28&q=80&m=6&f=jpg",
                                "qe": "t=w132h28qhigh"
                              },
                              "lg": {
                                "w": 160,
                                "h": 48,
                                "q": "w=160&h=48&q=80&m=6&f=jpg",
                                "qe": "t=w160h48qhigh"
                              }
                            }
                          }
                        },
                        "logoLink": {
                          "linkText": "",
                          "linkUrl": {
                            "destinationUrl": "/modern/",
                            "type": "internalLink"
                          },
                          "ariaLabel": "Fabrikam",
                          "openInNewTab": false
                        },
                        "myAccountLinks": [
                          {
                            "linkText": "My account",
                            "linkUrl": {
                              "destinationUrl": "/modern/myaccount",
                              "type": "internalLink"
                            },
                            "ariaLabel": "My account"
                          },
                          {
                            "linkText": "Order history",
                            "linkUrl": {
                              "destinationUrl": "/modern/orderhistory",
                              "type": "internalLink"
                            },
                            "ariaLabel": "Order history"
                          },
                          {
                            "linkText": "Quick order",
                            "linkUrl": {
                              "destinationUrl": "/modern/quick-order",
                              "type": "internalLink"
                            },
                            "ariaLabel": "Quick order"
                          }
                        ],
                        "shouldShowWishlistCount": true
                      },
                      "data": {},
                      "modules": {
                        "menuBar": [
                          {
                            "typeName": "navigation-menu",
                            "id": "IDNMAwGxa_c26y77k-navigation-menu",
                            "friendlyName": "Navigation menu",
                            "config": {
                              "categoryImageSettings": {
                                "quality": 80,
                                "disableLazyLoad": false,
                                "lazyload": true,
                                "viewports": {
                                  "xs": {
                                    "w": 300,
                                    "h": 350,
                                    "q": "w=300&h=350&q=80&m=6&f=jpg",
                                    "qe": "t=w300h350qhigh"
                                  },
                                  "lg": {
                                    "w": 300,
                                    "h": 350,
                                    "q": "w=300&h=350&q=80&m=6&f=jpg",
                                    "qe": "t=w300h350qhigh"
                                  },
                                  "xl": {
                                    "w": 300,
                                    "h": 350,
                                    "q": "w=300&h=350&q=80&m=6&f=jpg",
                                    "qe": "t=w300h350qhigh"
                                  }
                                }
                              },
                              "menuLevelSupport": 2.0,
                              "rootMenuNavigation": "Shop",
                              "cmsNavItems": [
                                {
                                  "linkText": "Contact",
                                  "linkUrl": {
                                    "destinationUrl": "/modern/contact",
                                    "type": "internalLink"
                                  },
                                  "image": {
                                    "$type": "image",
                                    "_id": "MC7tpy",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MC7tpz?ver=04bc",
                                    "altText": "contact us",
                                    "copyright": "",
                                    "binaryHash": "z4uKWh1NYtpF4O08gPtwBg==",
                                    "title": "Contact us",
                                    "imageSettings": {
                                      "quality": 70,
                                      "disableLazyLoad": false,
                                      "lazyload": true,
                                      "viewports": {
                                        "xs": {
                                          "w": 300,
                                          "h": 350,
                                          "q": "w=300&h=350&q=70&m=6&f=jpg",
                                          "qe": "t=w300h350qhigh"
                                        },
                                        "lg": {
                                          "w": 300,
                                          "h": 350,
                                          "q": "w=300&h=350&q=70&m=6&f=jpg",
                                          "qe": "t=w300h350qhigh"
                                        },
                                        "xl": {
                                          "w": 300,
                                          "h": 350,
                                          "q": "w=300&h=350&q=70&m=6&f=jpg",
                                          "qe": "t=w300h350qhigh"
                                        }
                                      }
                                    }
                                  },
                                  "imageLink": {},
                                  "ariaLabel": "Contact",
                                  "openInNewTab": false,
                                  "subMenus": [
                                    {
                                      "linkText": "Store Locations",
                                      "linkUrl": {
                                        "destinationUrl": "/modern/storelocator",
                                        "type": "internalLink"
                                      },
                                      "ariaLabel": "Store locations",
                                      "image": {
                                        "$type": "image",
                                        "_id": "MC1NKG",
                                        "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MC1NKH?ver=5944",
                                        "altText": "fgfg",
                                        "location": {},
                                        "copyright": "",
                                        "binaryHash": "4Txt53Hx0aip4+m7TkupUg==",
                                        "title": "testcc",
                                        "imageSettings": {
                                          "quality": 80,
                                          "disableLazyLoad": false,
                                          "lazyload": true,
                                          "viewports": {
                                            "xs": {
                                              "w": 300,
                                              "h": 350,
                                              "q": "w=300&h=350&q=80&m=6&f=jpg",
                                              "qe": "t=w300h350qhigh"
                                            },
                                            "lg": {
                                              "w": 300,
                                              "h": 350,
                                              "q": "w=300&h=350&q=80&m=6&f=jpg",
                                              "qe": "t=w300h350qhigh"
                                            },
                                            "xl": {
                                              "w": 300,
                                              "h": 350,
                                              "q": "w=300&h=350&q=80&m=6&f=jpg",
                                              "qe": "t=w300h350qhigh"
                                            }
                                          }
                                        }
                                      },
                                      "imageLink": {
                                        "destinationUrl": "/modern/",
                                        "type": "internalLink"
                                      },
                                      "subMenus": [
                                        {
                                          "linkText": "North America",
                                          "linkUrl": {
                                            "destinationUrl": "/modern/termsandconditions",
                                            "type": "internalLink"
                                          },
                                          "ariaLabel": "North America locations",
                                          "imageLink": {}
                                        },
                                        {
                                          "linkText": "Asia",
                                          "linkUrl": {
                                            "destinationUrl": "/modern/termsandconditions",
                                            "type": "internalLink"
                                          },
                                          "ariaLabel": "terms",
                                          "imageLink": {}
                                        },
                                        {
                                          "linkText": "South America",
                                          "linkUrl": {
                                            "destinationUrl": "/modern/termsandconditions",
                                            "type": "internalLink"
                                          },
                                          "ariaLabel": "south america locations",
                                          "imageLink": {}
                                        }
                                      ]
                                    },
                                    {
                                      "linkText": "Directors",
                                      "linkUrl": {
                                        "destinationUrl": "/modern/termsandconditions",
                                        "type": "internalLink"
                                      },
                                      "ariaLabel": "directors",
                                      "imageLink": {}
                                    },
                                    {
                                      "linkText": "Support",
                                      "linkUrl": {
                                        "destinationUrl": "/modern/termsandconditions",
                                        "type": "internalLink"
                                      },
                                      "ariaLabel": "Support",
                                      "imageLink": {}
                                    }
                                  ]
                                }
                              ],
                              "categoryPromotionalContent": [
                                {
                                  "categoryName": "Pants",
                                  "image": {
                                    "$type": "image",
                                    "_id": "LAlz0",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/LAlz1?ver=cdc0",
                                    "altText": "Digital Gift Card",
                                    "location": {},
                                    "copyright": "",
                                    "binaryHash": "eGNMVWZR5ZLjV4JMLRFPog==",
                                    "title": "Digital Gift Card",
                                    "imageSettings": {
                                      "quality": 80,
                                      "disableLazyLoad": false,
                                      "lazyload": true,
                                      "viewports": {
                                        "xs": {
                                          "w": 300,
                                          "h": 350,
                                          "q": "w=300&h=350&q=80&m=6&f=jpg",
                                          "qe": "t=w300h350qhigh"
                                        },
                                        "lg": {
                                          "w": 300,
                                          "h": 350,
                                          "q": "w=300&h=350&q=80&m=6&f=jpg",
                                          "qe": "t=w300h350qhigh"
                                        },
                                        "xl": {
                                          "w": 300,
                                          "h": 350,
                                          "q": "w=300&h=350&q=80&m=6&f=jpg",
                                          "qe": "t=w300h350qhigh"
                                        }
                                      }
                                    }
                                  },
                                  "text": "Fabrikam content",
                                  "linkUrl": {
                                    "type": "externalLink",
                                    "destinationUrl": "https://r1tie.fabrikam.com/modern/fabrikam-fashion-us/virtual-catalog/68719488536.c"
                                  }
                                },
                                {
                                  "categoryName": "MenShoes",
                                  "image": {
                                    "$type": "image",
                                    "_id": "LA164M",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/LA164R?ver=f344",
                                    "altText": "KitImage1",
                                    "copyright": "",
                                    "binaryHash": "XfDspEA5xx3UcE4ZFycbAw==",
                                    "title": "",
                                    "imageSettings": {
                                      "quality": 80,
                                      "disableLazyLoad": false,
                                      "lazyload": true,
                                      "viewports": {
                                        "xs": {
                                          "w": 300,
                                          "h": 350,
                                          "q": "w=300&h=350&q=80&m=6&f=jpg",
                                          "qe": "t=w300h350qhigh"
                                        },
                                        "lg": {
                                          "w": 300,
                                          "h": 350,
                                          "q": "w=300&h=350&q=80&m=6&f=jpg",
                                          "qe": "t=w300h350qhigh"
                                        },
                                        "xl": {
                                          "w": 300,
                                          "h": 350,
                                          "q": "w=300&h=350&q=80&m=6&f=jpg",
                                          "qe": "t=w300h350qhigh"
                                        }
                                      }
                                    }
                                  },
                                  "text": "New Collection",
                                  "linkUrl": {
                                    "type": "categoryLink",
                                    "categoryId": 68719478053
                                  }
                                },
                                {
                                  "categoryName": "Necklaces",
                                  "image": {
                                    "$type": "image",
                                    "_id": "MCy8b",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCy96?ver=3940",
                                    "altText": "No Data Available",
                                    "copyright": "",
                                    "binaryHash": "0pKmAZy/6Fb0sVAGByUucA==",
                                    "title": "",
                                    "imageSettings": {
                                      "quality": 80,
                                      "disableLazyLoad": false,
                                      "lazyload": true,
                                      "viewports": {
                                        "xs": {
                                          "w": 300,
                                          "h": 350,
                                          "q": "w=300&h=350&q=80&m=6&f=jpg",
                                          "qe": "t=w300h350qhigh"
                                        },
                                        "lg": {
                                          "w": 300,
                                          "h": 350,
                                          "q": "w=300&h=350&q=80&m=6&f=jpg",
                                          "qe": "t=w300h350qhigh"
                                        },
                                        "xl": {
                                          "w": 300,
                                          "h": 350,
                                          "q": "w=300&h=350&q=80&m=6&f=jpg",
                                          "qe": "t=w300h350qhigh"
                                        }
                                      }
                                    }
                                  },
                                  "text": "Find the perfect valentines gift!",
                                  "linkUrl": {
                                    "type": "categoryLink",
                                    "categoryId": 68719485537
                                  }
                                }
                              ],
                              "navigationMenuSource": "all",
                              "enableMultilevelMenu": true,
                              "enabletopMenu": false,
                              "displayCategoryImage": true,
                              "displayPromotionalImage": true
                            },
                            "data": {}
                          }
                        ],
                        "search": [
                          {
                            "typeName": "search",
                            "id": "IDNMAwGxa_c26y77k-search",
                            "friendlyName": "Search",
                            "config": {
                              "suggestionTypeCriterion": [],
                              "topResultsCount": 5.0,
                              "imageSettings": {
                                "quality": 80,
                                "disableLazyLoad": false,
                                "lazyload": true,
                                "viewports": {
                                  "xs": {
                                    "w": 80,
                                    "h": 94,
                                    "q": "w=80&h=94&q=80&m=6&f=jpg",
                                    "qe": "t=w80h94qhigh"
                                  },
                                  "sm": {
                                    "w": 148,
                                    "h": 174,
                                    "q": "w=148&h=174&q=80&m=6&f=jpg",
                                    "qe": "t=w148h174qhigh"
                                  },
                                  "lg": {
                                    "w": 148,
                                    "h": 174,
                                    "q": "w=148&h=174&q=80&m=6&f=jpg",
                                    "qe": "t=w148h174qhigh"
                                  }
                                }
                              },
                              "searchplaceholderText": "Search in Fabrikam "
                            }
                          }
                        ],
                        "cartIcon": [
                          {
                            "typeName": "cart-icon",
                            "id": "IDNMAwGxa_c26y77k-cart-icon",
                            "friendlyName": "Cart Icon",
                            "config": {
                              "imageSettings": {
                                "quality": 80,
                                "disableLazyLoad": false,
                                "lazyload": true,
                                "viewports": {
                                  "xs": {
                                    "w": 80,
                                    "h": 94,
                                    "q": "w=80&h=94&q=80&m=6&f=jpg",
                                    "qe": "t=w80h94qhigh"
                                  },
                                  "sm": {
                                    "w": 148,
                                    "h": 174,
                                    "q": "w=148&h=174&q=80&m=6&f=jpg",
                                    "qe": "t=w148h174qhigh"
                                  },
                                  "lg": {
                                    "w": 148,
                                    "h": 174,
                                    "q": "w=148&h=174&q=80&m=6&f=jpg",
                                    "qe": "t=w148h174qhigh"
                                  }
                                }
                              },
                              "enableHoverCart": true,
                              "cartLinesSortOrder": "descending",
                              "isAnonymousCheckout": true,
                              "className": ""
                            }
                          }
                        ],
                        "storeSelector": [
                          {
                            "typeName": "store-selector",
                            "id": "IDNMAwGxa_c26y77k-store-selector",
                            "friendlyName": "Store selector",
                            "config": {
                              "termsOfServiceLink": {
                                "linkText": "Microsoft Bing Maps Terms",
                                "linkUrl": {
                                  "type": "externalLink",
                                  "destinationUrl": "https://www.microsoft.com/en-us/maps/product/terms-april-2011"
                                }
                              },
                              "mode": "findStores",
                              "searchRadiusUnit": "miles",
                              "lookupRadius": 50.0,
                              "style": "dialog",
                              "setAsPreferredStore": true,
                              "enablePickupFilterToShowStore": false,
                              "showAllStores": true,
                              "autoSuggestionEnabled": true,
                              "autoSuggestOptions": {
                                "maxResults": 5
                              }
                            },
                            "modules": {
                              "maps": [
                                {
                                  "typeName": "map",
                                  "id": "IDNMAwGxa_c26y77k-map",
                                  "friendlyName": "Map",
                                  "config": {
                                    "heading": {
                                      "text": "",
                                      "tag": "h2"
                                    },
                                    "pushpinOptions": {
                                      "size": 1.5,
                                      "color": "#616365",
                                      "selectionColor": "#4C833A",
                                      "showIndex": true
                                    }
                                  }
                                }
                              ]
                            }
                          }
                        ],
                        "siteOptions": [
                          {
                            "typeName": "site-picker",
                            "id": "IDNMAwGxa_c26y77k-site-picker",
                            "friendlyName": "Site picker",
                            "config": {
                              "heading": {
                                "text": "Choose a country",
                                "tag": "h3"
                              },
                              "siteOptions": [
                                {
                                  "siteName": "Belgium (fr-be)",
                                  "siteImage": {
                                    "$type": "image",
                                    "_id": "MCfjwZ",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCfjx0?ver=08c6",
                                    "altText": "BE_50.jpg",
                                    "focalRegion": {
                                      "x1": 60,
                                      "x2": 60,
                                      "y1": 29,
                                      "y2": 29
                                    },
                                    "height": 50,
                                    "quality": 94,
                                    "width": 75,
                                    "copyright": "",
                                    "binaryHash": "txkPP+AhM+4EzdqyHHFZuA==",
                                    "title": "BE_50.jpg",
                                    "imageSettings": {
                                      "quality": 80,
                                      "disableLazyLoad": false,
                                      "lazyload": true
                                    }
                                  },
                                  "siteRedirectUrl": {
                                    "type": "externalLink",
                                    "destinationUrl": "https://r1tie.fabrikam.com/modern/fr-be"
                                  }
                                },
                                {
                                  "siteName": "Belgium (nl-be)",
                                  "siteImage": {
                                    "$type": "image",
                                    "_id": "MCfjwZ",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCfjx0?ver=08c6",
                                    "altText": "BE_50.jpg",
                                    "focalRegion": {
                                      "x1": 60,
                                      "x2": 60,
                                      "y1": 29,
                                      "y2": 29
                                    },
                                    "height": 50,
                                    "quality": 94,
                                    "width": 75,
                                    "copyright": "",
                                    "binaryHash": "txkPP+AhM+4EzdqyHHFZuA==",
                                    "title": "BE_50.jpg",
                                    "imageSettings": {
                                      "quality": 80,
                                      "disableLazyLoad": false,
                                      "lazyload": true
                                    }
                                  },
                                  "siteRedirectUrl": {
                                    "type": "externalLink",
                                    "destinationUrl": "https://r1tie.fabrikam.com/modern/nl-be"
                                  }
                                },
                                {
                                  "siteName": "Canada (en-ca)",
                                  "siteImage": {
                                    "$type": "image",
                                    "_id": "MCeyHL",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCeyHW?ver=ba09",
                                    "altText": "Flag",
                                    "height": 50,
                                    "quality": 94,
                                    "width": 75,
                                    "copyright": "",
                                    "binaryHash": "/MMypjKIWyYkKi1P2+INaw==",
                                    "title": "",
                                    "imageSettings": {
                                      "quality": 80,
                                      "disableLazyLoad": false,
                                      "lazyload": true
                                    }
                                  },
                                  "siteRedirectUrl": {
                                    "type": "externalLink",
                                    "destinationUrl": "https://r1tie.fabrikam.com/modern/en-ca"
                                  }
                                },
                                {
                                  "siteName": "Canada (fr-ca)",
                                  "siteImage": {
                                    "$type": "image",
                                    "_id": "MCeyHL",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCeyHW?ver=ba09",
                                    "altText": "Flag",
                                    "height": 50,
                                    "quality": 94,
                                    "width": 75,
                                    "copyright": "",
                                    "binaryHash": "/MMypjKIWyYkKi1P2+INaw==",
                                    "title": "",
                                    "imageSettings": {
                                      "quality": 80,
                                      "disableLazyLoad": false,
                                      "lazyload": true
                                    }
                                  },
                                  "siteRedirectUrl": {
                                    "type": "externalLink",
                                    "destinationUrl": "https://r1tie.fabrikam.com/modern/fr-ca"
                                  }
                                },
                                {
                                  "siteName": "France",
                                  "siteImage": {
                                    "$type": "image",
                                    "_id": "MCeyHP",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCeyI0?ver=4939",
                                    "altText": "Flag",
                                    "copyright": "",
                                    "quality": 94,
                                    "width": 75,
                                    "height": 50,
                                    "binaryHash": "Qm+npoeOep33lqrmH9HchA==",
                                    "title": "",
                                    "imageSettings": {
                                      "quality": 80,
                                      "disableLazyLoad": false,
                                      "lazyload": true
                                    }
                                  },
                                  "siteRedirectUrl": {
                                    "type": "externalLink",
                                    "destinationUrl": "https://r1tie.fabrikam.com/modern/fr-fr"
                                  }
                                },
                                {
                                  "siteName": "Germany",
                                  "siteImage": {
                                    "$type": "image",
                                    "_id": "MCeyHV",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCeyHZ?ver=dc36",
                                    "altText": "Flag",
                                    "copyright": "",
                                    "quality": 94,
                                    "width": 83,
                                    "height": 50,
                                    "binaryHash": "3HQgC0nuRbl/iR0v5aY5sg==",
                                    "title": "",
                                    "imageSettings": {
                                      "quality": 80,
                                      "disableLazyLoad": false,
                                      "lazyload": true
                                    }
                                  },
                                  "siteRedirectUrl": {
                                    "type": "externalLink",
                                    "destinationUrl": "https://r1tie.fabrikam.com/modern/de"
                                  }
                                },
                                {
                                  "siteName": "Great Britian",
                                  "siteImage": {
                                    "$type": "image",
                                    "_id": "MCfUtP",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCfUtQ?ver=77f6",
                                    "altText": "GB_50.jpg",
                                    "height": 46,
                                    "quality": 94,
                                    "width": 75,
                                    "copyright": "",
                                    "binaryHash": "H+RpmC+2prAa4zhEBrcnMw==",
                                    "title": "GB_50.jpg",
                                    "imageSettings": {
                                      "quality": 80,
                                      "disableLazyLoad": false,
                                      "lazyload": true
                                    }
                                  },
                                  "siteRedirectUrl": {
                                    "type": "externalLink",
                                    "destinationUrl": "https://r1tie.fabrikam.com/modern/eu"
                                  }
                                },
                                {
                                  "siteName": "Netherlands",
                                  "siteImage": {
                                    "$type": "image",
                                    "_id": "MCfjx2",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCfjx3?ver=016f",
                                    "altText": "NL_50.jpg",
                                    "height": 56,
                                    "quality": 94,
                                    "width": 75,
                                    "copyright": "",
                                    "binaryHash": "pgzRj4sj46sBMG1y0zbBuw==",
                                    "title": "NL_50.jpg",
                                    "imageSettings": {
                                      "quality": 80,
                                      "disableLazyLoad": false,
                                      "lazyload": true
                                    }
                                  },
                                  "siteRedirectUrl": {
                                    "type": "externalLink",
                                    "destinationUrl": "https://r1tie.fabrikam.com/modern/nl"
                                  }
                                },
                                {
                                  "siteName": "New Zealand",
                                  "siteImage": {
                                    "$type": "image",
                                    "_id": "MCeyHT",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCeyI1?ver=0fbf",
                                    "altText": "Flag",
                                    "copyright": "",
                                    "quality": 94,
                                    "width": 82,
                                    "height": 50,
                                    "binaryHash": "Mc3qyS+4i/79uMfkj5XT2g==",
                                    "title": "",
                                    "imageSettings": {
                                      "quality": 80,
                                      "disableLazyLoad": false,
                                      "lazyload": true
                                    }
                                  },
                                  "siteRedirectUrl": {
                                    "type": "externalLink",
                                    "destinationUrl": "https://r1tie.fabrikam.com/modern/en-nz"
                                  }
                                },
                                {
                                  "siteName": "United States",
                                  "siteImage": {
                                    "$type": "image",
                                    "_id": "MCeyHU",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCeyHY?ver=0635",
                                    "altText": "Flag",
                                    "height": 50,
                                    "quality": 94,
                                    "width": 83,
                                    "copyright": "",
                                    "binaryHash": "DfNRRsGWdRhfuBuRzkAZDA==",
                                    "title": "",
                                    "imageSettings": {
                                      "quality": 80,
                                      "disableLazyLoad": false,
                                      "lazyload": true
                                    }
                                  },
                                  "siteRedirectUrl": {
                                    "type": "externalLink",
                                    "destinationUrl": "https://r1tie.fabrikam.com/modern"
                                  }
                                },
                                {
                                  "siteName": "United States (es-us)",
                                  "siteImage": {
                                    "$type": "image",
                                    "_id": "MCeyHU",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCeyHY?ver=0635",
                                    "altText": "Flag",
                                    "height": 50,
                                    "quality": 94,
                                    "width": 83,
                                    "copyright": "",
                                    "binaryHash": "DfNRRsGWdRhfuBuRzkAZDA==",
                                    "title": "",
                                    "imageSettings": {
                                      "quality": 80,
                                      "disableLazyLoad": false,
                                      "lazyload": true
                                    }
                                  },
                                  "siteRedirectUrl": {
                                    "type": "externalLink",
                                    "destinationUrl": "https://r1tie.fabrikam.com/modern/es"
                                  }
                                },
                                {
                                  "siteName": "Japan",
                                  "siteImage": {
                                    "$type": "image",
                                    "_id": "MCfZou",
                                    "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCfZov?ver=0261",
                                    "altText": "JA_50.jpg",
                                    "height": 53,
                                    "quality": 94,
                                    "width": 75,
                                    "copyright": "",
                                    "binaryHash": "O43RP74mYkHYiwQyOPV45A==",
                                    "title": "JA_50.jpg",
                                    "imageSettings": {
                                      "quality": 80,
                                      "disableLazyLoad": false,
                                      "lazyload": true
                                    }
                                  },
                                  "siteRedirectUrl": {
                                    "type": "externalLink",
                                    "destinationUrl": "https://r1tie.fabrikam.com/modern/ja"
                                  }
                                }
                              ]
                            },
                            "data": {}
                          }
                        ]
                      }
                    },
                    {
                      "typeName": "country-picker",
                      "id": "IDNMAwGxa_c26y77k-country-picker",
                      "friendlyName": "Country/region picker",
                      "platform": {
                        "layout": "mediumWidth"
                      },
                      "config": {
                        "heading": {
                          "text": "Choose a site",
                          "tag": "h2"
                        },
                        "countryList": [
                          {
                            "displayString": "Belgium",
                            "displaySubString": "fr-be",
                            "image": {
                              "$type": "image",
                              "_id": "MCfjwZ",
                              "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCfjx0?ver=08c6",
                              "altText": "BE_50.jpg",
                              "focalRegion": {
                                "x1": 60,
                                "x2": 60,
                                "y1": 29,
                                "y2": 29
                              },
                              "height": 50,
                              "quality": 94,
                              "width": 75,
                              "copyright": "",
                              "binaryHash": "txkPP+AhM+4EzdqyHHFZuA==",
                              "title": "BE_50.jpg",
                              "imageSettings": {
                                "quality": 80,
                                "disableLazyLoad": false,
                                "lazyload": true
                              }
                            },
                            "url": "https://r1tie.fabrikam.com/modern/fr-be"
                          },
                          {
                            "displayString": "Belgium",
                            "displaySubString": "nl-be",
                            "image": {
                              "$type": "image",
                              "_id": "MCfjwZ",
                              "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCfjx0?ver=08c6",
                              "altText": "BE_50.jpg",
                              "focalRegion": {
                                "x1": 60,
                                "x2": 60,
                                "y1": 29,
                                "y2": 29
                              },
                              "height": 50,
                              "quality": 94,
                              "width": 75,
                              "copyright": "",
                              "binaryHash": "txkPP+AhM+4EzdqyHHFZuA==",
                              "title": "BE_50.jpg",
                              "imageSettings": {
                                "quality": 80,
                                "disableLazyLoad": false,
                                "lazyload": true
                              }
                            },
                            "url": "https://r1tie.fabrikam.com/modern/nl-be"
                          },
                          {
                            "displayString": "Canada",
                            "displaySubString": "English",
                            "image": {
                              "$type": "image",
                              "_id": "MCeyHL",
                              "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCeyHW?ver=ba09",
                              "altText": "Flag",
                              "height": 50,
                              "quality": 94,
                              "width": 75,
                              "copyright": "",
                              "binaryHash": "/MMypjKIWyYkKi1P2+INaw==",
                              "title": "",
                              "imageSettings": {
                                "quality": 80,
                                "disableLazyLoad": false,
                                "lazyload": true
                              }
                            },
                            "url": "https://r1tie.fabrikam.com/modern/en-ca"
                          },
                          {
                            "displayString": "Canada",
                            "displaySubString": "French",
                            "image": {
                              "$type": "image",
                              "_id": "MCeyHL",
                              "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCeyHW?ver=ba09",
                              "altText": "Flag",
                              "height": 50,
                              "quality": 94,
                              "width": 75,
                              "copyright": "",
                              "binaryHash": "/MMypjKIWyYkKi1P2+INaw==",
                              "title": "",
                              "imageSettings": {
                                "quality": 80,
                                "disableLazyLoad": false,
                                "lazyload": true
                              }
                            },
                            "url": "https://r1tie.fabrikam.com/modern/fr-ca"
                          },
                          {
                            "displayString": "France",
                            "image": {
                              "$type": "image",
                              "_id": "MCeyHP",
                              "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCeyI0?ver=4939",
                              "altText": "Flag",
                              "copyright": "",
                              "quality": 94,
                              "width": 75,
                              "height": 50,
                              "binaryHash": "Qm+npoeOep33lqrmH9HchA==",
                              "title": "",
                              "imageSettings": {
                                "quality": 80,
                                "disableLazyLoad": false,
                                "lazyload": true
                              }
                            },
                            "url": "https://r1tie.fabrikam.com/modern/fr-fr"
                          },
                          {
                            "displayString": "Germany",
                            "image": {
                              "$type": "image",
                              "_id": "MCeyHV",
                              "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCeyHZ?ver=dc36",
                              "altText": "Flag",
                              "copyright": "",
                              "quality": 94,
                              "width": 83,
                              "height": 50,
                              "binaryHash": "3HQgC0nuRbl/iR0v5aY5sg==",
                              "title": "",
                              "imageSettings": {
                                "quality": 80,
                                "disableLazyLoad": false,
                                "lazyload": true
                              }
                            },
                            "url": "https://r1tie.fabrikam.com/modern/de"
                          },
                          {
                            "displayString": "Great Britian",
                            "image": {
                              "$type": "image",
                              "_id": "MCfUtP",
                              "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCfUtQ?ver=77f6",
                              "altText": "GB_50.jpg",
                              "height": 46,
                              "quality": 94,
                              "width": 75,
                              "copyright": "",
                              "binaryHash": "H+RpmC+2prAa4zhEBrcnMw==",
                              "title": "GB_50.jpg",
                              "imageSettings": {
                                "quality": 80,
                                "disableLazyLoad": false,
                                "lazyload": true
                              }
                            },
                            "url": "https://r1tie.fabrikam.com/modern/eu"
                          },
                          {
                            "displayString": "Japan",
                            "image": {
                              "$type": "image",
                              "_id": "MCfZou",
                              "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCfZov?ver=0261",
                              "altText": "JA_50.jpg",
                              "height": 53,
                              "quality": 94,
                              "width": 75,
                              "copyright": "",
                              "binaryHash": "O43RP74mYkHYiwQyOPV45A==",
                              "title": "JA_50.jpg",
                              "imageSettings": {
                                "quality": 80,
                                "disableLazyLoad": false,
                                "lazyload": true
                              }
                            },
                            "url": "https://r1tie.fabrikam.com/modern/ja"
                          },
                          {
                            "displayString": "Netherlands",
                            "image": {
                              "$type": "image",
                              "_id": "MCfjx2",
                              "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCfjx3?ver=016f",
                              "altText": "NL_50.jpg",
                              "height": 56,
                              "quality": 94,
                              "width": 75,
                              "copyright": "",
                              "binaryHash": "pgzRj4sj46sBMG1y0zbBuw==",
                              "title": "NL_50.jpg",
                              "imageSettings": {
                                "quality": 80,
                                "disableLazyLoad": false,
                                "lazyload": true
                              }
                            },
                            "url": "https://r1tie.fabrikam.com/modern/nl"
                          },
                          {
                            "displayString": "New Zealand",
                            "image": {
                              "$type": "image",
                              "_id": "MCeyHT",
                              "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCeyI1?ver=0fbf",
                              "altText": "Flag",
                              "copyright": "",
                              "quality": 94,
                              "width": 82,
                              "height": 50,
                              "binaryHash": "Mc3qyS+4i/79uMfkj5XT2g==",
                              "title": "",
                              "imageSettings": {
                                "quality": 80,
                                "disableLazyLoad": false,
                                "lazyload": true
                              }
                            },
                            "url": "https://r1tie.fabrikam.com/modern/en-nz"
                          },
                          {
                            "displayString": "United states",
                            "image": {
                              "$type": "image",
                              "_id": "MCeyHU",
                              "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCeyHY?ver=0635",
                              "altText": "Flag",
                              "height": 50,
                              "quality": 94,
                              "width": 83,
                              "copyright": "",
                              "binaryHash": "DfNRRsGWdRhfuBuRzkAZDA==",
                              "title": "",
                              "imageSettings": {
                                "quality": 80,
                                "disableLazyLoad": false,
                                "lazyload": true
                              }
                            },
                            "url": "https://r1tie.fabrikam.com/modern"
                          },
                          {
                            "displayString": "United States",
                            "displaySubString": "es-us",
                            "image": {
                              "$type": "image",
                              "_id": "MCeyHU",
                              "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCeyHY?ver=0635",
                              "altText": "Flag",
                              "height": 50,
                              "quality": 94,
                              "width": 83,
                              "copyright": "",
                              "binaryHash": "DfNRRsGWdRhfuBuRzkAZDA==",
                              "title": "",
                              "imageSettings": {
                                "quality": 80,
                                "disableLazyLoad": false,
                                "lazyload": true
                              }
                            },
                            "url": "https://r1tie.fabrikam.com/modern/es"
                          }
                        ],
                        "actionLink": {
                          "linkText": "",
                          "linkUrl": {}
                        }
                      },
                      "data": {}
                    }
                  ]
                }
              }
            ],
            "footer": [
              {
                "typeName": "default-container",
                "id": "IDNMAxJzO_5lztnip",
                "friendlyName": "Default container",
                "config": {
                  "heading": {},
                  "layout": "stacked",
                  "containerType": "container",
                  "clientRender": true,
                  "childrenWidth": "four",
                  "className": "ms-footer"
                },
                "modules": {
                  "content": [
                    {
                      "typeName": "footer-category",
                      "id": "IDNMAxJzO_5lztnip-footer-category",
                      "friendlyName": "Footer category",
                      "config": {
                        "categoryClassName": "nav-item"
                      },
                      "modules": {
                        "content": [
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item",
                            "friendlyName": "Footer item",
                            "config": {
                              "heading": {
                                "text": "Customer Service",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "Link text ",
                                "linkUrl": {}
                              }
                            }
                          },
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__1",
                            "friendlyName": "Footer item 2",
                            "config": {
                              "heading": {
                                "text": "",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "FAQ",
                                "linkUrl": {
                                  "destinationUrl": "/modern/storefaq",
                                  "type": "internalLink"
                                },
                                "ariaLabel": "FAQ"
                              }
                            },
                            "data": {}
                          },
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__2",
                            "friendlyName": "Footer item 3",
                            "config": {
                              "heading": {
                                "text": "",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "Returns & refunds",
                                "linkUrl": {
                                  "destinationUrl": "/modern/returns",
                                  "type": "internalLink"
                                },
                                "ariaLabel": "Returns and refunds"
                              }
                            },
                            "data": {}
                          },
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__16",
                            "friendlyName": "Footer item 17",
                            "config": {
                              "heading": {
                                "text": "",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "Terms and conditions",
                                "linkUrl": {
                                  "destinationUrl": "/modern/termsandconditions",
                                  "type": "internalLink"
                                }
                              }
                            },
                            "data": {}
                          },
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__15",
                            "friendlyName": "Footer item 16",
                            "config": {
                              "heading": {
                                "text": "",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "Privacy Policy",
                                "linkUrl": {
                                  "destinationUrl": "https://go.microsoft.com/fwlink/?LinkId=521839",
                                  "type": "externalLink"
                                },
                                "ariaLabel": "Privacy policy for Fabrikam"
                              }
                            }
                          },
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__17",
                            "friendlyName": "Footer item 18",
                            "platform": {
                              "layout": "mediumWidth"
                            },
                            "config": {
                              "msdyn365__moduleLayout": "mediumWidth",
                              "heading": {
                                "text": "",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "Order lookup",
                                "linkUrl": {
                                  "type": "externalLink",
                                  "destinationUrl": "https://r1tie.fabrikam.com/modern/orderlookup"
                                },
                                "ariaLabel": "Order lookup"
                              }
                            }
                          }
                        ]
                      }
                    },
                    {
                      "typeName": "footer-category",
                      "id": "IDNMAxJzO_5lztnip-footer-category__0",
                      "friendlyName": "Footer category 1",
                      "config": {
                        "categoryClassName": "nav-item"
                      },
                      "modules": {
                        "content": [
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__3",
                            "friendlyName": "Footer item 4",
                            "config": {
                              "heading": {
                                "text": "Fabrikam Store",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "Link text ",
                                "linkUrl": {}
                              }
                            }
                          },
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__0",
                            "friendlyName": "Footer item 1",
                            "config": {
                              "heading": {
                                "text": "",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "Store locations",
                                "linkUrl": {
                                  "destinationUrl": "/modern/storelocator",
                                  "type": "internalLink"
                                }
                              }
                            },
                            "data": {}
                          },
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__5",
                            "friendlyName": "Footer item 6",
                            "config": {
                              "heading": {
                                "text": "",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "Store hours",
                                "linkUrl": {
                                  "destinationUrl": "/modern/comingsoon",
                                  "type": "internalLink"
                                },
                                "ariaLabel": "store and hours"
                              }
                            },
                            "data": {}
                          },
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__4",
                            "friendlyName": "Footer item 5",
                            "config": {
                              "heading": {
                                "text": "",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "Store events",
                                "linkUrl": {
                                  "destinationUrl": "/modern/comingsoon",
                                  "type": "internalLink"
                                },
                                "ariaLabel": "Store events"
                              }
                            },
                            "data": {}
                          },
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__6",
                            "friendlyName": "Footer item 7",
                            "config": {
                              "heading": {
                                "text": "",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "Fabrikam store support",
                                "linkUrl": {
                                  "destinationUrl": "/modern/comingsoon",
                                  "type": "internalLink"
                                },
                                "ariaLabel": "Fabrikam store support"
                              }
                            },
                            "data": {}
                          }
                        ]
                      }
                    },
                    {
                      "typeName": "footer-category",
                      "id": "IDNMAxJzO_5lztnip-footer-category__1",
                      "friendlyName": "Footer category 2",
                      "config": {
                        "categoryClassName": "nav-item"
                      },
                      "modules": {
                        "content": [
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__7",
                            "friendlyName": "Footer item 8",
                            "config": {
                              "heading": {
                                "text": "About us",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "Link text ",
                                "linkUrl": {}
                              }
                            }
                          },
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__8",
                            "friendlyName": "Footer item 9",
                            "config": {
                              "heading": {
                                "text": "",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "Our story",
                                "linkUrl": {
                                  "destinationUrl": "/modern/comingsoon",
                                  "type": "internalLink"
                                },
                                "ariaLabel": "Our Story"
                              }
                            },
                            "data": {}
                          },
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__9",
                            "friendlyName": "Footer item 10",
                            "config": {
                              "heading": {
                                "text": "",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "Careers with Fabrikam",
                                "linkUrl": {
                                  "destinationUrl": "/modern/comingsoon",
                                  "type": "internalLink"
                                },
                                "ariaLabel": "Careers with fabrikam"
                              }
                            },
                            "data": {}
                          },
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__10",
                            "friendlyName": "Footer item 11",
                            "config": {
                              "heading": {
                                "text": "",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "News",
                                "linkUrl": {
                                  "destinationUrl": "/modern/comingsoon",
                                  "type": "internalLink"
                                },
                                "ariaLabel": "News"
                              }
                            },
                            "data": {}
                          }
                        ]
                      }
                    },
                    {
                      "typeName": "footer-category",
                      "id": "IDNMAxJzO_5lztnip-footer-category__2",
                      "friendlyName": "Footer category 3",
                      "config": {
                        "categoryClassName": "nav-item social-items"
                      },
                      "modules": {
                        "content": [
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__11",
                            "friendlyName": "Footer item 12",
                            "config": {
                              "heading": {
                                "text": "Follow us",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "Link text ",
                                "linkUrl": {}
                              }
                            }
                          },
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__12",
                            "friendlyName": "Footer item 13",
                            "config": {
                              "heading": {
                                "text": "",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "",
                                "linkUrl": {
                                  "destinationUrl": "https://www.facebook.com/",
                                  "type": "externalLink"
                                },
                                "ariaLabel": "facebook",
                                "openInNewTab": true
                              },
                              "image": {
                                "$type": "image",
                                "_id": "MCsDj",
                                "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCsDk?ver=4a92",
                                "copyright": "",
                                "altText": "Facebook",
                                "binaryHash": "eSssNLV3QebTU/R0wA1ZQA==",
                                "location": {},
                                "caption": "Facebook",
                                "title": "Facebook",
                                "imageSettings": {
                                  "quality": 80,
                                  "disableLazyLoad": false,
                                  "lazyload": true,
                                  "viewports": {
                                    "xs": {
                                      "w": 24,
                                      "h": 24,
                                      "q": "w=24&h=24&q=80&m=6&f=jpg",
                                      "qe": "t=w24h24qhigh"
                                    },
                                    "lg": {
                                      "w": 24,
                                      "h": 24,
                                      "q": "w=24&h=24&q=80&m=6&f=jpg",
                                      "qe": "t=w24h24qhigh"
                                    }
                                  }
                                }
                              }
                            },
                            "data": {}
                          },
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__13",
                            "friendlyName": "Footer item 14",
                            "config": {
                              "heading": {
                                "text": "",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "",
                                "linkUrl": {
                                  "destinationUrl": "https://twitter.com/",
                                  "type": "externalLink"
                                },
                                "ariaLabel": "twitter",
                                "openInNewTab": true
                              },
                              "image": {
                                "$type": "image",
                                "_id": "MCsDp",
                                "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCsDq?ver=8565",
                                "copyright": "",
                                "photographer": "multiple assets ",
                                "altText": "Twitter",
                                "binaryHash": "x/thwGAjcoA078mylVnrig==",
                                "location": {},
                                "caption": "Twitter",
                                "title": "Twitter",
                                "imageSettings": {
                                  "quality": 80,
                                  "disableLazyLoad": false,
                                  "lazyload": true,
                                  "viewports": {
                                    "xs": {
                                      "w": 24,
                                      "h": 24,
                                      "q": "w=24&h=24&q=80&m=6&f=jpg",
                                      "qe": "t=w24h24qhigh"
                                    },
                                    "lg": {
                                      "w": 24,
                                      "h": 24,
                                      "q": "w=24&h=24&q=80&m=6&f=jpg",
                                      "qe": "t=w24h24qhigh"
                                    }
                                  }
                                }
                              }
                            },
                            "data": {}
                          },
                          {
                            "typeName": "footer-item",
                            "id": "IDNMAxJzO_5lztnip-footer-item__14",
                            "friendlyName": "Footer item 15",
                            "config": {
                              "heading": {
                                "text": "",
                                "tag": "h2"
                              },
                              "link": {
                                "linkText": "",
                                "linkUrl": {
                                  "destinationUrl": "https://www.instagram.com/",
                                  "type": "externalLink"
                                },
                                "ariaLabel": "instagram",
                                "openInNewTab": true
                              },
                              "image": {
                                "$type": "image",
                                "_id": "MCsDm",
                                "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCsDn?ver=eb2a",
                                "copyright": "",
                                "altText": "Instagram",
                                "binaryHash": "GyWvoIQDWXVgg2Z+eTu9kQ==",
                                "location": {},
                                "caption": "Instagram",
                                "title": "Instagram",
                                "imageSettings": {
                                  "quality": 80,
                                  "disableLazyLoad": false,
                                  "lazyload": true,
                                  "viewports": {
                                    "xs": {
                                      "w": 24,
                                      "h": 24,
                                      "q": "w=24&h=24&q=80&m=6&f=jpg",
                                      "qe": "t=w24h24qhigh"
                                    },
                                    "lg": {
                                      "w": 24,
                                      "h": 24,
                                      "q": "w=24&h=24&q=80&m=6&f=jpg",
                                      "qe": "t=w24h24qhigh"
                                    }
                                  }
                                }
                              }
                            },
                            "data": {}
                          },
                          {
                            "typeName": "text-block",
                            "id": "IDNMAxJzO_5lztnip-text-block",
                            "friendlyName": "Text block",
                            "config": {}
                          }
                        ]
                      }
                    },
                    {
                      "typeName": "footer-category",
                      "id": "IDNMAxJzO_5lztnip-footer-category__3",
                      "friendlyName": "Footer category 4",
                      "config": {},
                      "modules": {
                        "content": [
                          {
                            "typeName": "back-top-footer",
                            "id": "IDNMAxJzO_5lztnip-back-top-footer",
                            "friendlyName": "Back to top",
                            "config": {
                              "destination": "#",
                              "ariaLabel": "Back to top"
                            }
                          }
                        ]
                      }
                    },
                    {
                      "typeName": "footer-category",
                      "id": "IDNMAxJzO_5lztnip-footer-category__4",
                      "friendlyName": "Footer category 5",
                      "config": {},
                      "modules": {
                        "content": [
                          {
                            "typeName": "back-top-footer",
                            "id": "IDNMAxJzO_5lztnip-back-top-footer__0",
                            "friendlyName": "Back to top 1",
                            "config": {
                              "destination": "#",
                              "ariaLabel": "Back to top",
                              "title": "Back to top"
                            }
                          }
                        ]
                      }
                    }
                  ]
                }
              }
            ],
            "primary": [
              {
                "typeName": "default-container",
                "id": "default-container__3",
                "friendlyName": "Carousel container",
                "config": {
                  "heading": {},
                  "layout": "stacked",
                  "containerType": "fluid",
                  "childrenWidth": "four"
                },
                "modules": {
                  "content": [
                    {
                      "typeName": "carousel",
                      "id": "carousel",
                      "friendlyName": "Carousel",
                      "config": {
                        "keyboard": true,
                        "pauseOnHover": true,
                        "autoplay": true,
                        "interval": 10000.0,
                        "transitionType": "slide"
                      },
                      "modules": {
                        "content": [
                          {
                            "typeName": "content-block",
                            "id": "content-block",
                            "friendlyName": "Content block",
                            "platform": {
                              "layout": "full-width"
                            },
                            "config": {
                              "imageLink": {
                                "type": "categoryLink",
                                "categoryId": 68719478038
                              },
                              "imageAriaLabel": "WOMEN'S WEAR",
                              "msdyn365__moduleLayout": "full-width",
                              "heading": {
                                "text": "NEW ARRIVALS",
                                "tag": "h2"
                              },
                              "paragraph": "<p>Into the summer breeze with all new designer dresses</p>\n",
                              "image": {
                                "$type": "image",
                                "_id": "MCsD4",
                                "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCsD5?ver=390b",
                                "altText": "Shop Women dresses",
                                "location": {
                                  "city": "IDNMAzuPC",
                                  "country": "IDNMAzuPC"
                                },
                                "copyright": "",
                                "binaryHash": "c+ZYz2gwOWSpZ7t+ES86gA==",
                                "title": "Hero",
                                "imageSettings": {
                                  "quality": 99,
                                  "disableLazyLoad": true,
                                  "lazyload": true,
                                  "viewports": {
                                    "xs": {
                                      "w": 800,
                                      "h": 600,
                                      "q": "w=800&h=600&q=99&m=6&o=t&x=1368&y=207&f=jpg",
                                      "qe": "t=w800h600qmax"
                                    },
                                    "sm": {
                                      "w": 1200,
                                      "h": 900,
                                      "q": "w=1200&h=900&q=99&m=6&o=t&x=1368&y=207&f=jpg",
                                      "qe": "t=w1200h900qmax"
                                    },
                                    "md": {
                                      "w": 1600,
                                      "h": 900,
                                      "q": "w=1600&h=900&q=99&m=6&o=t&x=1368&y=207&f=jpg",
                                      "qe": "t=w1600h900qmax"
                                    },
                                    "lg": {
                                      "w": 1600,
                                      "h": 700,
                                      "q": "w=1600&h=700&q=99&m=6&o=t&x=1368&y=207&f=jpg",
                                      "qe": "t=w1600h700qmax"
                                    },
                                    "xl": {
                                      "w": 1600,
                                      "h": 700,
                                      "q": "w=1600&h=700&q=99&m=6&o=t&x=1368&y=207&f=jpg",
                                      "qe": "t=w1600h700qmax"
                                    }
                                  }
                                }
                              },
                              "links": [
                                {
                                  "linkText": "Shop now",
                                  "linkUrl": {
                                    "type": "categoryLink",
                                    "categoryId": 68719478056
                                  },
                                  "ariaLabel": "Shop now"
                                }
                              ],
                              "actionableRegion": "imageAndLinks",
                              "className": "textplacement__left texttheme__dark hero"
                            },
                            "data": {}
                          },
                          {
                            "typeName": "content-block",
                            "id": "content-block__0",
                            "friendlyName": "Content block 1",
                            "platform": {
                              "layout": "full-width"
                            },
                            "config": {
                              "msdyn365__moduleLayout": "full-width",
                              "heading": {
                                "text": "NEW ARRIVAL",
                                "tag": "h2"
                              },
                              "paragraph": "<p>Tasteful accessories walking a fine line between vintage and modern</p>\n",
                              "image": {
                                "$type": "image",
                                "_id": "MCsDa",
                                "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCsDb?ver=b144",
                                "altText": "Fashion accessories",
                                "location": {},
                                "copyright": "",
                                "binaryHash": "taXs05QYHSJqv/RsGf2pfg==",
                                "title": "",
                                "imageSettings": {
                                  "quality": 80,
                                  "disableLazyLoad": true,
                                  "lazyload": true,
                                  "viewports": {
                                    "xs": {
                                      "w": 800,
                                      "h": 600,
                                      "q": "w=800&h=600&q=80&m=8&x=508&y=386&s=650&d=487&f=jpg",
                                      "qe": "t=w800h600qhigh"
                                    },
                                    "sm": {
                                      "w": 1200,
                                      "h": 900,
                                      "q": "w=1200&h=900&q=80&m=8&x=1266&y=574&s=650&d=487&f=jpg",
                                      "qe": "t=w1200h900qhigh"
                                    },
                                    "md": {
                                      "w": 1600,
                                      "h": 900,
                                      "q": "w=1600&h=900&q=80&m=8&x=1188&y=220&s=1667&d=938&f=jpg",
                                      "qe": "t=w1600h900qhigh"
                                    },
                                    "lg": {
                                      "w": 1600,
                                      "h": 700,
                                      "q": "w=1600&h=700&q=80&m=6&f=jpg",
                                      "qe": "t=w1600h700qhigh"
                                    },
                                    "xl": {
                                      "w": 1600,
                                      "h": 700,
                                      "q": "w=1600&h=700&q=80&m=6&f=jpg",
                                      "qe": "t=w1600h700qhigh"
                                    }
                                  }
                                }
                              },
                              "links": [
                                {
                                  "linkText": "Shop now",
                                  "linkUrl": {
                                    "type": "categoryLink",
                                    "categoryId": 68719478036
                                  },
                                  "ariaLabel": "Shop now"
                                }
                              ],
                              "className": "textplacement__center texttheme__light hero"
                            },
                            "data": {}
                          },
                          {
                            "typeName": "content-block",
                            "id": "content-block__1",
                            "friendlyName": "Content block 2",
                            "platform": {
                              "layout": "full-width"
                            },
                            "config": {
                              "msdyn365__moduleLayout": "full-width",
                              "heading": {
                                "text": "NEW ARRIVAL",
                                "tag": "h2"
                              },
                              "paragraph": "<p>Ethically sourced ultra-comfortable knitwear</p>\n",
                              "image": {
                                "$type": "image",
                                "_id": "MCsDg",
                                "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCsDh?ver=6c17",
                                "altText": "Lady wearing a sweater",
                                "location": {},
                                "copyright": "",
                                "binaryHash": "JIQlwNVbIFZTDgyP7dNQCg==",
                                "title": "",
                                "imageSettings": {
                                  "quality": 80,
                                  "disableLazyLoad": false,
                                  "lazyload": true,
                                  "viewports": {
                                    "xs": {
                                      "w": 800,
                                      "h": 600,
                                      "q": "w=800&h=600&q=80&m=6&o=t&x=2851&y=358&f=jpg",
                                      "qe": "t=w800h600qhigh"
                                    },
                                    "sm": {
                                      "w": 1200,
                                      "h": 900,
                                      "q": "w=1200&h=900&q=80&m=8&x=0&y=217&s=2158&d=1618&f=jpg",
                                      "qe": "t=w1200h900qhigh"
                                    },
                                    "md": {
                                      "w": 1600,
                                      "h": 900,
                                      "q": "w=1600&h=900&q=80&m=8&x=1423&y=405&s=2877&d=1618&f=jpg",
                                      "qe": "t=w1600h900qhigh"
                                    },
                                    "lg": {
                                      "w": 1600,
                                      "h": 700,
                                      "q": "w=1600&h=700&q=80&m=6&o=t&x=2851&y=358&f=jpg",
                                      "qe": "t=w1600h700qhigh"
                                    },
                                    "xl": {
                                      "w": 1600,
                                      "h": 700,
                                      "q": "w=1600&h=700&q=80&m=8&x=169&y=0&s=4145&d=1813&f=jpg",
                                      "qe": "t=w1600h700qhigh"
                                    }
                                  }
                                }
                              },
                              "links": [
                                {
                                  "linkText": "Shop Now",
                                  "linkUrl": {
                                    "type": "categoryLink",
                                    "categoryId": 68719478060
                                  },
                                  "ariaLabel": "shop now"
                                }
                              ],
                              "className": "textplacement__right hero"
                            },
                            "data": {}
                          }
                        ]
                      }
                    },
                    {
                      "typeName": "spacer",
                      "id": "spacer",
                      "friendlyName": "Spacer",
                      "config": {
                        "spacerHeight": "spacer2x"
                      }
                    }
                  ]
                }
              }
            ]
          }
        }
      ],
      "htmlHead": [
        {
          "typeName": "default-page-summary",
          "id": "default-page-summary",
          "friendlyName": "Default page summary",
          "config": {
            "title": "Fabrikam Homepage",
            "faviconUrl": null,
            "sharingImage": {
              "$type": "image",
              "_id": "MCsCM",
              "src": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/MCsCN?ver=0634",
              "altText": "Womenswear",
              "height": 586,
              "location": {},
              "quality": 99,
              "width": 427,
              "copyright": "",
              "binaryHash": "P3X0lj2LbdIXGf+WfOntWw==",
              "title": "",
              "imageSettings": {
                "quality": 80,
                "disableLazyLoad": false,
                "lazyload": true
              }
            }
          },
          "data": {}
        },
        {
          "typeName": "default-metatags",
          "id": "default-metatags",
          "friendlyName": "Metatags",
          "config": {
            "metaTags": [
              ""
            ]
          }
        },
        {
          "typeName": "default-inline-script",
          "id": "default-inline-script",
          "friendlyName": "Inline script",
          "config": {}
        },
        {
          "typeName": "default-inline-script",
          "id": "IDNLA3avC_5ckfr0n",
          "friendlyName": "Default inline script",
          "config": {
            "inlineScript": "(function(c,l,a,r,i,t,y){\n        c[a]=c[a]||function(){(c[a].q=c[a].q||[]).push(arguments)};\n        t=l.createElement(r);t.async=1;t.src=\"https://www.clarity.ms/tag/\"+i;\n        y=l.getElementsByTagName(r)[0];y.parentNode.insertBefore(t,y);\n    })(window, document, \"clarity\", \"script\", \"5dt6rq72p5\");"
          }
        }
      ]
    }
  },
  "renderingContext": {
    "User": {
      "AuthenticationType": 1,
      "IsAuthenticated": true,
      "EmailAddress": "pysrikanth773@gmail.com",
      "FirstName": "Srikanth 1",
      "LastName": "Pyna12",
      "MemberName": "Srikanth 1 Pyna12",
      "DisplayMemberName": "Srikanth 1 Pyna12",
      "ErrorMessages": null,
      "Puid": 0,
      "Muid": null,
      "AgeGroup": 0,
      "Cid": 0,
      "SignInUrl": "https://r1tie.fabrikam.com/modern/onerf/signin?EEL=True",
      "MeUrl": null,
      "Name": "Srikanth 1 Pyna12",
      "Country": null,
      "OptOutWebActivityTracking": "false",
      "IsB2b": false,
      "IsOBORequest": false,
      "ObjectIdentifier": "c248d952-1291-446e-bf06-1c74c00337ad",
      "CustomerAccountNumber": "009199",
      "CatalogId": "0",
      "RetailServerErrorMessage": null,
      "RetailServerErrorCorrelationId": null,
      "RetailServerErrorCode": null,
      "ChannelId": "68719478279",
      "SignOutUrl": "https://r1tie.fabrikam.com/modern/onerf/signout",
      "BirthDate": "0001-01-01T00:00:00",
      "WindowsLiveId": null,
      "CompactTicket": null,
      "XboxLiveCompactTicket": null,
      "Gamertag": null,
      "Xuid": null,
      "IsChild": false,
      "IsParent": false,
      "FamilyId": 0,
      "IsFamilySafetyUser": false,
      "ProxyTicket": null,
      "Token": "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsifQ.eyJleHAiOjE2NTE1NjQ3NzYsIm5iZiI6MTY1MTU2MTE3NiwidmVyIjoiMS4wIiwiaXNzIjoiaHR0cHM6Ly9sb2dpbi5mYWJyaWthbS5jb20vMDcxODA4YzMtMDIxMy00ZjQzLWI1ZTItZGYwMTI2NmU0NGFlL3YyLjAvIiwic3ViIjoiYzI0OGQ5NTItMTI5MS00NDZlLWJmMDYtMWM3NGMwMDMzN2FkIiwiYXVkIjoiNWY2M2Y1MGQtZDdlNy00YTRlLTg1MzQtZTg3NTk5ZmMwZmQ5Iiwibm9uY2UiOiI2Mzc4NzE1Nzk3MDk0MjU0MzAuTURkaU56RTBZbUl0TnpWaU15MDBNekl3TFdFeFpqZ3RNbUV3TUdRNVltWmlPR1k1TlRZellUUTVNMkV0TkROaE1TMDBNR1l4TFRsbU1tUXRPR0V3WkRabU5EbGlZMk01IiwiaWF0IjoxNjUxNTYxMTc2LCJhdXRoX3RpbWUiOjE2NTE1NjExNzYsIm9pZCI6ImMyNDhkOTUyLTEyOTEtNDQ2ZS1iZjA2LTFjNzRjMDAzMzdhZCIsImdpdmVuX25hbWUiOiJTcmlrYW50aCAxIiwiZmFtaWx5X25hbWUiOiJQeW5hMTIiLCJlbWFpbHMiOlsicHlzcmlrYW50aDc3M0BnbWFpbC5jb20iXSwidGZwIjoiQjJDXzFfc2lzdV9yMXRpZV9yZWNvbW1lbmRlZCIsImNfaGFzaCI6Ikc5QlRFOWF3ZnhxdkpOYzRGYXdFcUEifQ.LmgKpHKBYHuNqQ9so_DzYHvkue0-BY32E8f0Zfzt1Vxl7hW2sDBcavh4o12Nu6dWTcnd5_NeWnIIAJpGpAGKjwcLE9svI0zBu0_HVQ2WJLJqWJz8DWKsQLHpmhKn0zbQTbXC3M_zTbu0xP1pNHhNuHAiL0NyZJcKF5hxnfB2b07yToPGAGVp3mpQKCCICr8rjRzPbFeugioHFbwVOUoQ8HBEpbSC9--6mQ8RLjCeM75FYbarr600CUgbe1DwZXcWLLZYObbLS3XCFV2Y0TCCTzun5vitWbJKvYjGJpP1bWu7hq8Mi-nSElJv65YCHCfnGUtNRhPZ4rJ2C1Lpj_vlag",
      "SigninName": "Srikanth 1 Pyna12",
      "Claims": [
        {
          "Key": "exp",
          "Value": "1651564776"
        },
        {
          "Key": "nbf",
          "Value": "1651561176"
        },
        {
          "Key": "ver",
          "Value": "1.0"
        },
        {
          "Key": "iss",
          "Value": "https://login.fabrikam.com/071808c3-0213-4f43-b5e2-df01266e44ae/v2.0/"
        },
        {
          "Key": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
          "Value": "c248d952-1291-446e-bf06-1c74c00337ad"
        },
        {
          "Key": "aud",
          "Value": "5f63f50d-d7e7-4a4e-8534-e87599fc0fd9"
        },
        {
          "Key": "nonce",
          "Value": "637871579709425430.MDdiNzE0YmItNzViMy00MzIwLWExZjgtMmEwMGQ5YmZiOGY5NTYzYTQ5M2EtNDNhMS00MGYxLTlmMmQtOGEwZDZmNDliY2M5"
        },
        {
          "Key": "iat",
          "Value": "1651561176"
        },
        {
          "Key": "auth_time",
          "Value": "1651561176"
        },
        {
          "Key": "http://schemas.microsoft.com/identity/claims/objectidentifier",
          "Value": "c248d952-1291-446e-bf06-1c74c00337ad"
        },
        {
          "Key": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname",
          "Value": "Srikanth 1"
        },
        {
          "Key": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname",
          "Value": "Pyna12"
        },
        {
          "Key": "emails",
          "Value": "pysrikanth773@gmail.com"
        },
        {
          "Key": "tfp",
          "Value": "B2C_1_sisu_r1tie_recommended"
        },
        {
          "Key": "c_hash",
          "Value": "G9BTE9awfxqvJNc4FawEqA"
        },
        {
          "Key": "ChannelId",
          "Value": "68719478279"
        },
        {
          "Key": "CustomerAccountNumber",
          "Value": "009199"
        },
        {
          "Key": "CatalogId",
          "Value": "0"
        },
        {
          "Key": "IsB2b",
          "Value": "False"
        },
        {
          "Key": "OptOutWebActivityTracking",
          "Value": "false"
        }
      ],
      "Roles": [],
      "OrganizationId": "",
      "TenantId": null
    },
    "DeviceType": "Pc",
    "experiments": {
      "userId": "7c5eaf75-bf62-fd46-4c27-7b3ae327c58f",
      "activeExperiments": [
        {
          "experimentId": "20615480142",
          "variantId": "20616940412",
          "moduleId": ""
        }
      ]
    },
    "tuid": "7e0ae077-c82a-c0f9-989e-3aaadfd86375",
    "segmentRequestId": null,
    "missingSegments": {},
    "requestUrl": "https://r1tie.fabrikam.com/modern",
    "sitePath": "/modern",
    "CrossChannelIdentity": [],
    "clientIPAddress": "49.205.229.247",
    "clientContext": {
      "deviceType": "Pc",
      "isBot": false,
      "requestUserAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36"
    },
    "canonicalUrl": "https://r1tie.fabrikam.com/modern",
    "canonicalDomain": "r1tie.fabrikam.com",
    "staticContext": {
      "staticCdnUrl": "/_scnr/",
      "staticCdnUrlWithLocale": "/en-us/_msdyn365/_scnr/"
    },
    "user": {
      "token": "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsifQ.eyJleHAiOjE2NTE1NjQ3NzYsIm5iZiI6MTY1MTU2MTE3NiwidmVyIjoiMS4wIiwiaXNzIjoiaHR0cHM6Ly9sb2dpbi5mYWJyaWthbS5jb20vMDcxODA4YzMtMDIxMy00ZjQzLWI1ZTItZGYwMTI2NmU0NGFlL3YyLjAvIiwic3ViIjoiYzI0OGQ5NTItMTI5MS00NDZlLWJmMDYtMWM3NGMwMDMzN2FkIiwiYXVkIjoiNWY2M2Y1MGQtZDdlNy00YTRlLTg1MzQtZTg3NTk5ZmMwZmQ5Iiwibm9uY2UiOiI2Mzc4NzE1Nzk3MDk0MjU0MzAuTURkaU56RTBZbUl0TnpWaU15MDBNekl3TFdFeFpqZ3RNbUV3TUdRNVltWmlPR1k1TlRZellUUTVNMkV0TkROaE1TMDBNR1l4TFRsbU1tUXRPR0V3WkRabU5EbGlZMk01IiwiaWF0IjoxNjUxNTYxMTc2LCJhdXRoX3RpbWUiOjE2NTE1NjExNzYsIm9pZCI6ImMyNDhkOTUyLTEyOTEtNDQ2ZS1iZjA2LTFjNzRjMDAzMzdhZCIsImdpdmVuX25hbWUiOiJTcmlrYW50aCAxIiwiZmFtaWx5X25hbWUiOiJQeW5hMTIiLCJlbWFpbHMiOlsicHlzcmlrYW50aDc3M0BnbWFpbC5jb20iXSwidGZwIjoiQjJDXzFfc2lzdV9yMXRpZV9yZWNvbW1lbmRlZCIsImNfaGFzaCI6Ikc5QlRFOWF3ZnhxdkpOYzRGYXdFcUEifQ.LmgKpHKBYHuNqQ9so_DzYHvkue0-BY32E8f0Zfzt1Vxl7hW2sDBcavh4o12Nu6dWTcnd5_NeWnIIAJpGpAGKjwcLE9svI0zBu0_HVQ2WJLJqWJz8DWKsQLHpmhKn0zbQTbXC3M_zTbu0xP1pNHhNuHAiL0NyZJcKF5hxnfB2b07yToPGAGVp3mpQKCCICr8rjRzPbFeugioHFbwVOUoQ8HBEpbSC9--6mQ8RLjCeM75FYbarr600CUgbe1DwZXcWLLZYObbLS3XCFV2Y0TCCTzun5vitWbJKvYjGJpP1bWu7hq8Mi-nSElJv65YCHCfnGUtNRhPZ4rJ2C1Lpj_vlag",
      "isAuthenticated": true,
      "isSignedIn": true,
      "signInUrl": "https://r1tie.fabrikam.com/_msdyn365/signin",
      "signOutUrl": "https://r1tie.fabrikam.com/_msdyn365/signout",
      "signUpUrl": "https://r1tie.fabrikam.com/_msdyn365/signup",
      "editProfileUrl": "https://r1tie.fabrikam.com/_msdyn365/editprofile",
      "signinName": "Srikanth 1 Pyna12",
      "firstName": "Srikanth 1",
      "lastName": "Pyna12",
      "optOutWebActivityTracking": false,
      "isB2b": false,
      "isOBORequest": false,
      "tenantId": null,
      "customerAccountNumber": "009199",
      "catalogId": "0",
      "name": "Srikanth 1 Pyna12",
      "emailAddress": "pysrikanth773@gmail.com",
      "retailServerErrorCode": null,
      "retailServerErrorMessage": null,
      "userClaims": {}
    },
    "previewContext": null,
    "telemetryPageName": "Homepage",
    "locale": "en-us",
    "textDirection": "ltr",
    "suggestedMarket": "IN",
    "marketSettings": {
      "countryRegion": "IN",
      "localeItems": [
        {
          "locale": "es",
          "localeBaseUrl": "r1tie.fabrikam.com/modern/es",
          "isDefaultLocale": false
        },
        {
          "locale": "en-nz",
          "localeBaseUrl": "r1tie.fabrikam.com/modern/en-nz",
          "isDefaultLocale": false
        },
        {
          "locale": "en-us",
          "localeBaseUrl": "r1tie.fabrikam.com/deleteme/2",
          "isDefaultLocale": false
        }
      ],
      "currentBaseUrl": "r1tie.fabrikam.com/modern",
      "isAutoRedirectEnabled": false
    },
    "urlTokens": {},
    "apiSettings": {
      "baseUrl": "https://comme2e-tie-ring1f9397e098fb7bd4fret.cloud.retail.test.dynamics.com/",
      "retailServerEndpointOverrideDomain": null,
      "channelId": 68719478279,
      "eCommerceChannelId": "rpbpwaytjjr",
      "eCommerceCrossChannelId": null,
      "catalogId": 0,
      "oun": "128",
      "baseImageUrl": "https://images-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/imageFileData/search?fileName=/",
      "ratingsReviewsEndpoint": "https://71d4e2df-e581-4d2a-8131-801e5dc8d536.rnr-ppe.ms",
      "ratingsReviewsProxyEndpoint": "https://r1tie.fabrikam.com/_msdyn365/rnr/",
      "rnr": {
        "id": "71d4e2df-e581-4d2a-8131-801e5dc8d536",
        "url": "https://71d4e2df-e581-4d2a-8131-801e5dc8d536.rnr-ppe.ms",
        "proxyUrl": "https://r1tie.fabrikam.com/_msdyn365/rnr/"
      },
      "externalCmsBaseUrls": null
    },
    "gridSettings": {
      "xs": {
        "w": 768
      },
      "sm": {
        "w": 991
      },
      "md": {
        "w": 1199
      },
      "lg": {
        "w": 1599
      },
      "xl": {
        "w": 1600
      }
    },
    "debugMode": 0,
    "operationId": "308b948d68699f41b11380db110eb691",
    "siteTheme": "fabrikam",
    "sameSiteRequired": true,
    "features": {
      "enable_react_statics_in_body": true,
      "allow_react_statics_in_body": true,
      "async_chunks": true,
      "defer_chunks": true,
      "enable_react_statics_in_body123": true,
      "enable_service_worker": true,
      "disable_cors": true,
      "disable_routeerrors_cache": true,
      "disable_render_result_compression": true,
      "enableAuthoringRemoveAndAddModule": true,
      "enable_performance_analysis": true,
      "enable_render_cache_all": true,
      "clientRender_modules": true,
      "scnr_blob": true,
      "action_timeout": true,
      "client_optimize_hydrate": true,
      "experimentation": true,
      "enableDataAnalytics": true
    },
    "siteStylePreset": "https://files-us-ppe.cms.commerce.dynamics.com/cms/api/gfhwnkhdlh/binary/MCkgOm",
    "pageFragmentStylePresetList": [],
    "telemetrySettings": {
      "eventHubEndPoint": "https://CDI-EventHubs-INT.servicebus.windows.net/ms-eventhub-ingestion/publishers/r1tie/messages",
      "eventHubAuthToken": "U2hhcmVkQWNjZXNzU2lnbmF0dXJlIHNyPUNESS1FdmVudEh1YnMtSU5ULnNlcnZpY2VidXMud2luZG93cy5uZXQlMmZtcy1ldmVudGh1Yi1pbmdlc3Rpb24lMmZwdWJsaXNoZXJzJTJmcjF0aWUlMmZtZXNzYWdlcyZzaWc9ZzFDOGNwU1p3cUZrYXlOZHpwbjNBJTJmWjlNM1puVmZEWmp6Q24zVmxkNkJzJTNkJnNlPTE2NTE2NDc1MzYmc2tuPVNlbmRPbmx5UG9saWN5",
      "operationId": "308b948d68699f41b11380db110eb691",
      "url": "_msdyn365/_scnr/global-statics/static/js/da.0.js",
      "environmentId": "r1tie",
      "commerceCoreEnvId": "71d4e2df-e581-4d2a-8131-801e5dc8d536",
      "operationalInsightsInstrumentationKey": ""
    }
  },
  "statusCode": 200,
  "redirectUrl": null,
  "appContext": {
    "config": {
      "isBulkPurchaseEnabled": true,
      "enableStockCheck": true,
      "inventoryLevel": "totalAvailable",
      "inventoryRanges": "all",
      "warehouseAggregation": "individual",
      "outOfStockThreshold": 0.0,
      "productListInventoryDisplay": "default",
      "maxQuantityForCartLineItem": 10.0,
      "hideRating": false,
      "geolocationApiUrl": "https://dev.virtualearth.net/REST/v1/",
      "reviewTextMaxLength": 500.0,
      "reviewTitleMaxLength": 500.0,
      "disableTooltip": true,
      "searchQueryStringParameter": "q",
      "searchInputMaxLength": 5.0,
      "addToCartBehavior": "showModal",
      "giftCardSupported": "external",
      "breadcrumbType": "categoryAndBack",
      "shouldUseNewImageComponent": true,
      "placeholderImageName": "Placeholder.png",
      "unitOfMeasureDisplayType": "buybox",
      "shouldEnableSinglePaymentAuthorizationCheckout": false,
      "dimensionsAsSwatchType": [
        "color",
        "size",
        "style"
      ],
      "dimensionsInProductCard": [
        "color",
        "size",
        "style"
      ],
      "dimensionToPreSelectInProductCard": "color",
      "isEnableShowOrHideSalesTaxECommerceEnabled": true
    },
    "routes": {
      "account": {
        "destinationUrl": "/modern/myaccount",
        "type": "internalLink"
      },
      "accountProfile": {
        "destinationUrl": "/modern/profile-edit",
        "type": "internalLink"
      },
      "backToShopping": {
        "destinationUrl": "/modern/home",
        "type": "internalLink"
      },
      "cart": {
        "destinationUrl": "/modern/cart",
        "type": "internalLink"
      },
      "checkout": {
        "destinationUrl": "/modern/checkout",
        "type": "internalLink"
      },
      "home": {
        "destinationUrl": "/modern/",
        "type": "internalLink"
      },
      "loyalty": {
        "destinationUrl": "/modern/loyalty",
        "type": "internalLink"
      },
      "loyaltyJoin": {
        "destinationUrl": "/modern/loyalty-join",
        "type": "internalLink"
      },
      "orderConfirmation": {
        "destinationUrl": "/modern/orderconfirmation",
        "type": "internalLink"
      },
      "orderDetails": {
        "destinationUrl": "/modern/orderdetails",
        "type": "internalLink"
      },
      "search": {
        "destinationUrl": "/modern/search",
        "type": "internalLink"
      },
      "wishlist": {
        "destinationUrl": "/modern/wishlist",
        "type": "internalLink"
      },
      "loyaltyTerms": {
        "destinationUrl": "/modern/loyalty-terms",
        "type": "internalLink"
      }
    },
    "platform": {
      "dnsPrefetchUrls": [
        "optimizely.com"
      ],
      "maintenanceMode": false,
      "enableBingMapGeoLoation": true,
      "disableServerSideErrorChecking": false,
      "enableRenderCaching": true,
      "disableCookieCompliance": false,
      "cartSessionExpiration": 0.0,
      "imageQuality": 80.0,
      "maxDepth": 200.0,
      "skipToMainText": "Skip to main",
      "enableCustomerAccountPayment": "all",
      "enableDefaultOrderQuantityLimits": "b2b",
      "parameterizedUrlPaths": [
        "/tv/episodes"
      ]
    },
    "contentSecurityPolicy": {
      "disableContentSecurityPolicy": true
    }
  }
}

```

## Third party Image and Video Usage restrictions

The software may include third party images and videos that are for personal use only and may not be copied except as provided by Microsoft within the demo websites.  You may install and use an unlimited number of copies of the demo websites., You may not publish, rent, lease, lend, or redistribute any images or videos without authorization from the rights holder, except and only to the extent that the applicable copyright law expressly permits doing so.
