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
    namespace CommerceRuntime.DocumentProvider.AtolSample.DataModel.Configuration
    {
        using System;
        using System.Runtime.Serialization;
        using System.Xml.Serialization;

        /// <summary>
        /// Represents a configuration property.
        /// </summary>
        [DataContract]
        [XmlType("ConfigurationProperty")]
        public class ConfigurationProperty
        {
            /// <summary>
            /// Gets or sets namespace.
            /// </summary>
            [DataMember]
            [XmlElement("Namespace")]
            public string Namespace { get; set; }

            /// <summary>
            /// Gets or sets property name.
            /// </summary>
            [DataMember]
            [XmlElement("Name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets type of value.
            /// </summary>
            [DataMember]
            [XmlElement("ValueType")]
            public string ValueType { get; set; }

            /// <summary>
            /// Gets or sets string value.
            /// </summary>
            [DataMember]
            [XmlElement("StringValue")]
            public string StringValue { get; set; }

            /// <summary>
            /// Gets or sets decimal value.
            /// </summary>
            [DataMember]
            [XmlAttribute("DecimalValue")]
            public decimal DecimalValue { get; set; }

            /// <summary>
            /// Gets or sets boolean value.
            /// </summary>
            [DataMember]
            [XmlElement("BooleanValue")]
            public string BooleanValue { get; set; }

            /// <summary>
            /// Gets or sets date time value.
            /// </summary>
            [DataMember]
            [XmlAttribute("DateValue")]
            public DateTime DateValue { get; set; }

            /// <summary>
            /// Gets or sets integer value.
            /// </summary>
            [DataMember]
            [XmlElement("IntegerValue")]
            public decimal IntegerValue { get; set; }
        }
    }
}
