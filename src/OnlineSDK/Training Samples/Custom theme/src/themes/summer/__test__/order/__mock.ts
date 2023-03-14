/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/**
 * Mocked mockShippingAddress.
 */
export const mockShippingAddress = {
    Name: 'Gloria Deng',
    Street: 'One Microsoft Way',
    City: 'Redmond',
    State: 'wa',
    ZipCode: '98052',
    ThreeLetterISORegionName: 'usa'
};

/**
 * Mocked mockStoreAddress.
 */
export const mockStoreAddress = {
    Name: 'Fabrikam Company Store',
    FullAddress: '15010 NE 36th St↵Redmond, WA 98052↵USA',
    Street: '15010 NE 36th St',
    City: 'Redmond',
    County: 'KING',
    ThreeLetterISORegionName: 'USA',
    State: 'WA',
    ZipCode: '98052'
};

/**
 * Mocked mockShipments.
 */
export const mockShipments = [
    {
        ShipmentId: '123',
        TrackingNumber: '123456',
        TrackingUrl: '/tracking'
    }
];

/**
 * Mocked mockTenderLines.
 */
export const mockTenderLines = [
    {
        TenderTypeId: '3',
        MaskedCardNumber: '************1111',
        Currency: 'USD',
        CardTypeId: 'VISA'
    },
    {
        TenderTypeId: '8',
        GiftCardId: 'gldeng_100_3',
        Currency: 'USD',
        Amount: 100
    },
    {
        TenderTypeId: '1',
        Currency: 'USD',
        Amount: 100
    },
    {
        TenderTypeId: '10',
        LoyaltyCardId: '890609',
        Currency: 'USD',
        Amount: 100
    },
    {
        TenderTypeId: '4',
        CustomerId: '005221', // Does this need to be changed to a dedicated test account once customer account payment functionality is fleshed out?
        Currency: 'USD',
        Amount: 400
    }
];

/**
 * Mocked mockTenderLinesWithAmount.
 */
export const mockTenderLinesWithAmount = [
    {
        Amount: 10,
        TenderTypeId: '3',
        MaskedCardNumber: '************1111',
        Currency: 'USD',
        CardTypeId: 'VISA'
    },
    {
        TenderTypeId: '8',
        GiftCardId: 'gldeng_100_3',
        Currency: 'USD',
        Amount: 100
    },
    {
        TenderTypeId: '1',
        Currency: 'USD',
        Amount: 100
    },
    {
        TenderTypeId: '10',
        LoyaltyCardId: '890609',
        Currency: 'USD',
        Amount: 100
    },
    {
        TenderTypeId: '4',
        CustomerId: '005221', // Does this need to be changed to a dedicated test account once customer account payment functionality is fleshed out?
        Currency: 'USD',
        Amount: 400
    }
];

/**
 * Mocked mockTenderLinesWithAuthorizedAmount.
 */
export const mockTenderLinesWithAuthorizedAmount = [
    {
        AuthorizedAmount: 10,
        TenderTypeId: '3',
        MaskedCardNumber: '************1111',
        Currency: 'USD',
        CardTypeId: 'VISA'
    },
    {
        TenderTypeId: '8',
        GiftCardId: 'gldeng_100_3',
        Currency: 'USD',
        Amount: 100
    },
    {
        TenderTypeId: '1',
        Currency: 'USD',
        Amount: 100
    },
    {
        TenderTypeId: '10',
        LoyaltyCardId: '890609',
        Currency: 'USD',
        Amount: 100
    },
    {
        TenderTypeId: '4',
        CustomerId: '005221', // Does this need to be changed to a dedicated test account once customer account payment functionality is fleshed out?
        Currency: 'USD',
        Amount: 400
    }
];

/**
 * Mocked mockSalesOrderWithAmount.
 */
export const mockSalesOrderWithAmount = {
    Id: '0HHRKonY4riLeLAjxZAZTltqsZrMVPvt',
    ReceiptEmail: 'john@example.com',
    ChannelId: 5_637_145_359,
    ChannelReferenceId: 'C764D04GLDEN',
    CurrencyCode: 'USD',
    SalesId: '012660',
    SalesLines: [
        {
            ProductId: 22_565_430_654,
            TotalAmount: 300,
            SalesStatusValue: 0,
            Quantity: 1,
            DeliveryMode: '60',
            ShippingAddress: mockShippingAddress,
            TrackingId: '123'
        },
        {
            ProductId: 22_565_430_655,
            TotalAmount: 200,
            SalesStatusValue: 0,
            Quantity: 2,
            DeliveryMode: '70',
            ShippingAddress: mockStoreAddress
        }
    ],
    Shipments: mockShipments,
    SubtotalAmount: 300,
    ChargeAmount: 0,
    SubtotalAmountWithoutTax: 300,
    TaxAmount: 19.5,
    TotalAmount: 319.5,
    TenderLines: mockTenderLinesWithAuthorizedAmount
};

