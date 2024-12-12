namespace Contoso.HardwareStation.Connector.AtolSample.AtolTasks
{
    internal static class AtolTasksList
    {
        /// <summary>
        /// Gets task to print previous not printed document that was saved in fiscal memory.
        /// </summary>
        public static string PrintPreviousDocumentTask => "{ \"type\" : \"continuePrint\" }";

        /// <summary>
        /// Gets task to get fiscal printer details.
        /// </summary>
        public static string GetDeviceInfoTask => "{ \"type\" : \"getDeviceInfo\" }";

        /// <summary>
        /// Gets task to get fiscal printer status.
        /// </summary>
        public static string GetDeviceStatusTask => "{ \"type\" : \"getDeviceStatus\" }";
    }
}
