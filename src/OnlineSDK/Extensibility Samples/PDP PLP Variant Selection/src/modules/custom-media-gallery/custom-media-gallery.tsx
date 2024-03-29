/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { Carousel, ICarouselProps } from '@msdyn365-commerce/components';
import { IImageData, IImageSettings, Image } from '@msdyn365-commerce/core';
import { SimpleProduct } from '@msdyn365-commerce/retail-proxy';
import { ArrayExtensions, ObjectExtensions } from '@msdyn365-commerce-modules/retail-actions';
import {
    defaultDataScale,
    IComponentNodeProps,
    ImagefullView,
    IModalViewProps,
    IModuleProps,
    inlineZoomImageOnHover,
    inlineZoomImageOnMouseMove,
    inlineZoomImageOnMouseOut,
    inlineZoomInitClick,
    INodeProps,
    ISingleSlideCarouselProps,
    isMobile,
    KeyCodes,
    NodeTag,
    onContainerZoomInit,
    onMouseMoveLensContainer,
    onMouseOutLensContainer,
    onMouseOverImageContainer,
    removeContainerZoomStyle,
    removeInlineZoomStyle,
    SingleSlideCarousel,
    VariantType
} from '@msdyn365-commerce-modules/utilities';
import classnames from 'classnames';
import { reaction } from 'mobx';
import * as React from 'react';

import { ICustomMediaGalleryData } from './custom-media-gallery.data';
import { imageSource, imageZoom, ICustomMediaGalleryProps, thumbnailsOrientation } from './custom-media-gallery.props.autogenerated';
import { getValidProductImages, validateProductImages } from './utils';

export interface IMediaGalleryState {
    animating: boolean;
    activeIndex: number;
    isImageZoomed: boolean;
    modalIsOpen: boolean;
    lastUpdate?: number;
    isMobileImageZoomed?: boolean;
    mediaGalleryItems?: IImageData[];
    selectedColor: string;
}

export interface IMediaGalleryThumbnailsViewProps {
    ThumbnailsContainerProps: INodeProps;
    SingleSlideCarouselComponentProps: INodeProps;
    items?: IMediaGalleryThumbnailItemViewProps[];
}

export interface IMediaGalleryThumbnailItemViewProps {
    ThumbnailItemContainerProps: INodeProps;
    Picture: React.ReactElement;
}

export interface IMediaGalleryViewProps extends ICustomMediaGalleryProps<ICustomMediaGalleryData> {
    state: IMediaGalleryState;
    MediaGallery: IModuleProps;
    CarouselProps: INodeProps;
    Thumbnails: IMediaGalleryThumbnailsViewProps;
    Modal?: React.ReactElement | null;
    callbackToggle?(): void;
    callbackThumbnailClick?(index: number): void;
    callbackThumbnailKeyDown?(index: number): void;
}

/**
 * Props for carousel.
 */
interface IMediaGalleryCarouselItems<ItemType> {
    items: ItemType[];
    keys: (string | undefined)[];
}

/**
 * Media gallery component.
 */
class MediaGallery extends React.Component<ICustomMediaGalleryProps<ICustomMediaGalleryData>, IMediaGalleryState> {
    private readonly _inlineZoomDivRef: Map<number, HTMLDivElement> = new Map();

    private readonly defaultGalleryImageSettings: IImageSettings = {
        viewports: {
            xs: { q: 'w=767&h=767&m=8', w: 0, h: 0 },
            sm: { q: 'w=600&h=600&m=8', w: 0, h: 0 },
            md: { q: 'w=600&h=772&m=8', w: 0, h: 0 },
            lg: { q: 'h=772&m=8', w: 0, h: 0 }
        },
        lazyload: true,
        cropFocalRegion: true
    };

    private readonly defaultThumbnailImageSettings: IImageSettings = {
        viewports: {
            xs: { q: 'w=100&m=8', w: 100, h: 0 },
            lg: { q: 'w=100&m=8', w: 100, h: 0 }
        },
        lazyload: true,
        cropFocalRegion: true
    };

    private readonly _zoomedImageSettings: IImageSettings;

