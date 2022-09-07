/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IOrderDetails contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export const enum addressOptions {
    showAddressWithSingleLine = 'showAddressWithSingleLine',
    showAddressWithMultiplelines = 'showAddressWithMultiplelines'
}

export interface IOrderDetailsConfig extends Msdyn365.IModuleConfig {
    heading?: IHeadingData;
    shouldShowQrCode?: boolean;
    showChannelInfo?: boolean;
    addressOptions?: addressOptions;
    showTimeslot?: boolean;
    showShippingChargesForLineItems?: boolean;
    contactNumber?: string;
    imageSettings?: Msdyn365.IImageSettings;
    isReorderingEnabled?: boolean;
    isGridViewEnabled?: boolean;
    className?: string;
}

export interface IOrderDetailsResources {
    loadingLabel: string;
    genericErrorMessage: string;
    orderItemLabel: string;
    orderItemsLabel: string;
    orderIdLabel: string;
    confirmationIdLabel: string;
    orderSummaryTitle: string;
    paymentMethodsTitle: string;
    shippingAddressTitle: string;
    noSalesOrderDetailsText: string;
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
    orderStatusReadyForPickup: string;
    orderStatusProcessing: string;
    orderStatusShipped: string;
    orderStatusPickedUp: string;
    orderStatusCanceled: string;
    emailSalesLines: string;
    orderStatusEmailed: string;
    phoneAriaLabel: string;
    buyItAgainLabel: string;
    buyItAgainAriaLabel: string;
    shipToLabel: string;
    storeLabel: string;
    productDimensionTypeColor: string;
    productDimensionTypeSize: string;
    productDimensionTypeStyle: string;
    productDimensionTypeAmount: string;
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
    posChannelNameText: string;
    onlineStoreChannelNameText: string;
    pickupDateTimeslotText: string;
    pickupTimeslotPlaceHolder: string;
    shippingCharges: string;
    orderDetailsListViewModeAriaLabel: string;
    orderDetailsDetailedViewModeAriaLabel: string;
    orderDetailsProductNumberText: string;
    orderDetailsProductText: string;
    orderDetailsUnitPriceText: string;
    orderDetailsModeOfDeliveryText: string;
    orderDetailsStatusText: string;
    orderDetailsQuantityText: string;
    orderDetailsUnitOfMeasureText: string;
    orderDetailsTotalText: string;
    orderDetailsActionsText: string;
    orderDetailsViewDetailsButtonText: string;
    orderDetailsViewDetailsButtonAriaLabel: string;
    orderDetailsProductDimensionsSeparator: string;
    orderDetailsQuantityMobileText: string;
    orderDetailsBuySelectedButtonText: string;
    orderDetailsBuySelectedButtonAriaLabel: string;
    orderDetailsDisableSelectionButtonText: string;
    orderDetailsDisableSelectionButtonAriaLabel: string;
    orderDetailsEnableSelectionButtonText: string;
    orderDetailsEnableSelectionButtonAriaLabel: string;
    orderDetailsSelectAllRadioAriaLabelText: string;
    orderDetailsSelectRadioAriaLabelText: string;
    orderDetailsBuySelectedAddingToCartErrorNotificationTitle: string;
    orderDetailsBuySelectedAddingToCartErrorNotificationCloseAriaLabel: string;
    orderDetailsBuyItAgainButtonText: string;
    orderDetailsBuyItAgainButtonAriaLabel: string;
    orderDetailsGoToCartText: string;
    orderDetailsDialogCloseText: string;
    orderDetailsSingleItemText: string;
    orderDetailsMultiLineItemFormatText: string;
    orderDetailsMultiLinesFormatText: string;
    orderDetailsHeaderMessageText: string;
    addedQuantityText: string;
    originalPriceText: string;
    currentPriceText: string;
    orderDetailsBuyItemsAgainHeaderText: string;
    qrCodeSrText: string;
    orderDetailsMultiItemsValidationErrorMessage: string;
    orderDetailsOneErrorText: string;
    orderDetailsMultiErrorText: string;
    orderDetailsUnavailableProductText: string;
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

export interface IOrderDetailsProps<T> extends Msdyn365.IModule<T> {
    resources: IOrderDetailsResources;
    config: IOrderDetailsConfig;
}
