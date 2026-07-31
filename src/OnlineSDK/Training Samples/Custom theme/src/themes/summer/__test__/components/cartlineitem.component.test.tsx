/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { SimpleProduct } from '@msdyn365-commerce/retail-proxy';
import { mount } from 'enzyme';
import * as React from 'react';

import CartLine, { ICartLineProps } from '../../views/components/cartlineitem.component';

const mockActionContext = {
    app: {
        config: {
            unitOfMeasureDisplayType: 'none'
        }
    },
    actionContext: {
        requestContext: {
            apiSettings: {
                baseImageUrl: 'https://cms-ppe-imageresizer-mr.trafficmanager.net/cms/api/fabrikamsb/imageFileData/search?fileName=/'
            }
        }
    },
    request: {
        gridSettings: {
            xs: { w: 767, h: 0 },
            sm: { w: 991, h: 0 },
            md: { w: 1199, h: 0 },
            lg: { w: 1599, h: 0 },
            xl: { w: 1600, h: 0 }
        },
        apiSettings: {
            channelId: 5637145359
        },
        channel: {
            PickupDeliveryModeCode: '70',
            channelDeliveryOptionConfig: 'channelDeliveryOptionConfig'
        },
        user: {
            isB2b: false
        }
    },
    cultureFormatter: {
        formatCurrency: jest.fn()
    }
};
const resource = {
    discountStringText: '',
    sizeString: '',
    colorString: '',
    configString: '',
    styleString: '',
    amountString: '',
    quantityDisplayString: '',
    inputQuantityAriaLabel: '',
    decrementButtonAriaLabel: '',
    incrementButtonAriaLabel: '',
    originalPriceText: '',
    currentPriceText: '',
    shippingChargesText: ''
};
const mockFunciton = jest.fn();
const simpleProduct: SimpleProduct = {
    Name: 'Crew neck server',
    RecordId: 123,
    IsGiftCard: true,
    BasePrice: 25,
    Price: 25,
    AdjustedPrice: 25,
    ProductTypeValue: 1,
    Dimensions: [
        {
            DimensionTypeValue: 3,
            DimensionValue: {
                RecordId: 1,
                Value: 'S'
            }
        },
        {
            DimensionTypeValue: 1,
            DimensionValue: {
                RecordId: 2,
                Value: 'Brown'
            }
        },
        {
            DimensionTypeValue: 2,
            DimensionValue: {
                RecordId: 2,
                Value: 'Basic'
            }
        },
        {
            DimensionTypeValue: 4,
            DimensionValue: {
                RecordId: 3,
                Value: 'Large'
            }
        },
        {
            DimensionTypeValue: 10,
            DimensionValue: {
                RecordId: 3,
                Value: 'Large'
            }
        }
    ]
};

