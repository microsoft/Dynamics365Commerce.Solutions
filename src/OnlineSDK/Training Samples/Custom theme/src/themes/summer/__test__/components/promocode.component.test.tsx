/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { ICartState } from '@msdyn365-commerce/global-state';
import { mount } from 'enzyme';
import * as React from 'react';

import PromoCode, { IPromoCodeProps } from '../../views/components/promocode.component';

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
        }
    },
    cultureFormatter: {
        formatCurrency: jest.fn()
    }
};
const mockFunciton = jest.fn();

const mockActiveCart = {
    Id: 'gB8DCi6AK3mzMavh08C4P18wFDbEEyto',
    Version: 123,
    Coupons: ['a', 'b'],
    cart: {
        CartLines: [{ LineId: 123, DiscountLines: [{ OfferId: 123, DiscountCode: 'CodeXyz', DiscountCost: 10 }] }]
    }
};

const moduleProps: IPromoCodeProps = {
    cart: {
        totalItemsInCart: 3,
        isEmpty: false,

        // @ts-expect-error
        cart: mockActiveCart,
        CartLines: [{ LineId: 123 }],
        addPromoCode: jest.fn().mockImplementation(async () => Promise.resolve({ status: 'SUCCESS' })),
        removePromoCodes: jest.fn().mockImplementation(async () => Promise.resolve({ status: 'SUCCESS' }))
    },
    promoCodeHeadingText: '',

    // @ts-expect-error
    context: mockActionContext,
    appliedPromoCodeHeadingText: '',
    removePromoAriaLabelFormat: 'removePromoAriaLabelFormat',
    promoPlaceholderText: 'promoPlaceholderText',
    promoCodeApplyButtonText: 'promoCodeApplyButtonText',
    collapseTimeOut: 1,
    removePromoText: 'removePromoText',
    invalidPromoCodeErrorText: 'invalidPromoCodeErrorText',
    failedToAddPromoCodeErrorText: 'failedToAddPromoCodeErrorText',
    duplicatePromoCodeErrorText: 'duplicatePromoCodeErrorText ',
    failedToRemovePromoCodeErrorText: 'failedToRemovePromoCodeErrorText',
    promoCodeApplyCallback: mockFunciton
};

describe('Cartline', () => {
    it('renders correctly with change event', () => {
        const wrapper = mount(<PromoCode {...moduleProps} />);

        // Submit handler
        wrapper.find('.msc-promo-code__form-container').simulate('submit');
        wrapper.setProps({ removePromoAriaLabelFormat: false });
        wrapper.find('.msc-promo-code__input-box').simulate('change', { target: { value: 123 } });
        wrapper.find('button.msc-promo-code__apply-btn').simulate('click');

        wrapper.find('.msc-promo-code__input-box').simulate('change', { target: { value: '' } });
        wrapper.find('button.msc-promo-code__apply-btn').simulate('click');

        wrapper.setProps({ cart: undefined });
        wrapper.find('.msc-promo-code__input-box').simulate('change', { target: { value: 1234 } });
        wrapper.find('button.msc-promo-code__apply-btn').simulate('click');

        const cart: ICartState = {
            totalItemsInCart: 3,
            isEmpty: false,

            // @ts-expect-error
            cart: mockActiveCart,
            addPromoCode: jest.fn().mockImplementation(async () => Promise.resolve({ substatus: 'ALREADYADDED' }))
        };

        wrapper.setProps({ cart });
        wrapper.find('.msc-promo-code__input-box').simulate('change', { target: { value: 1234 } });
        wrapper.find('button.msc-promo-code__apply-btn').simulate('click');

        const cartFail: ICartState = {
            totalItemsInCart: 3,
            isEmpty: false,

            // @ts-expect-error
            cart: mockActiveCart,
            addPromoCode: jest.fn().mockImplementation(async () => Promise.resolve({ status: 'Fail' }))
        };

        wrapper.setProps({ cart: cartFail });
        wrapper.find('.msc-promo-code__input-box').simulate('change', { target: { value: 1234 } });
        wrapper.find('button.msc-promo-code__apply-btn').simulate('click');

        const cartFailError: ICartState = {
            totalItemsInCart: 3,
            isEmpty: false,

            // @ts-expect-error
            cart: mockActiveCart,
            addPromoCode: jest.fn().mockImplementation(async () => Promise.reject(new Error('Error')))
        };

        wrapper.setProps({ cart: cartFailError });
        wrapper.find('.msc-promo-code__input-box').simulate('change', { target: { value: 1234 } });
        wrapper.find('button.msc-promo-code__apply-btn').simulate('click');

        expect(wrapper).toMatchSnapshot();
    });

    it('renders correctly remove', () => {
        const wrapper = mount(<PromoCode {...moduleProps} />);
        wrapper
            .find('button.msc-promo-code__line__btn-remove')
            .at(1)
            .simulate('click');
        const instance = wrapper.instance() as PromoCode;
        const cartFail: ICartState = {
            totalItemsInCart: 3,
            isEmpty: false,

            // @ts-expect-error
            cart: mockActiveCart,
            addPromoCode: jest.fn().mockImplementation(async () => Promise.reject(new Error('Error'))),
            removePromoCodes: jest.fn().mockImplementation(async () => Promise.reject(new Error('Error')))
        };
        wrapper.setProps({ cart: cartFail });
        wrapper
            .find('button.msc-promo-code__line__btn-remove')
            .at(1)
            .simulate('click');

        // @ts-expect-error
        // eslint-disable-next-line  @typescript-eslint/no-confusing-void-expression -- have to export this as this utility is used in fabrikam tests
        const removePromotion = instance._removePromotion(undefined, {});
        expect(removePromotion).toBeUndefined();

        // @ts-expect-error
        // eslint-disable-next-line  @typescript-eslint/no-confusing-void-expression -- have to export this as this utility is used in fabrikam tests
        const removePromotionError = instance._removePromotion(cartFail, { currentTarget: { getAttribute: mockFunciton } });
        expect(removePromotionError).toBeUndefined();

        // @ts-expect-error
        const discountAmount = instance._calculateDiscount('CodeXyz', mockActiveCart);
        expect(discountAmount).toBe(-10);
    });
});
