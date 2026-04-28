namespace Microsoft.Dynamics.Commerce.CustomerMigrationUtility.Core
{
    /// <summary>
    /// The response model.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="status">The status</param>
        /// <param name="userId">The user id</param>      
        public Response(Status status, string userId = null)
        {
            this.Status = status;
            this.UserId = userId;
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Gets the userId.
        /// </summary>
        public string UserId { get; set; }
    }
}