/**
 * Mocked mockSalesOrderWithAuthorizedAmount.
 */
export const mockSalesOrderWithAuthorizedAmount = {
    Id: '0HHRKonY4riLeLAjxZAZTltqsZrMVPvt',
    ReceiptEmail: 'john@example.com',
    ChannelId: 5_637_145_359,
    ChannelReferenceId: 'C764D04GLDEN',
    CurrencyCode: 'USD',
    SalesId: '012660',
    SalesLines: [
        {
            ProductId: 22_565_430_654,
            TotalAmount: 300,
            SalesStatusValue: 0,
            Quantity: 1,
            DeliveryMode: '60',
            ShippingAddress: mockShippingAddress,
            TrackingId: '123'
        },
        {
            ProductId: 22_565_430_655,
            TotalAmount: 200,
            SalesStatusValue: 0,
            Quantity: 2,
            DeliveryMode: '70',
            ShippingAddress: mockStoreAddress
        }
    ],
    Shipments: mockShipments,
    SubtotalAmount: 300,
    ChargeAmount: 0,
    SubtotalAmountWithoutTax: 300,
    TaxAmount: 19.5,
    TotalAmount: 319.5,
    TenderLines: mockTenderLinesWithAuthorizedAmount
};

/**
 * Mocked mockSalesOrder.
 */
export const mockSalesOrder = {
    Id: '0HHRKonY4riLeLAjxZAZTltqsZrMVPvt',
    ReceiptEmail: 'john@example.com',
    ChannelId: 5_637_145_359,
    ChannelReferenceId: 'C764D04GLDEN',
    CurrencyCode: 'USD',
    SalesId: '012660',
    SalesLines: [
        {
            ProductId: 22_565_430_654,
            TotalAmount: 300,
            SalesStatusValue: 0,
            Quantity: 1,
            DeliveryMode: '60',
            ShippingAddress: mockShippingAddress,
            TrackingId: '123'
        },
        {
            ProductId: 22_565_430_655,
            TotalAmount: 200,
            SalesStatusValue: 0,
            Quantity: 2,
            DeliveryMode: '70',
            ShippingAddress: mockStoreAddress
        }
    ],
    Shipments: mockShipments,
    SubtotalAmount: 300,
    ChargeAmount: 0,
    SubtotalAmountWithoutTax: 300,
    TaxAmount: 19.5,
    TotalAmount: 319.5,
    TenderLines: mockTenderLines
};

/**
 * Mocked mockSalesOrderWithReceiptId.
 */
export const mockSalesOrderWithReceiptId = {
    ...mockSalesOrder,
    ReceiptId: '012660-RECEIPT-ID',
    SalesId: ''
};

/**
 * Mocked mockSalesOrderWithReceiptIdAndOrderId.
 */
export const mockSalesOrderWithReceiptIdAndOrderId = {
    ...mockSalesOrder,
    ReceiptId: '012660-RECEIPT-ID',
    SalesId: '012660-ORDER-ID'
};

/**
 * Mocked mockTransaction.
 */
export const mockTransaction = {
    Id: 'UiE~EWeXya9o0byTIE7KeP8bIEjpGv9-',
    ReceiptEmail: 'john@example.com',
    ChannelId: 5_637_145_359,
    ChannelReferenceId: '0641E90GLDEN',
    CurrencyCode: 'USD',
    SalesId: undefined,
    SalesLines: [
        {
            ProductId: 22_565_430_654,
            TotalAmount: 300,
            SalesStatusValue: 0,
            Quantity: 1,
            DeliveryMode: '60',
            ShippingAddress: mockShippingAddress
        },
        {
            ProductId: 22_565_430_655,
            TotalAmount: 200,
            SalesStatusValue: 0,
            Quantity: 2,
            DeliveryMode: '70',
            ShippingAddress: mockStoreAddress
        }
    ],
    SubtotalAmount: 300,
    ChargeAmount: 0,
    SubtotalAmountWithoutTax: 300,
    TaxAmount: 19.5,
    TotalAmount: 319.5,
    TenderLines: mockTenderLines
};

