/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */
namespace Contoso
{
    namespace CommerceRuntime.DocumentProvider.DataModelEFR.Constants
    {
        /// <summary>
        /// The registration response result codes.
        /// </summary>
        /// <remark>
        /// This is defined by a third-party library, or by the tax authority, etc.
        /// </remark>
        public static class RegistrationResultCodeConstants
        {
            /// <summary>
            /// Http status 406 Not Acceptable.
            /// </summary>
            /// <remark>
            /// Could be retried.
            /// </remark>
            public const string CouldNotProcessTransaction = "NO";

            /// <summary>
            /// Http status 400 Bad Request.
            /// </summary>
            /// <remark>
            /// Terminate transaction.
            /// </remark>
            public const string InvalidRequestData = "BAD";

            /// <summary>
            /// Http status 200 OK.
            /// </summary>
            /// <remark>
            /// Print receipt.
            /// </remark>
            public const string TransactionProcessedSuccessfully = "OK";
        }
    }
}
