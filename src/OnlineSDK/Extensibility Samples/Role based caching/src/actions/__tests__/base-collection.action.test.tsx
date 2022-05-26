import { ICreateActionContext } from '@msdyn365-commerce/core-internal';
import { CollectionClassConstructor, createBaseCollectionInput } from '../base-collection.action';

describe('to update the cache key with user type', () => {
    it('throw exception if not call input correctly', () => {
        try {
            createBaseCollectionInput(
                ({
                    requestContext: {
                        urlTokens: {
                            pageType: 'Search'
                        },
                        apiSettings: {}
                    }
                } as unknown) as ICreateActionContext,
                {} as CollectionClassConstructor,
                undefined
            );
        } catch (error) {
            expect(error).toStrictEqual(new Error('[getFullProductsForCollection]Unable to create input'));
        }
    });
});
