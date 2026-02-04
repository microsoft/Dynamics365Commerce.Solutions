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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample
    {
        using System;

        /// <summary>
        /// The exception that is thrown when the configuration of the sample contains incorrect values.
        /// </summary>
        public class EpsonFP90IIIDocumentProviderConfigurationException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="EpsonFP90IIIDocumentProviderConfigurationException" /> class.
            /// </summary>
            public EpsonFP90IIIDocumentProviderConfigurationException()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="EpsonFP90IIIDocumentProviderConfigurationException" /> class.
            /// </summary>
            /// <param name="message">The message describing the error that occurred.</param>
            public EpsonFP90IIIDocumentProviderConfigurationException(string message)
                : base(message)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="EpsonFP90IIIDocumentProviderConfigurationException"/> class.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="inner">The inner exception.</param>
            public EpsonFP90IIIDocumentProviderConfigurationException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
    }
}