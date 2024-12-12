/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import {
    IGroup,
    IGroupDelivery,
    IGroups,
    IHelp,
    IOrderDetailsViewProps,
    IOrderInformation,
    IOrderSummary,
    IPaymentMethods,
    ISalesLine
} from '@msdyn365-commerce-modules/order-management';
import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

export const OrderInfomation: React.FC<IOrderInformation> = ({
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
        {salesId}
        {receiptId}
        {createdDate}
        {count}
        {amount}
        {channelReferenceId}
    </Node>
);

export const SalesLine: React.FC<ISalesLine> = ({ salesLineProps, salesLine, buyAgainButton, errors }) => (
    <Node {...salesLineProps}>
        {salesLine}
        {buyAgainButton}
        {errors}
    </Node>
);

export const GroupDelivery: React.FC<IGroupDelivery> = ({ deliveryProps, heading, count }) => (
    <Node {...deliveryProps}>
        {heading}
        {count}
    </Node>
);

export const Group: React.FC<IGroup> = ({ groupProps, delivery, address, salesLinesProps, salesLines, isCashAndCarryTransaction }) => (
    <Node {...groupProps}>
        {delivery && <GroupDelivery {...delivery} />}
        <Node className='ms-order-details__sales-lines_container'>
            {salesLines && (
                <Node {...salesLinesProps}>
                    {salesLines.map(salesLine => (
                        <React.Fragment key={salesLine.data.salesLine.LineId}>
                            {salesLine.data.deliveryType === 'ship' && salesLine.data.shipment ? salesLine.trackingInfo : null}
                            <SalesLine {...salesLine} />
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
    </Node>
);

export const Groups: React.FC<IGroups> = ({ groupsProps, groups }) => (
    <Node {...groupsProps}>
        {groups.map((group, index) => (
            <Group key={index} {...group} />
        ))}
    </Node>
);

export const OrderSummary: React.FC<IOrderSummary> = ({
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

export const Payment: React.FC<IPaymentMethods> = ({ paymentMethodsProps, title, methods }) => (
    <Node {...paymentMethodsProps}>
        {title}
        {methods}
    </Node>
);

export const Help: React.FC<IHelp> = ({ helpProps, needHelpLabel, helpLineNumberLabel, contactNumber }) => (
    <Node {...helpProps}>
        {needHelpLabel}
        {helpLineNumberLabel}
        {contactNumber}
    </Node>
);

const OrderDetailsView: React.FC<IOrderDetailsViewProps> = ({
    moduleProps,
    viewModes,
    tableViewActions,
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
            {orderInfomation && <OrderInfomation {...orderInfomation} />}
            {tableViewActions}
            {viewModes}
            {table}
            {groups && <Groups {...groups} />}
            <Node className='ms-order-details__order-summary-container'>
                {orderSummary && <OrderSummary {...orderSummary} />}
                {payment && <Payment {...payment} />}
            </Node>
            {help && <Help {...help} />}
        </Module>
    );
};

export default OrderDetailsView;
