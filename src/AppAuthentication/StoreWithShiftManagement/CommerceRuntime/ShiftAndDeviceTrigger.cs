/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso.CommerceRuntime.Triggers
{
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Retail.Diagnostics.Extensions;
    using Microsoft.Dynamics.Retail.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.Core;
    using System.Runtime.Caching;

    /// <summary>
    /// In this sample trigger, it shows how to support application authentication call to a brick and mortar store
    /// a. Set a Device and its configuration correctly.
    /// b. Maintain a unique shift id per day according to channel time zone, e.g. 2023/12/20, the shift id is 20231220.
    /// Why this trigger is important since brick and mortar store always assume there is a device configuration and shift information.
    /// </summary>
    public class ChannelDataServiceRequestTrigger : IRequestTriggerAsync
    {
        private const string ManagerPrivilegies = "MANAGERPRIVILEGES";
        private const string ChannelIdVariableName = "@bi_ChannelId";
        private const string ChannelIdColumnName = "CHANNELID";
        private const string DeviceIdVariableName = "@bi_DeviceId";
        private const string DeviceIdColumnName = "DEVICEID";
        private const int DeviceAbsoluteCacheTimeInMinutes = 10;
        private static Lazy<Dictionary<string, Tuple<long, long, string>>> storeToDeviceMapping;
        private static readonly MemoryCache cacheForDeviceInformation = new MemoryCache(typeof(ChannelDataServiceRequestTrigger).Name);

        /// <summary>
        /// Diagnostics events for pipeline request handler.
        /// </summary>
        private enum Events
        {
            /// <summary>
            /// Event indicating that device configuration not found.
            /// </summary>
            DeviceConfigurationNotFound,

            /// <summary>
            /// Event indicating that device not found.
            /// </summary>
            DeviceNotFound,

            /// <summary>
            /// Event indicating that more than one shift is open.
            /// </summary>
            MoreThanOneShiftOpen,

            /// <summary>
            /// Event indicating that creating shift is failed.
            /// </summary>
            CreateShiftFailed,

            /// <summary>
            /// Event indicating that closing shift is failed.
            /// </summary>
            CloseShiftFailed,

            /// <summary>
            /// Event indicating that the 1st device information is found.
            /// </summary>
            DeviceInformation,

            /// <summary>
            /// Event indicating that for the shift information, either newly opened or closed.
            /// </summary>
            ShiftInformation,
        }

        /// <summary>
        /// Gets the collection of request types supported by this trigger.
        /// </summary>
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                // This trigger will be executed for all request types.
                // We need to do this since it impacts any crt call for application authentication one.
                return Enumerable.Empty<Type>();
            }
        }

        /// <summary>
        /// Pre trigger to initialize two things correctly:
        /// a. Set the device information for the commerce principal in the case of application authentication.
        /// b. Manage shift informationin the crt request context.
        /// </summary>
        /// <param name="request">The request.</param>
        public async Task OnExecuting(Microsoft.Dynamics.Commerce.Runtime.Messages.Request request)
        {
            ThrowIf.Null(request, nameof(request));
            Type requestType = request.GetType();
            var context = request.RequestContext;

            CommercePrincipal principal = GetCommercePrincipal(context);
            if (principal == null)
            {
                return;
            }

            // Given we don't want to assume the order of triggers, we handle it differently.
            if (context.IsInitialized)
            {
                // This one means this extension code run after another pipeline trigger runs.
                // Given this trigger will run for each crt call, that's fine it run 2nd time.
                if (principal.TerminalId == 0)
                {
                    await PopulateIdentityInformation(context, principal).ConfigureAwait(false);
                }
                else
                {
                    PopulateContextWithShiftInformation(context, principal);
                }
            }
            else
            {
                await PopulateIdentityInformation(context, principal).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// This one will set the principal with device information.
        /// </summary>
        /// <param name="context">The current crt request context.</param>
        /// <param name="principal">The crt principal which is application authenticated.</param>
        private static async Task PopulateIdentityInformation(RequestContext context, CommercePrincipal principal)
        {
            var device = await Get1stDeviceInformationWithCache(context, principal).ConfigureAwait(false);

            if (null != device)
            {
                var identity = principal.Identity as CommerceIdentity;

                // This need to be come from either Header or extension configuration file.
                identity.TerminalId = device.TerminalRecordId;

                // This will need to add a new data request to using the crt.DevicesView_V2 to get both information.
                identity.DeviceRecordId = device.RecordId;
                identity.DeviceNumber = device.DeviceNumber;

                // This is required  for close the shift.
                identity.Roles.Add(ManagerPrivilegies);
            }
            else
            {
                Log($"Cannot found the first device by channel recid {principal.ChannelId}", Events.DeviceNotFound);
            }
        }


        /// <summary>
        /// This is a cache wrapper for the method Get1stDeviceInformation.
        /// </summary>
        /// <param name="context">The current crt request context.</param>
        /// <param name="principal">The crt principal which is application authenticated.</param>
        /// <returns>The cache device information.</returns>
        private static Task<Device> Get1stDeviceInformationWithCache(RequestContext context, CommercePrincipal principal)
        {
            Lazy<Task<Device>> getDevice = new Lazy<Task<Device>>(() => Get1stDeviceInformation(context, principal), System.Threading.LazyThreadSafetyMode.PublicationOnly);
            var getValue = cacheForDeviceInformation.AddOrGetExisting(principal.ChannelId.ToString(), getDevice, new CacheItemPolicy() { AbsoluteExpiration = DateTime.UtcNow.AddMinutes(DeviceAbsoluteCacheTimeInMinutes) });
            if (getValue == null)
            {
                return getDevice.Value;
            }
            else
            {
                return ((Lazy<Task<Device>>)getValue).Value;
            }
        }

        /// <summary>
        /// Get the 1st avaliable device information for the current channel.
        /// With ext.DeviceNumberPrefix key specified in commerceRuntime.Ext.config, it can filter by the prefix name of the device number.
        /// </summary>
        /// <param name="context">The current crt request context.</param>
        /// <param name="principal">The crt principal which is application authenticated.</param>
        /// <returns><The device information./returns>
        private static async Task<Device> Get1stDeviceInformation(RequestContext context, CommercePrincipal principal)
        {
            Device device = null;

            var query = new SqlPagedQuery(QueryResultSettings.FirstRecord)
            {
                From = "DEVICESVIEW_V2",
                Where = $"{ChannelIdColumnName} = {ChannelIdVariableName}",
                OrderBy = "RECID ASC",
            };

            // This is an example instead of always pick the 1st device, it can be also filter with device number prefix.
            if (context.Runtime.Configuration.Settings.TryGetValue("ext.DeviceNumberPrefix", out var deviceNumberPrefix))
            {
                if (string.IsNullOrWhiteSpace(deviceNumberPrefix))
                {
                    query.Where += $" AND {DeviceIdColumnName} like {DeviceIdVariableName}";
                    query.Parameters[DeviceIdVariableName] = deviceNumberPrefix + "%";
                }
            }

            query.Parameters[ChannelIdVariableName] = principal.ChannelId;

            using (var databaseContext = new DatabaseContext(context))
            {
                var results = await databaseContext.ReadEntityAsync<Device>(query).ConfigureAwait(false);
                device = results.FirstOrDefault();
            }

            Log($"Found {principal.ChannelId} {device?.TerminalRecordId} {device?.RecordId} {device?.DeviceNumber}", Events.DeviceInformation);

            return device;
        }

        /// <summary>
        /// Populate the shift information to the commerce principal in need.
        /// </summary>
        /// <param name="context">The current crt request context.</param>
        /// <param name="principal">The crt principal which is application authenticated.</param>
        private static void PopulateContextWithShiftInformation(RequestContext context, CommercePrincipal principal)
        {
            if (NeedPopulateShift(context, principal))
            {
#pragma warning disable AvoidBlockingCallsAnalyzer // Avoid blocking asynchronous execution. 
                principal.Shift = new Lazy<Shift>(() => UpdateOrGetShiftV2(context).GetAwaiter().GetResult());
#pragma warning restore AvoidBlockingCallsAnalyzer // Avoid blocking asynchronous execution.
            }
        }

        /// <summary>
        /// Check if populate shift information can be done.
        /// </summary>
        /// <param name="context">The current crt request context.</param>
        /// <param name="principal">The crt principal which is application authenticated.</param>
        /// <returns>True if shift is not set AND deviceConfiguration is avaliable. False for otherwise.</returns>
        private static bool NeedPopulateShift(RequestContext context, CommercePrincipal principal)
        {
            bool isShiftPopulated = principal.Shift != null;
            return context.GetDeviceConfiguration() != null && !isShiftPopulated;
        }

        /// <summary>
        /// It will do the following:
        /// a. Create today channel date shift id if not exists
        /// b. Close previous channel date shift id if exists
        /// c. Return today's shift information
        /// </summary>
        /// <param name="context">The current crt request context.</param>
        /// <returns>The today channel date shift information in format 'yyyyMMdd'</returns>
        private static async Task<Shift> UpdateOrGetShiftV2(RequestContext context)
        {
            var channelDateTime = context.GetNowInChannelTimeZone();
            var todayShiftId = long.Parse(channelDateTime.ToString("yyyyMMdd"));
            var minDateShiftId = long.Parse(DateTimeOffset.MinValue.ToString("yyyyMMdd"));

            ShiftDataQueryCriteria criteria = new ShiftDataQueryCriteria
            {
                ChannelId = context.GetPrincipal().ChannelId,
                TerminalId = context.GetTerminal().TerminalId,
                Status = (int)ShiftStatus.Open,
                SearchByTerminalId = true,
                SearchByCurrentTerminalId = true,
            };

            criteria.IncludeSharedShifts = false;
            GetShiftDataDataRequest dataServiceRequest = new GetShiftDataDataRequest(criteria, QueryResultSettings.AllRecords);
            IReadOnlyCollection<Shift> shifts = (await context.Runtime.ExecuteAsync<EntityDataServiceResponse<Shift>>(dataServiceRequest, context, skipRequestTriggers: true).ConfigureAwait(false)).PagedEntityCollection.Results;
            var todayShift = shifts.FirstOrDefault(s => s.ShiftId == todayShiftId);

            // Create today shift if not exists
            if (todayShift == null)
            {
                try
                {
                    todayShift = await CreateShift(context, todayShiftId).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    // in case creation failed, check if it already created.
                    shifts = (await context.Runtime.ExecuteAsync<EntityDataServiceResponse<Shift>>(dataServiceRequest, context, skipRequestTriggers: true).ConfigureAwait(false)).PagedEntityCollection.Results;
                    todayShift = shifts.FirstOrDefault(s => s.ShiftId == todayShiftId);

                    if (todayShift == null)
                    {
                        RetailLogger.Log.LogWarning(
                            Events.CreateShiftFailed,
                            "Create shift is failed for the channelId {channelId}, exception: {e}",
                            criteria.ChannelId.AsSystemMetadata(), e.AsSystemMetadata());
                        throw;
                    }
                }
            }

            try
            {
                // Find out past opened shift id to close.
                foreach (var pastShift in shifts.Where(s => s.ShiftId < todayShiftId && s.ShiftId > minDateShiftId))
                {
                    CommercePrincipal principal = context.GetPrincipal() as CommercePrincipal;

                    var backupValue = principal.Shift;

                    // Need to assign yesterday shift to current principal first for further call on closing it.
                    principal.Shift = new Lazy<Shift>(() => pastShift);

                    try
                    {
                        await CloseShift(context, pastShift.ShiftId).ConfigureAwait(false);
                    }
                    finally
                    {
                        // Reset it back.
                        principal.Shift = backupValue;
                    }
                }
            }
            catch (Exception e)
            {
                RetailLogger.Log.LogWarning(
                    Events.CloseShiftFailed,
                    "Close shift is failed for the channelId {channelId}, exception: {e}",
                    criteria.ChannelId.AsSystemMetadata(), e.AsSystemMetadata());
            }

            if (shifts.Count > 1)
            {
                var shiftsIdsStr = string.Join(",", shifts.Select(x => x.ShiftId));
                RetailLogger.Log.LogInformation(
                    Events.MoreThanOneShiftOpen,
                    "More than one shift is open for the channelId {channelId}, shiftstIds {shiftsIds}",
                    criteria.ChannelId.AsSystemMetadata(), shiftsIdsStr.AsSystemMetadata());
            }

            return todayShift;
        }

        /// <summary>
        /// Close shift based on the shiftid.
        /// </summary>
        /// <param name="context">The current crt request context.</param>
        /// <param name="shiftId">The shift id to be closed.</param>
        private static async Task CloseShift(RequestContext context, long shiftId)
        {
            var terminalId = context.GetTerminal().TerminalId;
            var status = ShiftStatus.Closed;
            var canForceClose = true;
            var request = new ChangeShiftStatusRequest
            {
                ShiftId = shiftId,
                ShiftTerminalId = terminalId,
                ToStatus = status,
                TransactionId = Guid.NewGuid().ToString(),
                CanForceClose = canForceClose,
                HasOfflinePendingTransactions = false,
            };

            RetailLogger.Log.CrtStoreOperationsOrderManagerUpdateShiftStatusStarted(shiftId, terminalId, status.ToString(), canForceClose.ToString());
            var response = await context.Runtime.ExecuteAsync<ChangeShiftStatusResponse>(request, context).ConfigureAwait(false);
            Shift closedShift = response.Shift;
            string shiftInfo = $"{closedShift?.ShiftId}:{closedShift?.TerminalId}:{closedShift?.Status}";
            RetailLogger.Log.CrtStoreOperationsOrderManagerUpdateShiftStatusSucceeded(shiftInfo);
            Log($"Close Shift {shiftInfo}", Events.ShiftInformation);
        }

        /// <summary>
        /// Create shift based on the shiftid.
        /// </summary>
        /// <param name="context">The current crt request context.</param>
        /// <param name="shiftId">The shift id to be created.</param>
        /// <returns>The newly created shift.</returns>
        private static async Task<Shift> CreateShift(RequestContext context, long shiftId)
        {
            var terminalId = context.GetTerminalId();

            var shift = new Shift
            {
                StartDateTime = context.GetNowInChannelTimeZone(),
                StatusDateTime = context.GetNowInChannelTimeZone(),
                StoreRecordId = context.GetPrincipal().ChannelId,
                ShiftId = shiftId,
                Status = ShiftStatus.Open,
                IsShared = false,
            };

            if (!string.IsNullOrWhiteSpace(context.GetOrgUnit()?.OrgUnitNumber))
            {
                shift.StoreId = context.GetOrgUnit()?.OrgUnitNumber;
            }

            if (!string.IsNullOrWhiteSpace(terminalId))
            {
                shift.TerminalId = terminalId;
                shift.CurrentTerminalId = terminalId;
            }

            var shiftRequest = new CreateShiftDataRequest(shift);
            await context.Runtime.ExecuteAsync<NullResponse>(shiftRequest, context).ConfigureAwait(false);
            Log($"Open Shift {shift?.ShiftId}", Events.ShiftInformation);
            return shift;
        }

        /// <summary>
        /// Get the crt principal in case it is application authenticated.
        /// </summary>
        /// <param name="context">The current crt request context.</param>
        /// <returns>The crt principal which is application authenticated. Return null for other cases.</returns>
        private static CommercePrincipal GetCommercePrincipal(RequestContext context)
        {
            ICommercePrincipal principal = context.GetPrincipal();
            if (principal != null && !principal.IsChannelAgnostic)
            {
                var commercePrincipal = principal as CommercePrincipal;
                if (commercePrincipal != null && commercePrincipal.IsApplication)
                {
                    return commercePrincipal;
                }
            }

            return null;
        }

        private static void Log(string input, Events eventName)
        {
            RetailLogger.Instance.LogInformation(eventName, input);
        }

        public Task OnExecuted(Microsoft.Dynamics.Commerce.Runtime.Messages.Request request, Response response)
        {
            return Task.CompletedTask;
        }
    }
}