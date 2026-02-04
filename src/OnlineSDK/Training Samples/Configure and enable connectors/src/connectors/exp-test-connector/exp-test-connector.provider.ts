/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable @typescript-eslint/no-explicit-any, max-lines */
import { IExperimentationProvider, IExperimentsResult, ISecretManager, IVariants, State } from '@msdyn365-commerce/core';
/**
 * A basic implementation of the ExperimentationProvider interface used for testing
 */
class ExpTestConnector implements IExperimentationProvider {
    private variantSet1: IVariants[] = [
        {
            variantId: 'var1',
            experimentId: 'expid1'
        },
        {
            variantId: 'var1',
            experimentId: 'expid4'
        },
        {
            variantId: 'var1',
            experimentId: 'expid6'
        },
        {
            variantId: 'var1',
            experimentId: 'expid7'
        },
        {
            variantId: 'var1',
            experimentId: 'expid8'
        },
        {
            variantId: 'var1',
            experimentId: 'expid9'
        },
        {
            variantId: 'var1',
            experimentId: 'expid10'
        },
        {
            variantId: 'var1',
            experimentId: 'expid11'
        },
        {
            variantId: 'var1',
            experimentId: 'expid12'
        }
    ];

    private variantSet2: IVariants[] = [
        {
            variantId: 'var2',
            experimentId: 'expid1'
        },
        {
            variantId: 'var2',
            experimentId: 'expid5'
        },
        {
            variantId: 'var2',
            experimentId: 'expid7'
        },
        {
            variantId: 'var2',
            experimentId: 'expid8'
        },
        {
            variantId: 'var2',
            experimentId: 'expid9'
        },
        {
            variantId: 'var2',
            experimentId: 'expid10'
        },
        {
            variantId: 'var2',
            experimentId: 'expid11'
        },
        {
            variantId: 'var2',
            experimentId: 'expid12'
        }
    ];

    private variantSet3: IVariants[] = [
        {
            variantId: 'var1',
            experimentId: 'expid5'
        },
        {
            variantId: 'var2',
            experimentId: 'expid13'
        },
        {
            variantId: 'var2',
            experimentId: 'expid14'
        },
        {
            variantId: 'var2',
            experimentId: 'expid15'
        },
        {
            variantId: 'var2',
            experimentId: 'expid16'
        },
        {
            variantId: 'var2',
            experimentId: 'expid17'
        },
        {
            variantId: 'var2',
            experimentId: 'expid18'
        },
        {
            variantId: 'var2',
            experimentId: 'expid19'
        },
        {
            variantId: 'var2',
            experimentId: 'expid20'
        },
        {
            variantId: 'var2',
            experimentId: 'expid21'
        },
        {
            variantId: 'var2',
            experimentId: 'expid22'
        },
        {
            variantId: 'var2',
            experimentId: 'expid23'
        },
        {
            variantId: 'var2',
            experimentId: 'expid24'
        },
        {
            variantId: 'var2',
            experimentId: 'expid25'
        }
    ];

    private variantSet4: IVariants[] = [
        {
            variantId: 'var1',
            experimentId: 'expid5'
        },
        {
            variantId: 'var1',
            experimentId: 'expid1'
        },
        {
            variantId: 'var1',
            experimentId: 'expid13'
        },
        {
            variantId: 'var1',
            experimentId: 'expid14'
        },
        {
            variantId: 'var1',
            experimentId: 'expid15'
        },
        {
            variantId: 'var1',
            experimentId: 'expid16'
        },
        {
            variantId: 'var1',
            experimentId: 'expid17'
        },
        {
            variantId: 'var1',
            experimentId: 'expid18'
        },
        {
            variantId: 'var1',
            experimentId: 'expid19'
        },
        {
            variantId: 'var1',
            experimentId: 'expid20'
        },
        {
            variantId: 'var1',
            experimentId: 'expid21'
        },
        {
            variantId: 'var1',
            experimentId: 'expid22'
        },
        {
            variantId: 'var1',
            experimentId: 'expid23'
        },
        {
            variantId: 'var1',
            experimentId: 'expid24'
        },
        {
            variantId: 'var1',
            experimentId: 'expid25'
        }
    ];

