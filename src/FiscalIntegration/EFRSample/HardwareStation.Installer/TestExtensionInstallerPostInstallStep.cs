namespace Contoso.HardwareStation.Installer
{
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Sdk.Installers;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Represents a sample extension installer step that runs after installation.
    /// </summary>
    [ExtensionInstallerStepExecutionPosition(ExtensionInstallerStepExecutionPhase.PostInstall, 1)]
    public class TestExtensionInstallerPostInstallStep : IExtensionInstallerStep
    {
        /// <summary>
        /// Gets the installer step name.
        /// </summary>
        public string Name { get; } = "TestExtensionInstallerPostInstallStep";

        /// <summary>
        /// Gets the installer step display name.
        /// </summary>
        public string DisplayName { get; } = "TestExtensionInstallerPostInstallStep";

        /// <summary>
        /// Gets the installer step description.
        /// </summary>
        public string Description { get; } = "A sample extension installer step that is executed after the standard installation steps.";

        /// <summary>
        /// Runs the extension installer step.
        /// </summary>
        /// <param name="context">The extension installer step context.</param>
        /// <returns>The task representing the asynchronous operation.</returns>
        public Task Run(IExtensionInstallerStepContext context)
        {
            context.Logger.LogInformation("************** Executing extension installer extension PostInstall step **************");

            context.PowerShellService.Run("echo 'About to print the value received from the PreInstallStep'");

            context.Logger.LogInformation($"Found configuration value: {context.GetConfigurationValue(ConfigurationKeys.SampleKey)}");

            return Task.CompletedTask;
        }
    }
}
