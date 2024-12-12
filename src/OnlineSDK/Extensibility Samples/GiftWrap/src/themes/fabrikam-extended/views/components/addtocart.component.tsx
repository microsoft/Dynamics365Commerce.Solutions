/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import MsDyn365, {
    IActionContext,
    getCatalogId,
    IComponent,
    IComponentProps,
    IGridSettings,
    IImageSettings,
    msdyn365Commerce,
    TelemetryEvent
} from '@msdyn365-commerce/core';
import { getCartState, ICartActionResult, ICartState } from '@msdyn365-commerce/global-state';
import {
    Cart,
    CommerceProperty,
    CartLineValidationResults,
    CartsDataActions,
    format,
    ProductAvailableQuantity,
    ProductDimension,
    ProductPrice,
    ProductsDataActions,
    ProductSearchCriteria,
    ProductType,
    SimpleProduct
} from '@msdyn365-commerce/retail-proxy';
import { ErrorNotification, NotificationsManager } from '@msdyn365-commerce-modules/notifications-core';
import { ArrayExtensions, generateProductImageUrl, ObjectExtensions } from '@msdyn365-commerce-modules/retail-actions';
import { getPayloadObject, getTelemetryAttributes, IPopupProps, ITelemetryContent, Popup } from '@msdyn365-commerce-modules/utilities';
import classnames from 'classnames';
import React, { useState } from 'react';

