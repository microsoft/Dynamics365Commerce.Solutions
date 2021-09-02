/**
* SAMPLE CODE NOTICE
* 
* THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
* OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
* THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
* NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;

namespace Contoso
{
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample
    {
        /// <summary>
        /// Represents a key for department mapping.
        /// </summary>
        public class ExtendedVATRate
        {
            /// <summary>
            /// VAT rate attribute mapping value.
            /// </summary>
            private int VATRate { get; }

            /// <summary>
            /// Product type attribute mapping value.
            /// </summary>
            private string ProductType { get; }

            /// <summary>
            /// VAT exempt nature attribute mapping value.
            /// </summary>
            private string VATExemptNature { get; }

            /// <summary>
            /// Constructor for ExtendedVATRate class.
            /// </summary>
            public ExtendedVATRate(int vatRate, string vatExemptNature, string productType)
            {
                VATRate = vatRate;
                VATExemptNature = vatExemptNature;
                ProductType = productType;
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <param name="o">An object to compare with this object.</param>
            /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>.</returns>
            public override bool Equals(object o)
            {
                ExtendedVATRate other = o as ExtendedVATRate;
                if (other == null)
                {
                    return false;
                }

                return this.VATRate == other.VATRate
                       && string.Equals(this.ProductType, other.ProductType, StringComparison.OrdinalIgnoreCase)
                       && string.Equals(this.VATExemptNature, other.VATExemptNature, StringComparison.OrdinalIgnoreCase);
            }

            /// <summary>
            /// Serves as a hash function for a particular type.
            /// </summary>
            /// <returns>A hash code for the current object.</returns>
            public override int GetHashCode()
            {
                int hashCode = 0;
                hashCode ^= string.IsNullOrWhiteSpace(this.ProductType) ? 0 : this.ProductType.GetHashCode();
                hashCode ^= string.IsNullOrWhiteSpace(this.VATExemptNature) ? 0 : this.VATExemptNature.GetHashCode();
                hashCode ^= string.IsNullOrWhiteSpace(this.ProductType) ? 0 : this.ProductType.GetHashCode();

                return hashCode;
            }
        }
    }
}