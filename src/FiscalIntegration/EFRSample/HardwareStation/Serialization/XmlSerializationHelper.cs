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
    namespace Commerce.HardwareStation.EFRSample
    {
        using System;
        using System.IO;
        using System.Xml;
        using System.Xml.Linq;
        using System.Xml.Serialization;

        /// <summary>
        /// The extended service for deserializing XML documents.
        /// </summary>
        internal static class XmlSerializationHelper
        {
            /// <summary>
            /// Deserializes XML string into generic container.
            /// </summary>
            /// <param name="source">The XML string.</param>
            /// <typeparam name="T">The object type.</typeparam>
            /// <returns>The deserialized object.</returns>
            public static T Deserialize<T>(string source) where T : class, new()
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                using (StringReader stringReader = new StringReader(source))
                {
                    using (XmlReader xmlReader = XmlReader.Create(stringReader))
                    {
                        return (T)xmlSerializer.Deserialize(xmlReader);
                    }
                }
            }

            /// <summary>
            /// Deserialize the XML string in TryParse way.
            /// </summary>
            /// <param name="source">The XML string.</param>
            /// <param name="result">The deserialized object.</param>
            /// <typeparam name="T">The object type.</typeparam>
            /// <returns>True if deserialized successfully; otherwise, false.</returns>
            public static bool TryDeserialize<T>(string source, out T result) where T : class, new()
            {
                try
                {
                    result = Deserialize<T>(source);
                    return true;
                }
                catch (Exception)
                {
                    result = default(T);
                    return false;
                }
            }

            /// <summary>
            /// Deserializes an XElement.
            /// </summary>
            /// <param name="element">The XML element.</param>
            /// <param name="rootElementName">The root element name.</param>
            /// <typeparam name="T">The object type.</typeparam>
            /// <returns>The deserialized object.</returns>
            public static T Deserialize<T>(XElement element, string rootElementName) where T : class, new()
            {
                XmlRootAttribute root = new XmlRootAttribute(rootElementName);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), root);
                using (XmlReader xmlReader = element.CreateReader())
                {
                    return (T)xmlSerializer.Deserialize(xmlReader);
                }
            }

            /// <summary>
            /// Deserializes an XElement.
            /// </summary>
            /// <param name="element">The XML element.</param>
            /// <param name="rootElementName">The root element name.</param>
            /// <param name="result">The deserialized object.</param>
            /// <typeparam name="T">The object type.</typeparam>
            /// <returns>True if deserialized successfully; otherwise, false.</returns>
            public static bool TryDeserialize<T>(XElement element, string rootElementName, out T result) where T : class, new()
            {
                try
                {
                    result = Deserialize<T>(element, rootElementName);
                    return true;
                }
                catch (Exception)
                {
                    result = default(T);
                    return false;
                }
            }
        }
    }
}
