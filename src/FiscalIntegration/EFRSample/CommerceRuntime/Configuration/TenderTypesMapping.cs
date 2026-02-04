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
    namespace CommerceRuntime.DocumentProvider.EFRSample
    {
        using System.Collections.Generic;

        /// <summary>
        /// Incapsulates tender type to payment type group mapping.
        /// </summary>
        internal class TenderTypeMapping
        {
            /// <summary>
            /// Mapping tender type to payment type group.
            /// </summary>
            private Dictionary<string, string> mapping;

            /// <summary>
            /// Initializes a new instance of the <see cref="TenderTypeMapping"/> class.
            /// </summary>
            /// <param name="mapping">The tender type mapping.</param>
            public TenderTypeMapping(Dictionary<string, string> mapping)
            {
                this.mapping = mapping;
            }

            /// <summary>
            /// Gets payment type group by tender type.
            /// </summary>
            /// <param name="tenderType">The tender type.</param>
            /// <returns>The payment type group.</returns>
            public string this[string tenderType]
            {
                get
                {
                    string paymentTypeGroup = string.Empty;

                    if (mapping != null)
                    {
                        mapping.TryGetValue(tenderType, out paymentTypeGroup);
                    }

                    return paymentTypeGroup;
                }
            }
        }
    }
}