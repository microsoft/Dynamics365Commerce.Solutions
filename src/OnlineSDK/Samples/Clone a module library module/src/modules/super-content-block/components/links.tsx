/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import * as Msdyn365 from '@msdyn365-commerce/core';
import { getPayloadObject, getTelemetryAttributes, ITelemetryContent, onTelemetryClick } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

import { ILinksData } from '../super-content-block.props.autogenerated';

export interface IContentCardLinks {
    links: ILinksData[];
    requestContext: Msdyn365.IRequestContext;
    telemetryContent: ITelemetryContent;
    role?: string;
    onTextChange?(index: number): (event: Msdyn365.ContentEditableEvent) => void;
}

/**
 *
 * ContentCardLinks component.
 * @extends {React.PureComponent<IContentCardLinks>}
 */
export class ContentCardLinks extends React.PureComponent<IContentCardLinks> {
    public render(): JSX.Element {
        const editableLinks = this._mapEditableLinks(this.props.links);
        return (
            <div className='ms-content-block__cta'>
                {editableLinks && editableLinks.length > 0 ? (
                    <Msdyn365.Links
                        links={editableLinks}
                        editProps={{ onTextChange: this.props.onTextChange, requestContext: this.props.requestContext }}
                    />
                ) : null}
            </div>
        );
    }

    private readonly _mapEditableLinks = (linkdata: ILinksData[]): Msdyn365.ILinksData[] | null => {
        if (!linkdata || linkdata.length === 0) {
            return null;
        }
        const editableLinks: Msdyn365.ILinksData[] = [];
        linkdata.forEach((link, index) => {
            // Construct telemetry attribute to render
            const payLoad = getPayloadObject('click', this.props.telemetryContent, '', '');
            const linkText = link.linkText ? link.linkText : '';
            payLoad.contentAction.etext = linkText;
            const attributes = getTelemetryAttributes(this.props.telemetryContent, payLoad);
            const btnClass = index === 0 ? 'msc-cta__primary' : 'msc-cta__secondary';
            const editableLink: Msdyn365.ILinksData = {
                ariaLabel: link.ariaLabel,
                className: btnClass,
                linkText: link.linkText,
                linkUrl: link.linkUrl.destinationUrl,
                openInNewTab: link.openInNewTab,
                role: this.props.role,
                additionalProperties: attributes,
                onClick: onTelemetryClick(this.props.telemetryContent, payLoad, linkText)
            };
            editableLinks.push(editableLink);
        });

        return editableLinks;
    };
}
export default ContentCardLinks;