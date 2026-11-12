using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.CommerceRuntime.DocumentProvider.PosnetSample
{
    /// <summary>
    /// Class that implements a post trigger for the GetExtensionPackageDefinitionsRequest request type.
    /// </summary>
    public class ExtensionPackageTrigger : IRequestTriggerAsync
    {
        /// <summary>
        /// Gets the supported requests for this trigger.
        /// </summary>
        public IEnumerable<Type> SupportedRequestTypes => new[] { typeof(GetExtensionPackageDefinitionsRequest) };

        /// <summary>
        /// Post trigger to retrieve extension package.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, nameof(request));
            ThrowIf.Null(response, nameof(response));

            var getExtensionsResponse = (GetExtensionPackageDefinitionsResponse)response;
            var extensionPackageDefinition = new ExtensionPackageDefinition
            {
                // Should match the PackageName used when packaging the customization package (i.e. in CustomizationPackage.props).
                Name = "Contoso.PosnetThermalFVSample",
                Publisher = "Contoso",
                IsEnabled = true,
            };

            getExtensionsResponse.ExtensionPackageDefinitions.Add(extensionPackageDefinition);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Pre trigger code.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task OnExecuting(Request request) => Task.CompletedTask;
    }
}
