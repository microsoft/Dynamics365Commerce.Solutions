/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import {
    ICategoryHierarchyViewProps,
    IRefineMenuViewProps,
    ISearchResultContainerViewProps,
    ISearchResultModalViewProps,
    ISortByViewProps,
    ITitleViewProps
} from '@msdyn365-commerce-modules/search-result-container';
import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

const SearchResultContainerView: React.FC<ISearchResultContainerViewProps> = props => {
    const {
        SearchResultContainer,
        products,
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
    const isRecoSearchPage = props.context.actionContext.requestContext.query?.recommendation;
    if (isMobile) {
        return (
            <Module {...SearchResultContainer}>
                {categoryHierarchy && renderCategoryHierarchy(categoryHierarchy)}
                {renderTitle(TitleViewProps)}
                {choiceSummary}
                {modalToggle}
                {createSearchResultModal(searchResultModal, refineMenu, sortByOptions, isRecoSearchPage)}
                <Node {...FeatureSearchContainer}>{similarLookProduct}</Node>
                <Node {...ProductsContainer}>
                    {errorMessage}
                    {products}
                </Node>
                {pagination}
            </Module>
        );
    }
    return (
        <Module {...SearchResultContainer}>
            {categoryHierarchy && <Node {...CategoryNavContainer}>{renderCategoryHierarchy(categoryHierarchy)}</Node>}
            <Node {...RefineAndProductSectionContainer}>
                {refineMenu && renderRefiner(refineMenu)}
                <Node {...ProductSectionContainer}>
                    <Node {...FeatureSearchContainer}>{similarLookProduct}</Node>
                    <div className='ms-search-result-wrapper-title-choice-summary'>
                        {TitleViewProps && renderTitle(TitleViewProps)}
                        {choiceSummary}
                    </div>
                    <div className='ms-search-result-wrapper-sort-by-category'>
                        {sortByOptions && !isRecoSearchPage && renderSort(sortByOptions)}
                    </div>
                    <Node {...ProductsContainer}>
                        {errorMessage}
                        {products}
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
    sortByDropDown: ISortByViewProps,
    isRecoSearchPage?: string
): JSX.Element => {
    return React.cloneElement(
        modalProps.modal,
        {},
        modalProps.modalHeader,
        createModalBody(modalProps, refineMenu, sortByDropDown, isRecoSearchPage),
        modalProps.modalFooter
    );
};

const createModalBody = (
    props: ISearchResultModalViewProps,
    refineMenu: IRefineMenuViewProps,
    sortByDropDown: ISortByViewProps,
    isRecoSearchPage?: string
): JSX.Element | null => {
    if (sortByDropDown) {
        return React.cloneElement(props.modalBody, {}, renderSort(sortByDropDown, isRecoSearchPage), renderRefiner(refineMenu));
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

const renderSort = (props: ISortByViewProps, isRecoSearchPage?: string): JSX.Element | null => {
    const { SortingContainer, sortByDropDown } = props;
    if (sortByDropDown && !isRecoSearchPage) {
        return <Node {...SortingContainer}>{sortByDropDown}</Node>;
    }
    return null;
};

const renderCategoryHierarchy = (props: ICategoryHierarchyViewProps): JSX.Element | null => {
    const { CategoryHierarchyContainer, categoryHierarchyList, categoryHierarchySeparator } = props;
    if (categoryHierarchyList) {
        return (
            <Node {...CategoryHierarchyContainer}>
                {categoryHierarchyList.map((category, index) => (
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
                    {title.titleText}({title.titleCount})
                </h2>
            </Node>
        );
    }
    return null;
};

export default SearchResultContainerView;
