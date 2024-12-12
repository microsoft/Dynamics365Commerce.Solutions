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
        using System.IO;
        using System.Reflection;
        using System.Runtime.InteropServices;
        using System.Text;
        using Microsoft.Win32;

        /// <summary>
        /// The class encapsulating the native Atol printer methods in dll.
        /// </summary>
        public static class NativeMethods
        {
            public const string DLLName = "fptr10.dll";

            /// <summary>
            /// Loads Atol driver.
            /// </summary>
            /// <param name="path">Path to the Atol driver dll.</param>
            public static void LoadDriver(string path)
            {
                try
                {
                    NativeMethods.libfptr_get_version_string();
                    return;
                }
                catch (Exception)
                {
                }
                path = (path ?? "");
                if (path.Length == 0)
                {
                    if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                    {
                        return;
                    }
                    if (!NativeMethods.TryLoadDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
                    {
                        path = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\ATOL\Drivers\10.0\KKT", "INSTALL_DIR", null);
                        if (path == null)
                        {
                            throw new FileNotFoundException("Driver not installed");
                        }
                        path = Path.Combine(path, "bin");
                    }
                }
                if (!NativeMethods.TryLoadDriver(path))
                {
                    throw new FileNotFoundException(string.Format("Driver library not found in \"{0}\"", path));
                }
            }

            private static bool TryLoadDriver(string path)
            {
                bool result = false;
                try
                {
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    {
                        if (path.Length == 0)
                        {
                            path = "fptr10.dll";
                        }
                        else if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            path = Path.Combine(path, "fptr10.dll");
                        }
                        IntPtr value = NativeMethods.LoadLibrary(path);
                        if (value == IntPtr.Zero)
                        {
                            NativeMethods.LoadLibrary(Path.Combine(Path.GetDirectoryName(path), "msvcp140.dll"));
                            value = NativeMethods.LoadLibrary(path);
                        }
                        result = (value != IntPtr.Zero);
                    }
                    else
                    {
                        result = false;
                    }
                }
                catch (FileNotFoundException reason)
                {
                    throw new FileNotFoundException(string.Format("Driver library not found in \"{0}\"", path), reason);
                }

                return result;
            }

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr LoadLibrary(string lpPathName);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr libfptr_get_version_string();

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_create(out IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_create_with_id(out IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string id);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_destroy(out IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_log_write_ex(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string tag, int level, [MarshalAs(UnmanagedType.LPWStr)] string message);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_show_properties(IntPtr handle, int parentType, IntPtr parent);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool libfptr_is_opened(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_error_code(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_error_description(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder value, int size);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_reset_error(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_set_settings(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string settings);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_get_settings(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder value, int size);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_single_setting(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string key, [MarshalAs(UnmanagedType.LPWStr)] string setting);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_get_single_setting(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string key, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder value, int size);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_param_int(IntPtr handle, int paramID, uint value);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_param_bool(IntPtr handle, int paramID, bool value);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_param_double(IntPtr handle, int paramID, double value);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_param_bytearray(IntPtr handle, int paramID, byte[] value, int size);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_param_datetime(IntPtr handle, int paramID, int year, int month, int day, int hour, int minute, int second);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_param_str(IntPtr handle, int paramID, [MarshalAs(UnmanagedType.LPWStr)] string value);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_non_printable_param_int(IntPtr handle, int paramID, uint value);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_non_printable_param_bool(IntPtr handle, int paramID, bool value);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_non_printable_param_double(IntPtr handle, int paramID, double value);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_non_printable_param_bytearray(IntPtr handle, int paramID, byte[] value, int size);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_non_printable_param_datetime(IntPtr handle, int paramID, int year, int month, int day, int hour, int minute, int second);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_non_printable_param_str(IntPtr handle, int paramID, [MarshalAs(UnmanagedType.LPWStr)] string value);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_user_param_int(IntPtr handle, int paramID, uint value);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_user_param_bool(IntPtr handle, int paramID, bool value);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_user_param_double(IntPtr handle, int paramID, double value);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_user_param_bytearray(IntPtr handle, int paramID, byte[] value, int size);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_user_param_datetime(IntPtr handle, int paramID, int year, int month, int day, int hour, int minute, int second);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_set_user_param_str(IntPtr handle, int paramID, [MarshalAs(UnmanagedType.LPWStr)] string value);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern uint libfptr_get_param_int(IntPtr handle, int paramID);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool libfptr_get_param_bool(IntPtr handle, int paramID);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern double libfptr_get_param_double(IntPtr handle, int paramID);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_get_param_bytearray(IntPtr handle, int paramID, byte[] value, int size);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void libfptr_get_param_datetime(IntPtr handle, int paramID, out IntPtr year, out IntPtr month, out IntPtr day, out IntPtr hour, out IntPtr minute, out IntPtr second);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_get_param_str(IntPtr handle, int paramID, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder value, int size);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_apply_single_settings(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_open(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_close(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_reset_params(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_run_command(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_beep(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_open_drawer(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_cut(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_device_poweroff(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_device_reboot(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_open_shift(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_reset_summary(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_init_device(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_query_data(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_cash_income(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_cash_outcome(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_open_receipt(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_cancel_receipt(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_close_receipt(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_check_document_closed(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_receipt_total(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_receipt_tax(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_registration(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_payment(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_report(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_print_text(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_print_cliche(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_begin_nonfiscal_document(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_end_nonfiscal_document(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_print_barcode(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_print_picture(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_print_picture_by_number(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_upload_picture_from_file(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_clear_pictures(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_write_device_setting_raw(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_read_device_setting_raw(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_commit_settings(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_init_settings(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_reset_settings(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_write_date_time(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_write_license(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_fn_operation(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_fn_query_data(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_fn_write_attributes(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_external_device_power_on(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_external_device_power_off(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_process_json(IntPtr handle);

            [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int libfptr_validate_json(IntPtr handle);
        }
    }
}