    public initialize(config: any, secretManager?: ISecretManager): Promise<boolean> {
        console.log(`exp-test-connector called with config: ${JSON.stringify(config)}`);
        return Promise.resolve(true);
    }

    public getConfigForClientSideInit(): Promise<any> {
        return Promise.resolve({});
    }
    public initializeClientSide(config: any): boolean {
        console.log(`Initialize client side called on exp-test-connector with config ${config}`);
        return true;
    }

    public getExperiments(): Promise<IExperimentsResult> {
        return Promise.resolve({
            name: 'exp-test-connector',
            experiments: [
                {
                    id: 'expid1',
                    friendlyName: 'experiment-1',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    link: 'www.exp-test-connector/expid1',
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var3',
                            friendlyName: 'variant-3',
                            status: State.Draft,
                            weight: '0.2'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid2',
                    friendlyName: 'experiment-2',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Paused,
                    link: 'www.exp-test-connector/expid2',
                    variations: [
                        {
                            id: 'var3',
                            friendlyName: 'variant-3',
                            status: State.Paused,
                            weight: '1.0'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid3',
                    friendlyName: 'experiment-3',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Archived,
                    variations: [
                        {
                            id: 'var4',
                            friendlyName: 'variant-4',
                            status: State.Archived,
                            weight: '1.0'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid4',
                    friendlyName: 'experiment-4',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid5',
                    friendlyName: 'experiment-5',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid6',
                    friendlyName: 'experiment-6',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid7',
                    friendlyName: 'experiment-7',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid8',
                    friendlyName: 'experiment-8',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid9',
                    friendlyName: 'experiment-9',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid10',
                    friendlyName: 'experiment-10',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid11',
                    friendlyName: 'experiment-11',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid12',
                    friendlyName: 'experiment-12',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid13',
                    friendlyName: 'experiment-13',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid14',
                    friendlyName: 'experiment-14',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid15',
                    friendlyName: 'experiment-15',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid16',
                    friendlyName: 'experiment-16',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid17',
                    friendlyName: 'experiment-17',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid18',
                    friendlyName: 'experiment-18',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid19',
                    friendlyName: 'experiment-19',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid20',
                    friendlyName: 'experiment-20',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid21',
                    friendlyName: 'experiment-21',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid22',
                    friendlyName: 'experiment-22',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid23',
                    friendlyName: 'experiment-23',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid24',
                    friendlyName: 'experiment-24',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                },
                {
                    id: 'expid25',
                    friendlyName: 'experiment-25',
                    description: 'This is a exp-test-connector experiment',
                    type: 'A/B Test',
                    status: State.Running,
                    variations: [
                        {
                            id: 'var1',
                            friendlyName: 'variant-1',
                            status: State.Active,
                            weight: '0.4'
                        },
                        {
                            id: 'var2',
                            friendlyName: 'variant-2',
                            status: State.Active,
                            weight: '0.6'
                        }
                    ],
                    createdDate: '05/01/2020',
                    lastModifiedDate: '05/01/2020',
                    lastModifiedBy: 'User 1'
                }
            ]
        });
    }
    public getVariantsForUser(userId: string, attributes?: { [index: string]: string } | undefined): IVariants[] {
        if (userId.match(/^[0-3].*$/)) {
            return this.variantSet1;
        } else if (userId.match(/^[4-7].*$/)) {
            return this.variantSet2;
        } else if (userId.match(/^[8-9|a-b].*$/)) {
            return this.variantSet3;
        } else {
            return this.variantSet4;
        }
    }
    public activateExperiment(userId: string, experiments: IVariants[], attributes?: { [index: string]: string } | undefined): boolean {
        console.log(
            `Active experiment called on exp-test-connector with userId: ${userId}, experiments: ${JSON.stringify(
                experiments
            )}, attributes: ${JSON.stringify(attributes)}`
        );
        return true;
    }
}

const connector = new ExpTestConnector();
export default connector;
