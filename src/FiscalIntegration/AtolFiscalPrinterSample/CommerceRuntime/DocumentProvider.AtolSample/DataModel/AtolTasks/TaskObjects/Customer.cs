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
    namespace CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask
    {
        using System.Runtime.Serialization;

        /// <summary>
        /// Represents a customer.
        /// </summary>
        [DataContract]
        public class Customer
        {
            /// <summary>
            /// Gets or sets customer name.
            /// </summary>
            [DataMember(Name = "name", EmitDefaultValue = false)]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets email or phone number of the customer.
            /// </summary>
            [DataMember(Name = "emailOrPhone", EmitDefaultValue = false)]
            public string EmailOrPhone { get; set; }

            /// <summary>
            /// Gets or sets VATIN of the customer.
            /// </summary>
            [DataMember(Name = "vatin", EmitDefaultValue = false)]
            public string Vatin { get; set; }
        }
    }
}