/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { buildMockModuleProps } from '@msdyn365-commerce/core-internal';
/// <reference types="jest" />

import * as React from 'react';
import * as renderer from 'react-test-renderer';

import CookieTest from '../cookie-test';
import { ICookieTestData } from '../cookie-test.data';
import { ICookieTestConfig, ICookieTestProps } from '../cookie-test.props.autogenerated';

const mockData: ICookieTestData = {
    actionResponse: {
        text: 'Sample Response Data'
    }
};

const mockConfig: ICookieTestConfig = {
    showText: 'cookie-test'
};

const mockActions = {};

describe('CookieTest', () => {
    let moduleProps: ICookieTestProps<ICookieTestData>;
    beforeAll(() => {
        moduleProps = buildMockModuleProps(mockData, mockActions, mockConfig) as ICookieTestProps<ICookieTestData>;
    });
    it('renders correctly', () => {
        const component: renderer.ReactTestRenderer = renderer.create(<CookieTest {...moduleProps} />);
        const tree: renderer.ReactTestRendererJSON | renderer.ReactTestRendererJSON[] | null = component.toJSON();
        expect(tree).toMatchSnapshot();
    });
});
