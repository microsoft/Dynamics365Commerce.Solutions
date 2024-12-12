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
    namespace CommerceRuntime.DocumentProvider.AtolSample.Handlers
    {
        using System;
        using System.Collections.Generic;
        using System.IO;
        using System.Runtime.Serialization.Json;
        using System.Text;
        using System.Threading.Tasks;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.Messages;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Encapsulates the implementation of the service to serializing a Atol commands.
        /// </summary>
        public class SerializationTaskService : IRequestHandlerAsync
        {
            /// <summary>
            /// Gets the collection of request supported types.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes => new[]
            {
                typeof(SerializeToAtolCommandRequest),
            };

            /// <summary>
            /// Represents the entry point of the request handler.
            /// </summary>
            /// <param name="request">The incoming request message.</param>
            /// <returns>The outgoing response message.</returns>
            public Task<Response> Execute(Request request)
            {
                switch (request)
                {
                    case SerializeToAtolCommandRequest serializeToAtolCommandRequest:
                        return Task.FromResult(SerializeTask(serializeToAtolCommandRequest));
                    default:
                        throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
                }

            }

            /// <summary>
            /// Serialize Atol command to json string.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response containing the json string of the serialized command.</returns>
            private Response SerializeTask(SerializeToAtolCommandRequest request)
            {
                string serializedCommand = string.Empty;
                using (MemoryStream ms = new MemoryStream())
                {
                    DataContractJsonSerializerSettings jsonSerializerSettings = new DataContractJsonSerializerSettings();
                    jsonSerializerSettings.EmitTypeInformation = System.Runtime.Serialization.EmitTypeInformation.Never;
                    jsonSerializerSettings.SerializeReadOnlyTypes = true;

                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(request.CommandForSerialization.GetType(), jsonSerializerSettings);
                    serializer.WriteObject(ms, request.CommandForSerialization);

                    serializedCommand = Encoding.UTF8.GetString(ms.ToArray());
                }

                return new SerializeToAtolCommandResponse(serializedCommand);
            }
        }
    }
}
