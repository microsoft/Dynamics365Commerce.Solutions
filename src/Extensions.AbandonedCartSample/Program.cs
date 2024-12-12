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
    namespace RetailServer.Ecommerce.AbandonedCartSample
    {
        using System;
        using RetailServer.Ecommerce.AbandonedCartSample.Options;
        using RetailServer.Ecommerce.AbandonedCartSample.Database;
        using RetailServer.Ecommerce.AbandonedCartSample.Messaging;
        using RetailServer.Ecommerce.AbandonedCartSample.Security;
        using Microsoft.Extensions.Configuration;
        using Microsoft.Extensions.DependencyInjection;
        using Microsoft.Extensions.Logging;

        public class Program
        {
            private static IServiceProvider _serviceProvider;

            public static void Main(string[] args)
            {
                try
                {
                    _serviceProvider = GetServiceProvider();
                    _serviceProvider.GetService<AbandonedCartApplication>().Run(args).Wait();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
                finally
                {
                    DisposeServices();
                    Console.WriteLine("End of application run, press any key to exit.");
                    Console.ReadKey();
                }
            }

            private static IServiceProvider GetServiceProvider()
            {
                var configuration = new ConfigurationBuilder()
                                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                        .Build();

                IServiceCollection serviceCollection =
                        new ServiceCollection()
                                        .AddLogging(l => l.AddConsole())
                                        .Configure<LoggerFilterOptions>(c => c.MinLevel = LogLevel.Trace)
                                        .AddHttpClient()

                                        .Configure<KeyVaultOptions>(configuration.GetSection(nameof(KeyVaultOptions)))
                                        .Configure<RetailServerClientOptions>(configuration.GetSection(nameof(RetailServerClientOptions)))
                                        .Configure<AzureCosmosOptions>(configuration.GetSection(nameof(AzureCosmosOptions)))
                                        .Configure<EmarsysClientOptions>(configuration.GetSection(nameof(EmarsysClientOptions)))
                                        .Configure<MediaOptions>(configuration.GetSection(nameof(MediaOptions)))

                                        .AddTransient<IKeyVaultClient, KeyVaultClient>()
                                        .AddTransient<ITokenProvider, TokenProvider>()
                                        .AddTransient<IRunStatusContainer, RunStatusContainer>()
                                        .AddTransient<IEmailProvider, EmarsysEmailProvider>()
                                        .AddTransient<AbandonedCartApplication>();

                return serviceCollection.BuildServiceProvider();
            }

            private static void DisposeServices()
            {
                if (_serviceProvider == null)
                {
                    return;
                }
                if (_serviceProvider is IDisposable)
                {
                    ((IDisposable)_serviceProvider).Dispose();
                }
            }
        }
    }
}
