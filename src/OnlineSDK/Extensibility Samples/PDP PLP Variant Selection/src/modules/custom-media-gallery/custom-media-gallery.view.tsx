/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as React from 'react';

import { IImageData } from '@msdyn365-commerce/core';
import { SimpleProduct } from '@msdyn365-commerce/retail-proxy';

import { Module, Node } from '@msdyn365-commerce-modules/utilities';
import { IMediaGalleryThumbnailItemViewProps, IMediaGalleryThumbnailsViewProps, IMediaGalleryViewProps } from './custom-media-gallery';

const MediaGalleryView: React.FC<IMediaGalleryViewProps> = props => {
    const { CarouselProps, Thumbnails, MediaGallery } = props;
    _updateImages(props);
    return (
        <Module {...MediaGallery}>
            <Node {...CarouselProps} />
            {_renderThumbnails(Thumbnails)}
        </Module>
    );
};

const _renderThumbnails = (thumbnails: IMediaGalleryThumbnailsViewProps): JSX.Element => {
    const { ThumbnailsContainerProps, SingleSlideCarouselComponentProps, items } = thumbnails;

    return (
        <Node {...ThumbnailsContainerProps}>
            {items && items.length > 1 && <Node {...SingleSlideCarouselComponentProps}>{items && items.map(_renderThumbnailItem)}</Node>}
        </Node>
    );
};

const _renderThumbnailItem = (thumbnail: IMediaGalleryThumbnailItemViewProps): JSX.Element => {
    const { ThumbnailItemContainerProps, Picture } = thumbnail;

    return <Node {...ThumbnailItemContainerProps}>{Picture}</Node>;
};

const _updateImages = (props: IMediaGalleryViewProps): void => {
    let image = '';
    // const masterProductIdKey = 'MasterProductId';
    const productImageKey = 'ColorSwatchProductImageUrl';
    const selectedColorSwatchKey = 'SelectedColorSwatch';
    if (props.data.product.result) {
        image = _getProductExtension(props.data.product.result, productImageKey);
        const selectedColorSwatch = _getProductExtension(props.data.product.result, selectedColorSwatchKey);
        const images = image.split(';');
        const mediaGalleryItems: IImageData[] = [];
        images.forEach(img => {
            if (img.length > 0) {
                mediaGalleryItems.push({ src: img });
            }
        });
        if (mediaGalleryItems && mediaGalleryItems.length > 0) {
            props.state.mediaGalleryItems = mediaGalleryItems;
            if (props.state.selectedColor !== selectedColorSwatch) {
                props.state.activeIndex = 0;
                props.state.selectedColor = selectedColorSwatch;
                props.state.lastUpdate = Date.now();
            }
        }
    }
};

const _getProductExtension = (simpleProduct: SimpleProduct, extensionPropertyKey: string): string => {
    const property =
        simpleProduct.ExtensionProperties && simpleProduct.ExtensionProperties.find(extension => extension.Key === extensionPropertyKey);
    if (property) {
        return property.Value?.StringValue || '';
    } else {
        return '';
    }
};

export default MediaGalleryView;
