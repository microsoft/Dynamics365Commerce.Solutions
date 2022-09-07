/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

/* eslint-disable no-duplicate-imports */
import { IActionContext, IImageData, IImageSettings } from '@msdyn365-commerce/core';
import { MediaLocation, SimpleProduct } from '@msdyn365-commerce/retail-proxy';
import { getMediaLocationsForSelectedVariant, MediaLocationsForSelectedVariantInput } from '@msdyn365-commerce-modules/retail-actions';

export async function getValidProductImages(
    productId: number,
    channelId: number,
    actionContext: IActionContext,
    imageSettings: IImageSettings,
    selectedProduct?: SimpleProduct
): Promise<IImageData[]> {
    const actionInput = new MediaLocationsForSelectedVariantInput(productId, channelId, selectedProduct);

    return getMediaLocationsForSelectedVariant(actionInput, actionContext)
        .then(mediaLocations => {
            if (mediaLocations) {
                return Promise.all(mediaLocations.map(mediaLocation => validateMediaLocaionAsync(mediaLocation, imageSettings))).then(
                    pairs => {
                        return pairs.filter(pair => pair[1]).map(pair => pair[0]);
                    }
                );
            }

            return [];
        })
        .catch(error => {
            actionContext.telemetry.exception(error);
            actionContext.telemetry.debug('Unable to get Media Locations for Selected Variant');
            return [];
        });
}

export async function validateProductImages(
    mediaLocations: MediaLocation[],
    actionContext: IActionContext,
    imageSettings: IImageSettings
): Promise<IImageData[]> {
    return Promise.all(mediaLocations.map(mediaLocation => validateMediaLocaionAsync(mediaLocation, imageSettings)))
        .then(pairs => {
            return pairs.filter(pair => pair[1]).map(pair => pair[0]);
        })
        .catch(error => {
            actionContext.telemetry.exception(error);
            actionContext.telemetry.debug('Unable to validate prodcut images');
            return [];
        });
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars, @typescript-eslint/no-unused-vars-experimental -- .
async function validateMediaLocaionAsync(mediaLocation: MediaLocation, imageSettings?: IImageSettings): Promise<[IImageData, boolean]> {
    const imageData = {
        src: mediaLocation.Uri || '',
        altText: mediaLocation.AltText || ''
    };

    if (imageData.src === '') {
        return [imageData, false];
    }

    return new Promise<[IImageData, boolean]>(resolve => {
        try {
            const http = new XMLHttpRequest();
            http.open('HEAD', imageData.src, true);

            http.addEventListener('load', () => {
                resolve([imageData, http.status === 200 || http.status === 201]);
            });

            http.addEventListener('error', () => {
                resolve([imageData, false]);
            });

            http.send();
        } catch {
            resolve([imageData, false]);
        }
    });
}
