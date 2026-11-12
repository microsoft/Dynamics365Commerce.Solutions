/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import * as Msdyn365 from '@msdyn365-commerce/core';
import {
    AsyncResult,
    ChannelDeliveryOptionConfiguration,
    ChannelIdentity,
    FeatureState,
    LoyaltyCard,
    OrgUnitLocation,
    SalesOrder,
    SimpleProduct
} from '@msdyn365-commerce/retail-proxy';
import { IOrderDetailsData, IOrderDetailsProps, OrderDetails } from '@msdyn365-commerce-modules/order-management';
import { wrapInResolvedAsyncResult } from '@msdyn365-commerce-modules/retail-actions';
import { render } from 'enzyme';
import * as React from 'react';

import renderView from '../../views/order-details.view';
import { mockConfigResources, mockProducts, mockSalesOrderWithAmount, mockTenderTypes } from './__mock';

enum HeadingTag {
    h1 = 'h1',
    h2 = 'h2',
    h3 = 'h3',
    h4 = 'h4',
    h5 = 'h5',
    h6 = 'h6'
}

interface IHeadingData {
    text: string;
    // eslint-disable-next-line @typescript-eslint/naming-convention -- have to export this as this utility is used in fabrikam tests
    tag?: HeadingTag;
}

interface IOrderDetailsConfig extends Msdyn365.IModuleConfig {
    heading?: IHeadingData;
    shouldShowQrCode?: boolean;
    // eslint-disable-next-line @typescript-eslint/naming-convention -- have to export this as this utility is used in fabrikam tests
    showChannelInfo?: boolean;
    addressOptions?: MockAddressOptions;
    // eslint-disable-next-line @typescript-eslint/naming-convention -- have to export this as this utility is used in fabrikam tests
    showTimeslot?: boolean;
    // eslint-disable-next-line @typescript-eslint/naming-convention -- have to export this as this utility is used in fabrikam tests
    showShippingChargesForLineItems?: boolean;
    contactNumber?: string;
    imageSettings?: Msdyn365.IImageSettings;
    isReorderingEnabled?: boolean;
    className?: string;
}

enum MockAddressOptions {
    showAddressWithSingleLine = 'showAddressWithSingleLine',
    showAddressWithMultiplelines = 'showAddressWithMultiplelines'
}

// @ts-expect-error
const mockContext: ICoreContext = Msdyn365.buildMockCoreContext({
    request: {
        gridSettings: {
            xs: { w: 767, h: 0 },
            sm: { w: 991, h: 0 },
            md: { w: 1199, h: 0 },
            lg: { w: 1599, h: 0 },
            xl: { w: 1600, h: 0 }
        },

        // @ts-expect-error
        apiSettings: {
            channelId: 5637145359
        },

        // @ts-expect-error
        channel: {
            PickupDeliveryModeCode: '70'
        },

        app: {
            config: { isEnableShowOrHideSalesTaxECommerceEnabled: false }
        }
    }
});
const loyaltyCard: LoyaltyCard = {
    CardNumber: '1234567890',
    LoyaltyEnrollmentDate: new Date('Wed Jul 03 2019 14:44:37 GMT-0700'),
    RewardPoints: [
        {
            RewardPointId: 'Fabrikam',
            RewardPointTypeValue: 1,
            RewardPointCurrency: 'USD',
            IsRedeemable: true,
            PointsExpiringSoon: 34,
            ActivePoints: 90,
            Description: 'Fabrikam awesome points'
        }
    ]
};
const mockDataWithSalesOrderWithAmount: IOrderDetailsData = {
    orderHydration: {
        status: 'SUCCESS',
        result: {
            salesOrder: mockSalesOrderWithAmount,
            products: mockProducts
        }
    } as AsyncResult<{
        salesOrder: SalesOrder;
        products: SimpleProduct[];
    }>,

    // @ts-expect-error
    tenderTypes: wrapInResolvedAsyncResult(mockTenderTypes),
    loyaltyCard: wrapInResolvedAsyncResult(loyaltyCard),
    channels: wrapInResolvedAsyncResult({
        channelIdentities: [] as ChannelIdentity[]
    }),
    orgUnitLocations: wrapInResolvedAsyncResult({
        orgUnitLocations: [] as OrgUnitLocation[]
    }),
    channelDeliveryOptionConfig: {
        status: 'SUCCESS',
        result: {}
    } as AsyncResult<ChannelDeliveryOptionConfiguration>,
    featureState: AsyncResult.resolve([] as FeatureState[])
};
const mockConfigSalesOrder: IOrderDetailsConfig = {
    contactNumber: '123-456-7890',
    showChannelInfo: true,
    shouldShowQrCode: true
};

describe('OrderDetails', () => {
    it('renders sales order with amount', () => {
        const modulePropsWithSalesOrderWithAmount: IOrderDetailsProps<IOrderDetailsData> = {
            ...(Msdyn365.buildMockModuleProps(
                mockDataWithSalesOrderWithAmount,
                {},
                mockConfigSalesOrder,
                mockContext
            ) as IOrderDetailsProps<IOrderDetailsData>),
            resources: mockConfigResources,

            // @ts-expect-error
            renderView
        };

        const component = render(<OrderDetails {...modulePropsWithSalesOrderWithAmount} />);
        expect(component).toMatchSnapshot();
    });
});
