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
    namespace CommerceRuntime.DocumentProvider.EFRSample.Serializers
    {
        using System;
        using System.IO;
        using System.Text;
        using System.Xml;
        using System.Xml.Serialization;

        /// <summary>
        /// The extended service for deserializing XML documents.
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        internal static class XmlSerializer<T>
        {
            /// <summary>
            /// Deserializes XML string into generic container.
            /// </summary>
            /// <param name="responseString">Serialized XML string.</param>
            /// <returns>Container with deserialized XML document.</returns>
            internal static T Deserialize(string responseString)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                using (StringReader stringReader = new StringReader(responseString))
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
            /// <typeparam name="T">The object data type.</typeparam>
            /// <returns>True if deserialized successfully; otherwise, false.</returns>
            internal static bool TryDeserialize(string source, out T result)
            {
                try
                {
                    result = Deserialize(source);
                    return true;
                }
                catch (Exception)
                {
                    result = default(T);
                    return false;
                }
            }

            /// <summary>
            /// Serializes an object into XML string.
            /// </summary>
            /// <param name="serializableObject">The object.</param>
            /// <returns>The string with xml.</returns>
            internal static string Serialize(T serializableObject)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                StringBuilder stringBuilder = new StringBuilder();

                using (TextWriter textWriter = new StringWriter(stringBuilder))
                {
                    xmlSerializer.Serialize(textWriter, serializableObject);
                }

                return stringBuilder.ToString();
            }
        }
    }
}
