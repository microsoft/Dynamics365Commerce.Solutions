/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

/* eslint-disable class-methods-use-this -- This class is implemented for testing purpose only */
/* eslint-disable @typescript-eslint/no-empty-function -- test file. */
/* eslint-disable no-duplicate-imports */
import { IDictionary, ITelemetry } from '@msdyn365-commerce/core';

/**
 *
 * MockTelemetry class.
 * @implements {ITelemetry}
 */
class MockTelemetry implements ITelemetry {
    // eslint-disable-next-line @typescript-eslint/naming-convention -- ignore the name.
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

    // eslint-disable-next-line @typescript-eslint/no-unused-vars, @typescript-eslint/no-unused-vars-experimental, @typescript-eslint/ban-types -- for testing.
    public setTelemetryAttribute(telemetryId: string, additionalTelemetryData?: object | undefined): IDictionary<string> {
        return {
            key: 'key',
            value: 'value'
        };
    }
}

export default MockTelemetry;
/* eslint-enable @typescript-eslint/no-empty-function -- test file. */
/* eslint-enable class-methods-use-this -- This class is implemented for testing purpose only */
