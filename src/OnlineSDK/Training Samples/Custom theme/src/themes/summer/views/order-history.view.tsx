/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import {
    IGroup,
    IGroupDelivery,
    IGroups,
    IHeader,
    IList,
    IOrderHistoryViewProps,
    IOrderInformation,
    ISalesOrder
} from '@msdyn365-commerce-modules/order-management';
import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

export const OrderHistoryOrderInfomation: React.FC<IOrderInformation> = ({
    orderInformationProps,
    salesId,
    receiptId,
    channelName,
    createdDate,
    count,
    amount,
    channelReferenceId
}) => (
    <Node {...orderInformationProps}>
        {channelName}
        {salesId}
        {receiptId}
        {createdDate}
        {count}
        {amount}
        {channelReferenceId}
    </Node>
);

export const OrderHistoryGroupDelivery: React.FC<IGroupDelivery> = ({
    deliveryProps,
    heading,
    count,
    processing,
    address,
    trackingInfo
}) => (
    <Node {...deliveryProps}>
        {heading}
        {count}
        {processing}
        {address}
        {trackingInfo}
    </Node>
);

export const OrderHistoryGroup: React.FC<IGroup> = ({ groupProps, delivery, salesLinesProps, salesLines }) => (
    <Node {...groupProps}>
        {salesLines && (
            <Node {...salesLinesProps}>
                {salesLines.map(salesLine => (
                    <React.Fragment key={salesLine.data.salesLine.LineId}>{salesLine.salesLine}</React.Fragment>
                ))}
            </Node>
        )}
    </Node>
);

export const OrderHistoryGroups: React.FC<IGroups> = ({ groupsProps, groups }) => (
    <Node {...groupsProps}>
        {groups.map((group, index) => (
            <OrderHistoryGroup key={index} {...group} />
        ))}
    </Node>
);

export const OrderHistoryHeader: React.FC<IHeader> = ({ headerProps, heading, orderCountLabel, extraActions }) => (
    <Node {...headerProps}>
        {heading}
        {orderCountLabel}
        {extraActions}
    </Node>
);

export const OrderHistorySalesOder: React.FC<ISalesOrder> = ({
    salesOrderProps,
    orderInfomation,
    groups,
    orderDetailsLink,
    expandProductsButton
}) => (
    <Node {...salesOrderProps}>
        {orderInfomation && <OrderHistoryOrderInfomation {...orderInfomation} />}
        {groups && <OrderHistoryGroups {...groups} />}
        {expandProductsButton}
        {orderDetailsLink}
    </Node>
);

export const OrderHistoryList: React.FC<IList> = ({ listProps, salesOrders }) => (
    <Node {...listProps}>
        {salesOrders && salesOrders.map((salesOrder, index) => <OrderHistorySalesOder key={index} {...salesOrder} />)}
    </Node>
);

const OrderHistoryView: React.FC<IOrderHistoryViewProps> = ({
    orderHistoryProps,
    header,
    alert,
    loading,
    emptyMessage,
    backToShoppingLink,
    list,
    table,
    moreButton
}) => (
    <Module {...orderHistoryProps}>
        {header && <OrderHistoryHeader {...header} />}
        {loading}
        {alert && (
            <>
                {alert}
                {backToShoppingLink}
            </>
        )}
        {emptyMessage && (
            <Node className='ms-order-history__empty-message-container'>
                {emptyMessage}
                {backToShoppingLink}
            </Node>
        )}
        {list && <OrderHistoryList {...list} />}
        {table}
        {moreButton && moreButton}
    </Module>
);

export default OrderHistoryView;
