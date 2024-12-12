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
    namespace HardwareStation.Connector.AtolSample.DataModel.Configuration
    {
        using System;
        using System.Runtime.Serialization;
        using System.Xml.Serialization;

        /// <summary>
        /// Represents configuration property.
        /// </summary>
        [DataContract]
        [XmlType("ConfigurationProperty")]
        public class ConfigurationProperty
        {
            /// <summary>
            /// Gets or sets namespace element.
            /// </summary>
            [DataMember]
            [XmlElement("Namespace")]
            public string Namespace { get; set; }

            /// <summary>
            /// Gets or sets name element.
            /// </summary>
            [DataMember]
            [XmlElement("Name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets value type element.
            /// </summary>
            [DataMember]
            [XmlElement("ValueType")]
            public string ValueType { get; set; }

            /// <summary>
            /// Gets or sets string value element.
            /// </summary>
            [DataMember]
            [XmlElement("StringValue")]
            public string StringValue { get; set; }

            /// <summary>
            /// Gets or sets decimal value element.
            /// </summary>
            [DataMember]
            [XmlAttribute("DecimalValue")]
            public decimal DecimalValue { get; set; }

            /// <summary>
            /// Gets or sets boolean value element.
            /// </summary>
            [DataMember]
            [XmlElement("BooleanValue")]
            public string BooleanValue { get; set; }

            /// <summary>
            /// Gets or sets date value element.
            /// </summary>
            [DataMember]
            [XmlAttribute("DateValue")]
            public DateTime DateValue { get; set; }

            /// <summary>
            /// Gets or sets integer value element.
            /// </summary>
            [DataMember]
            [XmlElement("IntegerValue")]
            public decimal IntegerValue { get; set; }

        }
    }
}
