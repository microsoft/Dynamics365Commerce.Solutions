namespace Contoso.HardwareStation.Installer
{
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Sdk.Installers;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Represents the sample pre-install extension installer step.
    /// </summary>
    [ExtensionInstallerStepExecutionPosition(ExtensionInstallerStepExecutionPhase.PreInstall, 1)]
    public class TestExtensionInstallerPreInstallStep : IExtensionInstallerStep
    {
        /// <summary>
        /// Gets the extension installer step name.
        /// </summary>
        public string Name { get; } = "TestExtensionInstallerPreInstallStep";

        /// <summary>
        /// Gets the extension installer step display name.
        /// </summary>
        public string DisplayName { get; } = "TestExtensionInstallerPreInstallStep";

        /// <summary>
        /// Gets the extension installer step description.
        /// </summary>
        public string Description { get; } = "A sample extension installer step that is executed before the standard installation steps.";

        /// <summary>
        /// Runs the extension installer step.
        /// </summary>
        /// <param name="context">The extension installer step context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Run(IExtensionInstallerStepContext context)
        {
            context.Logger.LogInformation("************** Executing extension installer extension PreInstall step **************");

            context.SetConfigurationValue(ConfigurationKeys.SampleKey, "SampleValueSetDuringPre-Install");

            return Task.CompletedTask;
        }
    }
}