/**
 * Mocked mockTransactionWithEmptyChannelReferenceIdAndWithEmptySalesId.
 */
export const mockTransactionWithEmptyChannelReferenceIdAndWithEmptySalesId = {
    Id: 'UiE~EWeXya9o0byTIE7KeP8bIEjpGv9-',
    ReceiptEmail: 'john@example.com',
    ChannelId: 5_637_145_359,
    ChannelReferenceId: '',
    CurrencyCode: 'USD',
    SalesId: undefined,
    SalesLines: [
        {
            ProductId: 22_565_430_654,
            TotalAmount: 300,
            SalesStatusValue: 0,
            Quantity: 1,
            DeliveryMode: '60',
            ShippingAddress: mockShippingAddress
        },
        {
            ProductId: 22_565_430_655,
            TotalAmount: 200,
            SalesStatusValue: 0,
            Quantity: 2,
            DeliveryMode: '70',
            ShippingAddress: mockStoreAddress
        }
    ],
    SubtotalAmount: 300,
    ChargeAmount: 0,
    SubtotalAmountWithoutTax: 300,
    TaxAmount: 19.5,
    TotalAmount: 319.5,
    TenderLines: mockTenderLines
};

/**
 * Mocked mockProducts.
 */
export const mockProducts = [
    {
        RecordId: 22_565_430_654,
        ItemId: '81307',
        Name: 'Red Leather Bag',
        ProductTypeValue: 4,
        BasePrice: 300,
        Price: 300,
        AdjustedPrice: 300,
        PrimaryImageUrl:
            'https://img-prod-cms-mr-microsoft-com.akamaized.net/cms/api/fabrikam/imageFileData/search?fileName=/Products%2F81307_000_001.png'
    },
    {
        RecordId: 22_565_430_655,
        ItemId: '81308',
        Name: 'Brown Leather Bag',
        ProductTypeValue: 4,
        BasePrice: 200,
        Price: 200,
        AdjustedPrice: 200,
        PrimaryImageUrl:
            'https://img-prod-cms-mr-microsoft-com.akamaized.net/cms/api/fabrikam/imageFileData/search?fileName=/Products%2F81307_000_001.png',

        Dimensions: [
            {
                DimensionTypeValue: 1,
                DimensionValue: {
                    Value: 'Brown'
                }
            },
            {
                DimensionTypeValue: 3,
                DimensionValue: {
                    Value: 'Map'
                }
            },
            {
                DimensionTypeValue: 4,
                DimensionValue: {
                    Value: 'Cool'
                }
            }
        ]
    }
];

/**
 * Mocked mockProductsWithGiftcard.
 */
export const mockProductsWithGiftcard = [
    {
        RecordId: 22_565_430_654,
        ItemId: '81307',
        Name: 'Red Leather Bag',
        ProductTypeValue: 4,
        BasePrice: 300,
        Price: 300,
        AdjustedPrice: 300,
        PrimaryImageUrl:
            'https://img-prod-cms-mr-microsoft-com.akamaized.net/cms/api/fabrikam/imageFileData/search?fileName=/Products%2F81307_000_001.png',
        IsGiftCard: true
    },
    {
        RecordId: 22_565_430_655,
        ItemId: '81308',
        Name: 'Brown Leather Bag',
        ProductTypeValue: 4,
        BasePrice: 200,
        Price: 200,
        AdjustedPrice: 200,
        PrimaryImageUrl:
            'https://img-prod-cms-mr-microsoft-com.akamaized.net/cms/api/fabrikam/imageFileData/search?fileName=/Products%2F81307_000_001.png',
        IsGiftCard: true,

        Dimensions: [
            {
                DimensionTypeValue: 1,
                DimensionValue: {
                    Value: 'Brown'
                }
            },
            {
                DimensionTypeValue: 3,
                DimensionValue: {
                    Value: 'Map'
                }
            },
            {
                DimensionTypeValue: 4,
                DimensionValue: {
                    Value: 'Cool'
                }
            }
        ]
    }
];

/**
 * Mocked mockRequestContextJson.
 */
export const mockRequestContextJson = {
    url: {
        serverPageUrl: 'https://localhost:3000'
    },
    locale: 'en-us',
    apiSettings: {
        baseUrl: '',
        baseImageUrl: '',
        channelId: 0,
        oun: '',
        ratingsReviewsEndpoint: ''
    },
    user: {
        token: 'Dummy token',
        isAuthenticated: true
    },
    urlTokens: {
        locale: 'en-us',
        categories: ['fashion-sunglasses']
    }
};

