/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/

import { IDictionary, ITelemetry } from '@msdyn365-commerce/core';

/**
 *
 * MockTelemetry class.
 * @implements {ITelemetry}
 */
class MockTelemetry implements ITelemetry {
    public log(): void {}

    public trace(): void {}

    public debug(): void {}

    public warning(): void {}

    public critical(): void {}

    public exception(): void {}

    public logEvent(): void {}

    public trackMetric(): void {}

    public trackEvent(): void {}

    public setTelemetryRequestContext(): void {}

    public setTelemetryModuleContext(): () => ITelemetry {
        return () => this;
    }

    public trackDependency(): void {}

    public information(): void {}

    public error(): void {}

    public setTelemetryAttribute(telemetryId: string, additionalTelemetryData?: object | undefined): IDictionary<string> {
        return {
            key: 'key',
            value: 'value'
        };
    }
}

export default MockTelemetry;
