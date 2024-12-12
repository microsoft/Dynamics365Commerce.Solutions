/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import { ProductSearchResult } from '@msdyn365-commerce/retail-proxy';
import * as React from 'react';
import {
    ICategoryHierarchyViewProps,
    IRefineMenuViewProps,
    ISearchResultContainerViewProps,
    ISortByViewProps,
    ITitleViewProps,
    ISearchResultContainerProps,
    ISearchResultModalViewProps
} from '@msdyn365-commerce-modules/search-result-container';
import { ProductComponent } from './components/product.component';

const SearchResultContainerView: React.FC<ISearchResultContainerViewProps> = props => {
    const {
        SearchResultContainer,
        pagination,
        ProductsContainer,
        ProductSectionContainer,
        choiceSummary,
        isMobile,
        modalToggle,
        searchResultModal,
        TitleViewProps,
        refineMenu,
        categoryHierarchy,
        sortByOptions,
        CategoryNavContainer,
        RefineAndProductSectionContainer,
        errorMessage,
        FeatureSearchContainer,
        similarLookProduct
    } = props;
    if (isMobile) {
        return (
            <Module {...SearchResultContainer}>
                {renderCategoryHierarchy(categoryHierarchy)}
                {renderTitle(TitleViewProps)}
                {choiceSummary}
                {modalToggle}
                {createSearchResultModal(searchResultModal, refineMenu, sortByOptions)}
                <Node {...FeatureSearchContainer}>{similarLookProduct}</Node>
                <Node {...ProductsContainer}>
                    {errorMessage}
                    {renderProducts(props)}
                </Node>
                {pagination}
            </Module>
        );
    }
    return (
        <Module {...SearchResultContainer}>
            <Node {...CategoryNavContainer}>
                {categoryHierarchy && renderCategoryHierarchy(categoryHierarchy)}
                {TitleViewProps && renderTitleCount(TitleViewProps)}
            </Node>
            <Node {...RefineAndProductSectionContainer}>
                {refineMenu && renderRefiner(refineMenu)}
                <Node {...ProductSectionContainer}>
                    {TitleViewProps && renderTitle(TitleViewProps)}
                    {choiceSummary}
                    {sortByOptions && renderSort(sortByOptions)}
                    <Node {...FeatureSearchContainer}>{similarLookProduct}</Node>
                    <Node {...ProductsContainer}>
                        {errorMessage}
                        {renderProducts(props)}
                    </Node>
                    {pagination}
                </Node>
            </Node>
        </Module>
    );
};

const createSearchResultModal = (
    modalProps: ISearchResultModalViewProps,
    refineMenu: IRefineMenuViewProps,
    sortByDropDown: ISortByViewProps
): JSX.Element => {
    return React.cloneElement(
        modalProps.modal,
        {},
        modalProps.modalHeader,
        createModalBody(modalProps, refineMenu, sortByDropDown),
        modalProps.modalFooter
    );
};

const createModalBody = (
    props: ISearchResultModalViewProps,
    refineMenu: IRefineMenuViewProps,
    sortByDropDown: ISortByViewProps
): JSX.Element | null => {
    if (sortByDropDown) {
        return React.cloneElement(props.modalBody, {}, renderSort(sortByDropDown), renderRefiner(refineMenu));
    }
    return null;
};

const renderRefiner = (props: IRefineMenuViewProps): JSX.Element | null => {
    const { refiners, RefineMenuContainer, RefinerSectionContainer } = props;
    if (refiners) {
        return (
            <Node {...RefinerSectionContainer}>
                <Node {...RefineMenuContainer}>
                    {refiners.map((submenu, index) => (
                        <React.Fragment key={index}>{submenu}</React.Fragment>
                    ))}
                </Node>
            </Node>
        );
    }
    return null;
};

const renderSort = (props: ISortByViewProps): JSX.Element | null => {
    const { SortingContainer, sortByDropDown } = props;
    if (sortByDropDown) {
        return <Node {...SortingContainer}>{sortByDropDown}</Node>;
    }
    return null;
};

const renderCategoryHierarchy = (props: ICategoryHierarchyViewProps): JSX.Element | null => {
    const { CategoryHierarchyContainer, categoryHierarchyList, categoryHierarchySeparator } = props;
    if (categoryHierarchyList) {
        /* eslint-disable  @typescript-eslint/no-explicit-any */
        return (
            <Node {...CategoryHierarchyContainer}>
                {categoryHierarchyList.map((category: any, index: number) => (
                    <React.Fragment key={index}>
                        {category}
                        {categoryHierarchyList && categoryHierarchyList[index + 1] && categoryHierarchySeparator}
                    </React.Fragment>
                ))}
            </Node>
        );
    }

    return null;
};

const renderTitle = (props: ITitleViewProps): JSX.Element | null => {
    const { title, TitleContainer } = props;
    if (title) {
        return (
            <Node {...TitleContainer}>
                <h2>
                    {title.titlePrefix}
                    {title.titleText}
                </h2>
            </Node>
        );
    }
    return null;
};

function renderProducts(props: ISearchResultContainerViewProps & ISearchResultContainerProps<{}>): JSX.Element | null {
    const { config, context, resources } = props;
    const liClass = 'ms-product-search-result__item';
    const products = (props.data.listPageState.result && props.data.listPageState.result.activeProducts) || [];
    if (!products || products.length === 0) {
        return null;
    }
    return (
        <ul className='list-unstyled'>
            {products.map((product: ProductSearchResult, index: number) => (
                <li
                    className={`${liClass} ${
                        getImageOrientation(product) === 'Landscape' ? 'product-placement__item-limage' : 'product-placement__item-pimage'
                    }`}
                    key={index}
                >
                    <ProductComponent
                        context={context}
                        imageSettings={config.imageSettings}
                        freePriceText={resources.priceFree}
                        originalPriceText={resources.originalPriceText}
                        currentPriceText={resources.currentPriceText}
                        ratingAriaLabel={resources.ratingAriaLabel}
                        id={props.id}
                        typeName={props.typeName}
                        data={{ product }}
                    />
                </li>
            ))}
        </ul>
    );
}
function getImageOrientation(product: ProductSearchResult): string {
    let imageOrientation: string = '';
    if (product.AttributeValues!.length > 0) {
        product.AttributeValues!.map(property => {
            if (property.Name === 'Image Orientation') {
                imageOrientation = property.TextValue!;
            }
        });
    }
    imageOrientation = 'Landscape';
    return imageOrientation;
}
const renderTitleCount = (props: ITitleViewProps): JSX.Element | null => {
    const { title, TitleContainer } = props;
    if (title) {
        return (
            <Node {...TitleContainer}>
                <h5>{title.titleCount}</h5>
            </Node>
        );
    }
    return null;
};

export default SearchResultContainerView;
