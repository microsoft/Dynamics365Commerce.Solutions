/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import MsDyn365, { IDictionary, IImageData, IImageSettings, Image } from '@msdyn365-commerce/core';
import { IMenuItemData, INavigationMenuViewProps } from '@msdyn365-commerce-modules/navigation-menu';
import { ArrayExtensions, generateImageUrl } from '@msdyn365-commerce-modules/retail-actions';
import {
    Drawer,
    getPayloadObject,
    getTelemetryAttributes,
    getTelemetryObject,
    ICollapseProps,
    IDrawerState,
    IPayLoad,
    ITelemetryContent,
    Module,
    Node,
    onTelemetryClick
} from '@msdyn365-commerce-modules/utilities';
import classnames from 'classnames';
import * as React from 'react';

interface INavigationState {
    parentMenu?: number;
    activeMenu?: number;
    categoryImage?: IImageData[] | null;
    categoryImageAltText: string;
    drawerKeyValue: IDictionary<boolean>;
}

/**
 *
 * NavigationMenuView component.
 * @extends {React.PureComponent<INavigationMenuViewProps>}
 */
export class NavigationMenuView extends React.PureComponent<INavigationMenuViewProps, INavigationState> {
    private currentLevel: number = 0;

    private readonly _positionInSetOffset: number = 1;

    private readonly escapeKey: number = 27;

    private readonly menuNode: React.RefObject<HTMLUListElement>;

    private readonly telemetryContent: ITelemetryContent;

    private readonly payLoad: IPayLoad;

    constructor(props: INavigationMenuViewProps) {
        super(props);
        this.menuNode = React.createRef();
        this.state = { activeMenu: undefined, parentMenu: undefined, categoryImageAltText: '', drawerKeyValue: {} };
        this._closeSubmenu = this._closeSubmenu.bind(this);
        this._escFunction = this._escFunction.bind(this);
        this.telemetryContent = getTelemetryObject(
            this.props.context.request.telemetryPageName!,
            this.props.friendlyName,
            this.props.telemetry
        );
        this.payLoad = getPayloadObject('click', this.telemetryContent, '', '');
    }

    public componentDidMount(): void {
        if (MsDyn365.isBrowser) {
            document.body.addEventListener('keydown', (this._escFunction as unknown) as EventListener, false);
            document.body.addEventListener('mousedown', this._handleClickOutside);
            document.body.addEventListener('focusout', this._handleFocusOutside);
        }
    }

    public componentWillUnmount(): void {
        if (MsDyn365.isBrowser) {
            document.removeEventListener('keydown', (this._escFunction as unknown) as EventListener, false);
            document.body.removeEventListener('mousedown', this._handleClickOutside, false);
            document.body.removeEventListener('focusout', this._handleFocusOutside, false);
        }
    }

    public render(): JSX.Element | null {
        const { isMobileView } = this.props;

        this.currentLevel = 1;
        return <>{isMobileView ? this._renderMobileMenu() : this._renderDesktopMenu()}</>;
    }

    /**
     * Method to render mobile menu.
     * @returns Jsx element.
     */
    private _renderMobileMenu(): JSX.Element {
        const { isMobileView, menuItemData, Navigation } = this.props;
        return (
            <Module {...Navigation} className={classnames(Navigation.className, isMobileView ? 'mobile-view' : 'desktop-view')}>
                {menuItemData.map((menuItem: IMenuItemData, posinset: number) => {
                    return this._renderDrawerMenu(menuItem, menuItemData.length, posinset + this._positionInSetOffset);
                })}
            </Module>
        );
    }

    /**
     * Method to render desktop menu.
     * @returns Jsx element.
     */
    private _renderDesktopMenu(): JSX.Element {
        const { MenuList, Navigation } = this.props;
        return (
            <Module {...Navigation} className={classnames(Navigation.className)}>
                <Node {...MenuList} ref={this.menuNode} tabIndex='-1'>
                    {this._renderDisplay()}
                </Node>
            </Module>
        );
    }

