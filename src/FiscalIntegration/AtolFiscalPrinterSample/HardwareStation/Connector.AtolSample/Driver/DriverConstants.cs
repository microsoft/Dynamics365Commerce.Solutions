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
        /// <summary>
        /// The device constants.
        /// </summary>
        public static class DriverConstants
        {
            public const int PARAM_NO_ERROR_IF_NOT_SUPPORTED = 65639;

            public const int PARAM_OFD_EXCHANGE_STATUS = 65640;

            public const int PARAM_FN_ERROR_DATA = 65641;

            public const int PARAM_FN_ERROR_CODE = 65642;

            public const int PARAM_JSON_DATA = 65645;

            public const int PARAM_NETWORK_ERROR = 65663;

            public const int PARAM_OFD_ERROR = 65664;

            public const int PARAM_FN_ERROR = 65665;

            public const int PARAM_ETHERNET_CONFIG_TIMEOUT = 65727;

            public const int PARAM_ETHERNET_DHCP = 65728;

            public const int PARAM_ETHERNET_IP = 65729;

            public const int PARAM_ETHERNET_MASK = 65730;

            public const int PARAM_ETHERNET_GATEWAY = 65731;

            public const int PARAM_ETHERNET_PORT = 65732;

            public const int PARAM_ETHERNET_DNS_IP = 65733;

            public const int PARAM_ETHERNET_DNS_STATIC = 65734;

            public const int PARAM_SCRIPT_CODE = 65773;

            public const int PARAM_SCRIPT_RESULT = 65774;

            public const int PARAM_SCRIPT_TYPE = 65775;

            public const int PARAM_WIFI_CONFIG_TIMEOUT = 65776;

            public const int PARAM_WIFI_DHCP = 65777;

            public const int PARAM_WIFI_IP = 65778;

            public const int PARAM_WIFI_MASK = 65779;

            public const int PARAM_WIFI_GATEWAY = 65780;

            public const int PARAM_WIFI_PORT = 65781;

            public const int PARAM_SETTING_NAME = 65827;

            public const int PARAM_SETTING_TYPE = 65828;

            public const int PARAM_REMOTE_CALL = 65830;

            public const int PARAM_SCRIPT_PARAMS = 65831;

            public const int PARAM_IGNORE_EMPTY = 65832;

            public const int PARAM_METHOD_DATA = 65833;

            public const int PARAM_METHOD_RESULT = 65834;

            public const int PARAM_RPC_SERVER_OS = 65835;

            public const int PARAM_RPC_SERVER_VERSION = 65836;

            public const int PARAM_RPC_DRIVER_VERSION = 65837;

            public const int PARAM_LOCKED = 65838;

            public const int PARAM_BOUND = 65839;

            public const int PARAM_COMMODITIES_TABLE_FAULT = 65840;

            public const int PARAM_HAS_ADDITIONAL_DATA = 65841;

            public const int PARAM_FISCAL_SIGN_ARCHIVE = 65842;

            public const int PARAM_COMMAND_GROUP = 65843;

            public const int PARAM_ERROR_CODE = 65844;

            public const int OK = 0;

            public const int PORT_COM = 0;

            public const int PORT_USB = 1;

            public const int PORT_TCPIP = 2;

            public const int PORT_BLUETOOTH = 3;

            public const int PORT_BITS_7 = 7;

            public const int PORT_BITS_8 = 8;

            public const int PORT_PARITY_NO = 0;

            public const int PORT_PARITY_ODD = 1;

            public const int PORT_PARITY_EVEN = 2;

            public const int PORT_PARITY_MARK = 3;

            public const int PORT_PARITY_SPACE = 4;

            public const int SCRIPT_EXECUTABLE = 0;

            public const int SCRIPT_JSON = 1;

            public const int SCRIPT_SETTINGS = 2;

            public const int OFD_CHANNEL_AUTO = 2;

            public const string SETTING_LIBRARY_PATH = "LibraryPath";

            public const string SETTING_MODEL = "Model";

            public const string SETTING_PORT = "Port";

            public const string SETTING_BAUDRATE = "BaudRate";

            public const string SETTING_BITS = "Bits";

            public const string SETTING_PARITY = "Parity";

            public const string SETTING_STOPBITS = "StopBits";

            public const string SETTING_IPADDRESS = "IPAddress";

            public const string SETTING_IPPORT = "IPPort";

            public const string SETTING_MACADDRESS = "MACAddress";

            public const string SETTING_COM_FILE = "ComFile";

            public const string SETTING_USB_DEVICE_PATH = "UsbDevicePath";

            public const string SETTING_BT_AUTOENABLE = "AutoEnableBluetooth";

            public const string SETTING_BT_AUTODISABLE = "AutoDisableBluetooth";

            public const string SETTING_OFD_CHANNEL = "OfdChannel";

            public const string SETTING_EXISTED_COM_FILES = "ExistedComFiles";

            public const string SETTING_SCRIPTS_PATH = "ScriptsPath";

            public const string SETTING_DOCUMENTS_JOURNAL_PATH = "DocumentsJournalPath";

            public const string SETTING_USE_DOCUMENTS_JOURNAL = "UseDocumentsJournal";

            public const string SETTING_AUTO_RECONNECT = "AutoReconnect";

            public const string SETTING_INVERT_CASH_DRAWER_STATUS = "InvertCashDrawerStatus";

            public const string SETTING_REMOTE_SERVER_ADDR = "RemoteServerAddr";

            public const string SETTING_REMOTE_SERVER_CONNECTION_TIMEOUT = "RemoteServerConnectionTimeout";

            public const int MODEL_UNKNOWN = 0;

            public const int MODEL_ATOL_25F = 57;

            public const int MODEL_ATOL_30F = 61;

            public const int MODEL_ATOL_55F = 62;

            public const int MODEL_ATOL_22F = 63;

            public const int MODEL_ATOL_52F = 64;

            public const int MODEL_ATOL_11F = 67;

            public const int MODEL_ATOL_77F = 69;

            public const int MODEL_ATOL_90F = 72;

            public const int MODEL_ATOL_60F = 75;

            public const int MODEL_ATOL_42FS = 77;

            public const int MODEL_ATOL_15F = 78;

            public const int MODEL_ATOL_50F = 80;

            public const int MODEL_ATOL_20F = 81;

            public const int MODEL_ATOL_91F = 82;

            public const int MODEL_ATOL_92F = 84;

            public const int MODEL_ATOL_SIGMA_10 = 86;

            public const int MODEL_ATOL_27F = 87;

            public const int MODEL_ATOL_SIGMA_7F = 90;

            public const int MODEL_ATOL_SIGMA_8F = 91;

            public const int MODEL_ATOL_1F = 93;

            public const int MODEL_KAZNACHEY_FA = 76;

            public const int MODEL_ATOL_22V2F = 95;

            public const int MODEL_ATOL_AUTO = 500;

            public const int PORT_BR_1200 = 1200;

            public const int PORT_BR_2400 = 2400;

            public const int PORT_BR_4800 = 4800;

            public const int PORT_BR_9600 = 9600;

            public const int PORT_BR_19200 = 19200;

            public const int PORT_BR_38400 = 38400;

            public const int PORT_BR_57600 = 57600;

            public const int PORT_BR_115200 = 115200;

            public const int PORT_BR_230400 = 230400;

            public const int PORT_BR_460800 = 460800;

            public const int PORT_BR_921600 = 921600;
        }
    }
}
