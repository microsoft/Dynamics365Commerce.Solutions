namespace Microsoft.Dynamics.Commerce.CustomerMigrationUtility.Core
{
    /// <summary>
    /// The enum of implemented display catalog APIs.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// Account is created.
        /// </summary>
        Success,

        /// <summary>
        /// Account is already exist.
        /// </summary>
        AlreadyExist,

        /// <summary>
        /// Skipped.
        /// </summary>
        Skipped,

        /// <summary>
        /// Exception occured.
        /// </summary>
        Failed,

        /// <summary>
        /// Not found.
        /// </summary>
        NotFound,

        /// <summary>
        /// Disabled.
        /// </summary>
        Disabled
    }
}
