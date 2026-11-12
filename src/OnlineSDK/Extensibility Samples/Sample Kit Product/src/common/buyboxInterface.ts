/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { ProductDimensionFull, RatingsSummary } from '@msdyn365-commerce/commerce-entities';
import * as Msdyn365 from '@msdyn365-commerce/core';
import { ICartState } from '@msdyn365-commerce/global-state';
import {
    AsyncResult,
    CommerceList,
    Customer,
    FeatureState,
    ProductDeliveryOptions,
    ProductPrice,
    SimpleProduct
} from '@msdyn365-commerce/retail-proxy';
import { IStoreSelectorStateManager } from '@msdyn365-commerce-modules/bopis-utilities';
import {
    IDimensionForSelectedVariant,
    IProductInventoryInformation,
    ISelectedProduct,
    OrderTemplate
} from '@msdyn365-commerce-modules/retail-actions';
import { INodeProps } from '@msdyn365-commerce-modules/utilities';

export interface IBuyboxState {
    quantity: number;
    min: number | undefined;
    max: number | undefined;
    errorState: IErrorState;
    selectedProduct?: Promise<SimpleProduct | null>;
    productAvailableQuantity?: IProductInventoryInformation;
    productPrice?: ProductPrice;
    productDeliveryOptions?: ProductDeliveryOptions;
    modalOpen?: boolean;
    isUpdatingDimension?: boolean;
    isUpdatingDeliveryOptions?: boolean;
    isServiceItem?: boolean;
    isPriceKeyedIn?: boolean;
    keyInPriceAmount?: number;
    isCustomPriceSelected?: boolean;
    activeIndex?: number;
    animating?: boolean;
    lastUpdate?: number;
    mediaGalleryItems?: Msdyn365.IImageData[] | undefined;
}

export interface IBuyboxCommonData {
    deliveryOptions: AsyncResult<ProductDeliveryOptions | undefined> | undefined;
    product: AsyncResult<SimpleProduct>;
    productDimensions: AsyncResult<ProductDimensionFull[]>;
    productPrice: AsyncResult<ProductPrice>;
    ratingsSummary: AsyncResult<RatingsSummary>;
    storeSelectorStateManager: AsyncResult<IStoreSelectorStateManager>;
    wishlists: AsyncResult<CommerceList[]>;
    orderTemplates: AsyncResult<OrderTemplate[]>;
    cart: AsyncResult<ICartState>;
    productAvailableQuantity: AsyncResult<IProductInventoryInformation[]>;
    customerInformation: AsyncResult<Customer>;
    featureState: AsyncResult<FeatureState[]>;
}
export declare type IBuyboxErrorHost = 'ADDTOCART' | 'FINDINSTORE' | 'WISHLIST' | 'SHOPSIMILARITEM' | 'ORDER_TEMPLATE';

export interface IErrorState {
    errorHost?: IBuyboxErrorHost;
    configureErrors: { [configureId: string]: string | undefined };
    quantityError?: string;
    customAmountError?: string;
    otherError?: string;
}

export interface IBuyboxAddToCartViewProps {
    ContainerProps: INodeProps;
    errorBlock?: React.ReactNode;
    button?: React.ReactNode;
}

export interface IBuyboxAddToOrderTemplateViewProps {
    ContainerProps: INodeProps;
    errorBlock?: React.ReactNode;
    button?: React.ReactNode;
}

export interface IBuyboxProductQuantityViewProps {
    ContainerProps: INodeProps;
    LabelContainerProps: INodeProps;

    heading: React.ReactNode;
    errors?: React.ReactNode;

    input: React.ReactNode;
}

export interface IBuyboxAddToWishlistViewProps {
    ContainerProps: INodeProps;
    errorBlock?: React.ReactNode;
    button?: React.ReactNode;
}

export interface IBuyboxShopSimilarLookViewProps {
    ContainerProps: INodeProps;
    errors?: React.ReactNode;
    input: React.ReactNode;
}

export enum ShopSimiliarButtonType {
    Looks = 'looks',
    Description = 'descriptions'
}

export interface IBuyboxKeyInPriceViewProps {
    ContainerProps: INodeProps;
    LabelContainerProps: INodeProps;
    heading: React.ReactNode;
    input: React.ReactNode;
}

export interface IBuyboxErrorBlockProps {
    configureErrors: { [configureId: string]: string | undefined };
    quantityError?: string;
    customAmountError?: string;
    otherError?: string;
    resources: IBuyboxCommonResources;
    showError: boolean;
}

export interface IBuyboxProductConfigureProps {
    product: SimpleProduct;
    productDimensions: ProductDimensionFull[];
    resources: IBuyboxCommonResources;
    channelId: number;
    actionContext: Msdyn365.IActionContext;
    errors: { [configureId: string]: string | undefined };

    dimensionChanged?(newValue: number): void;
}

export interface IBuyboxProductConfigureDropdownViewProps {
    ContainerProps: INodeProps;
    LabelContainerProps: INodeProps;