    public constructor(props: ICustomMediaGalleryProps<ICustomMediaGalleryData>) {
        super(props);
        this._toggleModal = this._toggleModal.bind(this);
        this.state = { activeIndex: 0, selectedColor: '', animating: false, isImageZoomed: false, modalIsOpen: false };

        this._zoomedImageSettings =
            props.config.zoomedImageSettings ?? props.config.galleryImageSettings ?? this.defaultGalleryImageSettings;

        this._onImageMouseOut = this._onImageMouseOut.bind(this);
        this._inlineZoomImageOnHover = this._inlineZoomImageOnHover.bind(this);
        this._onInlineImageClick = this._onInlineImageClick.bind(this);
        this._onContainerZoomImageClick = this._onContainerZoomImageClick.bind(this);
        this._onMouseOutLensContainer = this._onMouseOutLensContainer.bind(this);
    }

    public isMobile(): boolean {
        const size = isMobile({ variant: VariantType.Viewport, context: this.props.context.request });
        return size === 'xs';
    }

    public async componentDidMount(): Promise<void> {
        const source = this.props.config.imageSource || imageSource.pageContext;
        const shouldUpdateOnPartialDimensionSelection = this.props.config.shouldUpdateOnPartialDimensionSelection;

        if (source === imageSource.pageContext) {
            if (this.state.mediaGalleryItems === undefined && this.props.data.mediaLocationsForSelectedVariant.result) {
                const images = await validateProductImages(
                    this.props.data.mediaLocationsForSelectedVariant.result,
                    this.props.context.actionContext,
                    this.props.config.thumbnailImageSettings ?? this.defaultThumbnailImageSettings
                );
                this._setImages(images);
            } else if (this.state.mediaGalleryItems === undefined && this.props.data.product.result) {
                const product = this.props.data.product.result;
                const images = await getValidProductImages(
                    product.RecordId,
                    +this.props.context.request.apiSettings.channelId,
                    this.props.context.actionContext,
                    this.props.config.thumbnailImageSettings || this.defaultThumbnailImageSettings,
                    product.productVariant
                );

                this._setImages(images);
            } else {
                this._setImages([]);
            }

            reaction(
                () => {
                    const product = this.props.data.product.result;
                    if (!product) {
                        return null;
                    }

                    if (!shouldUpdateOnPartialDimensionSelection || ObjectExtensions.isNullOrUndefined(product.productVariant?.RecordId)) {
                        return `${product.RecordId}`;
                    }

                    return `${product.RecordId}-${product.productVariant!.RecordId}`;
                },
                async () => {
                    const product = this.props.data.product.result;
                    if (product) {
                        if (!this.props.config.getMediaLocationsDisableFlag) {
                            const images = await getValidProductImages(
                                product.RecordId,
                                +this.props.context.request.apiSettings.channelId,
                                this.props.context.actionContext,
                                this.props.config.thumbnailImageSettings ?? this.defaultThumbnailImageSettings,
                                shouldUpdateOnPartialDimensionSelection ? product.productVariant : undefined
                            );

                            this._setImages(images);
                        }
                    } else {
                        this._setImages([]);
                    }
                }
            );
        }

        if (source === imageSource.productId && this.props.data.mediaLocations.result) {
            const images = await validateProductImages(
                this.props.data.mediaLocations.result,
                this.props.context.actionContext,
                this.props.config.thumbnailImageSettings || this.defaultThumbnailImageSettings
            );
            this._setImages(images);
        }
    }

    public shouldComponentUpdate(nextProps: ICustomMediaGalleryProps<ICustomMediaGalleryData>, nextState: IMediaGalleryState): boolean {
        if (this.state === nextState && this.props.data === nextProps.data) {
            return false;
        }
        return true;
    }

