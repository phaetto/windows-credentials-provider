using System.Collections.Generic;
using System.Linq;

namespace WindowsCredentialProviderTest
{
    using System;
    using System.Runtime.InteropServices;
    using CredentialProvider.Interop;

    [ComVisible(true)]
    [Guid(Constants.CredentialProviderTileUID)]
    [ClassInterface(ClassInterfaceType.None)]
    public sealed class TestWindowsCredentialProviderTile : ITestWindowsCredentialProviderTile
    {
        public _CREDENTIAL_PROVIDER_USAGE_SCENARIO UsageScenario { get; set; }
        public List<_CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR> CredentialProviderFieldDescriptorList = new List<_CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR> {
            new _CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR
            {
                cpft = _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_SMALL_TEXT,
                dwFieldID = 0,
                pszLabel = "Rebootify Awesomeness",
            },
            new _CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR
            {
                cpft = _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_SUBMIT_BUTTON,
                dwFieldID = 1,
                pszLabel = "Login",
            }
        };

        public int Advise(ICredentialProviderCredentialEvents pcpce)
        {
            Log.LogMethodCall();
            return HResultValues.E_NOTIMPL;
        }

        public int UnAdvise()
        {
            Log.LogMethodCall();
            return HResultValues.E_NOTIMPL;
        }

        public int SetSelected(out int pbAutoLogon)
        {
            Log.LogMethodCall();
            pbAutoLogon = 0;
            return HResultValues.E_NOTIMPL;
        }

        public int SetDeselected()
        {
            Log.LogMethodCall();
            return HResultValues.E_NOTIMPL;
        }

        public int GetFieldState(uint dwFieldID, out _CREDENTIAL_PROVIDER_FIELD_STATE pcpfs,
            out _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE pcpfis)
        {
            Log.LogMethodCall();
            pcpfis = _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE;
            pcpfs = _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_BOTH;
            return HResultValues.S_OK;
        }

        public int GetStringValue(uint dwFieldID, out string ppsz)
        {
            Log.LogMethodCall();

            if (!CredentialProviderFieldDescriptorList
                .Any(x => x.dwFieldID == dwFieldID
                            && x.cpft == _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_SMALL_TEXT))
            {
                ppsz = string.Empty;
                return HResultValues.E_NOTIMPL;
            }

            var descriptor = CredentialProviderFieldDescriptorList
                .First(x => x.dwFieldID == dwFieldID
                                     && x.cpft == _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_SMALL_TEXT);

            ppsz = descriptor.pszLabel;
            return HResultValues.S_OK;
        }

        public int GetBitmapValue(uint dwFieldID, IntPtr phbmp)
        {
            Log.LogMethodCall();
            return HResultValues.E_NOTIMPL;
        }

        public int GetCheckboxValue(uint dwFieldID, out int pbChecked, out string ppszLabel)
        {
            Log.LogMethodCall();
            pbChecked = 0;
            ppszLabel = string.Empty;
            return HResultValues.E_NOTIMPL;
        }

        public int GetSubmitButtonValue(uint dwFieldID, out uint pdwAdjacentTo)
        {
            Log.LogMethodCall();

            if (!CredentialProviderFieldDescriptorList
                .Any(x => x.dwFieldID == dwFieldID
                          && x.cpft == _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_SUBMIT_BUTTON))
            {
                pdwAdjacentTo = 0;
                return HResultValues.E_NOTIMPL;
            }

            var descriptor = CredentialProviderFieldDescriptorList
                .First(x => x.dwFieldID == dwFieldID
                            && x.cpft == _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_SUBMIT_BUTTON);

            pdwAdjacentTo = descriptor.dwFieldID - 1;

            return HResultValues.S_OK;
        }

        public int GetComboBoxValueCount(uint dwFieldID, out uint pcItems, out uint pdwSelectedItem)
        {
            Log.LogMethodCall();
            pcItems = 0;
            pdwSelectedItem = 0;
            return HResultValues.E_NOTIMPL;
        }

        public int GetComboBoxValueAt(uint dwFieldID, uint dwItem, out string ppszItem)
        {
            Log.LogMethodCall();
            ppszItem = string.Empty;
            return HResultValues.E_NOTIMPL;
        }