    /**
     * Method to render drawer menu.
     * @param menuItem -Menuitem data.
     * @param setSize -Setsize data.
     * @param posinset -Current position.
     * @returns Jsx element.
     */
    private _renderDrawerMenu(menuItem: IMenuItemData, setSize: number, posinset: number): JSX.Element | null {
        if (menuItem && menuItem.subMenu && ArrayExtensions.hasElements(menuItem.subMenu)) {
            return this._renderDrawer(menuItem, setSize, posinset);
        }
        return this._renderLinkMenuItem(menuItem, undefined, true, false, setSize, posinset);
    }

    /**
     * Method to render drawer component.
     * @param menuItem -Menuitem data.
     * @param setSize -Setsize data.
     * @param posinset -Current position.
     * @returns Jsx element.
     */
    private _renderDrawer(menuItem: IMenuItemData, setSize?: number, posinset?: number): JSX.Element | null {
        const toggleButtonText = menuItem.linkText;
        const keyValue = this.state.drawerKeyValue;
        const buttonText = toggleButtonText !== undefined ? toggleButtonText : '';
        const keys = keyValue !== undefined ? keyValue : {};
        const { isMobileView } = this.props;

        let isDrawerOpen = false;
        if (keys[buttonText]) {
            isDrawerOpen = true;
        }
        const colProps: ICollapseProps = { isOpen: isDrawerOpen };
        return (
            <Drawer
                role='listbox'
                toggleButtonProps={{
                    'aria-setsize': setSize,
                    'aria-posinset': posinset,
                    role: 'option'
                }}
                ariaLabel={isMobileView ? menuItem.ariaLabel : undefined}
                key={menuItem.id}
                className='ms-nav__drawer'
                openGlyph='ms-nav__drawer-open'
                onToggle={this._onDrawerChange}
                collapseProps={colProps}
                closeGlyph='ms-nav__drawer-close'
                glyphPlacement='end'
                toggleButtonText={this._renderLinkText(toggleButtonText, setSize, posinset)}
            >
                <div role='listbox'>
                    {menuItem.subMenu!.map((menuSubItem: IMenuItemData, currentPos: number) => {
                        if (ArrayExtensions.hasElements(menuSubItem.subMenu)) {
                            return this._renderDrawer(menuSubItem, menuItem.subMenu?.length, currentPos + this._positionInSetOffset);
                        }
                        return this._renderDrawerLink(menuSubItem, menuItem.subMenu?.length, currentPos + this._positionInSetOffset);
                    })}
                </div>
            </Drawer>
        );
    }

    /**
     * Method to render link.
     * @param linkText -Text on Menu link.
     * @param setSize -Setsize data.
     * @param posinset -Current position.
     * @returns Jsx element.
     */
    private _renderLinkText(linkText: string | undefined, setSize: number | undefined, posinset: number | undefined): JSX.Element {
        return (
            <span aria-setsize={setSize} aria-posinset={posinset}>
                {linkText}
            </span>
        );
    }

    /**
     * Method to render drawer link.
     * @param item -Single Menuitem.
     * @param setSize -Setsize data.
     * @param posinset -Current position.
     * @returns Jsx element.
     */
    private _renderDrawerLink(item: IMenuItemData, setSize: number | undefined, posinset: number): JSX.Element | null {
        if (item && item.linkText && item.linkURL && item.linkURL.length > 0) {
            return this._renderLinkMenuItem(item, undefined, true, false, setSize, posinset);
        } else if (item && item.linkText && !item.linkURL) {
            return this._renderSpanMenuItem(item);
        }
        return null;
    }

