/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { buildHydratedMockActionContext, buildMockModuleProps } from '@msdyn365-commerce/core';
import { IReviewsListProps } from '@msdyn365-commerce-modules/ratings-reviews';
import { INodeProps, Modal, ModalBody, ModalFooter, ModalHeader } from '@msdyn365-commerce-modules/utilities';
import { render } from 'enzyme';
import * as React from 'react';

import ReviewsListview from '../../views/reviews-list.view';

interface IReviewsListData {}

const mockActionContext = buildHydratedMockActionContext();

// @ts-expect-error
mockActionContext.requestContext.apiSettings = { rnr: { id: 'rnrId', url: 'rnrUrl' } };

const resources = {
    nextButtonText: 'Next',
    previousButtonText: 'Previous',
    pageReviewCountText: 'Showing {0}-{1} out of {2} reviews',
    pageReviewAriaLabel: 'Show {0}-{1} out of {2} reviews',
    publisherResponseBadgeText: '{0} responded on {1}',
    mostHelpfulSortOptionText: 'Most helpful',
    mostRecentSortOptionText: 'Most recent',
    highestRatedSortOptionText: 'Highest rated',
    lowestRatedSortOptionText: 'Lowest rated',
    fiveStarFilterByOptionText: '5 stars',
    fourStarFilterByOptionText: '4 stars',
    threeStarFilterByOptionText: '3 stars',
    twoStarFilterByOptionText: '2 stars',
    oneStarFilterByOptionText: '1 star',
    allRatinsFilterByOptionText: 'All ratings',
    filterByDropdownText: 'Filter by:',
    sortByDropdownText: 'Sort by:',
    noReviewsMessage: 'No one has reviewed this product yet.',
    noReviewsWithSelectedFilterMessage: 'Showing 0 reviews.',
    wasReviewHelpfulText: 'Was this helpful?',
    yesButtonText: 'Yes',
    noButtonText: 'No',
    cancelReportReviewText: 'Cancel',
    okReportReviewText: 'Ok',
    feedbackThankYouText: 'Thank you for your feedback.',
    profanityContentText: 'Contains profanity',
    offensiveContentText: 'Contains offensive content',
    reportSpamText: 'Contains spam or advertising',
    feedbackErrorText: 'Something went wrong. Please try again.',
    reportConcernText: 'Report a concern:',
    reviewRatingNarratorText: 'User Rating: {0} out of 5',
    editReviewCardText: 'Edit',
    reportedText: 'reported',
    reportModalMessage: 'message',
    reportSubmittedMessage: 'sucesfully submitted, we are taking a look',
    privacyPolicyTextFormat: 'By clicking submit, you accept our {0}.',
    privacyPolicyTitle: 'Privacy Policy',
    reviewTextLabel: 'Review',
    reviewTitleLabel: 'Title',
    selectRatingAriaLabel: 'Set ratings as {0} out of {1} stars',
    selectRatingLabel: 'Choose a rating',
    writeReviewModalTitle: 'Write a review',
    editReviewModalTitle: 'Edit your review',
    discardReviewButtonText: 'Discard',
    errorMessageText: 'Something went wrong, please try again',
    submitReviewButtonText: 'Submit',
    reportReviewButtonText: 'Report',
    averageRatingAriaLabel: '{0} stars, {1}'
};

const mockActions = {};

// @ts-expect-error
const mockAuthenticatedContextSignedIn: ICoreContext = {
    request: {
        user: {
            token: 'Dummy token',
            isAuthenticated: true
        }
    },
    app: {
        config: {}
    },
    actionContext: mockActionContext
};

const modalProps = {
    modal: {
        className: 'ms-nav',
        tag: Modal
    } as INodeProps,
    modalHeader: {
        className: 'ms-nav',
        tag: ModalHeader
    } as INodeProps,
    modalFooter: {
        className: 'ms-nav',
        tag: ModalFooter
    } as INodeProps,
    modalBody: {
        className: 'ms-nav',
        tag: ModalBody
    }
};
const mockConfig = {
    reviewsShownOnEachPage: 1
};
describe('Reviews list', () => {
    it('Render correctly.', () => {
        const moduleProps: IReviewsListProps<IReviewsListData> = {
            ...(buildMockModuleProps({}, mockActions, mockConfig, mockAuthenticatedContextSignedIn) as IReviewsListProps<IReviewsListData>),
            resources,
            renderModuleAttributes: jest.fn()
        };
        const viewProps = {
            moduleProps: { moduleProps },
            userReview: [{ rating: 4, title: 'my title', reviewText: 'my text' }],
            reviewCards: [{ rating: 4, title: 'my title', reviewText: 'my text' }],
            state: { isFilterApplied: true },
            noReviewsWithFilterMessage: <div />,
            reviewModal: modalProps,
            reportReviewModal: modalProps
        };

        // @ts-expect-error
        const component = render(<ReviewsListview {...viewProps} />);
        expect(component).toMatchSnapshot();
    });
    it('Render correctly with userReview and zero card', () => {
        const moduleProps: IReviewsListProps<IReviewsListData> = {
            ...(buildMockModuleProps({}, mockActions, {}, mockAuthenticatedContextSignedIn) as IReviewsListProps<IReviewsListData>),
            resources,
            renderModuleAttributes: jest.fn()
        };
        const viewProps = {
            moduleProps: { moduleProps },
            userReview: [{ rating: 4, title: 'my title', reviewText: 'my text' }],
            reviewCards: [],
            state: { isFilterApplied: true, reported: true },
            noReviewsWithFilterMessage: <div />,
            reportReviewModal: {
                modal: <div />,
                modalHeader: <div />,
                modalFooter: <div />,
                modalBody: <div />,
                header: <div />,
                headerSubmitted: <div />,
                cancelButton: <div />,
                submitButton: <div />,
                succesfulButton: <div />,
                reportMessage: <div />,
                reportSubmittedMessage: <div />,
                radioButtons: <div />,
                error: <div />
            },
            reviewModal: {
                modal: <div />,
                modalHeader: <div />,
                modalFooter: <div />,
                modalBody: <div />,
                header: <div />,
                headerSubmitted: <div />,
                cancelButton: <div />,
                submitButton: <div />,
                succesfulButton: <div />,
                reportMessage: <div />,
                reportSubmittedMessage: <div />,
                radioButtons: <div />,
                error: <div />
            }
        };

        // @ts-expect-error
        const component = render(<ReviewsListview {...viewProps} />);
        expect(component).toMatchSnapshot();
    });
    it('Render correctly with cards', () => {
        const moduleProps: IReviewsListProps<IReviewsListData> = {
            ...(buildMockModuleProps({}, mockActions, {}, mockAuthenticatedContextSignedIn) as IReviewsListProps<IReviewsListData>),
            resources,
            renderModuleAttributes: jest.fn()
        };
        const viewProps = {
            moduleProps: { moduleProps },
            reviewCards: [],
            state: { isFilterApplied: false }
        };

        // @ts-expect-error
        const component = render(<ReviewsListview {...viewProps} />);
        expect(component).toMatchSnapshot();
    });
});
