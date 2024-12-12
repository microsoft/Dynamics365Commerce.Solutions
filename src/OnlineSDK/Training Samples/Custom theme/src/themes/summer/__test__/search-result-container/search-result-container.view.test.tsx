/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { buildMockModuleProps } from '@msdyn365-commerce/core';
import {
    ICategoryHierarchyViewProps,
    IRefineMenuViewProps,
    ISearchResultContainerViewProps,
    ISortByViewProps
} from '@msdyn365-commerce-modules/search-result-container';
import { INodeProps } from '@msdyn365-commerce-modules/utilities';
import { render } from 'enzyme';
import * as React from 'react';

import SearchResultContainerView from '../../views/search-result-container.view';

interface ISearchResultModalViewProps {
    modal: React.ReactElement;
    modalHeader: React.ReactElement;
    modalFooter: React.ReactElement;
    modalBody: React.ReactElement;
}
interface ISearchResultTitle {
    titlePrefix?: React.ReactNode;
    titleText?: React.ReactNode;
    titleCount?: React.ReactNode;
}

interface ITitleViewProps {
    // eslint-disable-next-line @typescript-eslint/naming-convention -- have to export this as this utility is used in fabrikam tests
    TitleContainer: INodeProps;
    title: ISearchResultTitle;
}
const dummyString = 'foo';
describe('Search result container tests - View', () => {
    const moduleProps: ISearchResultContainerViewProps = buildMockModuleProps({}, {}) as ISearchResultContainerViewProps;
    const searchResultModal: ISearchResultModalViewProps = {
        modal: <div>{dummyString}</div>,
        modalBody: <div>{dummyString} </div>,
        modalFooter: <div> {dummyString} </div>,
        modalHeader: <div> {dummyString} </div>
    };
    let categoryHierarchy: ICategoryHierarchyViewProps = {
        CategoryHierarchyContainer: { className: 'node-class' },
        categoryHierarchyList: [<span key={1}> {dummyString} </span>],
        categoryHierarchySeparator: <div> {dummyString} </div>
    };
    let sortByOptions: ISortByViewProps = {
        SortingContainer: { className: 'node-class' },
        sortByDropDown: <div>{dummyString}</div>
    };
    let refineMenu: IRefineMenuViewProps = {
        RefineMenuContainer: { className: 'node-class' },
        RefinerSectionContainer: { className: 'node-class' },
        refiners: [<span key={1}> {dummyString} </span>]
    };
    const mockTitle: ITitleViewProps = {
        TitleContainer: { className: 'node-class-TitleContent' },
        title: {
            titlePrefix: <span> {dummyString} </span>,
            titleText: <span> {dummyString} </span>,
            titleCount: <span> {dummyString} </span>
        }
    };
    it('renders correctly', () => {
        const mockProps = {
            ...moduleProps,
            className: 'className',
            products: <div>{dummyString}</div>,
            TitleViewProps: mockTitle,
            refineMenu,
            sortByOptions,
            categoryHierarchy,
            searchResultModal,
            SearchResultContainer: { moduleProps, className: 'module-class' }
        };
        const component = render(<SearchResultContainerView {...mockProps} />);
        expect(component).toMatchSnapshot();
    });
    it('renders category hierarchy list null', () => {
        categoryHierarchy = {
            CategoryHierarchyContainer: { className: 'node-class' },
            categoryHierarchyList: undefined,
            categoryHierarchySeparator: <div> {dummyString} </div>
        };
        const mockProps = {
            ...moduleProps,
            className: 'className',
            products: <div>{dummyString}</div>,
            TitleViewProps: mockTitle,
            refineMenu,
            sortByOptions,
            categoryHierarchy,
            searchResultModal,
            SearchResultContainer: { moduleProps, className: 'module-class' }
        };
        const component = render(<SearchResultContainerView {...mockProps} />);
        expect(component).toMatchSnapshot();
    });
    it('renders sorts as null', () => {
        sortByOptions = {
            SortingContainer: { className: 'node-class' },
            sortByDropDown: undefined
        };
        const mockProps = {
            ...moduleProps,
            className: 'className',
            products: <div>{dummyString}</div>,
            TitleViewProps: mockTitle,
            refineMenu,
            sortByOptions,
            categoryHierarchy,
            searchResultModal,
            SearchResultContainer: { moduleProps, className: 'module-class' }
        };
        const component = render(<SearchResultContainerView {...mockProps} />);
        expect(component).toMatchSnapshot();
    });
    it('renders refiner as null', () => {
        refineMenu = {
            RefineMenuContainer: { className: 'node-class' },
            RefinerSectionContainer: { className: 'node-class' },
            refiners: undefined
        };
        const mockProps = {
            ...moduleProps,
            className: 'className',
            products: <div>{dummyString}</div>,
            TitleViewProps: mockTitle,
            refineMenu,
            sortByOptions,
            categoryHierarchy,
            searchResultModal,
            SearchResultContainer: { moduleProps, className: 'module-class' }
        };
        const component = render(<SearchResultContainerView {...mockProps} />);
        expect(component).toMatchSnapshot();
    });
    it('renders title as blank', () => {
        const mockProps = {
            ...moduleProps,
            className: 'className',
            products: <div>{dummyString}</div>,
            TitleViewProps: mockTitle,
            refineMenu,
            sortByOptions,
            categoryHierarchy,
            searchResultModal,
            SearchResultContainer: { moduleProps, className: 'module-class' }
        };
        mockProps.isMobile = true;
        const component = render(<SearchResultContainerView {...mockProps} />);
        expect(component).toMatchSnapshot();
    });
});
