import { IActionContext, ICommerceApiSettings, IImageData } from '@msdyn365-commerce/core';
import { MediaLocation } from '@msdyn365-commerce/retail-proxy';
import { getMediaLocationsAsync } from '@msdyn365-commerce/retail-proxy/dist/DataActions/ProductsDataActions.g';
export async function getValidProductImages(productId: number, channelId: number, actionContext: IActionContext): Promise<IImageData[]> {
    const mediaLocations = getMediaLocationsAsync({ callerContext: actionContext, queryResultSettings: {} }, productId, channelId, 0).then(
        response => {
            // When there are only product master images in the list of media location, they are set to priority 5
            // When there are variant images in the list of media location, product master images are set to priority 10, and variant images are set to priority 10
            if (response.findIndex(x => x && x.Priority && x.Priority !== 4) >= 0) {
                // If there are variant images
                response.forEach(x => {
                    if (x && x.Priority && x.Priority !== 4 && x.Uri) {
                        x.Uri = ''; // Remove URL of all Product Master Images
                    }
                });
            }
            return response.map(
                (mediaLocation: MediaLocation): MediaLocation => {
                    mediaLocation.Uri = generateImageUrl(mediaLocation.Uri, actionContext.requestContext.apiSettings);
                    mediaLocation.AltText = mediaLocation.AltText ? mediaLocation.AltText : '';
                    return mediaLocation;
                }
            );
        }
    );

    if (mediaLocations) {
        return Promise.all((await mediaLocations).map(validateMediaLocaionAsync)).then(pairs => {
            return pairs.filter(pair => pair[1]).map(pair => pair[0]);
        });
    }

    return [];
}

async function validateMediaLocaionAsync(mediaLocation: MediaLocation): Promise<[IImageData, boolean]> {
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
            http.open('HEAD', mediaLocation.Uri!, true);

            http.onload = () => {
                resolve([imageData, http.status === 200 || http.status === 201]);
            };

            http.onerror = () => {
                resolve([imageData, false]);
            };

            http.send();
        } catch (e) {
            resolve([imageData, false]);
        }
    });
}

/**
 * Generates a Image URL based on data return from retail server
 * @param imageUrl The image url returned by Retail Server
 * @param ctx The request context
 */
export const generateImageUrl = (imageUrl: string | undefined, apiSettings: ICommerceApiSettings): string | undefined => {
    if (imageUrl) {
        // Images hosted in CMS include schema
        if (imageUrl.startsWith('http')) {
            return imageUrl;
        }

        // Images hosted in Retail Server must be encoded and joined with the base image url
        return apiSettings.baseImageUrl + encodeURIComponent(imageUrl);
    } else {
        // d365Commerce.telemetry.warning(`Unable to generate a proper Image URL for Product: ${product.RecordId}`);
        return undefined;
    }
};
