/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as React from 'react';
import { service, factories, models, IEmbedConfiguration } from 'powerbi-client';
import { UserAgentApplication, AuthError, AuthResponse } from 'msal';

const powerbi = new service.Service(factories.hpmFactory, factories.wpmpFactory, factories.routerFactory);

let accessToken = '';
let embedUrl = '';
let reportContainer: HTMLElement;
let reportRef: React.Ref<HTMLDivElement>;
let loading: JSX.Element;

export interface IPowerBiAuthProps {
    scope: string[];
    clientId: string;
    workspaceId: string;
    reportId: string;
}
interface IPowerBiState {
    accessToken: string;
    embedUrl: string;
    error: string[];
}

/**
 *
 * The PowerBiComponent component.
 * @extends {React.PureComponent<IPowerBiProps, IPowerBiState>}
 */
export default class PowerBiAuthComponent extends React.PureComponent<IPowerBiAuthProps, IPowerBiState> {
    constructor(props: IPowerBiAuthProps) {
        super(props);

        this.state = { accessToken: '', embedUrl: '', error: [] };

        reportRef = React.createRef();

        // Report container
        loading = (
            <div id='reportContainer' ref={reportRef}>
                Loading the report...
            </div>
        );
    }

    /**
     * Get Request header
     * @return Request header with Bearer token
     */
    public async getRequestHeader() {
        return {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${accessToken}`
        };
    }

    public render(): JSX.Element {
        if (this.state.error.length) {
            // Cleaning the report container contents and rendering the error message in multiple lines
            reportContainer.textContent = '';
            this.state.error.forEach(line => {
                reportContainer.appendChild(document.createTextNode(line));
                reportContainer.appendChild(document.createElement('br'));
            });
        } else if (this.state.accessToken !== '' && this.state.embedUrl !== '') {
            const embedConfiguration: IEmbedConfiguration = {
                type: 'report',
                tokenType: models.TokenType.Aad,
                accessToken,
                embedUrl,
                id: this.props.reportId
                /*
                // Enable this setting to remove gray shoulders from embedded report
                settings: {
                    background: models.BackgroundType.Transparent
                }
                */
            };

            const report = powerbi.embed(reportContainer, embedConfiguration);

            // Clear any other loaded handler events
            report.off('loaded');

            // Triggers when a content schema is successfully loaded
            report.on('loaded', function() {
                console.log('Report load successful');
            });

            // Clear any other rendered handler events
            report.off('rendered');

            // Triggers when a content is successfully embedded in UI
            report.on('rendered', function() {
                console.log('Report render successful');
            });

            // Clear any other error handler event
            report.off('error');

            // Below patch of code is for handling errors that occur during embedding
            report.on('error', function(event) {
                const errorMsg = event.detail;

                // Use errorMsg variable to log error in any destination of choice
                console.error(errorMsg);
            });
        }

        return loading;
    }

    // React function
    public async componentDidMount(): Promise<void> {
        if (reportRef !== null) {
            reportContainer = reportRef['current'];
        }

        // User input - null check
        if (this.props.workspaceId === '' || this.props.reportId === '') {
            this.setState({ error: ['Please assign values to workspace Id and report Id in Config.ts file'] });
        } else {
            // Authenticate the user and generate the access token
            this.authenticate();
        }
    }

    // React function
    public componentWillUnmount(): void {
        powerbi.reset(reportContainer);
    }

    // Authenticating to get the access token
    public authenticate(): void {
        // eslint-disable-next-line @typescript-eslint/no-this-alias
        const thisObj = this;

        const msalConfig = {
            auth: {
                clientId: this.props.clientId
                // scopes: [ 'openid', 'profile', 'user.read' ]
            }
        };

        const loginRequest = {
            scopes: this.props.scope
        };

        const msalInstance: UserAgentApplication = new UserAgentApplication(msalConfig);

        function successCallback(response: AuthResponse): void {
            if (response.tokenType === 'id_token') {
                thisObj.authenticate();
            } else if (response.tokenType === 'access_token') {
                accessToken = response.accessToken;
                thisObj.setUsername(response.account.name);

                // Refresh User Permissions
                thisObj.tryRefreshUserPermissions();
                thisObj.getembedUrl();
            } else {
                thisObj.setState({ error: [`Token type is: ${response.tokenType}`] });
            }
        }

        function failCallBack(error: AuthError): void {
            thisObj.setState({ error: [`Redirect error: ${error}`] });
        }

        msalInstance.handleRedirectCallback(successCallback, failCallBack);

        // check if there is a cached user
        if (msalInstance.getAccount()) {
            // get access token silently from cached id-token
            msalInstance
                .acquireTokenPopup(loginRequest)
                .then((response: AuthResponse) => {
                    // get access token from response: response.accessToken
                    accessToken = response.accessToken;
                    this.setUsername(response.account.name);
                    this.getembedUrl();
                })
                .catch((err: AuthError) => {
                    // refresh access token silently from cached id-token
                    // makes the call to handleredirectcallback
                    if (err.name === 'InteractionRequiredAuthError') {
                        msalInstance.acquireTokenRedirect(loginRequest);
                    } else {
                        thisObj.setState({ error: [err.toString()] });
                    }
                });
        } else {
            // user is not logged in or cached, you will need to log them in to acquire a token
            msalInstance.loginRedirect(loginRequest);
        }
    }

    public getembedUrl(): void {
        // eslint-disable-next-line @typescript-eslint/no-this-alias
        const thisObj: this = this;

        fetch(`https://api.powerbi.com/v1.0/myorg/groups/${this.props.workspaceId}/reports/${this.props.reportId}`, {
            headers: {
                Authorization: `Bearer ${accessToken}`
            },
            method: 'GET'
        })
            .then(function(response) {
                const errorMessage: string[] = [];
                errorMessage.push('Error occurred while fetching the embed URL of the report');
                errorMessage.push(`Request Id: ${response.headers.get('requestId')}`);

                response
                    .json()
                    .then(function(body) {
                        // Successful response
                        if (response.ok) {
                            embedUrl = body['embedUrl'];
                            thisObj.setState({ accessToken: accessToken, embedUrl: embedUrl });
                            // eslint-disable-next-line brace-style
                        }
                        // If error message is available
                        else {
                            errorMessage.push(`Error ${response.status}: ${body.error.code}`);

                            thisObj.setState({ error: errorMessage });
                        }
                    })
                    .catch(function() {
                        errorMessage.push(`Error ${response.status}:  An error has occurred`);

                        thisObj.setState({ error: errorMessage });
                    });
            })
            .catch(function(error) {
                // Error in making the API call
                thisObj.setState({ error: error });
            });
    }

    // Power BI REST API call to refresh User Permissions in Power BI
    // Refreshes user permissions and makes sure the user permissions are fully updated
    // https://docs.microsoft.com/rest/api/power-bi/users/refreshuserpermissions
    public tryRefreshUserPermissions(): void {
        fetch('https://api.powerbi.com/v1.0/myorg/RefreshUserPermissions', {
            headers: {
                Authorization: `Bearer ${accessToken}`
            },
            method: 'POST'
        })
            .then(function(response) {
                if (response.ok) {
                    console.log('User permissions refreshed successfully.');
                } else {
                    // Too many requests in one hour will cause the API to fail
                    if (response.status === 429) {
                        console.error('Permissions refresh will be available in up to an hour.');
                    } else {
                        console.error(response);
                    }
                }
            })
            .catch(function(error) {
                console.error(`Failure in making API call.${error}`);
            });
    }

    // Show username in the UI
    public setUsername(username: string): void {
        const welcome = document.getElementById('welcome');
        if (welcome !== null) {
            welcome.innerText = `Welcome, ${username}`;
        }
    }
}
