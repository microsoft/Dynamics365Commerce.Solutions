/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IOrderConfirmation contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export interface IOrderConfirmationConfig extends Msdyn365.IModuleConfig {
    heading?: IHeadingData;
    shouldShowQrCode?: boolean;
    showTimeslot?: boolean;
    contactNumber?: string;
    imageSettings?: Msdyn365.IImageSettings;
    className?: string;
}

export interface IOrderConfirmationResources {
    loadingLabel: string;
    genericErrorMessage: string;
    orderItemLabel: string;
    orderItemsLabel: string;
    orderIdLabel: string;
    confirmationIdLabel: string;
    orderSummaryTitle: string;
    paymentMethodsTitle: string;
    shippingAddressTitle: string;
    noSalesOrderText: string;
    creditCardEndingLabel: string;
    giftCardEndingLabel: string;
    amountCoveredLabel: string;
    loyaltyCardUsedLabel: string;
    cashUsedLabel: string;
    customerAccountUsedLabel: string;
    freePriceText: string;
    orderSummaryItemsTotalLabel: string;
    orderSummaryShippingFeeLabel: string;
    orderSummaryTaxLabel: string;
    orderSummaryGrandTotalLabel: string;
    salesLineQuantityText: string;
    processingLabel: string;
    needHelpLabel: string;
    helpLineNumberLabel: string;
    helpLineContactAriaLabel: string;
    pickedUpSalesLines: string;
    deliveredSalesLines: string;
    carryOutSalesLines: string;
    emailSalesLines: string;
    orderStatusReadyForPickup: string;
    orderStatusProcessing: string;
    orderStatusShipped: string;
    orderStatusPickedUp: string;
    orderStatusCanceled: string;
    orderStatusEmailed: string;
    phoneAriaLabel: string;
    buyItAgainLabel: string;
    buyItAgainAriaLabel: string;
    shipToLabel: string;
    storeLabel: string;
    productDimensionTypeColor: string;
    productDimensionTypeSize: string;
    productDimensionTypeStyle: string;
    trackingLabel: string;
    trackingComingSoonLabel: string;
    trackingAriaLabel: string;
    pointsEarnedLabel: string;
    receiptEmailMessage: string;
    receiptIdLabel: string;
    backToShopping: string;
    configString: string;
    phoneLabel: string;
    orderSummaryHeading: string;
    discountStringText: string;
    pickupItemsHeader: string;
    pickingItemsLabel: string;
    emailItemsHeader: string;
    emailingItemsLabel: string;
    shippingItemToYouLabel: string;
    orderPlacedOnLabel: string;
    pickupDateTimeslotText: string;
    pickupTimeslotPlaceHolder: string;
    qrCodeSrText: string;
    otherCharges: string;
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

export interface IOrderConfirmationProps<T> extends Msdyn365.IModule<T> {
    resources: IOrderConfirmationResources;
    config: IOrderConfirmationConfig;
}
