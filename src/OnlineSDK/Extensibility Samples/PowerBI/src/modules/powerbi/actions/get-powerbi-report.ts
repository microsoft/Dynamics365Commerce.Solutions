/*--------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * See License.txt in the project root for license information.
 *--------------------------------------------------------------*/
import { createObservableDataAction, IAction, IActionContext, ICreateActionContext } from '@msdyn365-commerce/core';
import { CacheType, IActionInput } from '@msdyn365-commerce/retail-proxy';
import { getPowerBIReportByReportIdAndGroupIdAsync } from '../../../themes/fabrikam-extended/data-actions/DataActionExtension.g';
import { IPowerBi } from '../../../themes/fabrikam-extended/data-actions/DataServiceEntities.g';

export class GetPowerBiReportInput implements IActionInput {
    public shouldCacheOutput = () => true;

    public getCacheKey = () => 'GetPowerBiReport';

    public getCacheObjectType = () => 'GetPowerBiReport';

    public dataCacheType = (): CacheType => 'none';

    public workspaceId?: string;

    public reportId?: string;

    public roleName?: string;

    public reportView?: string;
}

const createInput = (args: ICreateActionContext<{ workspaceId: string; reportId: string; role: string; reportView: string }>) => {
    const input = new GetPowerBiReportInput();

    input.workspaceId = args.config?.workspaceId;

    input.reportId = args.config?.reportId;

    input.roleName = args.config?.role;

    input.reportView = args.config?.reportView;

    return input;
};
/**
 * Get Power BI embed URL and token.
 * @param ctx Action context.
 */
async function action(input: GetPowerBiReportInput, ctx: IActionContext): Promise<IPowerBi> {
    if (input.reportView === 'single') {
        if (input.workspaceId && input.reportId && input.roleName) {
            return getPowerBIReportByReportIdAndGroupIdAsync(
                { callerContext: ctx },
                input.workspaceId,
                input.reportId,
                input.roleName
            ).then((result: IPowerBi) => {
                if (!result) {
                    ctx.trace('[getPowerBiReportAction] returned no data');
                    return {};
                }

                return result;
            });
        }
    }

    return {};
}

export const actionDataAction = createObservableDataAction({
    id: '@msdyn365-commerce-modules/powerbi/get-powerbi-report',
    action: <IAction<IPowerBi>>action,
    input: createInput
});

export default actionDataAction;
