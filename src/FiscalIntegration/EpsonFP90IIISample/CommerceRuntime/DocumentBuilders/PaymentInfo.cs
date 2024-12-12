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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample.DocumentBuilders
    {
        /// <summary>
        /// Represents a class for a payment information for fiscal printer.
        /// </summary>
        public class PaymentInfo
        {
            /// <summary>
            /// Description of a payment information for fiscal printer.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Payment type of a payment information for fiscal printer.
            /// </summary>
            public string PaymentType { get; set; }

            /// <summary>
            /// Payment index of a payment information for fiscal printer.
            /// </summary>
            public string PaymentIndex { get; set; }

            /// <summary>
            /// Constructor for PaymentInfo class.
            /// </summary>
            /// <param name="description">Description of a payment information for fiscal printer.</param>
            /// <param name="paymentType">Payment type of a payment information for fiscal printer.</param>
            /// <param name="paymentIndex">Payment index of a payment information for fiscal printer.</param>
            public PaymentInfo(string description, string paymentType, string paymentIndex)
            {
                Description = description;
                PaymentType = paymentType;
                PaymentIndex = paymentIndex;
            }
        }
    }
}
