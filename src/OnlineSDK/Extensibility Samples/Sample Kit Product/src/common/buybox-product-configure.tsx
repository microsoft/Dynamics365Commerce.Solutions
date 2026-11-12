/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { ICoreContext } from '@msdyn365-commerce/core-internal';
import { ProductDimension, ProductDimensionType } from '@msdyn365-commerce/retail-proxy';
import {
    ArrayExtensions,
    convertProductDimensionTypeToDimensionTypes,
    DimensionTypes,
    getDimensionValuesFromQuery,
    getInventoryLevelCodeFromDimensionValue,
    IDimensionForSelectedVariant,
    IDimensionsApp,
    IDimensionValueForSelectedVariant,
    InventoryLevelValues,
    ObjectExtensions
} from '@msdyn365-commerce-modules/retail-actions';
import { format, getTelemetryObject, ITelemetryContent } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import {
    IBuyboxCallbacks,
    IBuyboxCommonData,
    IBuyboxCommonResources,
    IBuyboxExtentedProps,
    IBuyboxProductConfigureDropdownViewProps,
    IBuyboxProductConfigureViewProps,
    IBuyboxState,
    IProductDetails
} from './buyboxInterface';
import { Dropdown, IDropdownItemProps, IDropdownOnSelectionChangeNotification, SelectionType } from './dropdown';

let telemetryContent: ITelemetryContent;

/**
 * Dimension pre-processing info.
 */
interface IProductDimensionInfo {
    dropdownName: string;
    dropdownId: string;
    dropdownList: IDropdownItemProps[];
}

/**
 * Props for rendering a dropdown.
 */
interface IProductDimensionDropdownProps extends IProductDimensionInfo {
    resources: IBuyboxCommonResources;
    className: string;
    context: ICoreContext<IDimensionsApp>;

    configureErrors: { [configureId: string]: string | undefined };

    getDropdownName(dimensionType: number, resources: IBuyboxCommonResources): string;
    dropdownCallback(notification: IDropdownOnSelectionChangeNotification): Promise<void>;
}

const mapProductDimensionFullToDropdownViewProps = (props: IProductDimensionDropdownProps): IBuyboxProductConfigureDropdownViewProps => {
    const dimensionId = Number.parseInt(props.dropdownId, 10) as ProductDimensionType;
    const dropdownToggleName = format(props.resources.selectDimensionFormatString, props.dropdownName);

    const errorMessage = props.configureErrors[props.dropdownId];

    let dimensionSelectionType = SelectionType.dropdown;

    const dimensionType = convertProductDimensionTypeToDimensionTypes(dimensionId);

    const dimensionsAsSwatchConfiguration = props.context.app.config.dimensionsAsSwatchType;
    if (
        ArrayExtensions.hasElements(dimensionsAsSwatchConfiguration) &&
        !dimensionsAsSwatchConfiguration.includes(DimensionTypes.none) &&
        dimensionsAsSwatchConfiguration.includes(dimensionType)
    ) {
        dimensionSelectionType = SelectionType.swatch;
    }

    return {
        ContainerProps: {
            className: `${props.className}__dropdown`
        },
        LabelContainerProps: {
            tag: 'label',
            className: `${props.className}__dropdown-quantity-label`,
            htmlFor: `${props.className}__dropown-quantity-input-${props.dropdownId}`
        },
        errors: errorMessage && (
            <span className='msc-alert msc-alert-noborder msc-alert-danger' role='alert' aria-live='assertive'>
                <span className='msi-exclamation-triangle' aria-hidden='true' />
                <span>{errorMessage}</span>
            </span>
        ),
        heading: <div>{props.dropdownName}</div>,
        select: (
            <Dropdown
                dropdownId={props.dropdownId}
                dropdownName={props.dropdownName}
                dropdownToggleName={dropdownToggleName}
                dropdownList={props.dropdownList}
                onChange={props.dropdownCallback}
                telemetryContent={telemetryContent}
                selectionType={dimensionSelectionType}
                apiSettings={props.context.request.apiSettings}
            />
        )
    };
};

