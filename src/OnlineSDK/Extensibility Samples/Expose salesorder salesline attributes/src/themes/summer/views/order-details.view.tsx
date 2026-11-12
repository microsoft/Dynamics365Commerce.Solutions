/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { IHelp, IOrderSummary, IPaymentMethods, IGroup, IGroupDelivery, IGroups } from '@msdyn365-commerce-modules/order-management';
import { IOrderInformation } from '@msdyn365-commerce-modules/order-management/src/modules/order-details/../../common/get-order-information';
import { ISalesLine } from '@msdyn365-commerce-modules/order-management/src/modules/order-details/../../common/get-sales-line';
import { IOrderDetailsViewProps } from '@msdyn365-commerce-modules/order-management/src/modules/order-details/./order-details';

export const OrderDetailsOrderInfomation: React.FC<IOrderInformation> = ({
    orderInformationProps,
    salesId,
    receiptId,
    createdDate,
    count,
    amount,
    channelReferenceId,
    channelName,
    channelAddress
}) => (
    <Node {...orderInformationProps}>
        {channelName}
        {channelAddress}
        {channelReferenceId}
        {salesId}
        {receiptId}
        {createdDate}
        {count}
        {amount}
    </Node>
);

export const OrderDetailsSalesLine: React.FC<ISalesLine> = ({ salesLineProps, salesLine, buyAgainButton, errors }) => (
    <Node {...salesLineProps}>
        {salesLine}
        {buyAgainButton}
        {errors}
    </Node>
);

export const OrderDetailsGroupDelivery: React.FC<IGroupDelivery> = ({ deliveryProps, heading, count }) => (
    <Node {...deliveryProps}>
        {heading}
        {count}
    </Node>
);

export const OrderDetailsGroup: React.FC<IGroup> = ({
    groupProps,
    delivery,
    address,
    salesLinesProps,
    salesLines,
    isCashAndCarryTransaction
}) => (
    <Node {...groupProps}>
        {delivery && <OrderDetailsGroupDelivery {...delivery} />}
        {salesLines && (
            <Node {...salesLinesProps}>
                {salesLines.map(salesLine => (
                    <React.Fragment key={salesLine.data.salesLine.LineId}>
                        {salesLine.data.deliveryType === 'ship' && salesLine.data.shipment ? salesLine.trackingInfo : null}
                        <OrderDetailsSalesLine {...salesLine} />
                        <div className='salesline_inventory'>InventoryLocationId: {salesLine.data.salesLine.InventoryLocationId}</div>
                        <div className='salesline_groupId'>SalesTaxGroupId: {salesLine.data.salesLine.SalesTaxGroupId}</div>
                        {!isCashAndCarryTransaction && salesLine.salesStatus}
                    </React.Fragment>
                ))}
            </Node>
        )}
        {!delivery.showTimeslot && address}
        {delivery.showTimeslot && (
            <Node {...delivery.pickupProps}>
                {address}
                {delivery.pickupDateTimeslot}
            </Node>
        )}
    </Node>
);

export const OrderDetailsGroups: React.FC<IGroups> = ({ groupsProps, groups }) => (
    <Node {...groupsProps}>
        {groups.map((group: IGroup, index: number) => (
            <OrderDetailsGroup key={index} {...group} />
        ))}
    </Node>
);

export const OrderDetailsOrderSummary: React.FC<IOrderSummary> = ({
    orderSummaryProps,
    heading,
    subtotal,
    shipping,
    tax,
    totalAmount,
    earnedPoints
}) => (
    <Node {...orderSummaryProps}>
        {heading}
        {subtotal}
        {shipping}
        {tax}
        {totalAmount}
        {earnedPoints}
    </Node>
);

export const OrderDetailsPayment: React.FC<IPaymentMethods> = ({ paymentMethodsProps, title, methods }) => (
    <Node {...paymentMethodsProps}>
        {title}
        {methods}
    </Node>
);

export const OrderDetailsHelp: React.FC<IHelp> = ({ helpProps, needHelpLabel, helpLineNumberLabel, contactNumber }) => (
    <Node {...helpProps}>
        {needHelpLabel}
        {helpLineNumberLabel}
        {contactNumber}
    </Node>
);

const OrderDetailsView: React.FC<IOrderDetailsViewProps> = ({
    moduleProps,
    tableViewActions,
    viewModes,
    table,
    heading,
    alert,
    loading,
    orderInfomation,
    orderSummary,
    payment,
    help,
    groups
}) => {
    return (
        <Module {...moduleProps}>
            {heading}
            {alert}
            {loading}
            {orderInfomation && <OrderDetailsOrderInfomation {...orderInfomation} />}
            {tableViewActions}
            {viewModes}
            {table}
            {groups && <OrderDetailsGroups {...groups} />}
            {orderSummary && <OrderDetailsOrderSummary {...orderSummary} />}
            {payment && <OrderDetailsPayment {...payment} />}
            {help && <OrderDetailsHelp {...help} />}
        </Module>
    );
};

export default OrderDetailsView;