/**
 * Mocked mockTenderTypes.
 */
export const mockTenderTypes = [
    {
        Function: 0,
        TenderTypeId: '1',
        Name: 'Cash',
        OperationId: 200,
        OperationName: 'Pay cash'
    },
    {
        Function: 1,
        TenderTypeId: '3',
        Name: 'Cards',
        OperationId: 201,
        OperationName: 'Pay card'
    },
    {
        Function: 0,
        TenderTypeId: '8',
        Name: 'Gift Card',
        OperationId: 214,
        OperationName: 'Pay gift card'
    },
    {
        Function: 1,
        TenderTypeId: '10',
        Name: 'Loyalty Cards',
        OperationId: 207,
        OperationName: 'Pay loyalty card'
    },
    {
        Function: 3,
        TenderTypeId: '4',
        Name: 'Customer Account',
        OperationId: 202,
        OperationName: 'Pay customer account'
    }
];

/**
 * Mocked mockProdutDeliveryOptions.
 */
export const mockProdutDeliveryOptions = [
    {
        ProductId: 4648,
        DeliveryOptions: [
            {
                ChargeGroup: 'test',
                Code: 'customer pickup',
                Description: 'test',
                RecordId: 45,
                DeliveryChargeLines: undefined,
                TotalChargeAmount: 67,
                ShippingChargeAmount: 6,
                ExtensionProperties: undefined
            },
            {
                ChargeGroup: 'test',
                Code: 'locker pickup',
                Description: 'test',
                RecordId: 46,
                DeliveryChargeLines: undefined,
                TotalChargeAmount: 67,
                ShippingChargeAmount: 6,
                ExtensionProperties: undefined
            }
        ]
    },
    {}
];

/**
 * Mocked resources.
 */
