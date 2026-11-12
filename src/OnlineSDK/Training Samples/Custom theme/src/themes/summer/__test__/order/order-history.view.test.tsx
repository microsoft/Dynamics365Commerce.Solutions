/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import * as Msdyn365 from '@msdyn365-commerce/core';
import { IHeader, IOrderDetailsData, IOrderDetailsProps, IOrderHistoryViewProps } from '@msdyn365-commerce-modules/order-management';
import { render } from 'enzyme';
import * as React from 'react';

import OrderHistoryView from '../../views/order-history.view';
import { mockConfigResources } from './__mock';

// @ts-expect-error
const mockContext: ICoreContext = Msdyn365.buildMockCoreContext({});

const orderHeader: IHeader = {
    headerProps: { className: 'xyz' },
    heading: '<h1>heading</h1>'
};
describe('OrderHistory', () => {
    it('renders OrderHistory correctly', () => {
        const modulePropsWithSalesOrderWithAmount: IOrderDetailsProps<IOrderDetailsData> = {
            ...(Msdyn365.buildMockModuleProps({}, {}, {}, mockContext) as IOrderDetailsProps<IOrderDetailsData>),
            resources: mockConfigResources,
            renderModuleAttributes: jest.fn()
        };
        const viewProps: IOrderHistoryViewProps = {
            orderHistoryProps: { className: 'class', moduleProps: modulePropsWithSalesOrderWithAmount },
            orderHistory: {
                salesOrders: [{ SalesId: '12' }],

                // @ts-expect-error
                products: [{ RecordId: 333 }]
            },
            header: orderHeader,
            alert: <div />,
            emptyMessage: <div />,
            list: {
                listProps: { className: 'class' },
                salesOrders: [
                    {
                        salesOrderProps: { className: 'class' },
                        orderInfomation: {
                            orderInformationProps: { className: 'class' },
                            channelName: <div />,
                            channelAddress: <div />,
                            salesId: <div />,
                            receiptId: <div />,
                            receiptEmail: <div />,
                            createdDate: <div />,
                            count: <div />,
                            amount: <div />,
                            channelReferenceId: <div />
                        },
                        groups: {
                            groupsProps: { className: 'class' },

                            groups: [
                                {
                                    groupProps: { className: 'class' },
                                    salesLinesProps: { className: 'class' },

                                    // @ts-expect-error
                                    data: {
                                        count: 1
                                    },
                                    isCashAndCarryTransaction: true
                                }
                            ]
                        },
                        orderDetailsLink: <div />
                    }
                ]
            },
            viewState: {
                isDataReady: true,
                hasError: false,
                isLoading: true,
                isShowMoreButton: true
            },
            moreButton: <div />
        };
        const component = render(<OrderHistoryView {...viewProps} />);
        expect(component).toMatchSnapshot();
    });
});
