/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import {
    IMediaGalleryThumbnailItemViewProps,
    IMediaGalleryThumbnailsViewProps,
    IMediaGalleryViewProps
} from '@msdyn365-commerce-modules/media-gallery';
import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import React from 'react';

/**
 * Render the thumbnail item images.
 * @param thumbnail - The carousel thumbnail line props.
 * @returns Return HTML having thumnailcontainer props with image.
 */
const renderThumbnailItem = (thumbnail: IMediaGalleryThumbnailItemViewProps): JSX.Element => {
    // eslint-disable-next-line @typescript-eslint/naming-convention --  Dependency from media-gallery.tsx file
    const { ThumbnailItemContainerProps, Picture } = thumbnail;

    return <Node {...ThumbnailItemContainerProps}>{Picture}</Node>;
};

/**
 * Render the Media gallery thumbnails to represent images in grid view.
 * @param thumbnails - The thumbnail view props.
 * @param props - The media gallery view props.
 * @returns - The single slide carousel component to render as media gallery image.
 */
const renderThumbnails = (thumbnails: IMediaGalleryThumbnailsViewProps, props: IMediaGalleryViewProps): JSX.Element => {
    // eslint-disable-next-line @typescript-eslint/naming-convention --  Dependency from media-gallery.tsx file
    const { ThumbnailsContainerProps, SingleSlideCarouselComponentProps, items } = thumbnails;
    return (
        <Node {...ThumbnailsContainerProps}>
            <Node {...SingleSlideCarouselComponentProps}>{items?.map(renderThumbnailItem)}</Node>
        </Node>
    );
};

/**
 * Render the Media gallery items using viewprops.
 * @param props - The media gallery view props.
 * @returns The media gallery module wrapping up images node.
 */
const mediaGalleryView: React.FC<IMediaGalleryViewProps> = props => {
    // eslint-disable-next-line @typescript-eslint/naming-convention --  Dependency from media-gallery.tsx file
    const { CarouselProps, Thumbnails, MediaGallery, Modal } = props;
    return (
        <Module {...MediaGallery}>
            <Node {...CarouselProps} />
            {Modal}
            {renderThumbnails(Thumbnails, props)}
        </Module>
    );
};

export default mediaGalleryView;
