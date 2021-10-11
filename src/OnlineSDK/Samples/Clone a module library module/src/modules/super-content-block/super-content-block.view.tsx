/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { IContentBlockAdditionalContentItemViewProps, IContentBlockAdditionalContentViewProps } from './components/additional-content';
import { IContentBlockViewProps } from './super-content-block';

/**
 * Render Additional Content.
 * @param additionalContent - Additional content view props.
 * @returns JSX Element.
 */
const renderAdditionalContent = (additionalContent: IContentBlockAdditionalContentViewProps) => {
    return (
        <Node {...additionalContent.additionalContentNode}>
            {additionalContent.additionalContentItems?.map((item: IContentBlockAdditionalContentItemViewProps) => {
                return (
                    <>
                        {item.heading}
                        <Node {...item.additionalContentItemContainer}>
                            {item.text}
                            <Node {...item.additionalContentItemLinks}>{item.links}</Node>
                        </Node>
                    </>
                );
            })}
        </Node>
    );
};

const ContentBlockView: React.FC<IContentBlockViewProps> = props => {
    const {
        contentBlockContainer,
        imageContainer,
        detailsContainer,
        title,
        text,
        links,
        image,
        contentBlockAnchorTag,
        imageLink,
        imageAriaLabel,
        additionalContent
    } = props;

    if (imageLink) {
        return (
            <Module {...contentBlockContainer}>
                <Node
                    {...contentBlockAnchorTag}
                    href={imageLink}
                    className={contentBlockAnchorTag ? contentBlockAnchorTag.className : ''}
                    aria-label={imageAriaLabel}
                >
                    <Node {...imageContainer}>{image}</Node>
                </Node>
                <Node {...detailsContainer}>
                    {title}
                    {text}
                    {links}
                    {additionalContent && renderAdditionalContent(additionalContent)}
                </Node>
            </Module>
        );
    }
    return (
        <Module {...contentBlockContainer}>
            <Node {...imageContainer}>{image}</Node>
            <Node {...detailsContainer}>
                {title}
                {text}
                {links}
                {additionalContent && renderAdditionalContent(additionalContent)}
            </Node>
        </Module>
    );
};

export default ContentBlockView;
