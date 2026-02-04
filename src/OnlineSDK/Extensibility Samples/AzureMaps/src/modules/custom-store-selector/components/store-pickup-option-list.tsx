/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { DeliveryOption } from '@msdyn365-commerce/retail-proxy';
import * as React from 'react';

/**
 * Store Pickup option list interface.
 */
export interface IStorePickUpOptionListProps {
    productPickupListOptionMenuText?: string;
    productPickupModeList?: DeliveryOption[];
    defaultOptionText?: string;
    onChange(deliveryCode: string): Promise<void>;
}

/**
 * Store Pickup option state interface.
 */
export interface IStorePickOptionListState {
    isFilterExpanded: boolean;
    selectedPickupMode?: string;
}

/**
 * Simple Pickup option dropdown with Button to get menu option.
 */
export class StorePickUpOptionList extends React.PureComponent<IStorePickUpOptionListProps, IStorePickOptionListState> {
    private readonly _storePickupListOptionMenu: React.RefObject<HTMLUListElement>;

    public constructor(props: IStorePickUpOptionListProps) {
        super(props);
        this._storePickupListOptionMenu = React.createRef();
        this.state = {
            isFilterExpanded: false,
            selectedPickupMode: ''
        };
    }

    public render(): JSX.Element {
        const { productPickupListOptionMenuText, productPickupModeList, defaultOptionText } = this.props;
        const selectedPickupModeDesc = defaultOptionText
            ? defaultOptionText
            : productPickupModeList?.find((delivery: DeliveryOption) => delivery.Code === this.state.selectedPickupMode)?.Description;
        const role = 'menu';
        return (
            <div className='ms-store-select__search-header-pickup'>
                <button className='ms-store-select__search-header-pickup-menu' onClick={this._rendeDeliveryOptions}>
                    {selectedPickupModeDesc !== undefined ? selectedPickupModeDesc : productPickupListOptionMenuText}
                    <span className='ms-store-select__search-header-pickup-span' />
                </button>
                {this.state.isFilterExpanded ? (
                    <ul className='ms-store-select__search-header-pickup-list' role={role} ref={this._storePickupListOptionMenu}>
                        {productPickupModeList?.map((delivery: DeliveryOption) => {
                            return (
                                <li className='ms-store-select__search-header-pickup-list-item' key={delivery.Code} role='presentation'>
                                    <a
                                        key={delivery.Code}
                                        data-value={delivery.Code}
                                        role='menuitem'
                                        className='ms-store-select__search-header-pickup-list-link'
                                        tabIndex={0}
                                        aria-label={delivery.Description}
                                        onClick={this._onSelect}
                                        onKeyDown={this._onKeyDown}
                                    >
                                        <span className='ms-store-select__search-header-pickup-list-item__text'>
                                            {delivery.Description}
                                        </span>
                                    </a>
                                </li>
                            );
                        })}
                    </ul>
                ) : null}
            </div>
        );
    }

    /**
     * Method to call when drop dwon get selected.
     */
    private readonly _rendeDeliveryOptions = () => {
        const isExpandOptionsState = this.state.isFilterExpanded;
        this.setState({
            isFilterExpanded: !isExpandOptionsState
        });
    };

    /**
     * Method to call on select pickup mode.
     * @param event - Mouse event.
     * @returns Void.
     */
    private readonly _onSelect = async (event: React.MouseEvent<HTMLAnchorElement, MouseEvent>): Promise<void> => {
        event.preventDefault();
        const deliveryCode = event.currentTarget.getAttribute('data-value');
        if (deliveryCode !== null) {
            this.setState({
                selectedPickupMode: deliveryCode
            });

            this._rendeDeliveryOptions();
            await this.props.onChange(deliveryCode);
        }
    };

    /**
     * Method to call on select pickup mode.
     * @param event - Mouse event.
     * @returns Void.
     */
    private readonly _onKeyDown = async (event: React.KeyboardEvent<HTMLAnchorElement>): Promise<void> => {
        event.preventDefault();
        const deliveryCode = event.currentTarget.getAttribute('data-value');
        if (deliveryCode !== null) {
            this.setState({
                selectedPickupMode: deliveryCode
            });

            this._rendeDeliveryOptions();
            await this.props.onChange(deliveryCode);
        }
    };
}