    /**
     * Method to generate menu.
     * @returns Jsx element.
     */
    private _renderDisplay(): JSX.Element[] {
        const { ListItem, menuItemData, isMobileView } = this.props;
        const { activeMenu } = this.state;
        const menuItemList: JSX.Element[] = [];

        if (isMobileView && activeMenu !== undefined && menuItemData.length > 0) {
            let menuItem: IMenuItemData = {};
            for (const menuItemDatum of menuItemData) {
                if (menuItemDatum && menuItemDatum.id === activeMenu) {
                    menuItem = menuItemDatum;
                    this.setState({ parentMenu: undefined });
                    break;
                }
                menuItem = this._getFromSubMenu(menuItemDatum) as IMenuItemData;
                if (menuItem && menuItem.id === activeMenu) {
                    break;
                }
            }

            menuItem &&
                menuItemList.push(
                    <Node key={menuItem.id} {...ListItem}>
                        {' '}
                        {this._createMenuItemList(menuItem)}{' '}
                    </Node>
                );
        } else {
            menuItemData.forEach((item: IMenuItemData, index: number) => {
                menuItemList.push(
                    <Node key={index} {...ListItem}>
                        {this._createMenuItemList(item)}
                    </Node>
                );
            });
        }

        return menuItemList;
    }

    /**
     * Method to get data for submenu.
     * @param item -Single Menuitem.
     * @returns IMenuItemData.
     */
    private _getFromSubMenu(item?: IMenuItemData): IMenuItemData | null {
        const subMenus = item && item.subMenu;
        if (subMenus && subMenus.length > 0) {
            for (let i = 0; i <= subMenus.length - 1; i++) {
                if (subMenus[i].id === this.state.activeMenu) {
                    this.setState({ parentMenu: item?.id });
                    return subMenus[i];
                }
                const found = this._getFromSubMenu(subMenus[i]);
                if (found) {
                    return found;
                }
            }
        }
        return null;
    }

    /**
     * Method to create item list.
     * @param menuItemData -Single Menuitem.
     * @returns Jsx element.
     */
    private _createMenuItemList(menuItemData: IMenuItemData): JSX.Element | null {
        if (menuItemData && menuItemData.subMenu && menuItemData.subMenu.length > 0) {
            if (this.props.isMobileView && this.state.activeMenu !== undefined) {
                return this._renderSubMenu(menuItemData.subMenu, menuItemData.id);
            }
            return (
                <>
                    {this._renderButtonMenuItem(menuItemData)}
                    {this._renderSubMenu(menuItemData.subMenu, menuItemData.id)}
                </>
            );
        } else if (menuItemData && menuItemData.linkText && menuItemData.linkURL && menuItemData.linkURL.length > 0) {
            return this._renderLinkMenuItem(menuItemData, menuItemData.id, false, true);
        } else if (menuItemData && menuItemData.linkText && !menuItemData.linkURL) {
            return this._renderSpanMenuItem(menuItemData, menuItemData.id, true);
        }

        return null;
    }

    private _renderSubMenu(subMenus?: IMenuItemData[], activeMenu?: number, IsSubMenu?: boolean): JSX.Element | null {
        const { isMobileView, ListItem } = this.props;
        const enableMultiSupportMenu = this.props.config.enableMultilevelMenu || false;
        const subMenuLevel = 3;
        const multiLevelSupportedMenu = this.props.config.menuLevelSupport || subMenuLevel;

        // Const isParentMenu:boolean= false;
        if (activeMenu && this.state.activeMenu !== activeMenu) {
            this.props.context.telemetry.error('Navigation Active menu content is empty, module wont render.');
            return null;
        }

        if (!subMenus || subMenus.length === 0) {
            this.props.context.telemetry.error('Navigation Submenu content is empty, module wont render.');
            return null;
        }

        let levelClassName: string = '';
        const menuOptions =
            subMenus &&
            subMenus.map((option: IMenuItemData, idx: number) => {
                const hasOptions = option.subMenu && option.subMenu.length > 0;
                let menuItem: JSX.Element | null;
                if (hasOptions && isMobileView) {
                    menuItem = this._renderButtonMenuItem(option, activeMenu, idx);
                } else {
                    menuItem = option.linkURL ? this._renderLinkMenuItem(option, idx) : this._renderSpanMenuItem(option);
                }
                let subMenu;
                const haveSubmenu = hasOptions && enableMultiSupportMenu && this.currentLevel <= Math.round(multiLevelSupportedMenu) - 1;
                if (haveSubmenu) {
                    this.currentLevel++;
                    levelClassName = enableMultiSupportMenu ? `level-${this.currentLevel.toString()}` : '';
                    subMenu = this._renderSubMenu(option.subMenu, isMobileView ? option.id : undefined, true);
                }
                return (
                    <Node {...ListItem} key={option.id} className={classnames(ListItem.className, haveSubmenu && 'havesubmenu')}>
                        {menuItem}
                        {subMenu}
                    </Node>
                );
            });
        return this._renderMenu(levelClassName, menuOptions, activeMenu, IsSubMenu);
    }

