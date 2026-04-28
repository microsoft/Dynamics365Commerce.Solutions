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
    /// Represents a pricing property definition.
    /// </summary>
    [DataContract]
    public class ContosoPricingPropertyDefinition : CommerceEntity
    {
        /// <summary>
        /// Record Id column.
        /// </summary>
        private const string RecordIdColumn = "RECID";

        /// <summary>
        /// Property type column.
        /// </summary>
        private const string ContosoPropertyTypeColumn = "CONTOSOPROPERTYTYPE";

        /// <summary>
        /// Initializes a new instance of the <see cref="ContosoPricingPropertyDefinition"/> class.
        /// </summary>
        public ContosoPricingPropertyDefinition() : base("ContosoPricingPropertyDefinition")
        {
        }

        /// <summary>
        /// Gets or sets the record identifier.
        /// </summary>
        [Key]
        [DataMember]
        [Column(RecordIdColumn)]
        public long RecordId
        {
            get { return (long)(this[RecordIdColumn] ?? 0L); }
            set { this[RecordIdColumn] = value; }
        }

        /// <summary>
        /// Gets or sets the property level.
        /// </summary>
        [IgnoreDataMember]
        [Column(ContosoPropertyTypeColumn)]
        public ContosoPropertyType ContosoPropertyType
        {
            get { return (ContosoPropertyType)(this[ContosoPropertyTypeColumn] ?? ContosoPropertyType.CustDlvMode); }
            set { this[ContosoPropertyTypeColumn] = value; }
        }
    }
}