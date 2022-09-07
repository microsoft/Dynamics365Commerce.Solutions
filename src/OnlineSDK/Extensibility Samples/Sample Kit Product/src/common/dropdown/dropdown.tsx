/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { ISwatchItem, SwatchComponent } from '@msdyn365-commerce/components';
import { DimensionTypes } from '@msdyn365-commerce-modules/retail-actions';
import { getPayloadObject, getTelemetryAttributes, IPayLoad } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { IDropdownItemProps, IDropdownProps, SelectionType } from './dropdown.props';

/**
 * State for a dropdown component.
 */
interface IDropdownState {
    /**
     * @friendlyName selectedIndex
     * @description Selected index.
     */
    selectedIndex: string;
}

/**
 *
 * DropDown component.
 * @extends {React.PureComponent<IDropdownProps, IDropdownState>}
 */
export class Dropdown extends React.PureComponent<IDropdownProps, IDropdownState> {
    private readonly selectMenu: React.RefObject<HTMLSelectElement> = React.createRef<HTMLSelectElement>();

    private readonly payLoad: IPayLoad;

    public constructor(props: IDropdownProps, state: IDropdownState) {
        super(props);

        const selectedItem = this.props.dropdownList.find(item => item.isDefault);
        if (selectedItem) {
            this.state = {
                selectedIndex: selectedItem.id.toString()
            };
            if (this.props.onChange) {
                // eslint-disable-next-line @typescript-eslint/no-floating-promises -- Update for default.
                this.props.onChange({
                    dropdownId: this.props.dropdownId,
                    selectId: selectedItem.id,
                    selectedValue: selectedItem.value
                });
            }
        } else {
            this.state = {
                selectedIndex: ''
            };
        }
        this.payLoad = getPayloadObject('click', props.telemetryContent!, this.props.dropdownName);
    }

    public render(): JSX.Element {
        if (this.props.apiSettings && this.props.selectionType === SelectionType.swatch) {
            return this._renderSwatch();
        }

        return this._renderDropdown();
    }

    private _renderDropdown(): JSX.Element {
        const { dropdownId, dropdownList, dropdownName, dropdownToggleName } = this.props;
        const attribute = getTelemetryAttributes(this.props.telemetryContent!, this.payLoad);
        return (
            <select
                id={`msc-dropdown__entry-${dropdownId}`}
                ref={this.selectMenu}
                aria-label={dropdownName}
                className='msc-dropdown__select'
                onChange={this._onChanged}
                {...attribute}
            >
                <option value='' aria-selected={this.state.selectedIndex === ''} selected={this.state.selectedIndex === ''} hidden disabled>
                    {dropdownToggleName}
                </option>
                {dropdownList.map(
                    (item: IDropdownItemProps) =>
                        item && (
                            <option
                                value={item.id}
                                key={item.id}
                                selected={this.state.selectedIndex === item.id.toString()}
                                aria-selected={this.state.selectedIndex === item.id.toString()}
                                className={item.isDisabled ? 'msc-dropdown__select__disabled' : undefined}
                            >
                                {item.value}
                            </option>
                        )
                )}
            </select>
        );
    }

    private _renderSwatch(): JSX.Element {
        const swatchItems = this.props.dropdownList.map<ISwatchItem>(item => {
            return {
                ...item,
                itemId: item.id,
                dimensionType: this.props.dropdownName as DimensionTypes
            };
        });
        return <SwatchComponent list={swatchItems} apiSettings={this.props.apiSettings!} onSelectItem={this._selectSwatch} />;
    }

    /**
     * Updates state on swatch selection.
     * @param item - Selected swatch item.
     */
    private readonly _selectSwatch = async (item: ISwatchItem) => {
        this.setState({ selectedIndex: item.itemId });

        if (this.props.onChange) {
            await this.props.onChange({
                dropdownId: this.props.dropdownId,
                selectId: item.itemId,
                selectedValue: item.value
            });
        }
    };

    private readonly _onChanged = async (event: React.ChangeEvent<HTMLSelectElement>) => {
        this.setState({ selectedIndex: event.target.value });

        if (this.props.onChange) {
            await this.props.onChange({
                dropdownId: this.props.dropdownId,
                selectId: event.target.value,
                selectedValue: event.target.innerText
            });
        }
    };
}