import { updateAsync, updateCartLinesAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/CartsDataActions.g';

import {
    IItemsAddedToCartResources,
    ItemsAddedToCartDialogComponent,
    ItemSuccessfullyAddedToCartNotification,
    MultiItemsSuccessfullyAddedToCartNotification,
    OrderDetailsProduct,
    PriceComponent
} from '@msdyn365-commerce/components';

/**
 * Interface for add to cart resources.
 */
export interface IAddToCartResources {
    goToCartText: string;
    continueShoppingText: string;
    closeNotificationLabel: string;
    headerItemOneText: string;
    headerItemFormatText: string;
    headerLinesFormatText?: string;
    headerMessageText: string;
    freePriceText: string;
    originalPriceText: string;
    currentPriceText: string;
    addedQuantityText: string;
    buyAgainNotificationTitle?: string;
    buyAgainNotificationCloseAriaLabel?: string;
    validationErrorMessage?: string;
    oneErrorText?: string;
    multiErrorsText?: string;
}

/**
 * Interface for add to cart components props.
 */
export interface IAddToCartComponentProps extends IComponentProps<{ product: SimpleProduct; price?: ProductPrice } | undefined> {
    className?: string;
    addToCartText: string;
    outOfStockText?: string;
    disabled?: boolean;
    quantity?: number;
    navigationUrl?: string;
    productAvailability?: ProductAvailableQuantity;
    getSelectedProduct?: Promise<SimpleProduct | null>;
    isNavigationToCartPageDisabled?: boolean;
    shouldSkipSiteSettings?: boolean;
    addToCartArialLabel?: string;
    products?: {
        productId: number;
        quantity: number;
        catalogId?: number;
    }[];
    orderDetailsProducts?: OrderDetailsProduct[];
    hasAvailableProducts?: boolean;
    imageSettings?: IImageSettings;
    gridSettings?: IGridSettings;

    isLoading?: boolean;
    isUpdatingDimension?: boolean;
    isLoadingDeliveryOptions?: boolean;
    isUpdatingDeliveryOptions?: boolean;
    isAddServiceItemToCart?: boolean;
    isAddEmailDeliveryItemToCart?: boolean;
    isPriceKeyedIn?: boolean;
    customPriceAmount?: number;
    isOrderQuantityLimitsFeatureEnabled?: boolean;

    dialogStrings?: IAddToCartResources;

    telemetryContent?: ITelemetryContent;

    catalogId?: number;

    isCustomPriceSelected?: boolean;
    maximumKeyInPrice?: number;
    minimumKeyInPrice?: number;
    defaultMaximumKeyInPrice?: number;
    defaultMinimumKeyInPrice?: number;
    isProductQuantityLoading?: boolean;

    onAdd?(result: ICartActionResult): void;
    onError?(result: IAddToCartFailureResult): void;
    changeUpdatingDimension?(isUpdatingDimension: boolean): void;
    changeUpdatingDeliveryOptions?(isUpdatingDeliveryOptions: boolean): void;
}

/**
 * Cart action Failure reason type.
 */
export declare type ICartActionFailureReason =
    | 'EMPTYINPUT'
    | 'MISSINGDIMENSION'
    | 'OUTOFSTOCK'
    | 'CARTACTIONFAILED'
    | 'INVALIDCUSTOMAMOUNT'
    | 'UPDATECARTLINEATTRIBUTE';

/**
 * Interface for add to cart failure result.
 */
export interface IAddToCartFailureResult {
    failureReason: ICartActionFailureReason;

    stockLeft?: number;
    cartActionResult?: ICartActionResult;
    missingDimensions?: ProductDimension[];
}

/**
 * This setting defines Gift Wrap Key.
 */
export const GiftWrapKey = 'isGiftWrap';

/**
 * This setting defines the experience when a product is added to cart. Corresponds to the configuration in Fabrikam.
 */
export enum AddToCartBehavior {
    /**
     * Navigate to cart page.
     */
    goToCart = 'goToCart',

    /**
     * Show item added to cart popup.
     */
    showModal = 'showModal',

    /**
     * Show mini cart popup.
     */
    showMiniCart = 'showMiniCart',

    /**
     * Show item added to cart notification.
     */
    showNotification = 'showNotification',

    /**
     * Do nothing and stay on the page.
     */
    nothing = 'nothing'
}

const zero = 0;
const defaultQuantity = 1;

/**
 * The propagate result.
 * @param props -- The add to cart component props.
 * @param result -- The cart action result.
 */
const propagateResult = (props: IAddToCartComponentProps, result: ICartActionResult): void => {
    if (props.onAdd) {
        props.onAdd(result);
    }
};

/**
 * The propagate error.
 * @param props -- The add to cart component props.
 * @param result -- The add to cart failure result.
 */
const propagateError = (props: IAddToCartComponentProps, result: IAddToCartFailureResult): void => {
    if (props.onError) {
        props.onError(result);
    }
};

/**
 * Retrieves add to cart input from the component props.
 * @param props - Add to cart component props.
 * @returns Add to cart input.
 */
const getAddToCartInputFromProps = async (props: IAddToCartComponentProps) => {
    const products = props.orderDetailsProducts?.filter(product => !ObjectExtensions.isNullOrUndefined(product.simpleProduct));
    const input = products?.map(product => {
        return {
            product: product.simpleProduct!,
            count: ObjectExtensions.isNullOrUndefined(product.salesLine.Quantity) ? defaultQuantity : product.salesLine.Quantity,
            catalogId: product.salesLine.CatalogId
        };
    });

    if (ArrayExtensions.hasElements(input)) {
        return input;
    }

    if (ArrayExtensions.hasElements(props.products)) {
        const actionContext = props.context.actionContext;
        const apiSettings = actionContext.requestContext.apiSettings;

        const searchCriteriaInput: ProductSearchCriteria = {
            Context: {
                ChannelId: apiSettings.channelId,
                CatalogId: props.catalogId ?? getCatalogId(props.context.actionContext.requestContext)
            },
            IncludeAttributes: false,
            Ids: props.products.map(product => product.productId)
        };

        const searchResult = await ProductsDataActions.searchByCriteriaAsync({ callerContext: actionContext }, searchCriteriaInput);
        const productSearchResultsWithImages = searchResult.map(productSearchResult => {
            const newProductSearchResult = { ...productSearchResult };
            const newImageUrl = generateProductImageUrl(newProductSearchResult, apiSettings);

            if (newImageUrl) {
                newProductSearchResult.PrimaryImageUrl = newImageUrl;
            }

            return newProductSearchResult;
        });

        return props.products.map(item => {
            const productData = productSearchResultsWithImages.find(product => product.RecordId === item.productId);
            let simpleProduct: SimpleProduct;
            if (productData) {
                simpleProduct = {
                    ...productData,
                    ProductTypeValue: ProductType.Variant,
                    AdjustedPrice: productData.Price,
                    BasePrice: productData.BasePrice ?? productData.Price
                };
            } else {
                // eslint-disable-next-line @typescript-eslint/consistent-type-assertions -- Cast to simple product since only record id is available.
                simpleProduct = {
                    RecordId: item.productId
                } as SimpleProduct;
            }

            return {
                product: simpleProduct,
                count: item.quantity,
                catalogId: item.catalogId
            };
        });
    }

    if (props.data?.product) {
        return [
            {
                product: props.data.product,
                count: props.quantity ?? defaultQuantity,
                catalogId: props.catalogId
            }
        ];
    }

    return [];
};

/**
 * Add one item to cart when it is not buy again.
 * @param props -- The props.
 * @param setDisabled -- The set disable call back.
 * @param openModal -- The open modal call back.
 */
const addOneItemToCart = async (
    props: IAddToCartComponentProps,
    setDisabled: (disabled: boolean) => void,
    openModal: (opened: boolean) => void
): Promise<void> => {
    // eslint-disable-next-line @typescript-eslint/no-unsafe-assignment, @typescript-eslint/no-unsafe-member-access -- read config file.
    const addToCartBehavior = props.context.app.config.addToCartBehavior;
    const data = props.data;

    const addToCartInput = await getAddToCartInputFromProps(props);
    const product = addToCartInput[0].product;
    const quantity = addToCartInput[0].count;

    const cartState = await getCartState(props.context.actionContext);
    const addToCartResult = await cartState.addProductToCart({
        product,
        count: quantity,
        availableQuantity: props.productAvailability?.AvailableQuantity,
        additionalProperties: { orderQuantityLimitsFeatureIsEnabled: props.isOrderQuantityLimitsFeatureEnabled },
        // eslint-disable-next-line @typescript-eslint/no-unsafe-assignment, @typescript-eslint/no-unsafe-member-access -- read config file.
        enableStockCheck: props.context.app.config.enableStockCheck,
        isPriceKeyedIn: props.isPriceKeyedIn,
        customPrice: props.customPriceAmount,
        isAddEmailDeliveryItemToCart: props.isAddEmailDeliveryItemToCart,
        shouldSkipSiteSettings: props.shouldSkipSiteSettings,
        catalogId: addToCartInput[0].catalogId
    });

    if (addToCartResult.status === 'SUCCESS') {
        const value = getProductExtensionProperty(props.data?.product!, GiftWrapKey);
        updateCartLineAttributeValues(
            props.context.actionContext,
            cartState.cart,
            props.data?.product!,
            GiftWrapKey,
            value.toString()
        ).catch(error => {
            propagateError(props, { failureReason: 'UPDATECARTLINEATTRIBUTE', cartActionResult: addToCartResult });
        });
        if (props.dialogStrings && addToCartBehavior === AddToCartBehavior.showModal) {
            setDisabled(false);
            openModal(true);
        } else if (props.dialogStrings && addToCartBehavior === AddToCartBehavior.showNotification) {
            setDisabled(false);
            const notification = new ItemSuccessfullyAddedToCartNotification(
                props.context,
                props.dialogStrings,
                props.imageSettings,
                props.gridSettings,
                product,
                data?.price,
                quantity,
                props.navigationUrl,
                props.telemetryContent!,
                props.id,
                props.typeName
            );
            NotificationsManager.instance().addNotification(notification);
        } else if (
            MsDyn365.isBrowser &&
            props.navigationUrl &&
            !props.isNavigationToCartPageDisabled &&
            (addToCartBehavior === undefined || addToCartBehavior === AddToCartBehavior.goToCart)
        ) {
            window.location.assign(props.navigationUrl);
        } else {
            setDisabled(false);
        }
        propagateResult(props, addToCartResult);
    } else {
        NotificationsManager.instance().addNotification(
            new ErrorNotification(
                addToCartResult.errorDetails?.LocalizedMessage ?? 'Add to cart failed',
                props.dialogStrings?.closeNotificationLabel ?? ''
            )
        );

        propagateError(props, { failureReason: 'CARTACTIONFAILED', cartActionResult: addToCartResult });
        setDisabled(false);
    }
};

/**
 * Callback to handle success of adding to cart.
 * @param props - Add to cart component props.
 * @param setDisabled - Callback to update disabled state of the component.
 * @param setItemsAddedToCartDialogOpen - Callback to update disabled state of the component.
 * @param setErrorMessage - Callback to update error message state of the component.
 * @param cartState - Current cart state.
 * @param addToCartInput - Input used for adding to cart.
 * @param addToCartResult - Result of adding to cart.
 */
const handleAddItemsToCartSuccess = async (
    props: IAddToCartComponentProps,
    setDisabled: (disabled: boolean) => void,
    setItemsAddedToCartDialogOpen: (opened: boolean) => void,
    setErrorMessage: (message: string) => void,
    cartState: ICartState,
    addToCartInput: {
        product: SimpleProduct;
        count: number;
    }[],
    addToCartResult: ICartActionResult
) => {
    // Validate cart for line errors.
    const validationResult: CartLineValidationResults = await CartsDataActions.validateForCheckoutAsync(
        { callerContext: props.context.actionContext, bypassCache: 'none' },
        cartState.cart.Id,
        cartState.cart.Version
    );
    const errorCount = (validationResult.ValidationFailuresByCartLines ?? []).length;
    const singleErrorCount = 1;
    const errorText = errorCount === singleErrorCount ? props.dialogStrings?.oneErrorText : props.dialogStrings?.multiErrorsText;
    const errorMessage =
        errorCount > zero ? format(props.dialogStrings?.validationErrorMessage ?? '', errorCount.toString(), errorText) : '';

    setErrorMessage(errorMessage);

    // eslint-disable-next-line @typescript-eslint/no-unsafe-assignment, @typescript-eslint/no-unsafe-member-access -- read config file.
    const addToCartBehavior = props.context.app.config.addToCartBehavior;

    if (props.dialogStrings && addToCartBehavior === AddToCartBehavior.showModal) {
        setDisabled(false);
        setItemsAddedToCartDialogOpen(true);
    } else if (props.dialogStrings && addToCartBehavior === AddToCartBehavior.showNotification) {
        setDisabled(false);
        const itemsAddedResource: IItemsAddedToCartResources = {
            viewCartButtonText: props.dialogStrings.goToCartText,
            closeButtonLabel: props.dialogStrings.closeNotificationLabel,
            itemsAddedToCartHeaderText: props.dialogStrings.headerMessageText,
            itemsAddedToCartFormatText: props.dialogStrings.headerItemFormatText,
            linesAddedToCartFormatText: props.dialogStrings.headerLinesFormatText ?? '{0}',
            itemsAddedValidationErrorMessage: errorMessage
        };

        const notification = new MultiItemsSuccessfullyAddedToCartNotification(
            props.context,
            itemsAddedResource,
            props.imageSettings,
            props.gridSettings,
            addToCartInput,
            props.navigationUrl,
            props.telemetryContent!,
            props.id,
            props.typeName
        );
        NotificationsManager.instance().addNotification(notification);
    } else if (
        MsDyn365.isBrowser &&
        props.navigationUrl &&
        !props.isNavigationToCartPageDisabled &&
        (addToCartBehavior === undefined || addToCartBehavior === AddToCartBehavior.goToCart)
    ) {
        window.location.assign(props.navigationUrl);
    } else {
        setDisabled(false);
    }
    propagateResult(props, addToCartResult);
};

/**
 * Add one or multi item to cart from buy again.
 * @param props -- The props.
 * @param setDisabled -- The set disable call back.
 * @param setItemsAddedToCartDialogOpen -- The open modal call back.
 * @param setErrorMessage -- The set error message call back.
 */
const addItemsToCart = async (
    props: IAddToCartComponentProps,
    setDisabled: (disabled: boolean) => void,
    setItemsAddedToCartDialogOpen: (opened: boolean) => void,
    setErrorMessage: (message: string) => void
): Promise<void> => {
    const addToCartInput = await getAddToCartInputFromProps(props);

    if (!ArrayExtensions.hasElements(addToCartInput)) {
        return;
    }

    const cartState = await getCartState(props.context.actionContext);
    const addToCartResult = await cartState.addProductsToCart(addToCartInput);

    if (addToCartResult.status === 'SUCCESS') {
        await handleAddItemsToCartSuccess(
            props,
            setDisabled,
            setItemsAddedToCartDialogOpen,
            setErrorMessage,
            cartState,
            addToCartInput,
            addToCartResult
        );
    } else {
        if (props.dialogStrings?.buyAgainNotificationTitle) {
            // For buy again show simple error notification.
            NotificationsManager.instance().addNotification(
                new ErrorNotification(
                    props.dialogStrings.buyAgainNotificationTitle,
                    props.dialogStrings.buyAgainNotificationCloseAriaLabel ?? ''
                )
            );
        } else {
            NotificationsManager.instance().addNotification(
                new ErrorNotification(
                    addToCartResult.errorDetails?.LocalizedMessage ?? 'Add to cart failed',
                    props.dialogStrings?.closeNotificationLabel ?? ''
                )
            );
        }

        propagateError(props, { failureReason: 'CARTACTIONFAILED', cartActionResult: addToCartResult });
        setDisabled(false);
    }
};

/**
 * Interface for add to cart component.
 */
export interface IAddtoCartComponent extends IComponent<IAddToCartComponentProps> {
    onClick(): (event: React.MouseEvent<HTMLElement>, props: IAddToCartComponentProps) => void;
}

const getProductExtensionProperty = (product: SimpleProduct, extensionPropertyKey: string): boolean => {
    const property =
        product.ExtensionProperties &&
        product.ExtensionProperties.find((extension: CommerceProperty) => extension.Key === extensionPropertyKey);
    if (property) {
        return property.Value?.BooleanValue || false;
    } else {
        return false;
    }
};

const updateCartLineAttributeValues = async (
    actionContext: IActionContext,
    cart: Cart,
    product: SimpleProduct,
    attributekey: string,
    value: string
) => {
    const cartLinesObj = cart.CartLines!.filter(cartLine => cartLine.ProductId === product.RecordId);

    // TODO: add line level attributes
    cartLinesObj[0].AttributeValues = cartLinesObj[0].AttributeValues!.filter(p => p.Name !== attributekey);

    cartLinesObj[0].AttributeValues?.push({
        // @ts-expect-error -- Need to provide data type.
        '@odata.type': '#Microsoft.Dynamics.Commerce.Runtime.DataModel.AttributeTextValue',
        ExtensionProperties: [],
        Name: attributekey,
        TextValue: value,
        TextValueTranslations: []
    });

    const newCart = {
        Id: cart.Id,
        CartLines: cartLinesObj
    };

    await updateCartLinesAsync({ callerContext: actionContext }, newCart.Id.toString(), newCart.CartLines, cart.Version);
    await updateAsync({ callerContext: actionContext }, newCart);
};

/**
 * On click function.
 * @param _event - The mouse event.
 * @param props - The props.
 * @param setDisabled - Flag to define whether the element is disabled.
 * @param openModal - Flag to specify if it should open in a modal window.
 * @param setItemsAddedToCartDialogOpen - Sets items added to cart while dialog is open.
 * @param setErrorMessage - Error message.
 */
const onClick = async (
    _event: React.MouseEvent<HTMLElement>,
    props: IAddToCartComponentProps,
    setDisabled: (disabled: boolean) => void,
    openModal: (opened: boolean) => void,
    setItemsAddedToCartDialogOpen: (opened: boolean) => void,
    setErrorMessage: (message: string) => void
): Promise<void> => {
    if (!ArrayExtensions.hasElements(props.products)) {
        const cartError = addToCartError(props);

        if (cartError) {
            propagateError(props, cartError);
            return;
        }

        setDisabled(true);
    }

    const hasOrderDetailsProducts =
        ArrayExtensions.hasElements(props.orderDetailsProducts) && props.orderDetailsProducts.length > defaultQuantity;
    const hasProducts = ArrayExtensions.hasElements(props.products) && props.products.length > defaultQuantity;

    const hasMultipleProducts = hasOrderDetailsProducts || hasProducts;

    if (props.shouldSkipSiteSettings && hasMultipleProducts) {
        await addItemsToCart(props, setDisabled, setItemsAddedToCartDialogOpen, setErrorMessage);
    } else {
        await addOneItemToCart(props, setDisabled, openModal);
    }
};

/**
 * Add to cart component action constant.
 */
const AddToCartComponentActions = {
    onClick
};

/**
 * Add to cart component.
 * @param props - The props.
 * @returns - The add to cart component.
 */
export const AddToCartFunctionalComponent: React.FC<IAddToCartComponentProps> = (props: IAddToCartComponentProps) => {
    const [disabled, setDisabled] = useState(false);
    const [modalOpen, setModalOpen] = useState(false);
    const [isItemsAddedToCartDialogOpen, setItemsAddedToCartDialogOpen] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');
    const [addToCartInput, setAddToCartInput] = useState<
        {
            product: SimpleProduct;
            count: number;
        }[]
    >([]);

    const onClickHandler = async (event: React.MouseEvent<HTMLElement>) => {
        await AddToCartComponentActions.onClick(event, props, setDisabled, setModalOpen, setItemsAddedToCartDialogOpen, setErrorMessage);
    };

    /**
     * Close dialog.
     */
    const closeItemsAddedToCartDialog = React.useCallback(() => {
        setItemsAddedToCartDialogOpen(false);
    }, []);

    const priceComponent = props.data?.price ? (
        <PriceComponent
            data={{ price: props.data.price }}
            context={props.context}
            id={props.id}
            typeName={props.typeName}
            freePriceText={props.dialogStrings?.freePriceText}
            originalPriceText={props.dialogStrings?.originalPriceText}
            currentPriceText={props.dialogStrings?.currentPriceText}
        />
    ) : (
        ''
    );

    const popupProps: IPopupProps = {
        context: props.context,
        className: 'msc-add-to-cart',
        id: props.id,
        typeName: props.typeName,
        data: { product: props.data?.product, price: props.data?.price },
        dialogStrings: props.dialogStrings,
        imageSettings: props.imageSettings,
        gridSettings: props.context.request.gridSettings,
        productQuantity: props.quantity !== undefined ? props.quantity : defaultQuantity,
        priceComponent,
        navigationUrl: props.navigationUrl,
        modalOpen,
        setModalOpen,
        telemetryContent: props.telemetryContent
    };

    const itemsAddedResource: IItemsAddedToCartResources = {
        viewCartButtonText: props.dialogStrings?.goToCartText ?? '',
        closeButtonLabel: props.dialogStrings?.closeNotificationLabel ?? 'Close',
        itemsAddedToCartHeaderText: props.dialogStrings?.headerMessageText ?? '',
        itemsAddedToCartFormatText: props.dialogStrings?.headerItemFormatText ?? '',
        linesAddedToCartFormatText: props.dialogStrings?.headerLinesFormatText ?? '{0}',
        itemsAddedValidationErrorMessage: errorMessage
    };

    const renderModalPopup = <Popup {...popupProps} />;
    const label = getLinkText(props);
    const payload = getPayloadObject(TelemetryEvent.AddToCart, props.telemetryContent!, label, '');
    const attributes = getTelemetryAttributes(props.telemetryContent!, payload);
    const buttonClassName = props.shouldSkipSiteSettings ? '' : 'msc-add-to-cart ';

    React.useEffect(() => {
        /**
         * Retrieves add to cart input and updates the state.
         */
        const retrieveAddToCartInput = async () => {
            const retrievedInput = await getAddToCartInputFromProps(props);
            setAddToCartInput(retrievedInput);
        };

        // eslint-disable-next-line @typescript-eslint/no-floating-promises -- Call async method as per the documentation of react useEffect.
        retrieveAddToCartInput();
    }, [props, setAddToCartInput]);

    return (
        <>
            {renderModalPopup}
            <button
                className={classnames(buttonClassName, props.className)}
                aria-label={props.addToCartArialLabel ?? label}
                title={props.addToCartArialLabel ?? label}
                {...attributes}
                onClick={onClickHandler}
                disabled={props.disabled || disabled || isIntermediateState(props) || shouldShowOutOfStock(props, false)}
            >
                {label}
            </button>
            <ItemsAddedToCartDialogComponent
                className='msc-lines-added-to-cart-dialog'
                id={props.id}
                typeName={props.typeName}
                context={props.context}
                dialogStrings={itemsAddedResource}
                data={{
                    products: addToCartInput
                }}
                isModalOpen={isItemsAddedToCartDialogOpen}
                onClose={closeItemsAddedToCartDialog}
            />
        </>
    );
};

// Set default props
AddToCartFunctionalComponent.defaultProps = {
    quantity: defaultQuantity
};

/**
 * Resolves whether product is in stock.
 * @param props - The add to cart component props.
 * @returns Gets the link text.
 */
const getLinkText = (props: IAddToCartComponentProps): string => {
    return shouldShowOutOfStock(props, false) && props.outOfStockText ? props.outOfStockText : props.addToCartText;
};

/**
 * Resolves whether product is in stock.
 * @param props - The add to cart component props.
 * @returns The add to cart failure result.
 */
const addToCartError = (props: IAddToCartComponentProps): IAddToCartFailureResult | undefined => {
    const {
        data,
        productAvailability,
        isCustomPriceSelected,
        customPriceAmount,
        maximumKeyInPrice,
        minimumKeyInPrice,
        defaultMaximumKeyInPrice = 100,
        defaultMinimumKeyInPrice = 10
    } = props;

    if (!data || !data.product.RecordId) {
        // No product exists, won't be able to add to cart
        return { failureReason: 'EMPTYINPUT' };
    }

    if (data.product.Dimensions) {
        const missingDimensions = data.product.Dimensions.filter(
            dimension => !(dimension.DimensionValue && dimension.DimensionValue.Value)
        );

        if (ArrayExtensions.hasElements(missingDimensions)) {
            // At least one dimension with no value exists on the product, won't be able to add to cart
            return { failureReason: 'MISSINGDIMENSION', missingDimensions };
        }
    }

    if (shouldShowOutOfStock(props, true)) {
        const defaultAvailableQuantity = 0; // Showing as out of stock if no available products found.
        const availableQuantity = productAvailability?.AvailableQuantity ?? defaultAvailableQuantity;
        const stockLeft = Math.max(availableQuantity, defaultAvailableQuantity);

        return { failureReason: 'OUTOFSTOCK', stockLeft };
    }

    // When Custom price is selected, if there is no keyed-in price or keyed-in price is out of limit, should return error.
    if (
        isCustomPriceSelected &&
        (!customPriceAmount ||
            customPriceAmount > (maximumKeyInPrice || defaultMaximumKeyInPrice) ||
            customPriceAmount < (minimumKeyInPrice || defaultMinimumKeyInPrice))
    ) {
        return { failureReason: 'INVALIDCUSTOMAMOUNT' };
    }

    // Only allow adding to cart if not showing out of stock
    return undefined;
};

/**
 * Resolves whether product is in stock.
 * @param props - The add to cart component props.
 * @param includeCurrentQuantity - Flag to specify whether current quantity should be included.
 * @returns The dialog element.
 */
const shouldShowOutOfStock = (props: IAddToCartComponentProps, includeCurrentQuantity: boolean): boolean => {
    if (props.context.app.config.enableStockCheck === undefined || props.context.app.config.enableStockCheck === false) {
        return false;
    }

    // When skip site settings do not need show out of stock on adding to cart
    if (props.shouldSkipSiteSettings) {
        return false;
    }

    if (
        props.isLoading ||
        props.isProductQuantityLoading ||
        props.isUpdatingDimension ||
        props.isLoadingDeliveryOptions ||
        props.isUpdatingDeliveryOptions ||
        props.isAddServiceItemToCart
    ) {
        // Out of stock turn off, don't bother showing out of stock
        return false;
    }

    if (!props.data || !props.data.product.RecordId) {
        // No product exists, don't bother showing out of stock
        return false;
    }

    const hasAvailableProducts = props.hasAvailableProducts ?? true;
    if (!hasAvailableProducts) {
        return true;
    }

    if (props.data.product.Dimensions) {
        if (props.data.product.Dimensions.find(dimension => !(dimension.DimensionValue && dimension.DimensionValue.Value))) {
            // At least one dimension with no value exists on the product, so also don't show out of stock
            return false;
        }
    }

    const includedQuantityNumber = includeCurrentQuantity && props.quantity ? props.quantity : defaultQuantity;

    return !(
        props.productAvailability &&
        props.productAvailability.AvailableQuantity !== undefined &&
        props.productAvailability.AvailableQuantity >= includedQuantityNumber
    );
};

const isIntermediateState = (props: IAddToCartComponentProps): boolean => {
    if (props.data?.product.Dimensions) {
        if (props.data.product.Dimensions.find(dimension => !(dimension.DimensionValue && dimension.DimensionValue.Value))) {
            // At least one dimension with no value exists on the product, so also not in intermediate state
            return false;
        }
    }

    if (!props.isLoading && !props.isUpdatingDimension && !props.isLoadingDeliveryOptions && !props.isUpdatingDeliveryOptions) {
        return false;
    }

    return true;
};

export const AddToCartComponent: React.FunctionComponent<IAddToCartComponentProps> = msdyn365Commerce.createComponentOverride<
    // @ts-expect-error -- Compatible issue with the component override.
    IAddtoCartComponent
>('AddToCart', { component: AddToCartFunctionalComponent, ...AddToCartComponentActions });

export default AddToCartComponent;
