/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IBuybox containerModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';
import * as React from 'react';

export const enum titleHeadingTag {
    h1 = 'h1',
    h2 = 'h2',
    h3 = 'h3',
    h4 = 'h4',
    h5 = 'h5',
    h6 = 'h6'
}

export interface IBuyboxConfig extends Msdyn365.IModuleConfig {
    titleHeadingTag?: titleHeadingTag;
    enableShopSimilarLooks?: boolean;
    enableShopSimilarDescription?: boolean;
    enableKeyInPrice?: boolean;
    minimumKeyInPrice?: number;
    maximumKeyInPrice?: number;
    className?: string;
    clientRender?: boolean;
}

export interface IBuyboxResources {
    findInStoreLinkText: string;
    findInStoreHeaderText: string;
    findInStoreDescriptionText: string;
    priceFree: string;
    originalPriceText: string;
    currentPriceText: string;
    addToCartText: string;
    outOfStockText: string;
    averageRatingAriaLabel: string;
    selectDimensionFormatString: string;
    productDimensionTypeColor: string;
    productDimensionTypeConfiguration: string;
    productDimensionTypeSize: string;
    productDimensionTypeStyle: string;
    productDimensionTypeAmount: string;
    createOrderTemplateHeader: string;
    orderTemplateTitle: string;
    orderTemplateNameAriaLabel: string;
    createOrderTemplateDescription: string;
    createNewOrderTemplateButtonText: string;
    createOrderTemplateButtonText: string;
    cancelNewOrderTemplateCreationButtonText: string;
    defaultOrderTemplateName: string;
    addToOrderTemplateHeader: string;
    addToOrderTemplateButtonTooltip: string;
    noOrderTemplatesMessage: string;
    noOrderTemplatesDescription: string;
    createAnOrderTemplateButtonText: string;
    cancelOrderTemplateCreationButtonText: string;
    selectTemplatesText: string;
    addToTemplateButtonText: string;
    lineItemsText: string;
    addToOrderTemplateButtonText: string;
    addToOrderTemplateMessage: string;
    addItemToOrderTemplateError: string;
    viewOrderTemplateButtonText: string;
    continueShoppingButtonText: string;
    itemAddedToOrderTemplateHeaderItemOneText: string;
    itemAddedToOrderTemplateHeaderItemFormatText: string;
    itemAddedToOrderTemplateHeaderMessageText: string;
    duplicatedProductsHeader: string;
    duplicatedProductsDescription: string;
    updateQuantityButtonText: string;
    cancelDuplicateItemsButtonText: string;
    addToWishlistButtonText: string;
    removeFromWishlistButtonText: string;
    nameOfWishlist: string;
    inputQuantityAriaLabel: string;
    addToWishlistMessage: string;
    removedFromWishlistMessage: string;
    addItemToWishlistError: string;
    removeItemFromWishlistError: string;
    productQuantityHeading: string;
    errorMessageOutOfStock: string;
    errorMessageOutOfRangeOneLeft: string;
    errorMessageOutOfRangeFormat: string;
    productDimensionTypeColorErrorMessage: string;
    productDimensionTypeConfigurationErrorMessage: string;
    productDimensionTypeSizeErrorMessage: string;
    productDimensionTypeStyleErrorMessage: string;
    productDimensionTypeAmountErrorMessage: string;
    buyboxErrorMessageHeader: string;
    addedToCartFailureMessage: string;
    maxQuantityLimitText: string;
    invalidSmallCustomAmountText: string;
    invalidLargeCustomAmountText: string;
    shopSimilarLooksText: string;
    shopSimilarDescriptionText: string;
    buyBoxGoToCartText: string;
    buyBoxContinueShoppingText: string;
    buyBoxSingleItemText: string;
    buyBoxMultiLineItemFormatText: string;
    buyBoxHeaderMessageText: string;
    maxQuantityText: string;
    minQuantityText: string;
    addedQuantityText: string;
    buyboxKeyInPriceLabelHeading: string;
    descriptionTextToShowAllPickupOptions: string;
    closeNotificationLabel: string;
    findInStoreNotAvailableTextButton: string;
    findInStoreNotAvailableDescription: string;
    informationIconText: string;
    priceRangeSeparator: string;
    bulkPurchaseLinkText: string;
    swatchItemAriaLabel: string;
    salesAgreementPricePrompt: string;
    salesAgreementExpirationDatePrompt: string;
    salesAgreementCommittedQuantityPrompt: string;
    salesAgreementRemainingQuantityPrompt: string;
    decrementButtonAriaLabel: string;
    incrementButtonAriaLabel: string;
}

export interface IBuyboxProps<T> extends Msdyn365.IModule<T> {
    resources: IBuyboxResources;
    config: IBuyboxConfig;
    slots: {
        mediaGallery: React.ReactNode[];
        storeSelector: React.ReactNode[];
        textBlocks: React.ReactNode[];
        socialShare: React.ReactNode[];
        productComparisonButton: React.ReactNode[];
    };
}