    public render(): JSX.Element {
        const { id, config, resources } = this.props;

        const { className, showPaginationTooltip } = config;

        const isVertical: boolean = config.thumbnailsOrientation === thumbnailsOrientation.vertical;
        const isFullscreenAllowed: boolean = this.isMobile() || config.allowFullScreen || false;
        const zoomViewMode: string =
            config.imageZoom === imageZoom.inline ? imageZoom.inline : config.imageZoom === imageZoom.container ? imageZoom.container : '';

        const mediaGalleryCarouselItems = this._getMediaGalleryItems(isFullscreenAllowed, zoomViewMode);
        const mediaGalleryThumbnailCarouselItems = this._getMediaGalleryThumbnailItems();
        const viewProps: IMediaGalleryViewProps = {
            ...(this.props as ICustomMediaGalleryProps<ICustomMediaGalleryData>),
            state: this.state,
            MediaGallery: {
                moduleProps: this.props,
                className: classnames(`ms-media-gallery ${isVertical ? 'vertical' : ''}`, className)
            },
            Modal: isFullscreenAllowed ? this.imageModalSlider(zoomViewMode) : null,
            callbackToggle: this.openModalDialog,
            callbackThumbnailClick: this._generateOnThumbnailClick,
            callbackThumbnailKeyDown: this._generateOnThumbnailKeyDown,
            CarouselProps: {
                tag: Carousel,
                className: 'ms-media-gallery__carousel',
                items: mediaGalleryCarouselItems.items,
                activeIndex: this.state.activeIndex,
                next: this.next,
                selectedColor: this.state.selectedColor,
                previous: this.previous,
                interval: false,
                directionTextPrev: resources.previousScreenshotFlipperText,
                directionTextNext: resources.nextScreenshotFlipperText,
                onIndicatorsClickHandler: this.goToIndex,
                showPaginationTooltip: showPaginationTooltip === true,
                indicatorAriaText: resources.ariaLabelForSlide,
                handleOnExited: this.onExited,
                handleOnExiting: this.onExiting,
                key: mediaGalleryCarouselItems.keys
            } as IComponentNodeProps<ICarouselProps>,
            Thumbnails: {
                ThumbnailsContainerProps: { className: 'ms-media-gallery__thumbnails-container' },
                SingleSlideCarouselComponentProps: {
                    tag: SingleSlideCarousel,
                    className: 'ms-media-gallery__thumbnails',
                    vertical: isVertical,
                    flipperPrevLabel: resources.previousScreenshotFlipperText,
                    flipperNextLabel: resources.nextScreenshotFlipperText,
                    parentId: id,
                    useTabList: true,
                    key: JSON.stringify(mediaGalleryThumbnailCarouselItems.keys)
                } as IComponentNodeProps<ISingleSlideCarouselProps>,
                items: mediaGalleryThumbnailCarouselItems.items
            }
        };

        return this.props.renderView(viewProps) as React.ReactElement;
    }

    /**
     * Zoomed out image on previous/next click.
     */
    public updateZoomedInImage(): void {
        this.setState({ isImageZoomed: false });
    }

    private readonly onExiting = () => {
        this.setState({ animating: true });
    };

    private readonly onExited = () => {
        this.setState({ animating: false });
    };

    /**
     * On click next in carousel.
     */
    private readonly next = (): void => {
        removeInlineZoomStyle();
        if (this.isLastItem() === undefined) {
            return;
        }

        const nextIndex = this.isLastItem() ? 0 : this.state.activeIndex + 1;
        this.goToIndex(nextIndex);

        this.updateZoomedInImage();
    };

    /**
     * On click previous in carousel.
     */
    private readonly previous = (): void => {
        removeInlineZoomStyle();
        const images = this.state.mediaGalleryItems;
        const nextIndex = this.isFirstItem() ? (images ? images.length - 1 : 0) : this.state.activeIndex - 1;
        this.goToIndex(nextIndex);
        this.updateZoomedInImage();
    };
    private _getProductExtension = (simpleProduct: SimpleProduct, extensionPropertyKey: string): string => {
        const property =
            simpleProduct.ExtensionProperties &&
            simpleProduct.ExtensionProperties.find(extension => extension.Key === extensionPropertyKey);
        if (property) {
            return property.Value?.StringValue || '';
        } else {
            return '';
        }
    };
    private readonly goToIndex = (index: number): void => {
        const selectedColorSwatchKey = 'SelectedColorSwatch';
        let selectedColorSwatch = '';
        if (this.props.data.product.result) {
            selectedColorSwatch = this._getProductExtension(
                this.props.data.product.result && this.props.data.product.result,
                selectedColorSwatchKey
            );
        }
        if (this.state.animating) {
            return;
        }
        this.setState({ activeIndex: index, selectedColor: selectedColorSwatch });
    };