export function getBuyboxProductConfigure(
    props: IBuyboxExtentedProps<IBuyboxCommonData>,
    state: IBuyboxState,
    callbacks: IBuyboxCallbacks,
    productDetails?: IProductDetails,
    selectedDimensions?: ProductDimension[]
): IBuyboxProductConfigureViewProps | undefined {
    const { resources, typeName } = props;

    const {
        errorState: { configureErrors }
    } = state;

    const { getDropdownName } = callbacks;

    const context = props.context as ICoreContext<IDimensionsApp>;

    // Since the data action can return no value, and the type is non-nullable due to backward compatibility reasons, the null check is required.
    // eslint-disable-next-line @typescript-eslint/no-unnecessary-condition -- Explicitly check for null/undefined.
    const product = props.data.product?.result ? props.data.product.result : productDetails?.product;

    // Since the data action can return no value, and the type is non-nullable due to backward compatibility reasons, the null check is required.
    // eslint-disable-next-line @typescript-eslint/no-unnecessary-condition -- Explicitly check for null/undefined.
    const productDimensionsData = props.data.productDimensions?.result ?? productDetails?.productDimensions;

    // eslint-disable-next-line @typescript-eslint/no-unsafe-assignment,@typescript-eslint/no-unsafe-member-access -- app context is generic
    const enableStockCheck = props.context.app.config.enableStockCheck;

    const hasProductDimensions = ArrayExtensions.hasElements(productDimensionsData);

    if (!product || !hasProductDimensions) {
        return undefined;
    }

    const className = _getClassNamePrefix(typeName);

    const onChanged = async (notification: IDropdownOnSelectionChangeNotification) =>
        _onChanged(notification, callbacks.dimensionSelectedAsync);

    telemetryContent = getTelemetryObject(context.request.telemetryPageName!, props.typeName, props.telemetry);

    const inventoryLevel = context.app.config.inventoryLevel;

    let dimensionsInfo: IProductDimensionInfo[] = [];

    const productDimensions = productDimensionsData as IDimensionForSelectedVariant[];

    dimensionsInfo = productDimensions.map(productDimensionFull => {
        const dropdownName = getDropdownName(productDimensionFull.DimensionTypeValue, resources);
        const dropdownId = productDimensionFull.DimensionTypeValue.toString();

        const dimensions = (productDimensionFull.dimensionValuesWithInventory ??
            productDimensionFull.DimensionValues ??
            []) as IDimensionValueForSelectedVariant[];
        const validProductDimensionsFull = dimensions.filter(
            value => !ObjectExtensions.isNullOrUndefined(value.DimensionValue?.RecordId ?? value.RecordId)
        );

        const dropdownList: IDropdownItemProps[] = validProductDimensionsFull.map<IDropdownItemProps>(dimensionValuesWithInventory => {
            const dimensionValue = dimensionValuesWithInventory.DimensionValue ?? dimensionValuesWithInventory;
            const inventoryLevelCode = getInventoryLevelCodeFromDimensionValue(dimensionValuesWithInventory, inventoryLevel);
            return {
                value: dimensionValue.Value ?? '',
                id: dimensionValue.RecordId.toString(),
                colorHexCode: dimensionValue.ColorHexCode,
                imageUrl: dimensionValue.ImageUrl,
                isDisabled: enableStockCheck && inventoryLevelCode === InventoryLevelValues.outOfStock,
                swatchItemAriaLabel: resources.swatchItemAriaLabel ? resources.swatchItemAriaLabel : ''
            };
        });
        return {
            dropdownName,
            dropdownId,
            dropdownList
        };
    });

    const matchedDimensions = selectedDimensions ?? getDimensionValuesFromQuery(context.request.url.requestUrl);
    for (const dimensionInfo of dimensionsInfo) {
        const currentDimensionsFromQuery = matchedDimensions.filter(
            dimensionFromQuery => dimensionFromQuery.DimensionTypeValue.toString() === dimensionInfo.dropdownId
        );
        if (!ArrayExtensions.hasElements(currentDimensionsFromQuery)) {
            continue;
        }
        const currentDimensionFromQuery = currentDimensionsFromQuery[0];
        const selectedDimensionItem = dimensionInfo.dropdownList.filter(
            dimensionItem =>
                dimensionItem.value.toLocaleLowerCase() === currentDimensionFromQuery.DimensionValue?.Value?.toLocaleLowerCase()
        );
        if (!ArrayExtensions.hasElements(selectedDimensionItem)) {
            continue;
        }
        selectedDimensionItem[0].isDefault = true;
    }

    const dropdowns: IBuyboxProductConfigureDropdownViewProps[] = dimensionsInfo.map(dimensionInfo => {
        return mapProductDimensionFullToDropdownViewProps({
            ...dimensionInfo,
            resources,
            configureErrors,
            getDropdownName,
            dropdownCallback: onChanged,
            className,
            context
        });
    });

    return {
        ContainerProps: {
            className: `${className}__configure`
        },
        dropdowns
    };
}

const _onChanged = async (
    notification: IDropdownOnSelectionChangeNotification,
    dimensionChanged: (newValue: number, selectedDimensionValue: string) => Promise<void>
): Promise<void> => {
    await dimensionChanged(+notification.dropdownId, notification.selectId);
};

const _getClassNamePrefix = (typeName: string): string => {
    return typeName.toLocaleLowerCase() === 'quickview' ? 'ms-quickView' : 'ms-buybox';
};
