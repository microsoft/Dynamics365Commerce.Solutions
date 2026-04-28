namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel
    {
        using System;
        using System.Collections.Generic;
        using System.Runtime.Serialization;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;
        using Microsoft.Dynamics.Commerce.Runtime;

        /// <summary>
        /// Implements document response.
        /// </summary>
        [DataContract]
        sealed class PosnetDocumentResponse : IPosnetDocumentResponse
        {
            /// <summary>
            /// Indicates whether the request was executed successfully.
            /// </summary>
            [DataMember]
            public bool Success { get; set; }

            /// <summary>
            /// The collection of responses.
            /// </summary>
            [DataMember]
            public IEnumerable<IPosnetCommandResponse> Responses { get; set; }

            /// <summary>
            /// Initializes a new instance of <see cref="PosnetDocumentResponse"/>.
            /// </summary>
            /// <param name="success">Indicates whether the request was executed successfully.</param>
            /// <param name="responses">The collection of responses.</param>
            public PosnetDocumentResponse(bool success, IEnumerable<IPosnetCommandResponse> responses)
            {
                ThrowIf.Null(responses, nameof(responses));

                this.Responses = responses;
                this.Success = success;
                this.Responses = responses;
            }
        }
    }
}
