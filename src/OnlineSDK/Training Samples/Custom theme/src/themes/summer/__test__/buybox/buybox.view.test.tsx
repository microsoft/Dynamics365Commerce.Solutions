/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { buildMockModuleProps } from '@msdyn365-commerce/core';
import { IBuyboxViewProps } from '@msdyn365-commerce-modules/buybox';
import { Button } from '@msdyn365-commerce-modules/utilities';
import { mount, render } from 'enzyme';
import * as React from 'react';

import BuyboxView from '../../views/buybox.view';
import { mockResources } from './mock-resources';

const mockSlots = {
    mediaGallery: [],
    storeSelector: [],
    socialShare: <div />,
    textBlocks: ['x', 'y']
};
describe('Buybox unit tests - View', () => {
    it('renders correctly', () => {
        const moduleProps: IBuyboxViewProps = buildMockModuleProps({}, {}) as IBuyboxViewProps;
        const mockProps = {
            ...moduleProps,
            ModuleProps: {
                moduleProps,
                className: 'ms-buybox'
            },
            keyInPrice: {
                ContainerProps: {
                    className: ''
                },
                LabelContainerProps: {
                    className: ''
                },
                heading: '<h1>d</h1>',
                input: <input />
            },
            resources: mockResources,
            ProductInfoContainerProps: {
                className: 'ms-buybox__content'
            },
            state: {
                quantity: 1,
                min: 1,
                max: 1
            },
            MediaGalleryContainerProps: {
                className: 'ms-buybox__media-gallery'
            },
            callbacks: {
                updateQuantity: jest.fn(),
                updateKeyInPrice: jest.fn(),
                updateErrorState: jest.fn(),
                dimensionSelectedAsync: jest.fn(),
                updateSelectedProduct: jest.fn(),
                getDropdownName: jest.fn(),
                changeModalOpen: jest.fn(),
                changeUpdatingDimension: jest.fn()
            },
            title: '<div>Title</div>',
            description: '<div>description</div>',
            findInStore: {
                ContainerProps: {
                    className: 'ms-buybux__find-in-store'
                },
                heading: '<div>heading</div>',
                description: '<div>description</div>',
                errors: '<div>errors</div>',
                button: '<div>button</div>',
                modal: '<div>modal</div>',
                storeSelector: '<div>storeSelector</div>',
                openFindInStoreDialog: jest.fn()
            },
            mediaGallery: '<div>mediaGallery</div>',
            price: '<div>price</div>',
            inventoryLevelLabel: '<div>inventoryLevelLabel</div>',
            addToCart: {
                ContainerProps: {
                    className: 'ms-buybox__add-to-cart-container'
                },
                errorBlock: '<div>errorBlock</div>',
                button: '<div>button</div>'
            },
            addToOrderTemplate: {
                ContainerProps: {
                    className: 'ms-buybox__add-to-order-template'
                },
                errorBlock: '<div>errorBlock</div>',
                button: '<div>button</div>'
            },
            addToWishlist: {
                ContainerProps: {
                    className: 'ms-buybox__add-to-wishlist-container'
                },
                errorBlock: '<div>errorBlock</div>',
                button: '<div>button</div>'
            },
            rating: '<div>rating</div>',
            quantity: {
                ContainerProps: {
                    className: 'ms-buybux__quantity'
                },
                LabelContainerProps: {
                    className: 'ms-buybux__quantity-label'
                },
                heading: '<div>heading</div>',
                errors: '<div>errors</div>',
                input: '<div>input</div>'
            },
            configure: {
                ContainerProps: {
                    className: 'ms-buybux__configure'
                },

                dropdowns: [
                    {
                        ContainerProps: { className: 'ms-buybox__configure-dropdown' },
                        LabelContainerProps: { className: 'ms-buybox__configure-dropdown-label' },
                        heading: '<div>Heading 1</div>',
                        errors: '<div>errors 1</div>',
                        select: '<div>select 1</div>'
                    },
                    {
                        ContainerProps: { className: 'ms-buybox__configure-dropdown' },
                        LabelContainerProps: { className: 'ms-buybox__configure-dropdown-label' },
                        heading: '<div>heading</div>',
                        errors: '<div>errors</div>',
                        select: '<div>select 2</div>'
                    }
                ]
            },
            shopSimilarLook: {
                ContainerProps: {
                    className: 'ms-buybox__shopsimilarlooks'
                },
                input: (
                    <Button title='Shop Similar looks' className='ms-buybox__shop-similar-looks-button' aria-label='Shop Similar looks' />
                )
            },
            shopSimilarDescription: {
                ContainerProps: {
                    className: 'ms-buybox__shopsimilardescription'
                },
                input: (
                    <Button
                        title='Shop Similar description'
                        className='ms-buybox__shop-similar-looks-description'
                        aria-label='Shop Similar description'
                    />
                )
            },
            slots: mockSlots
        };

        // @ts-expect-error
        const component = mount(<BuyboxView {...mockProps} />);
        component.find('.quantity-input').simulate('blur');
        component.setProps({
            callbacks: {
                updateQuantity: undefined,
                updateKeyInPrice: jest.fn(),
                updateErrorState: jest.fn(),
                dimensionSelectedAsync: jest.fn(),
                updateSelectedProduct: jest.fn(),
                getDropdownName: jest.fn(),
                changeModalOpen: jest.fn(),
                changeUpdatingDimension: jest.fn()
            }
        });
        component.find('.quantity-input').simulate('blur');
        expect(component).toMatchSnapshot();
    });
    it('renders correctly with props', () => {
        const moduleProps: IBuyboxViewProps = buildMockModuleProps({}, {}) as IBuyboxViewProps;
        const mockProps = {
            ...moduleProps,
            ModuleProps: {
                moduleProps,
                className: 'ms-buybox'
            },
            keyInPrice: {
                ContainerProps: {
                    className: ''
                },
                LabelContainerProps: {
                    className: ''
                },
                heading: '<h1>heading<h1/>',
                input: <input />
            },
            resources: mockResources,
            ProductInfoContainerProps: {
                className: 'ms-buybox__content'
            },
            state: {
                quantity: 1,
                min: 1,
                max: 1
            },
            MediaGalleryContainerProps: {
                className: 'ms-buybox__media-gallery'
            },
            callbacks: {
                updateQuantity: jest.fn(),
                updateKeyInPrice: jest.fn(),
                updateErrorState: jest.fn(),
                dimensionSelectedAsync: jest.fn(),
                updateSelectedProduct: jest.fn(),
                getDropdownName: jest.fn(),
                changeModalOpen: jest.fn(),
                changeUpdatingDimension: jest.fn()
            },
            title: '<div>Title</div>',
            description: '<div>description</div>',
            findInStore: {
                ContainerProps: {
                    className: 'ms-buybux__find-in-store'
                },
                heading: '<div>heading</div>',
                description: '<div>description</div>',
                errors: '<div>errors</div>',
                button: '<div>button</div>',
                modal: '<div>modal</div>',
                storeSelector: '<div>storeSelector</div>',
                openFindInStoreDialog: jest.fn()
            },
            mediaGallery: '<div>mediaGallery</div>',
            price: '<div>price</div>',
            inventoryLevelLabel: '<div>inventoryLevelLabel</div>',
            addToCart: {
                ContainerProps: {
                    className: 'ms-buybox__add-to-cart-container'
                },
                errorBlock: '<div>errorBlock</div>',
                button: '<div>button</div>'
            },
            addToOrderTemplate: {
                ContainerProps: {
                    className: 'ms-buybox__add-to-order-template'
                },
                errorBlock: '<div>errorBlock</div>',
                button: '<div>button</div>'
            },
            addToWishlist: {
                ContainerProps: {
                    className: 'ms-buybox__add-to-wishlist-container'
                },
                errorBlock: '<div>errorBlock</div>',
                button: '<div>button</div>'
            },
            rating: '<div>rating</div>',
            quantity: {
                ContainerProps: {
                    className: 'ms-buybux__quantity'
                },
                LabelContainerProps: {
                    className: 'ms-buybux__quantity-label'
                },
                heading: '<div>heading</div>',
                errors: '<div>errors</div>',
                input: '<div>input</div>'
            },
            configure: {
                ContainerProps: {
                    className: 'ms-buybux__configure'
                },

                dropdowns: [
                    {
                        ContainerProps: { className: 'ms-buybox__configure-dropdown' },
                        LabelContainerProps: { className: 'ms-buybox__configure-dropdown-label' },
                        heading: '<div>Heading 1</div>',
                        errors: '<div>errors 1</div>',
                        select: '<div>select 1</div>'
                    },
                    {
                        ContainerProps: { className: 'ms-buybox__configure-dropdown' },
                        LabelContainerProps: { className: 'ms-buybox__configure-dropdown-label' },
                        heading: '<div>heading</div>',
                        errors: '<div>errors</div>',
                        select: '<div>select 2</div>'
                    }
                ]
            },
            shopSimilarLook: {
                ContainerProps: {
                    className: 'ms-buybox__shopsimilarlooks'
                },
                input: (
                    <Button title='Shop Similar looks' className='ms-buybox__shop-similar-looks-button' aria-label='Shop Similar looks' />
                )
            },
            shopSimilarDescription: {
                ContainerProps: {
                    className: 'ms-buybox__shopsimilardescription'
                },
                input: (
                    <Button
                        title='Shop Similar description'
                        className='ms-buybox__shop-similar-looks-description'
                        aria-label='Shop Similar description'
                    />
                )
            },
            slots: {
                mediaGallery: undefined,
                storeSelector: undefined,
                socialShare: undefined,
                textBlocks: undefined
            }
        };

        // @ts-expect-error
        const component = render(<BuyboxView {...mockProps} />);
        expect(component).toMatchSnapshot();
    });
});