export const mockConfigResources = {
    orderSummaryHeading: 'Order summary',
    receiptEmailMessage: 'Confirmation email sent to ',
    receiptIdLabel: 'Receipt#',
    loadingLabel: 'Loading...',
    freePriceText: 'FREE',
    orderSummaryItemsTotalLabel: 'Subtotal',
    orderSummaryShippingFeeLabel: 'Shipping',
    orderSummaryTaxLabel: 'Tax',
    orderSummaryGrandTotalLabel: 'Grand total',
    salesLineQuantityText: 'Quantity:',
    processingLabel: 'Processing',
    creditCardEndingLabel: 'card ending in',
    giftCardEndingLabel: 'Gift card ending in',
    amountCoveredLabel: 'Amount covered:',
    loyaltyCardUsedLabel: 'Loyalty card used',
    cashUsedLabel: 'Cash',
    customerAccountUsedLabel: 'Customer Account',
    orderIdLabel: 'Order ID:',
    confirmationIdLabel: 'Confirmation Id',
    orderSummaryTitle: 'Order summary',
    paymentMethodsTitle: 'Payment method',
    shippingAddressTitle: 'Shipping address',
    noSalesOrderDetailsText: 'No order details found',
    needHelpLabel: 'Need help?',
    helpLineNumberLabel: 'Call',
    helpLineContactAriaLabel: 'Need help, call',
    orderItemLabel: 'item',
    orderItemsLabel: 'items',
    pickedUpSalesLines: 'Store pickup',
    deliveredSalesLines: 'Delivery',
    carryOutSalesLines: 'Purchased',
    orderStatusReadyForPickup: 'Ready for pickup',
    orderStatusProcessing: 'Processing',
    orderStatusShipped: 'Shipped',
    orderStatusPickedUp: 'Picked up',
    orderStatusCanceled: 'Canceled',
    phoneLabel: 'Phone: ',
    phoneAriaLabel: 'Shop phone',
    buyItAgainLabel: 'Buy it again',
    buyItAgainAriaLabel: 'Buy it again for {productName}',
    shipToLabel: 'Ship to',
    storeLabel: 'Store',
    productDimensionTypeColor: 'Color:',
    productDimensionTypeSize: 'Size:',
    productDimensionTypeStyle: 'Style:',
    productDimensionTypeAmount: 'Amount',
    genericErrorMessage: 'Something went wrong. Please try again later.',
    trackingLabel: 'Tracking number:',
    trackingComingSoonLabel: 'Not yet available',
    trackingAriaLabel: 'Track your order',
    pointsEarnedLabel: 'Points earned',
    backToShopping: 'Back to shopping',
    configString: 'Configuration',
    discountStringText: 'Savings ',
    posChannelNameText: 'Purchased at ',
    onlineStoreChannelNameText: 'Online purchase',
    emailSalesLines: 'Email delivery',
    orderStatusEmailed: 'Emailed',
    pickupDateTimeslotText: 'Pickup date and time slot',
    pickupTimeslotPlaceHolder: '{0} - {1}',
    shippingCharges: 'Shipping Charges',
    orderDetailsListViewModeAriaLabel: 'orderDetailsListViewModeAriaLabel',
    orderDetailsDetailedViewModeAriaLabel: 'orderDetailsDetailedViewModeAriaLabel',
    orderDetailsProductNumberText: 'orderDetailsProductNumberText',
    orderDetailsProductText: 'orderDetailsProductText',
    orderDetailsUnitPriceText: 'orderDetailsUnitPriceText',
    orderDetailsModeOfDeliveryText: 'orderDetailsModeOfDeliveryText',
    orderDetailsStatusText: 'orderDetailsStatusText',
    orderDetailsQuantityText: 'orderDetailsQuantityText',
    orderDetailsUnitOfMeasureText: 'orderDetailsUnitOfMeasureText',
    orderDetailsTotalText: 'orderDetailsTotalText',
    orderDetailsProductDimensionsSeparator: 'orderDetailsProductDimensionsSeparator',
    orderDetailsQuantityMobileText: 'orderDetailsQuantityMobileText',
    orderDetailsActionsText: 'orderDetailsActionsText',
    orderDetailsViewDetailsButtonText: 'orderDetailsViewDetailsButtonText',
    orderDetailsViewDetailsButtonAriaLabel: 'orderDetailsViewDetailsButtonAriaLabel',
    orderDetailsBuySelectedButtonText: 'orderDetailsBuySelectedButtonText',
    orderDetailsBuySelectedButtonAriaLabel: 'orderDetailsBuySelectedButtonAriaLabel',
    orderDetailsSelectAllRadioAriaLabelText: 'orderDetailsSelectAllRadioAriaLabelText',
    orderDetailsSelectRadioAriaLabelText: 'orderDetailsSelectRadioAriaLabelText',
    orderDetailsBuySelectedAddingToCartErrorNotificationTitle: 'orderDetailsBuySelectedAddingToCartErrorNotificationTitle',
    orderDetailsBuySelectedAddingToCartErrorNotificationCloseAriaLabel:
        'orderDetailsBuySelectedAddingToCartErrorNotificationCloseAriaLabel',
    orderDetailsBuyItAgainButtonText: 'orderDetailsBuyItAgainButtonText',
    orderDetailsBuyItAgainButtonAriaLabel: 'orderDetailsBuyItAgainButtonAriaLabel',
    orderDetailsDisableSelectionButtonText: 'orderDetailsDisableSelectionButtonText',
    orderDetailsDisableSelectionButtonAriaLabel: 'orderDetailsDisableSelectionButtonAriaLabel',
    orderDetailsEnableSelectionButtonText: 'orderDetailsEnableSelectionButtonText',
    orderDetailsEnableSelectionButtonAriaLabel: 'orderDetailsEnableSelectionButtonAriaLabel',
    orderDetailsGoToCartText: 'orderDetailsGoToCartText',
    orderDetailsDialogCloseText: 'orderDetailsDialogCloseText',
    orderDetailsSingleItemText: 'orderDetailsSingleItemText',
    orderDetailsMultiLineItemFormatText: 'orderDetailsMultiLineItemFormatText',
    orderDetailsMultiLinesFormatText: 'orderDetailsMultiLinesFormatText',
    orderDetailsHeaderMessageText: 'orderDetailsHeaderMessageText',
    orderDetailsBuyItemsAgainHeaderText: 'orderDetailsBuyItemsAgainHeaderText',
    orderDetailsMultiItemsValidationErrorMessage: 'orderDetailsMultiItemsValidationErrorMessage',
    orderDetailsOneErrorText: 'orderDetailsOneErrorText',
    orderDetailsMultiErrorText: 'orderDetailsMultiErrorText',
    addedQuantityText: 'addedQuantityText',
    originalPriceText: 'originalPriceText',
    currentPriceText: 'currentPriceText',
    qrCodeSrText: 'Scan QR code',
    orderDetailsUnavailableProductText: 'orderDetailsUnavailableProductText',
    orderPlacedByText: 'orderPlacedByText',
    orderPlacedByFullText: 'orderPlacedByFullText',
    orderOnBehalfOfText: 'orderOnBehalfOfText',
    orderPlacedByYouText: 'orderPlacedByYouText'
};
