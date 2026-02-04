/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable no-duplicate-imports */
import { RegularStoreHours } from '@msdyn365-commerce/retail-proxy';
import get from 'lodash/get';

import { IStoreSelectorLocationLineItemResources } from '.';

export interface IHoursDisplayInfo {
    startDayShort: string;
    startDayFull: string;
    endDayShort?: string;
    endDayFull?: string;
    timeRange: string;
}

interface IHoursInfo {
    isClosed: boolean;
    openTime?: number;
    closeTime?: number;
}

type DayType = 'MON' | 'TUE' | 'WED' | 'THU' | 'FRI' | 'SAT' | 'SUN';

export const secondsToTime = (seconds: number | undefined): string | undefined => {
    if (seconds === undefined) {
        return undefined;
    }
    let hours = Math.floor(seconds / (60 * 60));
    const ampm = hours >= 12 ? 'pm' : 'am';
    hours %= 12;
    hours = hours ? hours : 12;
    const divisorForMinutes = seconds % (60 * 60);
    const minutes = Math.floor(divisorForMinutes / 60) < 10 ? `0${Math.floor(divisorForMinutes / 60)}` : Math.floor(divisorForMinutes / 60);
    return `${hours}:${minutes} ${ampm}`;
};

const getShortTimeString = (day: DayType, resources: IStoreSelectorLocationLineItemResources): string => {
    switch (day) {
        case 'MON':
            return resources.days.monday;
        case 'TUE':
            return resources.days.tuesday;
        case 'WED':
            return resources.days.wednesday;
        case 'THU':
            return resources.days.thursday;
        case 'FRI':
            return resources.days.friday;
        case 'SAT':
            return resources.days.saturday;
        default:
            // Case 'SUN':
            return resources.days.sunday;
    }
};

const getFullTimeString = (day: DayType, resources: IStoreSelectorLocationLineItemResources): string => {
    switch (day) {
        case 'MON':
            return resources.days.mondayFull;
        case 'TUE':
            return resources.days.tuesdayFull;
        case 'WED':
            return resources.days.wednesdayFull;
        case 'THU':
            return resources.days.thursdayFull;
        case 'FRI':
            return resources.days.fridayFull;
        case 'SAT':
            return resources.days.saturdayFull;
        default:
            // Case 'SUN':
            return resources.days.sundayFull;
    }
};

const buildHoursDisplayInfo = (
    startDay: DayType,
    endDay: DayType,
    hoursInfo: string,
    resources: IStoreSelectorLocationLineItemResources
): IHoursDisplayInfo => {
    return {
        startDayShort: getShortTimeString(startDay, resources),
        startDayFull: getFullTimeString(startDay, resources),
        endDayShort: startDay !== endDay ? getShortTimeString(endDay, resources) : undefined,
        endDayFull: startDay !== endDay ? getFullTimeString(endDay, resources) : undefined,
        timeRange: hoursInfo
    };
};

const shouldMergeLines = (firstLine: IHoursInfo, secondLine: IHoursInfo): boolean => {
    if (firstLine.isClosed && secondLine.isClosed) {
        return true;
    }

    if (!firstLine.isClosed && !secondLine.isClosed) {
        return firstLine.openTime === secondLine.openTime && firstLine.closeTime === secondLine.closeTime;
    }

    return false;
};

const getStoreHoursData = (storeHours: RegularStoreHours, dateForLookup: string): IHoursInfo => {
    return {
        isClosed: get(storeHours, `IsClosedOn${dateForLookup}`, false),
        openTime: get(storeHours, `${dateForLookup}OpenTime`, undefined),
        closeTime: get(storeHours, `${dateForLookup}CloseTime`, undefined)
    };
};

const buildStoreHoursString = (hoursInfo: IHoursInfo, resources: IStoreSelectorLocationLineItemResources): string => {
    if (hoursInfo.isClosed) {
        return resources.closedText;
    }
    const openFrom: string | undefined = secondsToTime(hoursInfo.openTime);
    const openTo: string | undefined = secondsToTime(hoursInfo.closeTime);

    return `${openFrom}–${openTo}`;
};

export const buildStoreHours = (storeHours: RegularStoreHours, resources: IStoreSelectorLocationLineItemResources): IHoursDisplayInfo[] => {
    const storeHoursList: IHoursDisplayInfo[] = [];

    const mondayHours = getStoreHoursData(storeHours, 'Monday');
    const tuesdayHours = getStoreHoursData(storeHours, 'Tuesday');
    const wednesdayHours = getStoreHoursData(storeHours, 'Wednesday');
    const thursdayHours = getStoreHoursData(storeHours, 'Thursday');
    const fridayHours = getStoreHoursData(storeHours, 'Friday');
    const saturdayHours = getStoreHoursData(storeHours, 'Saturday');
    const sundayHours = getStoreHoursData(storeHours, 'Sunday');

    let periodStart: DayType = 'MON';
    let periodEnd: DayType = 'MON';

    if (!shouldMergeLines(mondayHours, tuesdayHours)) {
        storeHoursList.push(buildHoursDisplayInfo(periodStart, periodEnd, buildStoreHoursString(mondayHours, resources), resources));

        periodStart = 'TUE';
    }
    periodEnd = 'TUE';

    if (!shouldMergeLines(tuesdayHours, wednesdayHours)) {
        storeHoursList.push(buildHoursDisplayInfo(periodStart, periodEnd, buildStoreHoursString(tuesdayHours, resources), resources));

        periodStart = 'WED';
    }
    periodEnd = 'WED';

    if (!shouldMergeLines(wednesdayHours, thursdayHours)) {
        storeHoursList.push(buildHoursDisplayInfo(periodStart, periodEnd, buildStoreHoursString(wednesdayHours, resources), resources));

        periodStart = 'THU';
    }
    periodEnd = 'THU';

    if (!shouldMergeLines(thursdayHours, fridayHours)) {
        storeHoursList.push(buildHoursDisplayInfo(periodStart, periodEnd, buildStoreHoursString(thursdayHours, resources), resources));

        periodStart = 'FRI';
    }
    periodEnd = 'FRI';

    if (!shouldMergeLines(fridayHours, saturdayHours)) {
        storeHoursList.push(buildHoursDisplayInfo(periodStart, periodEnd, buildStoreHoursString(fridayHours, resources), resources));

        periodStart = 'SAT';
    }
    periodEnd = 'SAT';

    if (!shouldMergeLines(saturdayHours, sundayHours)) {
        storeHoursList.push(buildHoursDisplayInfo(periodStart, periodEnd, buildStoreHoursString(saturdayHours, resources), resources));

        periodStart = 'SUN';
    }
    periodEnd = 'SUN';

    storeHoursList.push(buildHoursDisplayInfo(periodStart, periodEnd, buildStoreHoursString(sundayHours, resources), resources));

    return storeHoursList;
};
