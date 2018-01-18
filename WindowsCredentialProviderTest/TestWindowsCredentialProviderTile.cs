// Uncomment for autologin
// #define AUTOLOGIN

using CredentialProvider.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsCredentialProviderTest.OnDemandLogon;

namespace WindowsCredentialProviderTest
{
    [ComVisible(true)]
    [Guid(Constants.CredentialProviderTileUID)]
    [ClassInterface(ClassInterfaceType.None)]
    public sealed class TestWindowsCredentialProviderTile : ITestWindowsCredentialProviderTile
    {
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

        private readonly TestWindowsCredentialProvider testWindowsCredentialProvider;
        private readonly _CREDENTIAL_PROVIDER_USAGE_SCENARIO usageScenario;
        private ICredentialProviderCredentialEvents credentialProviderCredentialEvents;
        private TimerOnDemandLogon timerOnDemandLogon;
        private bool shouldAutoLogin = false;

        public TestWindowsCredentialProviderTile(
            TestWindowsCredentialProvider testWindowsCredentialProvider,
            _CREDENTIAL_PROVIDER_USAGE_SCENARIO usageScenario
        )
        {
            this.testWindowsCredentialProvider = testWindowsCredentialProvider;
            this.usageScenario = usageScenario;
        }

        public int Advise(ICredentialProviderCredentialEvents pcpce)
        {
            Log.LogMethodCall();

            if (pcpce != null)
            {
                credentialProviderCredentialEvents = pcpce;
                var intPtr = Marshal.GetIUnknownForObject(pcpce);
                Marshal.AddRef(intPtr);
            }

            return HResultValues.S_OK;
        }

        public int UnAdvise()
        {
            Log.LogMethodCall();

            if (credentialProviderCredentialEvents != null)
            {
                var intPtr = Marshal.GetIUnknownForObject(credentialProviderCredentialEvents);
                Marshal.Release(intPtr);
                credentialProviderCredentialEvents = null;
            }

            return HResultValues.S_OK;
        }

        public int SetSelected(out int pbAutoLogon)
        {
            Log.LogMethodCall();

#if AUTOLOGIN 
            if (!shouldAutoLogin)
            {
                timerOnDemandLogon = new TimerOnDemandLogon(
                    testWindowsCredentialProvider.CredentialProviderEvents,
                    credentialProviderCredentialEvents,
                    this,
                    CredentialProviderFieldDescriptorList[0].dwFieldID,
                    testWindowsCredentialProvider.CredentialProviderEventsAdviseContext);

                timerOnDemandLogon.TimerEnded += TimerOnDemandLogon_TimerEnded;

                pbAutoLogon = 0;
            }
            else
            {
                // We got the info from the async timer
                pbAutoLogon = 1;
            }
#else
            pbAutoLogon = 0; // Auto-logon when the tile is selected
#endif

            return HResultValues.S_OK;
        }

        private void TimerOnDemandLogon_TimerEnded()
        {
            // Sync other data from your async service here
            shouldAutoLogin = true;
        }

        public int SetDeselected()
        {
            Log.LogMethodCall();

            timerOnDemandLogon?.Dispose();
            timerOnDemandLogon = null;

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

            var searchFunction = FieldSearchFunctionGenerator(dwFieldID, new []
            {
                _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_SMALL_TEXT,
                _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_LARGE_TEXT,
            });

            if (!CredentialProviderFieldDescriptorList.Any(searchFunction))
            {
                ppsz = string.Empty;
                return HResultValues.E_NOTIMPL;
            }

            var descriptor = CredentialProviderFieldDescriptorList.First(searchFunction);

            ppsz = descriptor.pszLabel;
            return HResultValues.S_OK;
        }

        public int GetBitmapValue(uint dwFieldID, IntPtr phbmp)
        {
            Log.LogMethodCall();

            var searchFunction = FieldSearchFunctionGenerator(dwFieldID, new[] { _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_TILE_IMAGE });

            if (!CredentialProviderFieldDescriptorList.Any(searchFunction))
            {
                phbmp = IntPtr.Zero;
                return HResultValues.E_NOTIMPL;
            }

            var descriptor = CredentialProviderFieldDescriptorList.First(searchFunction);
            phbmp = IntPtr.Zero; // TODO: show a bitmap

            return HResultValues.E_NOTIMPL;
        }

        public int GetCheckboxValue(uint dwFieldID, out int pbChecked, out string ppszLabel)
        {
            Log.LogMethodCall();

            var searchFunction = FieldSearchFunctionGenerator(dwFieldID, new[] { _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_CHECKBOX });

            if (!CredentialProviderFieldDescriptorList.Any(searchFunction))
            {
                pbChecked = 0;
                ppszLabel = string.Empty;
                return HResultValues.E_NOTIMPL;
            }

            var descriptor = CredentialProviderFieldDescriptorList.First(searchFunction);
            pbChecked = 0; // TODO: selection state
            ppszLabel = descriptor.pszLabel;

            return HResultValues.E_NOTIMPL;
        }

        public int GetSubmitButtonValue(uint dwFieldID, out uint pdwAdjacentTo)
        {
            Log.LogMethodCall();

            var searchFunction = FieldSearchFunctionGenerator(dwFieldID, new [] { _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_SUBMIT_BUTTON });

            if (!CredentialProviderFieldDescriptorList.Any(searchFunction))
            {
                pdwAdjacentTo = 0;
                return HResultValues.E_NOTIMPL;
            }

            var descriptor = CredentialProviderFieldDescriptorList.First(searchFunction);

            pdwAdjacentTo = descriptor.dwFieldID - 1; // TODO: selection state

            return HResultValues.S_OK;
        }

        public int GetComboBoxValueCount(uint dwFieldID, out uint pcItems, out uint pdwSelectedItem)
        {
            Log.LogMethodCall();

            var searchFunction = FieldSearchFunctionGenerator(dwFieldID, new[] { _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_COMBOBOX });

            if (!CredentialProviderFieldDescriptorList.Any(searchFunction))
            {
                pcItems = 0;
                pdwSelectedItem = 0;
                return HResultValues.E_NOTIMPL;
            }

            var descriptor = CredentialProviderFieldDescriptorList.First(searchFunction);
            pcItems = 0; // TODO: selection state
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

            // TODO: change state

            return HResultValues.E_NOTIMPL;
        }

        public int SetCheckboxValue(uint dwFieldID, int bChecked)
        {
            Log.LogMethodCall();

            // TODO: change state

            return HResultValues.E_NOTIMPL;
        }

        public int SetComboBoxSelectedValue(uint dwFieldID, uint dwSelectedItem)
        {
            Log.LogMethodCall();

            // TODO: change state

            return HResultValues.E_NOTIMPL;
        }

        public int CommandLinkClicked(uint dwFieldID)
        {
            Log.LogMethodCall();

            // TODO: change state

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
            finally
            {
                shouldAutoLogin = false; // Block auto-login from going full-retard
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

        private Func<_CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR, bool> FieldSearchFunctionGenerator(uint dwFieldID, _CREDENTIAL_PROVIDER_FIELD_TYPE[] allowedFieldTypes)
        {
            return x =>
                x.dwFieldID == dwFieldID
                && allowedFieldTypes.Contains(x.cpft);
        }
    }
}