        public int SetStringValue(uint dwFieldID, string psz)
        {
            Log.LogMethodCall();
            return HResultValues.E_NOTIMPL;
        }

        public int SetCheckboxValue(uint dwFieldID, int bChecked)
        {
            Log.LogMethodCall();
            return HResultValues.E_NOTIMPL;
        }

        public int SetComboBoxSelectedValue(uint dwFieldID, uint dwSelectedItem)
        {
            Log.LogMethodCall();
            return HResultValues.E_NOTIMPL;
        }

        public int CommandLinkClicked(uint dwFieldID)
        {
            Log.LogMethodCall();
            return HResultValues.E_NOTIMPL;
        }

        public int GetSerialization(out _CREDENTIAL_PROVIDER_GET_SERIALIZATION_RESPONSE pcpgsr,
            out _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION pcpcs, out string ppszOptionalStatusText,
            out _CREDENTIAL_PROVIDER_STATUS_ICON pcpsiOptionalStatusIcon)
        {
            Log.LogMethodCall();

            try
            {
                pcpgsr = _CREDENTIAL_PROVIDER_GET_SERIALIZATION_RESPONSE.CPGSR_RETURN_CREDENTIAL_FINISHED;
                pcpcs = new _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION();

                var username = "<domain>\\<username>";
                var password = "<password>";
                var inCredSize = 0;
                var inCredBuffer = Marshal.AllocCoTaskMem(0);

                if (!PInvoke.CredPackAuthenticationBuffer(0, username, password, inCredBuffer, ref inCredSize))
                {
                    Marshal.FreeCoTaskMem(inCredBuffer);
                    inCredBuffer = Marshal.AllocCoTaskMem(inCredSize);

                    if (PInvoke.CredPackAuthenticationBuffer(0, username, password, inCredBuffer, ref inCredSize))
                    {
                        ppszOptionalStatusText = string.Empty;
                        pcpsiOptionalStatusIcon = _CREDENTIAL_PROVIDER_STATUS_ICON.CPSI_SUCCESS;

                        pcpcs.clsidCredentialProvider = Guid.Parse(Constants.CredentialProviderUID);
                        pcpcs.rgbSerialization = inCredBuffer;
                        pcpcs.cbSerialization = (uint)inCredSize;

                        RetrieveNegotiateAuthPackage(out var authPackage);
                        pcpcs.ulAuthenticationPackage = authPackage;

                        return HResultValues.S_OK;
                    }

                    ppszOptionalStatusText = "Failed to pack credentials";
                    pcpsiOptionalStatusIcon = _CREDENTIAL_PROVIDER_STATUS_ICON.CPSI_ERROR;
                    return HResultValues.E_FAIL;
                }
            }
            catch (Exception)
            {
                // In case of any error, do not bring down winlogon
            }

            pcpgsr = _CREDENTIAL_PROVIDER_GET_SERIALIZATION_RESPONSE.CPGSR_NO_CREDENTIAL_NOT_FINISHED;
            pcpcs = new _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION();
            ppszOptionalStatusText = string.Empty;
            pcpsiOptionalStatusIcon = _CREDENTIAL_PROVIDER_STATUS_ICON.CPSI_NONE;
            return HResultValues.E_NOTIMPL;
        }

        public int ReportResult(int ntsStatus, int ntsSubstatus, out string ppszOptionalStatusText,
            out _CREDENTIAL_PROVIDER_STATUS_ICON pcpsiOptionalStatusIcon)
        {
            Log.LogMethodCall();
            ppszOptionalStatusText = string.Empty;
            pcpsiOptionalStatusIcon = _CREDENTIAL_PROVIDER_STATUS_ICON.CPSI_NONE;
            return HResultValues.E_NOTIMPL;
        }

        private int RetrieveNegotiateAuthPackage(out uint authPackage)
        {
            // TODO: better checking on the return codes

            var status = PInvoke.LsaConnectUntrusted(out var lsaHandle);

            using (var name = new PInvoke.LsaStringWrapper("Negotiate"))
            {
                status = PInvoke.LsaLookupAuthenticationPackage(lsaHandle, ref name._string, out authPackage);
            }

            PInvoke.LsaDeregisterLogonProcess(lsaHandle);

            return (int)status;
        }
    }
}
