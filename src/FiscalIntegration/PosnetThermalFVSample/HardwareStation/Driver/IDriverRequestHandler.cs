namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        /// <summary>
        /// Declares a request handler.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResponse">The response type.</typeparam>
        public interface IDriverRequestHandler<in TRequest, out TResponse>
        {
            /// <summary>
            /// Executes the command.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            TResponse ExecuteCommand(TRequest request);
        }
    }
}
