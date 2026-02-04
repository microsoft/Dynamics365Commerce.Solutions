namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.Driver
    {
        using System;

        /// <summary>
        /// The printing status.
        /// </summary>
        internal enum PrintingStatusType : UInt32
        {
            /// <summary>
            /// No error.
            /// </summary>
            NoError = 0,

            /// <summary>
            /// Lever is up.
            /// </summary>
            LeverUp = 1,

            /// <summary>
            /// Mechanism error.
            /// </summary>
            MechanismError = 2,

            /// <summary>
            /// Cover is up.
            /// </summary>
            CoverUp = 3,

            /// <summary>
            /// No paper for the receipt copy.
            /// </summary>
            NoPaperForCopy = 4,

            /// <summary>
            /// No paper for the original receipt. 
            /// </summary>
            NoPaperForOriginal = 5,

            /// <summary>
            /// Wrong temperature or power suuply.
            /// </summary>
            WrongTemperatureOrPowerSupply = 6,

            /// <summary>
            /// Momentary power supply shortage.
            /// </summary>
            MomentaryPowerSupplyShortage = 7,

            /// <summary>
            /// Cutter error.
            /// </summary>
            CutterError = 8,

            /// <summary>
            /// Charger error.
            /// </summary>
            ChargerError = 9,

            /// <summary>
            /// Cover up when cutting.
            /// </summary>
            CoverUpWhenCutting = 10,

            /// <summary>
            /// Unknown status.
            /// </summary>
            Other = 65537
        }
    }
}