    private _getMediaGalleryThumbnailItems(): IMediaGalleryCarouselItems<IMediaGalleryThumbnailItemViewProps> {
        const mediaGalleryItems = this.state.mediaGalleryItems;
        const thumbnailImageSettings = this.props.config.thumbnailImageSettings;
        if (thumbnailImageSettings) {
            thumbnailImageSettings.cropFocalRegion = true;
        }

        const hasMediaGalleryItems = ArrayExtensions.hasElements(mediaGalleryItems);

        if (!hasMediaGalleryItems) {
            if (this.state.lastUpdate) {
                const defaultKey = 0;
                return {
                    items: [this._getEmptyThumbnailItem(thumbnailImageSettings, defaultKey, this.state.activeIndex)],
                    keys: ['empty']
                };
            }
            return { items: [], keys: [] };
        }

        return {
            items: [
                ...mediaGalleryItems!.map((item: IImageData, index: number) =>
                    this._getThumbnailItem(
                        item,
                        thumbnailImageSettings ?? this.defaultThumbnailImageSettings,
                        index,
                        this.state.activeIndex
                    )
                )
            ],
            keys: [...mediaGalleryItems!.map(item => item.src)]
        };
    }

    private _getMediaGalleryItems(isFullscreenAllowed: boolean, zoomViewMode: string): IMediaGalleryCarouselItems<React.ReactNode> {
        const mediaGalleryItems = this.state.mediaGalleryItems;
        const galleryImageSettings = this.props.config.galleryImageSettings;
        if (galleryImageSettings) {
            galleryImageSettings.cropFocalRegion = true;
        }

        const zoomView = isFullscreenAllowed ? 'fullscreen' : zoomViewMode;

        const hasMediaGalleryItems = ArrayExtensions.hasElements(mediaGalleryItems);

        if (!hasMediaGalleryItems) {
            if (this.state.lastUpdate) {
                return { items: [this._renderEmptyImage(galleryImageSettings)], keys: ['empty'] };
            }
            return { items: [], keys: [] };
        }

        return {
            items: [
                ...mediaGalleryItems!.map((item: IImageData, index: number) =>
                    this._renderCarouselItemImageView(zoomView, item, galleryImageSettings ?? this.defaultGalleryImageSettings, index)
                )
            ],
            keys: [...mediaGalleryItems!.map(item => item.src)]
        };
    }

    private _setImages(images?: IImageData[]): void {
        const curatedImage = this.props.config.images || [];
        const shouldHidePrimaryImages = this.props.config.shouldHideMasterProductImagesForVariant ?? true;

        let imagesToSet = images ?? [];
        if (shouldHidePrimaryImages) {
            // Currently returned image response is having duplicate image
            // along with non-selected variant and previous selection is not getting cleared. With the help of index we
            // select item from 0 index till it is having non repeating image source name ending with index number.
            // Here we are keeping '.' to make sure that we are considering any digit just before '.' in image source path
            imagesToSet = imagesToSet.filter((item, index) => {
                let incrementedIndex = index;
                const imageIndex = `${++incrementedIndex}.`;
                return item.src?.includes(imageIndex);
            });
        }

        this.setState({
            mediaGalleryItems: [...imagesToSet, ...curatedImage],
            activeIndex: 0,
            lastUpdate: Date.now()
        });
    }

    private _renderCarouselItemImageView(
        zoomView: string,
        image: IImageData,
        imageSettings: IImageSettings,
        index: number,
        isInPopup: boolean = false
    ): React.ReactNode {
        if (this.isMobile()) {
            if (isInPopup) {
                return this._renderImageInContainerOnZoom(image, this._zoomedImageSettings, index);
            }
            return this._renderImageFullScreenOnZoom(image, imageSettings, index);
        }

        switch (zoomView) {
            case 'container': {
                return this._renderImageInContainerOnZoom(image, this._zoomedImageSettings, index);
            }
            case 'inline': {
                return this._renderImageInlineOnZoom(image, this._zoomedImageSettings, index);
            }
            case 'fullscreen': {
                return this._renderImageFullScreenOnZoom(image, imageSettings, index);
            }

            // No default
        }
        return <>{this._getCarouselItem(image, imageSettings, index, isInPopup)}</>;
    }

