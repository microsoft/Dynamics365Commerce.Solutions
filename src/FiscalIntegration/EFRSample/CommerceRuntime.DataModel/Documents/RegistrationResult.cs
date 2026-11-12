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
    namespace CommerceRuntime.DocumentProvider.DataModelEFR.Documents
    {
        using System;
        using System.Xml.Serialization;

        /// <summary>
        /// The sales transaction registration result.
        /// </summary>
        [Serializable]
        public class RegistrationResult
        {
            private const string ResultCodeAttributeName = "RC";
            private const string ErrorCodeElementName = "ErrorCode";
            private const string UserMessageElementName = "UserMessage";
            private const string DebugMessageElementName = "Warning";

            /// <summary>
            /// Gets or sets the result code.
            /// </summary>
            [XmlAttribute(AttributeName = ResultCodeAttributeName)]
            public string ResultCode { get; set; }

            /// <summary>
            /// Gets or sets the error code.
            /// </summary>
            [XmlElement(ElementName = ErrorCodeElementName)]
            public string ErrorCode { get; set; }

            /// <summary>
            /// Gets or sets the user message.
            /// </summary>
            [XmlElement(ElementName = UserMessageElementName)]
            public string UserMessage { get; set; }

            /// <summary>
            /// Gets or sets the debug message.
            /// </summary>
            [XmlElement(ElementName = DebugMessageElementName)]
            public string DebugMessage { get; set; }
        }
    }
}
