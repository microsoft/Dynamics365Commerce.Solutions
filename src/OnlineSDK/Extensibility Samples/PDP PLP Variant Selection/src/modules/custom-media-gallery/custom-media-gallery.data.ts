/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import { AsyncResult, MediaLocation } from '@msdyn365-commerce/retail-proxy';
import { Image } from '@msdyn365-commerce-modules/data-types';
import { ISelectedProduct } from '@msdyn365-commerce-modules/retail-actions';

export interface IMediaGalleryItem {
    content: Image;
    thumbnail: Image;
}

export interface ICustomMediaGalleryData {
    product: AsyncResult<ISelectedProduct>;
    mediaLocations: AsyncResult<MediaLocation[]>;
    mediaLocationsForSelectedVariant: AsyncResult<MediaLocation[]>;
}
