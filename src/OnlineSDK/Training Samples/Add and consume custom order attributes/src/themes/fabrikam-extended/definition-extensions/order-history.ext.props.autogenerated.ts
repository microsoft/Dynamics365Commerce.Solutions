/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IOrderHistory contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface IOrderHistoryConfig extends Msdyn365.IModuleConfig {
    heading?: IHeadingData;
    imageSettings?: Msdyn365.IImageSettings;
    showChannelInfo?: boolean;
    pageSize?: number;
    isGridViewEnabled?: boolean;
    className?: string;
    clientRender?: boolean;
    shouldShowCount?: boolean;
}

export interface IOrderHistoryResources {
    loadingLabel: string;
    genericErrorMessage: string;
    noOrderHistoryLable: string;
    orderIdLabel: string;
    orderPlacedByText: string;
    orderPlacedByFullText: string;
    orderPlacedByYouText: string;
    orderOnBehalfOfText: string;
    processingLabel: string;
    confirmationIdLabel: string;
    detailsLabel: string;
    detailsAriaLabel: string;
    orderCountLabel: string;
    ordersCountLabel: string;
    orderItemLabel: string;
    orderItemsLabel: string;
    trackingLabel: string;
    trackingComingSoonLabel: string;
    trackingAriaLabel: string;
    backToShopping: string;
    moreButtonText: string;
    receiptEmailMessage: string;
    receiptIdLabel: string;
    freePriceText: string;
    pickedUpSalesLines: string;
    deliveredSalesLines: string;
    carryOutSalesLines: string;
    emailSalesLines: string;
    orderStatusEmailed: string;
    orderStatusReadyForPickup: string;
    orderStatusProcessing: string;
    orderStatusShipped: string;
    orderStatusPickedUp: string;
    orderStatusCanceled: string;
    phoneAriaLabel: string;
    buyItAgainLabel: string;
    buyItAgainAriaLabel: string;
    shipToLabel: string;
    storeLabel: string;
    shippingAddressTitle: string;
    phoneLabel: string;
    productDimensionTypeColor: string;
    productDimensionTypeSize: string;
    productDimensionTypeStyle: string;
    configString: string;
    salesLineQuantityText: string;
    discountStringText: string;
    posChannelNameText: string;
    onlineStoreChannelNameText: string;
    orderHistoryFilterYourOrderHistory: string;
    orderHistoryFilterOrganizationWide: string;
    pickupDateTimeslotText: string;
    pickupTimeslotPlaceHolder: string;
    orderHistoryListViewModeAriaLabel: string;
    orderHistoryDetailedViewModeAriaLabel: string;
    orderHistoryOrderNumberText: string;
    orderHistoryCreatedDateText: string;
    orderHistoryItemsText: string;
    orderHistoryTotalText: string;
    orderHistoryOriginText: string;
    orderHistoryOrderNumberIsNotAvailable: string;
    orderHistoryViewDetailsButtonText: string;
    orderHistoryViewDetailsButtonAriaLabel: string;
    orderHistoryCreatedDateMobileDescriptionText: string;
    orderHistoryExpandProductsButtonText: string;
    callCenterChannelNameText: string;
}

export const enum HeadingTag {
    h1 = 'h1',
    h2 = 'h2',
    h3 = 'h3',
    h4 = 'h4',
    h5 = 'h5',
    h6 = 'h6'
}

export interface IHeadingData {
    text: string;
    tag?: HeadingTag;
}

export interface IOrderHistoryProps<T> extends Msdyn365.IModule<T> {
    resources: IOrderHistoryResources;
    config: IOrderHistoryConfig;
}
