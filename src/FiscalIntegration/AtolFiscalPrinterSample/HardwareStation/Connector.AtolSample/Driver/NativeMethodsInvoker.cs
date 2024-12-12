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
        using System.Runtime.InteropServices;
        using System.Text;

        /// <summary>
        /// Invokes fiscal device methods in dll.
        /// </summary>
        public class NativeMethodsInvoker : INativeMethodsInvoker
        {
            private IntPtr M_handle = IntPtr.Zero;

            public NativeMethodsInvoker()
            {
                NativeMethods.LoadDriver(null);
                int num = NativeMethods.libfptr_create(out this.M_handle);
                if (num == -2)
                {
                    throw new ArgumentException("Invalid [id] format");
                }
                if (num != 0)
                {
                    throw new Exception("Can`t create driver handle");
                }
            }
            public NativeMethodsInvoker(string libraryPath)
            {
                NativeMethods.LoadDriver(libraryPath);
                int num = NativeMethods.libfptr_create(out this.M_handle);
                if (num == -2)
                {
                    throw new ArgumentException("Invalid [id] format");
                }
                if (num != 0)
                {
                    throw new Exception("Can`t create driver handle");
                }
            }

            public NativeMethodsInvoker(string id, string libraryPath)
            {
                NativeMethods.LoadDriver(libraryPath);
                int num = NativeMethods.libfptr_create_with_id(out this.M_handle, id);
                if (num == -2)
                {
                    throw new ArgumentException("Invalid [id] format");
                }
                if (num != 0)
                {
                    throw new Exception("Can`t create driver handle");
                }
            }

            ~NativeMethodsInvoker()
            {
                if (this.M_handle != IntPtr.Zero)
                {
                    NativeMethods.libfptr_destroy(out this.M_handle);
                }
            }
            public void Destroy()
            {
                if (this.M_handle != IntPtr.Zero)
                {
                    NativeMethods.libfptr_destroy(out M_handle);
                }
            }

            public string Version()
            {
                return Marshal.PtrToStringAnsi(NativeMethods.libfptr_get_version_string());
            }

            public bool IsOpened()
            {
                return NativeMethods.libfptr_is_opened(this.M_handle);
            }

            public int ErrorCode()
            {
                return NativeMethods.libfptr_error_code(this.M_handle);
            }

            public string ErrorDescription()
            {
                StringBuilder stringBuilder = new StringBuilder(256);
                int num = NativeMethods.libfptr_error_description(this.M_handle, stringBuilder, stringBuilder.Capacity);
                if (num > stringBuilder.Capacity)
                {
                    stringBuilder.EnsureCapacity(num);
                    num = NativeMethods.libfptr_error_description(this.M_handle, stringBuilder, stringBuilder.Capacity);
                }
                return stringBuilder.ToString();
            }

            public void ResetError()
            {
                NativeMethods.libfptr_reset_error(this.M_handle);
            }

            public int SetSettings(string settings)
            {
                return NativeMethods.libfptr_set_settings(this.M_handle, settings);
            }

            public string GetSettings()
            {
                StringBuilder stringBuilder = new StringBuilder(128);
                int num = NativeMethods.libfptr_get_settings(this.M_handle, stringBuilder, stringBuilder.Capacity);
                if (num > stringBuilder.Capacity)
                {
                    stringBuilder.EnsureCapacity(num);
                    num = NativeMethods.libfptr_get_settings(this.M_handle, stringBuilder, stringBuilder.Capacity);
                }
                return stringBuilder.ToString();
            }

            public void SetSingleSetting(string key, string setting)
            {
                NativeMethods.libfptr_set_single_setting(this.M_handle, key, setting);
            }

            public int ApplySingleSettings()
            {
                return NativeMethods.libfptr_apply_single_settings(this.M_handle);
            }

            public string GetSingleSetting(string key)
            {
                throw new NotImplementedException();
            }

            public void SetParam(int paramID, uint value)
            {
                NativeMethods.libfptr_set_param_int(this.M_handle, paramID, value);
            }

            public void SetParam(int paramID, bool value)
            {
                NativeMethods.libfptr_set_param_bool(this.M_handle, paramID, value);
            }

            public void SetParam(int paramID, double value)
            {
                NativeMethods.libfptr_set_param_double(this.M_handle, paramID, value);
            }

            public void SetParam(int paramID, byte[] value)
            {
                NativeMethods.libfptr_set_param_bytearray(this.M_handle, paramID, value, value.Length);
            }

            public void SetParam(int paramID, DateTime value)
            {
                NativeMethods.libfptr_set_param_datetime(this.M_handle, paramID, value.Year, value.Month,
                    value.Day, value.Hour, value.Minute, value.Second);
            }

            public void SetParam(int paramID, string value)
            {
                NativeMethods.libfptr_set_param_str(this.M_handle, paramID, value);
            }

            public int GetPararamInt(int paramId)
            {
                return Convert.ToInt32(NativeMethods.libfptr_get_param_int(this.M_handle, paramId));
            }

            public bool GetParamBool(int paramID)
            {
                return NativeMethods.libfptr_get_param_bool(this.M_handle, paramID);
            }

            public double GetParamDouble(int paramID)
            {
                return NativeMethods.libfptr_get_param_double(this.M_handle, paramID);
            }

            public byte[] GetParamByteArray(int paramID)
            {
                byte[] array = new byte[128];
                int num = NativeMethods.libfptr_get_param_bytearray(this.M_handle, paramID, array, array.Length);
                if (num > array.Length)
                {
                    Array.Resize<byte>(ref array, num);
                    NativeMethods.libfptr_get_param_bytearray(this.M_handle, paramID, array, array.Length);
                }
                else
                {
                    Array.Resize<byte>(ref array, num);
                }
                return array;
            }

            public string GetParamString(int paramID)
            {
                StringBuilder stringBuilder = new StringBuilder(128);
                int num = NativeMethods.libfptr_get_param_str(this.M_handle, paramID, stringBuilder, stringBuilder.Capacity);
                if (num > stringBuilder.Capacity)
                {
                    stringBuilder.EnsureCapacity(num);
                    num = NativeMethods.libfptr_get_param_str(this.M_handle, paramID, stringBuilder, stringBuilder.Capacity);
                }
                return stringBuilder.ToString();
            }

            public int Open()
            {
                return NativeMethods.libfptr_open(this.M_handle);
            }

            public int Close()
            {
                return NativeMethods.libfptr_close(this.M_handle);
            }

            public int ResetParams()
            {
                throw new NotImplementedException();
            }

            public int RunCommand()
            {
                throw new NotImplementedException();
            }

            public int Beep()
            {
                return NativeMethods.libfptr_beep(this.M_handle);
            }

            public int InitDevice()
            {
                return NativeMethods.libfptr_init_device(this.M_handle);
            }

            public int ProcessJson()
            {
                return NativeMethods.libfptr_process_json(this.M_handle);
            }

            public int ValidateJson()
            {
                return NativeMethods.libfptr_validate_json(this.M_handle);
            }
        }
    }
}