/**
 * Copyright (c) Microsoft Corporation
 * All rights reserved. See License.txt in the project root for license information.
 * IMediaGallery contentModule Interface Properties
 * THIS FILE IS AUTO-GENERATED - MANUAL MODIFICATIONS WILL BE LOST
 */

import * as Msdyn365 from '@msdyn365-commerce/core';

export const enum imageSource {
    pageContext = 'pageContext',
    productId = 'productId'
}

export const enum imageZoom {
    inline = 'inline',
    container = 'container'
}

export const enum thumbnailsOrientation {
    vertical = 'vertical',
    horizontal = 'horizontal'
}

export interface IMediaGalleryConfig extends Msdyn365.IModuleConfig {
    imageSource?: imageSource;
    images?: Msdyn365.IImageData[];
    productId?: string;
    imageZoom?: imageZoom;
    allowFullScreen?: boolean;
    dataScale?: string;
    zoomedImageSettings?: Msdyn365.IImageSettings;
    thumbnailsOrientation?: thumbnailsOrientation;
    thumbnailImageSettings?: Msdyn365.IImageSettings;
    galleryImageSettings?: Msdyn365.IImageSettings;
    shouldHideMasterProductImagesForVariant?: boolean;
    showPaginationTooltip?: boolean;
    shouldUpdateOnPartialDimensionSelection?: boolean;
    className?: string;
    skipImageValidation?: boolean;
    clientRender?: boolean;
    video?: Msdyn365.IVideoData;
}

export interface IMediaGalleryResources {
    nextScreenshotFlipperText: string;
    previousScreenshotFlipperText: string;
    fullScreenTitleText: string;
    ariaLabelForSlide: string;
}

export interface IMediaGalleryProps<T> extends Msdyn365.IModule<T> {
    resources: IMediaGalleryResources;
    config: IMediaGalleryConfig;
}