    private readonly _getCarouselItem = (
        image: IImageData,
        imageSettings: IImageSettings,
        index: number,
        isInPopup: boolean = false
    ): React.ReactNode => (
        <Image
            requestContext={this.props.context.actionContext.requestContext}
            className='ms-media-gallery__item'
            {...image}
            gridSettings={this.props.context.request.gridSettings!}
            imageSettings={imageSettings}
            loadFailureBehavior='default'
            id={`${this.props.id}__carousel-item__${index}`}
            shouldSkipToMainImage={isInPopup}
        />
    );

    private readonly _getThumbnailItem = (
        image: IImageData,
        imageSettings: IImageSettings,
        index: number,
        modifiedActiveIndex: number
    ): IMediaGalleryThumbnailItemViewProps => {
        const classes = classnames(
            'ms-media-gallery__thumbnail-item',
            modifiedActiveIndex === index ? 'ms-media-gallery__thumbnail-item-active' : ''
        );

        return {
            ThumbnailItemContainerProps: {
                tag: 'li' as NodeTag,
                className: classes,
                role: 'tab',
                tabIndex: 0,
                key: index,
                'aria-label': image.altText,
                'aria-selected': modifiedActiveIndex === index,
                onClick: this._generateOnThumbnailClick(index),
                onKeyDown: this._generateOnThumbnailKeyDown(index)
            },
            Picture: (
                <Image
                    requestContext={this.props.context.actionContext.requestContext}
                    className='ms-media-gallery__thumbnail'
                    {...image}
                    gridSettings={this.props.context.request.gridSettings!}
                    imageSettings={imageSettings}
                    loadFailureBehavior='default'
                />
            )
        };
    };

    /**
     * Ref Handler.
     * @param index -Remove item click function.
     * @returns Set inline zoom.
     */
    private readonly _refHandler = (index: number) => (divRef: HTMLDivElement) => {
        this._inlineZoomDivRef.set(index, divRef);
    };

    private readonly _generateOnThumbnailKeyDown = (index: number) => {
        return (event: React.KeyboardEvent) => {
            if (event.which === KeyCodes.Enter || event.which === KeyCodes.Space) {
                event.preventDefault();

                this.goToIndex(index);
            }
        };
    };

    private readonly _generateOnThumbnailClick = (index: number) => {
        return (event: React.MouseEvent<HTMLLIElement>) => {
            event.preventDefault();

            this.goToIndex(index);
        };
    };

    private _renderImageInlineOnZoom(image: IImageData, imageSettings: IImageSettings, index: number): React.ReactNode {
        return (
            <div
                className={`ms-inline-zoom ${this.state.isImageZoomed ? 'zoomed' : ''}`}
                ref={this._refHandler(index)}
                data-scale={this.props.config.dataScale ?? defaultDataScale}
            >
                <Image
                    requestContext={this.props.context.actionContext.requestContext}
                    className='ms-media-gallery__item'
                    {...image}
                    gridSettings={this.props.context.request.gridSettings!}
                    imageSettings={imageSettings}
                    loadFailureBehavior='default'
                    onClick={this._onInlineImageClick}
                    onMouseOver={(this.state.isImageZoomed && this._inlineZoomImageOnHover) || undefined}
                    id={`${this.props.id}__carousel-item__${index}`}
                    shouldSkipToMainImage
                />
                <Image
                    role='presentation'
                    requestContext={this.props.context.actionContext.requestContext}
                    className='ms-inline-zoom__zoomedImg'
                    {...image}
                    onClick={this._onImageMouseOut}
                    gridSettings={this.props.context.request.gridSettings!}
                    imageSettings={imageSettings}
                    loadFailureBehavior='default'
                    id={`${this.props.id}__zoom__${index}`}
                    onMouseMove={(this.state.isImageZoomed && inlineZoomImageOnMouseMove) || undefined}
                    shouldSkipToMainImage
                />
            </div>
        );
    }

    private _inlineZoomImageOnHover(event: React.MouseEvent<HTMLImageElement>): void {
        inlineZoomImageOnHover(event, this.props.config.dataScale ?? String(defaultDataScale));
    }