    /**
     * Method to render button menu item.
     * @param option -Single Menuitem.
     * @param activeMenu -Active menu number.
     * @param index -Active menu index.
     * @returns Jsx element.
     */
    private _renderButtonMenuItem(option: IMenuItemData, activeMenu?: number, index?: number): JSX.Element | null {
        const { Button } = this.props;
        return (
            <Node
                key={index}
                {...Button}
                onClick={this._handleDropdownToggle(option, activeMenu)}
                onFocus={this._closeSubmenu}
                aria-haspopup={!(this.state.activeMenu && this.state.activeMenu === option.id)}
                aria-expanded={!!(this.state.activeMenu && this.state.activeMenu === option.id)}
                data-parent={activeMenu}
            >
                {option.linkText}
            </Node>
        );
    }

    /**
     * Method to render link menu item.
     * @param option -Single Menuitem.
     * @param index -Active menu index.
     * @param hoverEffect -Active menu effect.
     * @param isParent -Is parent menu.
     * @param setSize -Setsize data.
     * @param posinset -Current position.
     * @returns Jsx element.
     */
    private _renderLinkMenuItem(
        option: IMenuItemData,
        index?: number,
        hoverEffect: boolean = true,
        isParent: boolean = false,
        setSize?: number,
        posinset?: number
    ): JSX.Element | null {
        const { Link, isMobileView } = this.props;
        const linkText = option.linkText ? option.linkText : '';
        const imagesource = option.imageSource ? option.imageSource : '';
        this.payLoad.contentAction.etext = linkText;
        const attributes = getTelemetryAttributes(this.telemetryContent, this.payLoad);
        return (
            <Node
                aria-setsize={isMobileView ? setSize : undefined}
                aria-posinset={isMobileView ? posinset : undefined}
                aria-label={isMobileView ? option.ariaLabel : undefined}
                {...Link}
                key={index}
                onFocus={isParent && this._closeSubmenu}
                target={option.shouldOpenNewTab ? '_blank' : undefined}
                onMouseOver={hoverEffect && this._updateCategoryImage(imagesource, option)}
                href={option.linkURL}
                {...attributes}
                onClick={onTelemetryClick(this.telemetryContent, this.payLoad, linkText)}
            >
                {option.linkText}
            </Node>
        );
    }

    /**
     * Method to render promotional link.
     * @param linkText -Link text.
     * @param linkUrl -Link url.
     * @returns Jsx element.
     */
    private _renderPromotionalLink(linkText?: string, linkUrl?: string): JSX.Element | null {
        const { Link } = this.props;
        this.payLoad.contentAction.etext = linkText;
        const attributes = getTelemetryAttributes(this.telemetryContent, this.payLoad);
        if (linkText && linkUrl) {
            return (
                <Node {...Link} href={linkUrl} {...attributes} onClick={onTelemetryClick(this.telemetryContent, this.payLoad, linkText)}>
                    {linkText}
                </Node>
            );
        }
        return null;
    }

