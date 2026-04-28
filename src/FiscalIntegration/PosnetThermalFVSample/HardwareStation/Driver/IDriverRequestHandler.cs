namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        /// <summary>
        /// Interface declares request handler
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResponse">The response type.</typeparam>
        public interface IDriverRequestHandler<in TRequest, out TResponse>
        {
            TResponse ExecuteCommand(TRequest request);
        }
    }
}