    private _handleMobileViewZoomedImageClick(event: React.MouseEvent<HTMLImageElement>) {
        const target = event.currentTarget;
        const mobileZoomedInImageClassName = 'msc-mobile-zoomed-in';
        if (!this.state.isMobileImageZoomed) {
            const bounds = target.getBoundingClientRect();
            const dataScale = Number(this.props.config.dataScale ?? defaultDataScale);

            const positionX = event.clientX - bounds.left;
            const positionY = event.clientY - bounds.top;
            const scaledPositionX = positionX * dataScale;
            const scaledPositionY = positionY * dataScale;

            target.style.transform = `scale(${dataScale})`;
            target.classList.add(mobileZoomedInImageClassName);
            target.parentElement!.style.overflow = 'auto';
            target.parentElement!.scrollTo(scaledPositionX - positionX, scaledPositionY - positionY);
            this.setState({
                isMobileImageZoomed: true
            });
        } else {
            target.style.transform = '';
            target.classList.remove(mobileZoomedInImageClassName);
            target.parentElement!.style.overflow = '';
            this.setState({
                isMobileImageZoomed: false
            });
        }
    }

    private _onInlineImageClick(event: React.MouseEvent<HTMLImageElement>): void {
        if (window.innerWidth <= 768) {
            // $msv-breakpoint-m
            this._handleMobileViewZoomedImageClick(event);
            return;
        }
        inlineZoomInitClick(event, this.props.config.dataScale ?? String(defaultDataScale));
        this.setState({
            isImageZoomed: true
        });
    }

    private _onImageMouseOut(event: React.MouseEvent<HTMLImageElement, MouseEvent>): void {
        inlineZoomImageOnMouseOut(event);
        this.setState({
            isImageZoomed: false
        });
    }

    private _onContainerZoomImageClick(event: React.MouseEvent<HTMLImageElement>): void {
        if (window.innerWidth <= 768) {
            // $msv-breakpoint-m
            this._handleMobileViewZoomedImageClick(event);
            return;
        }

        onContainerZoomInit(event);

        this.setState({
            isImageZoomed: true
        });
    }

    private _onMouseOutLensContainer(event: React.MouseEvent<HTMLImageElement>): void {
        onMouseOutLensContainer(event);

        this.setState({
            isImageZoomed: false
        });
    }

    private _renderImageInContainerOnZoom(image: IImageData, imageSettings: IImageSettings, index: number): React.ReactNode {
        return (
            <div className='ms-containerZoom__container'>
                <div
                    data-scale={this.props.config.dataScale ?? defaultDataScale}
                    className='ms-containerZoom__zoom-lens'
                    role='presentation'
                    onMouseOut={this._onMouseOutLensContainer}
                    onClick={this._onMouseOutLensContainer}
                    onMouseMove={(this.state.isImageZoomed && onMouseMoveLensContainer) || undefined}
                />
                <Image
                    requestContext={this.props.context.actionContext.requestContext}
                    className='ms-media-gallery__item'
                    {...image}
                    gridSettings={this.props.context.request.gridSettings!}
                    onClick={this._onContainerZoomImageClick}
                    imageSettings={imageSettings}
                    loadFailureBehavior='default'
                    onMouseOver={(this.state.isImageZoomed && onMouseOverImageContainer) || undefined}
                    id={`${this.props.id}__carousel-item__${index}`}
                    shouldSkipToMainImage
                />
            </div>
        );
    }

    private _renderImageFullScreenOnZoom(image: IImageData, imageSettings: IImageSettings, index: number): React.ReactNode {
        return (
            <div className='ms-fullscreen-section'>
                <Image
                    requestContext={this.props.context.actionContext.requestContext}
                    className='ms-media-gallery__item'
                    {...image}
                    gridSettings={this.props.context.request.gridSettings!}
                    imageSettings={imageSettings}
                    loadFailureBehavior='default'
                />
                <div className='ms-fullscreen-section__overlay'>
                    <a
                        href='javascript:void(0);'
                        title={this.props.resources.fullScreenTitleText}
                        role='button'
                        onClick={this.openModalDialog}
                        className='ms-fullscreen-section__magnifying-glass-icon'
                    />
                </div>
            </div>
        );
    }

    private _mapProductToImageData(product: SimpleProduct): IImageData {
        return {
            src: product.PrimaryImageUrl || ''
        };
    }