    /**
     * Method to render span menu item.
     * @param option -Single Menu Item.
     * @param index -Index.
     * @param isParent -Is parent menu.
     * @returns Jsx element.
     */
    private _renderSpanMenuItem(option: IMenuItemData, index?: number, isParent: boolean = false): JSX.Element | null {
        const { Span } = this.props;
        return (
            <Node key={index} {...Span} onFocus={isParent && this._closeSubmenu}>
                {option.linkText}
            </Node>
        );
    }

    /**
     * Method to render menu.
     * @param level -Menu level.
     * @param menuOptions -Menu lists.
     * @param currentItem -Menu current.
     * @param submenu -Mneu has submenu or not.
     * @returns Jsx element.
     */
    private _renderMenu(level: string, menuOptions: JSX.Element[], currentItem?: number, submenu?: boolean): JSX.Element | null {
        const { DivContainer, MenuList, ImageDivContainer, showCategoryImage, isMobileView, showPromotionalContent } = this.props;
        const categoryImageDisplay =
            !isMobileView && showCategoryImage && this.state.categoryImage !== null && this.state.categoryImage !== undefined && !submenu;
        const promotionalContentDisplay =
            !isMobileView && showPromotionalContent && ArrayExtensions.hasElements(this.state.categoryImage) && !submenu;
        const DivContainerClass = this.currentLevel > 2 || categoryImageDisplay ? DivContainer!.className : 'ms-nav__deafult';
        this.currentLevel = 1;
        return (
            <Node {...DivContainer} className={DivContainerClass}>
                <Node
                    {...MenuList}
                    className={classnames(
                        MenuList.className,
                        level,
                        categoryImageDisplay && 'havecateImage',
                        categoryImageDisplay &&
                            this.props.config.menuLevelSupport &&
                            this.props.config.menuLevelSupport > 2 &&
                            'navmenu-multi-level'
                    )}
                >
                    {menuOptions}
                </Node>
                <div className='category-image-container'>
                    {categoryImageDisplay &&
                        this.state.categoryImage &&
                        this.state.categoryImage.map(item => (
                            // eslint-disable-next-line react/jsx-key
                            <Node {...ImageDivContainer} className={ImageDivContainer!.className}>
                                {this.state.categoryImage && this._getCategoryImage(item)}
                                {promotionalContentDisplay && this._renderPromotionalLink(item.altText, item.additionalProperties?.linkUrl)}
                            </Node>
                        ))}
                </div>
            </Node>
        );
    }

    private readonly _updateCategoryImage = (categoryImageSrc: string, option: IMenuItemData) => () => {
        const linkText = option && option.linkText ? option.linkText : '';
        const promotionalImage: IImageData[] = [{ src: categoryImageSrc, altText: linkText }];

        // Read category and promotional image in one array
        if (ArrayExtensions.hasElements(option.promotionalContent)) {
            option.promotionalContent.map(item => {
                if (item && item.image) {
                    const imageSrc = item.image.src;
                    const promotionalItemImageSettings = item.image.imageSettings;
                    promotionalImage.push({
                        src: imageSrc,
                        altText: item.text,
                        imageSettings: promotionalItemImageSettings,
                        additionalProperties: { linkUrl: item.linkUrl.destinationUrl }
                    });
                }
            });
        }
        this.setState({
            categoryImage: promotionalImage.length > 0 ? promotionalImage : [{ src: 'empty' }],
            categoryImageAltText: linkText
        });
    };

    /**
     * Method to handle dropdwon change.
     * @param data -Menuitem data.
     * @param parentId -Menu parent.
     * @returns Jsx element.
     */
    private readonly _handleDropdownToggle = (data: IMenuItemData, parentId?: number) => () => {
        if (!this.props.isMobileView) {
            this.setState({
                activeMenu: this.state.activeMenu && this.state.activeMenu === data.id! ? undefined : data.id!,
                parentMenu: parentId
            });
            if (this.props.showCategoryImage) {
                this._updateCategoryImage(data.imageSource!, data)();
            }
        } else {
            this.setState({
                activeMenu: data.id,
                parentMenu: parentId
            });
        }

        this._resetFocus();
    };

