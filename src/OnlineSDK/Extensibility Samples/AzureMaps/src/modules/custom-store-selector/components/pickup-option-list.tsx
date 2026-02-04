/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { DeliveryOption } from '@msdyn365-commerce/retail-proxy';
import * as React from 'react';

/**
 * Interface Pickup option list.
 */
export interface IPickUpOptionListProps {
    pickupListOptionMenuText: string;
    pickupmodeList?: DeliveryOption[];
    onChange(deliveryCode: string): Promise<void>;
}

/**
 * Interface pickup option list state.
 */
export interface IPickOptionListState {
    expandSiteOptions: boolean;
}

/**
 * Simple Pickup option dropdown with Button to get menu option.
 */
export class PickUpOptionList extends React.PureComponent<IPickUpOptionListProps, IPickOptionListState> {
    private readonly _pickupListOptionMenu: React.RefObject<HTMLUListElement>;

    private readonly _pickupListOptionButton: React.RefObject<HTMLButtonElement>;

    private readonly eventsToBeBind = ['click', 'touchstart'];

    public constructor(props: IPickUpOptionListProps) {
        super(props);
        this._renderDeliveryOptions = this._renderDeliveryOptions.bind(this);
        this._pickupListOptionMenu = React.createRef();
        this._pickupListOptionButton = React.createRef();
        this.state = {
            expandSiteOptions: false
        };
    }

    public componentDidMount(): void {
        for (const event of this.eventsToBeBind) {
            window.addEventListener(event, this.handleDocumentClick, true);
        }
    }

    public componentWillUnmount(): void {
        for (const event of this.eventsToBeBind) {
            window.removeEventListener(event, this.handleDocumentClick, true);
        }
    }

    public render(): JSX.Element {
        const { pickupListOptionMenuText, pickupmodeList } = this.props;

        return (
            <div className='ms-store-select__location-line-pickup'>
                <button
                    className='ms-store-select__location-line-pickup-menu'
                    onClick={this._renderDeliveryOptions}
                    ref={this._pickupListOptionButton}
                >
                    {pickupListOptionMenuText}
                    <span className='ms-store-select__location-line-pickup-span' />
                </button>
                {this.state.expandSiteOptions ? (
                    <ul className='ms-store-select__location-line-pickup-list' role='menu' ref={this._pickupListOptionMenu}>
                        {pickupmodeList?.map((delivery: DeliveryOption) => {
                            return (
                                <li role='menu' className='ms-store-select__location-line-pickup-list-item' key={delivery.Code}>
                                    <a
                                        key={delivery.Code}
                                        data-value={delivery.Code}
                                        className='ms-store-select__location-line-pickup-list-link'
                                        tabIndex={0}
                                        aria-label={delivery.Description}
                                        onClick={this._onSelect}
                                        role='menuitem'
                                    >
                                        <span className='ms-store-select__location-line-pickup-list-item__text'>
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
     * Handle the document click.
     * @param event -- Event object.
     */
    private readonly handleDocumentClick = (event: Event): void => {
        const htmlElement = event.target as HTMLElement;
        if (
            !(
                (this._pickupListOptionButton.current !== null &&
                    (event.target === this._pickupListOptionButton.current ||
                        this._pickupListOptionButton.current.contains(htmlElement))) ||
                (this._pickupListOptionMenu.current !== null &&
                    (event.target === this._pickupListOptionMenu.current || this._pickupListOptionMenu.current.contains(htmlElement)))
            )
        ) {
            this.setState({
                expandSiteOptions: false
            });
        }
    };

    private _renderDeliveryOptions(event: React.MouseEvent<HTMLButtonElement, MouseEvent>): void {
        event.stopPropagation();
        const isExpandSiteOptionsState = this.state.expandSiteOptions;
        this.setState({
            expandSiteOptions: !isExpandSiteOptionsState
        });
    }

    /**
     * Select Delivery option.
     * @param event -- Event Object.
     */
    private readonly _onSelect = async (event: React.MouseEvent<HTMLAnchorElement, MouseEvent>): Promise<void> => {
        event.preventDefault();
        const deliveryCode = event.currentTarget.getAttribute('data-value');
        if (deliveryCode && deliveryCode.length > 0) {
            await this.props.onChange(deliveryCode);
        }
    };
}
