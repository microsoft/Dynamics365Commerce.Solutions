/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

const mockStates = [
    {
        CountryRegionId: 'USA',
        StateId: 'AK',
        StateName: 'Alaska',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'AL',
        StateName: 'Alabama',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'AR',
        StateName: 'Arkansas',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'AS',
        StateName: 'American Samoa',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'AZ',
        StateName: 'Arizona',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'CA',
        StateName: 'California',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'CO',
        StateName: 'Colorado',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'CT',
        StateName: 'Connecticut',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'DC',
        StateName: 'District of Columbia',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'DE',
        StateName: 'Delaware',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'FL',
        StateName: 'Florida',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'FM',
        StateName: 'Federated States of Micronesia',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'GA',
        StateName: 'Georgia',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'GU',
        StateName: 'Guam',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'HI',
        StateName: 'Hawaii',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'IA',
        StateName: 'Iowa',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'ID',
        StateName: 'Idaho',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'IL',
        StateName: 'Illinois',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'IN',
        StateName: 'Indiana',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'KS',
        StateName: 'Kansas',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'KY',
        StateName: 'Kentucky',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'LA',
        StateName: 'Louisiana',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'MA',
        StateName: 'Massachusetts',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'MD',
        StateName: 'Maryland',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'ME',
        StateName: 'Maine',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'MH',
        StateName: 'Marshall Islands',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'MI',
        StateName: 'Michigan',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'MN',
        StateName: 'Minnesota',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'MO',
        StateName: 'Missouri',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'MP',
        StateName: 'Northern Mariana Islands',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'MS',
        StateName: 'Mississippi',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'MT',
        StateName: 'Montana',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'NC',
        StateName: 'North Carolina',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'ND',
        StateName: 'North Dakota',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'NE',
        StateName: 'Nebraska',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'NH',
        StateName: 'New Hampshire',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'NJ',
        StateName: 'New Jersey',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'NM',
        StateName: 'New Mexico',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'NV',
        StateName: 'Nevada',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'NY',
        StateName: 'New York',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'OH',
        StateName: 'Ohio',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'OK',
        StateName: 'Oklahoma',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'OR',
        StateName: 'Oregon',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'PA',
        StateName: 'Pennsylvania',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'PR',
        StateName: 'Puerto Rico',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'PW',
        StateName: 'Palau',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'RI',
        StateName: 'Rhode Island',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'SC',
        StateName: 'South Carolina',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'SD',
        StateName: 'South Dakota',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'TN',
        StateName: 'Tennesee',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'TX',
        StateName: 'Texas',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'UT',
        StateName: 'Utah',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'VA',
        StateName: 'Virginia',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'VI',
        StateName: 'Virgin Islands',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'VT',
        StateName: 'Vermont',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'WA',
        StateName: 'Washington',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'WI',
        StateName: 'Wisconsin',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'WV',
        StateName: 'West Virginia',
        ExtensionProperties: []
    },
    {
        CountryRegionId: 'USA',
        StateId: 'WY',
        StateName: 'Wyoming',
        ExtensionProperties: []
    }
];

export default mockStates;