const cartLine = {
    Quantity: 1,
    Description: 'a',
    LineId: 12,
    Price: 10,
    NetAmountWithoutTax: 25,
    DiscountAmount: 10,
    DiscountLines: [
        { SaleLineNumber: 123, EffectivePercentage: 10 },
        { SaleLineNumber: 1233, EffectivePercentage: 0 },
        { SaleLineNumber: 123, EffectivePercentage: 10, DiscountLineTypeValue: 5 }
    ],
    NetPrice: 27,
    DeliveryMode: 1,
    ChargeLines: [
        {
            Description: 'Test Charge 1',
            IsShipping: false,
            CalculatedAmount: 1.5
        },
        {
            Description: 'Test Charge 2',
            IsShipping: true,
            CalculatedAmount: 0.8
        }
    ]
};
describe('Cartline', () => {
    it('renders correctly with change event', () => {
        const moduleProps: ICartLineProps = {
            isSalesLine: true,
            isCartStateReady: true,

            // @ts-expect-error
            imageSettings: {
                cropFocalRegion: true
            },
            channelDeliveryOptionConfig: {
                PickupDeliveryModeCodes: ['PickupDeliveryModeCodes']
            },
            showShippingChargesForLineItems: true,
            productUrl: 'http://www.xyz.com',
            resources: resource,
            typeName: 'CartLineItem',

            // @ts-expect-error
            context: mockActionContext,
            inventoryInformationLabel: undefined,
            isQuantityEditable: true,
            quantityOnChange: mockFunciton,
            data: {
                product: simpleProduct,

                // @ts-expect-error
                cartLine
            }
        };
        const wrapper = mount(<CartLine {...moduleProps} />);
        const quantityWrapper = wrapper.find('.quantity');
        const dataIsGift = {
            product: { ...simpleProduct, IsGiftCard: false },
            cartLine: { ...cartLine, ChargeLines: [] }
        };
        wrapper.setProps({ data: dataIsGift });
        quantityWrapper.find('.quantity-input').simulate('change', { target: { value: 123 } });
        setTimeout(() => {
            expect(mockFunciton).toHaveBeenCalled();
        });
    });
    it('renders correctly with change event without callback', () => {
        mockActionContext.app.config.unitOfMeasureDisplayType = 'xyz';
        const moduleProps: ICartLineProps = {
            isSalesLine: true,
            isCartStateReady: true,

            // @ts-expect-error
            imageSettings: {
                cropFocalRegion: true
            },
            showShippingChargesForLineItems: true,
            productUrl: 'http://www.xyz.com',
            resources: resource,
            typeName: 'CartLineItem',

            // @ts-expect-error
            context: mockActionContext,
            isQuantityEditable: true,
            errorMessage: 'xyz',
            inventoryInformationLabel: 'inventoryInformationLabel',
            data: {
                product: simpleProduct,

                // @ts-expect-error
                cartLine: { cartLine, IsInvoiceLine: false }
            }
        };
        const wrapper = mount(<CartLine {...moduleProps} />);
        const quantityWrapper = wrapper.find('.quantity');
        quantityWrapper.find('.quantity-input').simulate('change', { target: { value: 123 } });
        expect(wrapper).toMatchSnapshot();
    });
    it('renders correctly with isOutOfStock', () => {
        mockActionContext.app.config.unitOfMeasureDisplayType = 'xyz';
        const moduleProps: ICartLineProps = {
            isSalesLine: true,
            isCartStateReady: true,

            // @ts-expect-error
            imageSettings: {
                cropFocalRegion: true
            },
            showShippingChargesForLineItems: true,
            productUrl: 'http://www.xyz.com',
            resources: resource,
            typeName: 'CartLineItem',

            // @ts-expect-error
            context: mockActionContext,
            isQuantityEditable: false,
            errorMessage: 'xyz',
            isOutOfStock: false,
            inventoryInformationLabel: 'inventoryInformationLabel',
            data: {
                product: { ...simpleProduct, DefaultUnitOfMeasure: 'xyz' },

                // @ts-expect-error
                cartLine: { ...cartLine, IsInvoiceLine: false }
            }
        };
        const wrapperisOutOfStock = mount(<CartLine {...moduleProps} />);
        expect(wrapperisOutOfStock).toMatchSnapshot();
    });
    it('renders correctly without Product', () => {
        const moduleProps: ICartLineProps = {
            isSalesLine: true,
            isCartStateReady: true,

            // @ts-expect-error
            imageSettings: {
                cropFocalRegion: true
            },
            showShippingChargesForLineItems: true,
            productUrl: 'http://www.xyz.com',
            resources: resource,
            typeName: 'CartLineItem',

            // @ts-expect-error
            context: mockActionContext,
            isQuantityEditable: false,
            errorMessage: 'xyz',
            isOutOfStock: false,
            inventoryInformationLabel: 'inventoryInformationLabel',
            data: {
                // @ts-expect-error
                product: { RecordId: 123 },

                // @ts-expect-error
                cartLine: { ...cartLine, IsInvoiceLine: true }
            }
        };
        const wrapperisOutOfStock = mount(<CartLine {...moduleProps} />);
        expect(wrapperisOutOfStock).toMatchSnapshot();
    });
    it('renders correctly with isOutOfStock product', () => {
        mockActionContext.app.config.unitOfMeasureDisplayType = 'xyz';
        const moduleProps: ICartLineProps = {
            isSalesLine: true,
            isCartStateReady: true,

            // @ts-expect-error
            imageSettings: {
                cropFocalRegion: true
            },
            showShippingChargesForLineItems: true,
            productUrl: 'http://www.xyz.com',
            resources: resource,
            typeName: 'CartLineItem',

            // @ts-expect-error
            context: mockActionContext,
            isQuantityEditable: false,
            errorMessage: 'xyz',
            isOutOfStock: true,
            inventoryInformationLabel: 'inventoryInformationLabel',
            data: {
                product: { ...simpleProduct, DefaultUnitOfMeasure: 'xyz' },

                // @ts-expect-error
                cartLine
            }
        };
        const wrapperisOutOfStock = mount(<CartLine {...moduleProps} />);
        expect(wrapperisOutOfStock).toMatchSnapshot();
    });
});