    private _renderEmptyImage(imageSettings: IImageSettings | undefined): React.ReactNode {
        return (
            <div className='ms-media-gallery__item'>
                <Image
                    requestContext={this.props.context.actionContext.requestContext}
                    className='ms-media-gallery__item__image'
                    src='empty'
                    gridSettings={this.props.context.request.gridSettings!}
                    imageSettings={imageSettings ?? this.defaultGalleryImageSettings}
                    loadFailureBehavior='empty'
                />
            </div>
        );
    }

    private _getEmptyThumbnailItem(
        imageSettings: IImageSettings | undefined,
        index: number,
        modifiedActiveIndex: number
    ): IMediaGalleryThumbnailItemViewProps {
        const classes = classnames(
            'ms-media-gallery__thumbnail-item',
            modifiedActiveIndex === index ? 'ms-media-gallery__thumbnail-item-active' : ''
        );
        return {
            ThumbnailItemContainerProps: {
                tag: 'li' as NodeTag,
                className: classes,
                role: 'tab',
                tabIndex: 0,
                key: 'empty',
                'aria-label': '',
                'aria-selected': modifiedActiveIndex === index,
                onClick: this._generateOnThumbnailClick(index),
                onKeyDown: this._generateOnThumbnailKeyDown(index)
            },
            Picture: (
                <Image
                    requestContext={this.props.context.actionContext.requestContext}
                    className='ms-media-gallery__thumbnail-item__image'
                    src='empty'
                    gridSettings={this.props.context.request.gridSettings!}
                    imageSettings={imageSettings ?? this.defaultThumbnailImageSettings}
                    loadFailureBehavior='empty'
                />
            )
        };
    }

    private readonly isFirstItem = () => this.state.activeIndex === 0;

    private readonly isLastItem = () => {
        const images = this.state.mediaGalleryItems;
        return images && this.state.activeIndex === images.length - 1;
    };

    private readonly openModalDialog = (): void => {
        this._toggleModal();
    };

    private _toggleModal(): void {
        if (this.state.modalIsOpen) {
            const parentDiv = this._inlineZoomDivRef.get(this.state.activeIndex);
            if (parentDiv && parentDiv.children && parentDiv.children.length >= 2) {
                const image = parentDiv.children[1].querySelector('img');
                if (image) {
                    image.removeAttribute('style');
                }
            }
            this.setState({
                isImageZoomed: false
            });
        }
        this.setState({
            isImageZoomed: false,
            modalIsOpen: !this.state.modalIsOpen
        });
        removeInlineZoomStyle();
        removeContainerZoomStyle();
    }

    private readonly imageModalSlider = (ZoomView: string): React.ReactElement => {
        const {
            data: {
                product: { result: product }
            },
            resources
        } = this.props;
        let mediaGalleryItems = this.state.mediaGalleryItems;

        if (!mediaGalleryItems && product) {
            mediaGalleryItems = [this._mapProductToImageData(product)];
        }
        if (this.state.modalIsOpen) {
            const carouselprops = {
                tag: Carousel,
                className: 'ms-media-gallery__carousel',
                items:
                    mediaGalleryItems &&
                    mediaGalleryItems.map((item: IImageData, index: number) =>
                        this._renderCarouselItemImageView(ZoomView, item, this._zoomedImageSettings, index, true)
                    ),
                activeIndex: this.state.activeIndex,
                next: this.next,
                previous: this.previous,
                interval: false,
                directionTextPrev: resources.previousScreenshotFlipperText,
                directionTextNext: resources.nextScreenshotFlipperText,
                onIndicatorsClickHandler: this.goToIndex,
                showPaginationTooltip: true,
                hideIndicator: false,
                keyboard: false,
                handleOnExited: this.onExited,
                handleOnExiting: this.onExiting,
                isDisabledFunctionality: this.state.isMobileImageZoomed
            } as IComponentNodeProps<ICarouselProps>;

            const carousel = <Carousel {...carouselprops} />;

            const imageModalSliderProps: IModalViewProps = {
                modalIsOpen: this.state.modalIsOpen,
                ontoggle: this._toggleModal,
                galleryCarousel: carousel,
                classNames: classnames('ms-media-gallery__modal', 'msc-modal-input-required')
            };
            return ImagefullView(imageModalSliderProps) as React.ReactElement;
        }
        return <div />;
    };
}

export default MediaGallery;