    heading: React.ReactNode;
    errors?: React.ReactNode;

    select: React.ReactNode;
}

export interface IBuyboxProductConfigureViewProps {
    ContainerProps: INodeProps;

    dropdowns: IBuyboxProductConfigureDropdownViewProps[];
}

export interface IBuyboxCallbacks {
    updateQuantity(newQuantity: number): boolean;
    updateKeyInPrice(keyInPrice: number): void;
    updateErrorState(newErrorState: IErrorState): void;
    updateSelectedProduct(
        selectedProduct: Promise<SimpleProduct | null>,
        newInventory: IProductInventoryInformation | undefined,
        newPrice: ProductPrice | undefined,
        newDeliveryOptions: ProductDeliveryOptions | undefined
    ): void;
    getDropdownName(dimensionType: number, resources: IBuyboxCommonResources): string;
    dimensionSelectedAsync(selectedDimensionId: number, selectedDimensionValueId: string): Promise<void>;
    changeModalOpen(isModalOpen: boolean): void;
    changeUpdatingDimension(isUpdatingDimension: boolean): void;
    changeUpdatingDeliveryOptions?(isUpdatingDeliveryOptions: boolean): void;
}

export interface IBuyboxExtentedProps<T> extends Msdyn365.IModule<T> {
    resources: IBuyboxCommonResources;
    config: IBuyboxCommonConfig;
    slots: {};
}

export interface IBuyboxCommonConfig {
    titleHeadingTag?: titleHeadingTag;
    className?: string;
    enableShopSimilarLooks?: boolean;
    enableShopSimilarDescription?: boolean;
    enableKeyInPrice?: boolean;
    minimumKeyInPrice?: number;
    maximumKeyInPrice?: number;
}

export interface IProductDetails {
    product?: ISelectedProduct | null;
    productPrice?: ProductPrice | null;
    ratingsSummary?: RatingsSummary;
    productAvailableQuantity?: IProductInventoryInformation[];
    cart?: ICartState | undefined;
    productDimensions?: IDimensionForSelectedVariant[] | null;
}

export enum titleHeadingTag {
    h1 = 'h1',
    h2 = 'h2',
    h3 = 'h3',
    h4 = 'h4',
    h5 = 'h5',
    h6 = 'h6'
}

export interface IBuyboxCommonResources {
    priceFree: string;
    invalidSmallCustomAmountText: string;
    invalidLargeCustomAmountText: string;
    buyBoxGoToCartText: string;
    buyBoxContinueShoppingText: string;
    closeNotificationLabel: string;
    buyBoxSingleItemText: string;
    buyBoxMultiLineItemFormatText: string;
    buyBoxHeaderMessageText: string;
    originalPriceText: string;
    currentPriceText: string;
    addedQuantityText: string;
    addToCartText: string;
    outOfStockText: string;
    averageRatingAriaLabel: string;
    addToOrderTemplateHeader: string;
    noOrderTemplatesMessage: string;
    noOrderTemplatesDescription: string;
    createAnOrderTemplateButtonText: string;
    createNewOrderTemplateButtonText: string;
    cancelOrderTemplateCreationButtonText: string;
    selectTemplatesText: string;
    addToTemplateButtonText: string;
    lineItemsText: string;
    createOrderTemplateHeader: string;
    orderTemplateTitle: string;
    orderTemplateNameAriaLabel: string;
    createOrderTemplateDescription: string;
    defaultOrderTemplateName: string;
    createOrderTemplateButtonText: string;
    cancelNewOrderTemplateCreationButtonText: string;
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
    addToWishlistMessage: string;
    removedFromWishlistMessage: string;
    addItemToWishlistError: string;
    removeItemFromWishlistError: string;
    nameOfWishlist: string;
    productQuantityHeading: string;
    inputQuantityAriaLabel: string;
    buyboxKeyInPriceLabelHeading: string;
    buyboxErrorMessageHeader: string;
    maxQuantityText: string;
    minQuantityText: string;
    selectDimensionFormatString: string;
    errorMessageOutOfStock: string;
    errorMessageOutOfRangeOneLeft: string;
    errorMessageOutOfRangeFormat: string;
    addToOrderTemplateButtonText: string;
    addedToCartFailureMessage: string;
    maxQuantityLimitText: string;
    productDimensionTypeColorErrorMessage: string;
    productDimensionTypeConfigurationErrorMessage: string;
    productDimensionTypeSizeErrorMessage: string;
    productDimensionTypeAmountErrorMessage: string;
    productDimensionTypeStyleErrorMessage: string;
    shopSimilarLooksText?: string;
    shopSimilarDescriptionText?: string;
    addToOrderTemplateButtonTooltip: string;
    loadingText?: string;
    informationIconText?: string;
    priceRangeSeparator?: string;
    swatchItemAriaLabel?: string;
    salesAgreementPricePrompt?: string;
    salesAgreementExpirationDatePrompt?: string;
    salesAgreementCommittedQuantityPrompt?: string;
    salesAgreementRemainingQuantityPrompt?: string;
}