    /**
     * Method to call when focus lost from menu.
     */
    private readonly _resetFocus = () => {
        if (this.props.isMobileView) {
            setTimeout(() => {
                this.menuNode && this.menuNode.current && this.menuNode.current.focus();
            }, 0);
        }
    };

    /**
     * Method to handle click outside of menu.
     * @param event -HTML event.
     */
    private readonly _handleClickOutside = (event: MouseEvent) => {
        if (this.menuNode.current && !this.menuNode.current.contains(event.target as Node)) {
            this.setState({
                activeMenu: undefined,
                categoryImage: null
            });
        }
    };

    /**
     * Method to handle click outside of menu.
     * @param event -HTML event.
     */
    private readonly _handleFocusOutside = (event: FocusEvent) => {
        if (this.menuNode.current && !this.menuNode.current.contains(event.relatedTarget as Node)) {
            this._closeSubmenu();
        }
    };

    /**
     * Method to handle close submenu event.
     */
    private _closeSubmenu(): void {
        if (!this.props.isMobileView) {
            this.setState({ activeMenu: undefined });
        }
    }

    /**
     * Method to handle escape key event.
     * @param event -HTML event.
     */
    private readonly _escFunction = (event: React.KeyboardEvent) => {
        if (event.keyCode === this.escapeKey) {
            if (!this.props.isMobileView) {
                const navDrawerList = Array.from(this.menuNode.current!.childNodes);
                for (const item of navDrawerList) {
                    const navDrawerButton = item.firstChild as HTMLButtonElement;
                    if (navDrawerButton.getAttribute('aria-expanded') === 'true') {
                        navDrawerButton.focus();
                    }
                }
            }
            this._closeSubmenu();
        }
    };

    private readonly _getCategoryImage = (categoryImage?: IImageData): React.ReactNode | null => {
        if (!categoryImage || !categoryImage.src) {
            return null;
        }

        const categoryImageUrl = generateImageUrl(categoryImage.src, this.props.context.actionContext.requestContext.apiSettings);
        const defaultImageSettings: IImageSettings = {
            viewports: {
                xs: { q: 'w=300&h=250&m=8', w: 0, h: 0 },
                sm: { q: 'w=300&h=250&m=8', w: 0, h: 0 },
                md: { q: 'w=300&h=250&m=8', w: 0, h: 0 },
                lg: { q: 'w=300&h=250&m=8', w: 0, h: 0 }
            },
            lazyload: true
        };
        if (categoryImageUrl !== undefined) {
            const ImageData: IImageData = { src: categoryImageUrl };
            return (
                <Image
                    requestContext={this.props.context.actionContext.requestContext}
                    className='ms-nav-image__item'
                    {...ImageData}
                    gridSettings={this.props.context.request.gridSettings!}
                    imageSettings={categoryImage.imageSettings ?? this.props.config.categoryImageSettings ?? defaultImageSettings}
                    loadFailureBehavior='hide'
                    role='tabpanel'
                    id={`${categoryImage.src}__categoryImage`}
                    altText={categoryImage.altText}
                />
            );
        }
        return null;
    };

    /**
     * Method to handle escape key event.
     * @param drawerState -HTML event.
     */
    private readonly _onDrawerChange = (drawerState: IDrawerState) => {
        const { drawerKeyValue } = this.state;
        const drawerText: string = drawerState.buttonText !== undefined ? drawerState.buttonText : '';
        const newPair = { [drawerText]: drawerState.isOpen };
        this.setState({ drawerKeyValue: { ...drawerKeyValue, ...newPair } });
    };
}

export default NavigationMenuView;
