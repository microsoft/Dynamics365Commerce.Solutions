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

export const enum altTextSource {
    product = 'product',
    cms = 'cms'
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
    altTextSource?: altTextSource;
    showPaginationTooltip?: boolean;
    shouldUpdateOnPartialDimensionSelection?: boolean;
    className?: string;
    skipImageValidation?: boolean;
    autoplay?: boolean;
    mute?: boolean;
    playPauseTrigger?: boolean;
    controls?: boolean;
    clientRender?: boolean;
}

export interface IMediaGalleryResources {
    nextScreenshotFlipperText: string;
    previousScreenshotFlipperText: string;
    fullScreenTitleText: string;
    ariaLabelForSlide: string;
    playLabel: string;
    pauseLabel: string;
    pausedLabel: string;
    playingLabel: string;
    unMuteLabel: string;
    muteLabel: string;
    fullScreenLabel: string;
    exitFullScreenLabel: string;
    seekBarLabel: string;
    videoTimeDurationLabel: string;
    closedCaptionLabel: string;
    optionButtonLabel: string;
    sliderThumbLabel: string;
    volumeThumbLabel: string;
    playVideoTitleText: string;
}

export interface IMediaGalleryProps<T> extends Msdyn365.IModule<T> {
    resources: IMediaGalleryResources;
    config: IMediaGalleryConfig;
}
