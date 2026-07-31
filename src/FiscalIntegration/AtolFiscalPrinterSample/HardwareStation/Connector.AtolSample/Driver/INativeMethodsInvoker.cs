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
    namespace HardwareStation.Connector.AtolSample.Driver
    {
        using System;


        /// <summary>
        /// Provides the common interface to invoke the Atol fiscal device methods.
        /// </summary>
        public interface INativeMethodsInvoker
        {
            void Destroy();

            string Version();

            bool IsOpened();

            int ErrorCode();

            string ErrorDescription();

            void ResetError();

            int SetSettings(string settings);

            string GetSettings();

            void SetSingleSetting(string key, string setting);

            int ApplySingleSettings();

            string GetSingleSetting(string key);

            void SetParam(int paramID, uint value);

            void SetParam(int paramID, bool value);

            void SetParam(int paramID, double value);

            void SetParam(int paramID, byte[] value);

            void SetParam(int paramID, DateTime value);

            void SetParam(int paramID, string value);

            string GetParamString(int paramID);

            byte[] GetParamByteArray(int paramID);

            double GetParamDouble(int paramID);

            bool GetParamBool(int paramID);

            int Open();

            int Close();

            int ResetParams();

            int RunCommand();

            int Beep();

            int InitDevice();

            int ProcessJson();

            int ValidateJson();
        }
    }
}