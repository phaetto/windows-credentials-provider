namespace WindowsCredentialProviderTest
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using CredentialProvider.Interop;

    [ComVisible(true)]
    [Guid("298D9F84-9BC5-435C-9FC2-EB3746625954")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Rebootify.TestWindowsCredentialProvider")]
    public class TestWindowsCredentialProvider : ITestWindowsCredentialProvider, IDisposable
    {
        private bool isDisposing;

        public TestWindowsCredentialProvider()
        {
            Log.LogText("TestWindowsCredentialProvider: Started");
        }

        public int SetUsageScenario(_CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus, uint dwFlags)
        {
            Log.LogText("I am running");

            switch (cpus)
            {
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_UNLOCK_WORKSTATION:
                    return HResultValues.S_OK;

                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_LOGON:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CREDUI:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CHANGE_PASSWORD:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_PLAP:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_INVALID:
                    return HResultValues.E_NOTIMPL;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public int SetSerialization(ref _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION pcpcs)
        {
            return HResultValues.E_NOTIMPL;
        }

        public int Advise(ICredentialProviderEvents pcpe, uint upAdviseContext)
        {
            return HResultValues.E_NOTIMPL;
        }

        public int Advise(ICredentialProviderCredentialEvents pcpce)
        {
            return HResultValues.E_NOTIMPL;
        }

        public int UnAdvise()
        {
            return HResultValues.E_NOTIMPL;
        }

        int ICredentialProviderCredential.UnAdvise()
        {
            return HResultValues.E_NOTIMPL;
        }

        int ICredentialProvider.UnAdvise()
        {
            return HResultValues.E_NOTIMPL;
        }

        public int SetSelected(out int pbAutoLogon)
        {
            pbAutoLogon = 0;
            return HResultValues.E_NOTIMPL;
        }

        public int SetDeselected()
        {
            return HResultValues.E_NOTIMPL;
        }

        public int GetFieldState(uint dwFieldID, out _CREDENTIAL_PROVIDER_FIELD_STATE pcpfs,
            out _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE pcpfis)
        {
            pcpfis = _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE;
            pcpfs = _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_HIDDEN;
            return HResultValues.E_NOTIMPL;
        }

        public int GetStringValue(uint dwFieldID, out string ppsz)
        {
            ppsz = string.Empty;
            return HResultValues.E_NOTIMPL;
        }

        public int GetBitmapValue(uint dwFieldID, IntPtr phbmp)
        {
            return HResultValues.E_NOTIMPL;
        }

        public int GetCheckboxValue(uint dwFieldID, out int pbChecked, out string ppszLabel)
        {
            pbChecked = 0;
            ppszLabel = string.Empty;
            return HResultValues.E_NOTIMPL;
        }

        public int GetSubmitButtonValue(uint dwFieldID, out uint pdwAdjacentTo)
        {
            pdwAdjacentTo = 0;
            return HResultValues.E_NOTIMPL;
        }

        public int GetComboBoxValueCount(uint dwFieldID, out uint pcItems, out uint pdwSelectedItem)
        {
            pcItems = 0;
            pdwSelectedItem = 0;
            return HResultValues.E_NOTIMPL;
        }

        public int GetComboBoxValueAt(uint dwFieldID, uint dwItem, out string ppszItem)
        {
            ppszItem = string.Empty;
            return HResultValues.E_NOTIMPL;
        }

        public int SetStringValue(uint dwFieldID, string psz)
        {
            return HResultValues.E_NOTIMPL;
        }

        public int SetCheckboxValue(uint dwFieldID, int bChecked)
        {
            return HResultValues.E_NOTIMPL;
        }

        public int SetComboBoxSelectedValue(uint dwFieldID, uint dwSelectedItem)
        {
            return HResultValues.E_NOTIMPL;
        }

        public int CommandLinkClicked(uint dwFieldID)
        {
            return HResultValues.E_NOTIMPL;
        }

        public int GetSerialization(out _CREDENTIAL_PROVIDER_GET_SERIALIZATION_RESPONSE pcpgsr,
            out _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION pcpcs, out string ppszOptionalStatusText,
            out _CREDENTIAL_PROVIDER_STATUS_ICON pcpsiOptionalStatusIcon)
        {

            pcpgsr = _CREDENTIAL_PROVIDER_GET_SERIALIZATION_RESPONSE.CPGSR_NO_CREDENTIAL_NOT_FINISHED;
            pcpcs = new _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION();
            ppszOptionalStatusText = string.Empty;
            pcpsiOptionalStatusIcon = _CREDENTIAL_PROVIDER_STATUS_ICON.CPSI_NONE;
            return HResultValues.E_NOTIMPL;
        }

        public int ReportResult(int ntsStatus, int ntsSubstatus, out string ppszOptionalStatusText,
            out _CREDENTIAL_PROVIDER_STATUS_ICON pcpsiOptionalStatusIcon)
        {
            ppszOptionalStatusText = string.Empty;
            pcpsiOptionalStatusIcon = _CREDENTIAL_PROVIDER_STATUS_ICON.CPSI_NONE;
            return HResultValues.E_NOTIMPL;
        }

        public int GetFieldDescriptorCount(out uint pdwCount)
        {
            pdwCount = 0;
            return HResultValues.E_NOTIMPL;
        }

        public int GetFieldDescriptorAt(uint dwIndex, IntPtr ppcpfd)
        {
            return HResultValues.E_NOTIMPL;
        }

        public int GetCredentialCount(out uint pdwCount, out uint pdwDefault, out int pbAutoLogonWithDefault)
        {
            pdwCount = 0;
            pdwDefault = 0;
            pbAutoLogonWithDefault = 0;
            return HResultValues.E_NOTIMPL;
        }

        public int GetCredentialAt(uint dwIndex, out ICredentialProviderCredential ppcpc)
        {
            ppcpc = (ICredentialProviderCredential)this;
            return HResultValues.S_OK;
        }

        public async void Dispose()
        {
            if (isDisposing)
            {
                return;
            }

            await Log.LogText("TestWindowsCredentialProvider: Disposed");
            isDisposing = true;
        }
    }
}
