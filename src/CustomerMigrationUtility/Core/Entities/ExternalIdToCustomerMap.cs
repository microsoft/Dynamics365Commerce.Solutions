// ----------------------------------------------------------------------------------------------------
// <copyright file="ExternalIdToCustomerMap.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------

namespace Microsoft.Dynamics.Commerce.CustomerMigrationUtility.Core
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a ExternalIdToCustomerMap.
    /// </summary>
    [JsonObject]
    public class ExternalIdToCustomerMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalIdToCustomerMap" /> class.
        /// </summary>
        public ExternalIdToCustomerMap()
        {
            this.ODataType = "#Microsoft.Dynamics.DataEntities.ExternalIdToCustomerMap";
            this.IsActivatedOdataType = "#Microsoft.Dynamics.DataEntities.NoYes";
            this.IsActivated = "Yes";
        }

        /// <summary>
        ///     Gets or sets OData type for this entity.
        /// </summary>
        [JsonProperty(PropertyName = "@odata.type")]
        public string ODataType { get; set; }

        /// <summary>
        /// Gets or Sets the CustomerAccountNumber.
        /// </summary>
        [JsonProperty("CustomerAccountNumber")]
        public string CustomerAccountNumber { get; set; }

        /// <summary>
        /// Gets or Sets the DataAreaId.
        /// </summary>
        [JsonProperty("dataAreaId")]
        public string DataAreaId { get; set; }

        /// <summary>
        /// Gets or Sets the ExternalIdentityId.
        /// </summary>
        [JsonProperty("ExternalIdentityId")]
        public string ExternalIdentityId { get; set; }

        /// <summary>
        /// Gets or Sets the IsActivatedOdataType.
        /// </summary>
        [JsonProperty("IsActivated@odata.type")]
        public string IsActivatedOdataType { get; set; }

        /// <summary>
        /// Gets or Sets the IsActivated.
        /// </summary>
        [JsonProperty("IsActivated")]
        public string IsActivated { get; set; }

        /// <summary>
        /// Gets or Sets the ProviderId.
        /// </summary>
        [JsonProperty("ProviderId")]
        public long ProviderId { get; set; }
    }
}
