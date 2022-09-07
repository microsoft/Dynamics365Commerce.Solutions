/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { IAddToCartFailureResult } from '@msdyn365-commerce/components';
import { IAny, ICoreContext, IGeneric } from '@msdyn365-commerce/core';
import { ICartState } from '@msdyn365-commerce/global-state';
import { OrgUnitLocation, ProductAvailableQuantity, SimpleProduct } from '@msdyn365-commerce/retail-proxy';

import { IBuyboxCommonResources } from '../..';
import { getDimensionErrorString, getGenericError, getQuantityLimit } from '../error-utilities';
import * as AsyncResult from '../error-utilities';

describe('Error Utilities', () => {
    it('should render getQuantityLimit', () => {
        const context = ({
            app: {
                config: {
                    maxQuantityForCartLineItem: undefined
                }
            }
        } as unknown) as ICoreContext<IGeneric<IAny>>;

        const context2 = ({
            app: {
                config: {
                    maxQuantityForCartLineItem: null
                }
            }
        } as unknown) as ICoreContext<IGeneric<IAny>>;
        const wrapper = getQuantityLimit(context, {
            AvailableQuantity: undefined
        });

        const wrapper2 = getQuantityLimit(context2, {
            AvailableQuantity: undefined
        });

        expect(wrapper).toBeDefined();
        expect(wrapper2).toBeDefined();
    });

    it('getGenericError', () => {
        const result = {
            failureReason: 'CARTACTIONFAILED',
            cartActionResult: {
                substatus: 'MAXQUANTITY',
                validationResults: [
                    {
                        LocalizedMessage: 'test',
                        MemberNames: ['test1', 'test2'],
                        ErrorContext: 'error',
                        ErrorResourceId: '1'
                    },
                    {
                        LocalizedMessage: 'test',
                        MemberNames: ['test1', 'test2'],
                        ErrorContext: 'error',
                        ErrorResourceId: '2'
                    }
                ]
            }
        } as IAddToCartFailureResult;
        const cartData = {
            cart: {
                Id: '1234',
                CartLines: [
                    {
                        LineId: '1234',
                        ProductId: 2,
                        DeliveryMode: '60',
                        FulfillmentStoreId: '1'
                    },
                    {
                        LineId: '2345',
                        ProductId: 1,
                        DeliveryMode: '70',
                        FulfillmentStoreId: '1'
                    }
                ],
                Coupons: [
                    {
                        Code: 'WeeklyAd'
                    },
                    {
                        Code: 'Take20'
                    }
                ]
            }
        } as ICartState | undefined;
        const resources = {
            maxQuantityLimitText: 'Max reached'
        } as IBuyboxCommonResources;
        const coreContext = ({
            actionContext: {
                requestContext: {
                    channel: {
                        PickupDeliveryModeCode: '70'
                    }
                }
            },
            app: {
                config: {
                    maxQuantityForCartLineItem: 4
                }
            }
        } as unknown) as ICoreContext<IGeneric<IAny>>;
        const product = {
            RecordId: 1
        } as SimpleProduct | undefined;
        const productAvailability = {} as ProductAvailableQuantity | undefined;
        const location = {
            OrgUnitNumber: '1'
        } as OrgUnitLocation | undefined;
        jest.spyOn(AsyncResult, 'getElementIndex').mockReturnValue(1);
        const wrapper = getGenericError(result, cartData, resources, coreContext, product, productAvailability, location);
        expect(wrapper).toBeDefined();
    });

    it('getGenericError with validationResults', () => {
        const result = {
            failureReason: 'CARTACTIONFAILED',
            cartActionResult: {
                substatus: 'NOTFOUND',
                validationResults: [
                    {
                        LocalizedMessage: 'test',
                        MemberNames: ['test1', 'test2'],
                        ErrorContext: 'error',
                        ErrorResourceId: '1'
                    },
                    {
                        LocalizedMessage: 'test',
                        MemberNames: ['test1', 'test2'],
                        ErrorContext: 'error',
                        ErrorResourceId: '2'
                    }
                ]
            }
        } as IAddToCartFailureResult;
        const cartData = {
            cart: {
                Id: '1234',
                CartLines: [
                    {
                        LineId: '1234',
                        ProductId: 2,
                        DeliveryMode: '60',
                        FulfillmentStoreId: '1'
                    },
                    {
                        LineId: '2345',
                        ProductId: 1,
                        DeliveryMode: '70',
                        FulfillmentStoreId: '1'
                    }
                ],
                Coupons: [
                    {
                        Code: 'WeeklyAd'
                    },
                    {
                        Code: 'Take20'
                    }
                ]
            }
        } as ICartState | undefined;
        const resources = {} as IBuyboxCommonResources;
        const coreContext = {
            actionContext: {
                requestContext: {
                    channel: {
                        PickupDeliveryModeCode: '70'
                    }
                }
            }
        } as ICoreContext<IGeneric<IAny>>;
        const product = {
            RecordId: 1
        } as SimpleProduct | undefined;
        const productAvailability = {} as ProductAvailableQuantity | undefined;
        const location = {
            OrgUnitNumber: '1'
        } as OrgUnitLocation | undefined;
        jest.spyOn(AsyncResult, 'getElementIndex').mockReturnValue(1);
        const wrapper = getGenericError(result, cartData, resources, coreContext, product, productAvailability, location);
        expect(wrapper).toBeDefined();
    });

    it('should render getDimensionErrorString', () => {
        const resources = {
            productDimensionTypeAmountErrorMessage: 'test'
        } as IBuyboxCommonResources;
        const wrapper = getDimensionErrorString(4, resources, true);

        expect(wrapper).toStrictEqual('test');
    });
});
