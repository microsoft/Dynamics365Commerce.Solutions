/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { IReviewModalViewProps } from '@msdyn365-commerce-modules/ratings-reviews';
import {
    IReportReviewModalViewProps,
    IReviewCardViewProps,
    IReviewsListState,
    IReviewsListViewProps
} from '@msdyn365-commerce-modules/ratings-reviews/src/modules/reviews-list';
import { IModuleProps, Module, Node } from '@msdyn365-commerce-modules/utilities';
import * as React from 'react';

const ReviewsListview: React.FC<IReviewsListViewProps> = props => {
    const {
        averageRating,
        filterByDropdown,
        moduleProps,
        noReviewsMessage,
        noReviewsWithFilterMessage,
        pageControls,
        refineReviewsProps,
        reportReviewModal,
        reviewsListProps,
        reviewCards,
        reviewCount,
        reviewModal,
        sortByDropdown,
        state,
        userReview
    } = props;

    if (!userReview && reviewCards.length === 0 && !state.isFilterApplied) {
        return <Module {...moduleProps}>{noReviewsMessage}</Module>;
    }

    return (
        <Module {...moduleProps}>
            {averageRating}
            {reviewCount}
            <Node {...refineReviewsProps}>
                {sortByDropdown}
                {filterByDropdown}
            </Node>
            <Node {...reviewsListProps}>
                {userReview && buildReviewCard(userReview)}
                {reviewCards.map(review => {
                    return buildReviewCard(review);
                })}
            </Node>
            {reviewCards.length === 0 && state.isFilterApplied && noReviewsWithFilterMessage}
            {pageControls}
            {createReviewModal(reviewModal, moduleProps)}
            {createReportModal(reportReviewModal, state)}
        </Module>
    );
};

const buildReviewCard = (props: IReviewCardViewProps) => {
    return (
        <Node {...props.cardProps}>
            <Node {...props.headerProps}>
                {props.rating}
                {props.name}
                {props.date}
            </Node>
            <Node {...props.cardBodyProps}>
                <Node {...props.reviewProps}>
                    {props.reviewTitle}
                    {props.reviewText}
                </Node>
                <Node {...props.responseProps}>
                    {props.responseName}
                    {props.responseDate}
                    {props.responseText}
                </Node>
                <Node {...props.controlsProps}>
                    {props.ratingHelpfulLabel}
                    {props.like}
                    {props.dislike}
                    {props.edit}
                    {props.report}
                </Node>
            </Node>
        </Node>
    );
};

const createReviewModal = (props: IReviewModalViewProps, moduleProps: IModuleProps): JSX.Element => {
    return (
        <Module {...props.modal} {...moduleProps}>
            {props.modalHeader}
            <Node {...props.modalBody}>
                <Node {...props.form}>
                    <Node {...props.inputRow}>
                        {props.rating}
                        {props.ratingLabel}
                    </Node>
                    <Node {...props.inputRow}>
                        {props.titleLabel}
                        {props.titleInput}
                    </Node>
                    <Node {...props.inputRow}>
                        {props.textLabel}
                        {props.textInput}
                    </Node>
                    {props.privacyPolicyUrl}
                    {props.error}
                </Node>
            </Node>
            <Node {...props.modalFooter}>
                {props.submitButton}
                {props.cancelButton}
            </Node>
        </Module>
    );
};

const createReportModal = (props: IReportReviewModalViewProps, state: IReviewsListState): JSX.Element => {
    return (
        <Node {...props.modal}>
            <Node {...props.modalHeader}>{state.reported ? props.headerSubmitted : props.header}</Node>
            <Node {...props.modalBody}>
                {state.reported ? props.reportSubmittedMessage : [props.reportMessage, props.radioButtons, props.error]}
            </Node>
            <Node {...props.modalFooter}>{state.reported ? props.succesfulButton : [props.submitButton, props.cancelButton]}</Node>
        </Node>
    );
};

export default ReviewsListview;
