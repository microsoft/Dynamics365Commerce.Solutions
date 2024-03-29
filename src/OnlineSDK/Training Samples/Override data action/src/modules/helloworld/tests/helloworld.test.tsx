/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import { buildMockModuleProps } from '@msdyn365-commerce/core-internal';
/// <reference types="jest" />

// tslint:disable-next-line:no-unused-variable
import * as React from 'react';
import * as renderer from 'react-test-renderer';

import Helloworld from '../helloworld';
import { IHelloworldData } from '../helloworld.data';
import { IHelloworldConfig, IHelloworldProps } from '../helloworld.props.autogenerated';

const mockData: IHelloworldData = {
    actionResponse: {
        text: 'Sample Response Data'
    }
};

const mockConfig: IHelloworldConfig = {
    title: 'helloworld'
};

const mockActions = {};

describe('Helloworld', () => {
    let moduleProps: IHelloworldProps<IHelloworldData>;
    beforeAll(() => {
        moduleProps = buildMockModuleProps(mockData, mockActions, mockConfig) as IHelloworldProps<IHelloworldData>;
    });
    it('renders correctly', () => {
        const component: renderer.ReactTestRenderer = renderer.create(<Helloworld {...moduleProps} />);
        const tree = component.toJSON();
        expect(tree).toMatchSnapshot();
    });
});
