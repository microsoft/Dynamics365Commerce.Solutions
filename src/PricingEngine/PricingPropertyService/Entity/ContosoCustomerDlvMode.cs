/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */
 
namespace Contoso.DataModel
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;

    /// <summary>
    /// Represents a customer's delivery mode.
    /// </summary>
    [DataContract]
    public class ContosoCustomerDlvMode : CommerceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContosoCustomerDlvMode"/> class.
        /// </summary>
        public ContosoCustomerDlvMode() : base("ContosoCustomerDlvMode")
        {
        }

        /// <summary>
        /// Gets or sets the record identifier.
        /// </summary>
        [Key]
        [DataMember]
        [Column("AccountNum")]
        public string RecordId
        {
            get { return (string)(this["AccountNum"] ?? string.Empty); }
            set { this["AccountNum"] = value; }
        }

        /// <summary>
        /// Gets or sets the delivery mode.
        /// </summary>
        [DataMember]
        [Column("DlvMode")]
        public string DlvMode
        {
            get { return (string)(this["DlvMode"] ?? string.Empty); }
            set { this["DlvMode"] = value; }
        }
    }
}