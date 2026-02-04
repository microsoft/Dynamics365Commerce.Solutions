/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { IProductRefinerHierarchy } from '@msdyn365-commerce/commerce-entities';
import { ProductRefinerValue } from '@msdyn365-commerce/retail-proxy';
import { getPayloadObject, getTelemetryAttributes, IPayLoad } from '@msdyn365-commerce-modules/utilities';
import classnames from 'classnames';
import { computed } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';

import { IChoiceSummaryProps } from './choice-summary.props';
import { isMatchingRefinementCriterion, isRangeType } from './utilities';

interface IRefinerMap {
    key: string;
    value: ProductRefinerValue;
}

/**
 * ChoiceSummary component.
 */
@observer
export default class ChoiceSummary extends React.PureComponent<IChoiceSummaryProps> {
    private readonly closeButtonGlyph: string = 'msi-close-btn';

    private readonly payLoad: IPayLoad;

    @computed get selectedRefinersMap(): IRefinerMap[] {
        const { selectedChoices } = this.props;
        return selectedChoices.map((selectedRefiner: ProductRefinerValue) => {
            return {
                key: this._getKeyForRefinerValue(selectedRefiner),
                value: selectedRefiner
            } as IRefinerMap;
        });
    }

    constructor(props: Readonly<IChoiceSummaryProps>) {
        super(props);
        this.payLoad = getPayloadObject('click', this.props.telemetryContent!, '');
    }

    public render(): JSX.Element {
        const { clearAllText, label, classNames, choiceAriaLabel, closeAriaLabel } = this.props;
        const items = this.selectedRefinersMap;
        this.payLoad.contentAction.etext = clearAllText;
        const clearAllAttributes = getTelemetryAttributes(this.props.telemetryContent!, this.payLoad);
        return (
            <div className='msc-choice-summary'>
                {items.length > 0 && label && <span className='msc-choice-summary__label'>{label}</span>}
                <ul className={classnames(classNames, 'msc-choice-summary__list', 'list-unstyled')}>
                    {items.map((item: IRefinerMap) => {
                        this.payLoad.contentAction.etext = item.key;
                        const attribute = getTelemetryAttributes(this.props.telemetryContent!, this.payLoad);

                        return (
                            <li className='msc-choice-summary__list-item' key={item.key}>
                                <a
                                    className='msc-choice-summary__item'
                                    href={this.props.urlBuilder(item.value, false)}
                                    aria-label={`${item.key} ${choiceAriaLabel}`}
                                    onClick={this._onClick}
                                    role='button'
                                    {...attribute}
                                >
                                    {item.key}
                                    <span
                                        className={`${this.closeButtonGlyph} msc-choice-summary__glyph`}
                                        role='button'
                                        aria-label={closeAriaLabel}
                                    />
                                </a>
                            </li>
                        );
                    })}
                </ul>
                {items.length > 0 && clearAllText && (
                    <a
                        href={this.props.urlBuilder({}, true)}
                        className='msc-choice-summary__clear-all'
                        {...clearAllAttributes}
                        onClick={this._onClick}
                    >
                        {clearAllText}
                    </a>
                )}
            </div>
        );
    }

    private _getKeyForRefinerValue(productRefinerValue: ProductRefinerValue): string {
        const { choiceFormat, choiceRangeValueFormat, refinerHierarchy, telemetry } = this.props;
        const overallFormat = choiceFormat || '{1}';
        const rangeFormat = choiceRangeValueFormat;
        let refinerName = '';
        if (refinerHierarchy && refinerHierarchy.find) {
            const parent = refinerHierarchy.find(
                (hierarchy: IProductRefinerHierarchy) =>
                    !!hierarchy.Values.find((value: ProductRefinerValue) => isMatchingRefinementCriterion(value, productRefinerValue))
            );

            if (!parent) {
                telemetry.warning('[choice-summary] could not find parent of selected refiner value');
            } else {
                refinerName = parent.KeyName || '';
            }
        }

        let refinerValueName: string;
        if (isRangeType(productRefinerValue.DataTypeValue)) {
            refinerValueName = rangeFormat
                .replace('{0}', this._formatPrice(productRefinerValue.LeftValueBoundString, productRefinerValue.UnitText))
                .replace('{1}', this._formatPrice(productRefinerValue.RightValueBoundString, productRefinerValue.UnitText));
        } else {
            refinerValueName = productRefinerValue.LeftValueBoundLocalizedString || productRefinerValue.LeftValueBoundString || '';
        }

        return overallFormat.replace('{0}', refinerName).replace('{1}', refinerValueName);
    }

    private _formatPrice(amount: string | undefined, currency: string | undefined): string {
        if (!amount || !currency) {
            this.props.telemetry.trace('[choice-summary] could not format price');
            return amount || '';
        }
        let result = amount;

        try {
            result = this.props.context!.cultureFormatter.formatCurrency(Number(amount), currency);
        } catch (error) {
            this.props.telemetry.warning(`Failed to format price for ${result}: ${error}`);
        }

        return result;
    }

    private readonly _onClick = (e: React.MouseEvent<HTMLAnchorElement>): void => {
        e.preventDefault();
        e.stopPropagation();

        const target = e.currentTarget as HTMLElement;
        const clearAll = target.getAttribute('class')!.includes('choice-summary__clear-all');
        const selectedRefiner = clearAll ? undefined : this._getSelectedRefinerChoice(target);

        if (this.props.onChoiceClicked) {
            this.props.onChoiceClicked({
                clearAll,
                itemClicked: target,
                choiceClicked: selectedRefiner,
                nextItemToFocus: target.nextSibling as HTMLElement
            });
        }
    };

    private _getSelectedRefinerChoice(itemClicked: HTMLElement): ProductRefinerValue | undefined {
        const result = this.selectedRefinersMap.find(
            selected => (itemClicked.textContent && itemClicked.textContent.trim()) === selected.key
        );
        return (result && result.value) || undefined;
    }
}
