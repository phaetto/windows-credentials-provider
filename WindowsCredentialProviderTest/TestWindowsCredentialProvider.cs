namespace WindowsCredentialProviderTest
{
    using System;
    using System.Runtime.InteropServices;
    using CredentialProvider.Interop;

    [ComVisible(true)]
    [Guid("298D9F84-9BC5-435C-9FC2-EB3746625954")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Rebootify.TestWindowsCredentialProvider")]
    public class TestWindowsCredentialProvider : ITestWindowsCredentialProvider
    {
        private bool isDisposing;
        private ICredentialProviderEvents credentialProviderEvents;
        private ITestWindowsCredentialProviderTile credentialTile = new TestWindowsCredentialProviderTile();
        private _CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR credentialProviderFieldDescriptor = new _CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR
        {
            cpft = _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_SMALL_TEXT,
            dwFieldID = 0,
            pszLabel = "Rebootify Awesomeness",
        };

        public TestWindowsCredentialProvider()
        {
            Log.LogText("TestWindowsCredentialProvider: Created object");
        }

        public int SetUsageScenario(_CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus, uint dwFlags)
        {
            Log.LogMethodCall();

            switch (cpus)
            {
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CREDUI:
                    return HResultValues.S_OK;

                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_UNLOCK_WORKSTATION:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_LOGON:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CHANGE_PASSWORD:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_PLAP:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_INVALID:
                    return HResultValues.E_NOTIMPL;
                default:
                    return HResultValues.E_INVALIDARG;
            }
        }

        public int SetSerialization(ref _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION pcpcs)
        {
            Log.LogMethodCall();
            return HResultValues.E_NOTIMPL;
        }

        public int Advise(ICredentialProviderEvents pcpe, uint upAdviseContext)
        {
            Log.LogMethodCall();

            if (pcpe != null)
            {
                credentialProviderEvents = pcpe;
                var intPtr = Marshal.GetIUnknownForObject(pcpe);
                Marshal.AddRef(intPtr);
            }

            return HResultValues.S_OK;
        }

        public int UnAdvise()
        {
            Log.LogMethodCall();

            if (credentialProviderEvents != null)
            {
                var intPtr = Marshal.GetIUnknownForObject(credentialProviderEvents);
                Marshal.Release(intPtr);
                credentialProviderEvents = null;
            }

            return HResultValues.S_OK;
        }

        public int GetFieldDescriptorCount(out uint pdwCount)
        {
            Log.LogMethodCall();
            pdwCount = 1;
            return HResultValues.S_OK;
        }

        public int GetFieldDescriptorAt(uint dwIndex, [Out] IntPtr ppcpfd) /* _CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR** */
        {
            Log.LogMethodCall();

            if (dwIndex > 0)
            {
                return HResultValues.E_INVALIDARG;
            }

            // TODO: garbage collect
            var pcpfd = Marshal.AllocHGlobal(Marshal.SizeOf(credentialProviderFieldDescriptor)); /* _CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR* */
            Marshal.StructureToPtr(credentialProviderFieldDescriptor, pcpfd, false); /* pcpfd = &credentialProviderFieldDescriptor */
            Marshal.StructureToPtr(pcpfd, ppcpfd, false); /* *ppcpfd = pcpfd */

            return HResultValues.S_OK;
        }

        public int GetCredentialCount(out uint pdwCount, out uint pdwDefault, out int pbAutoLogonWithDefault)
        {
            Log.LogMethodCall();

            pdwCount = 1;
            pdwDefault = unchecked ((uint)-1);
            pbAutoLogonWithDefault = 0;
            return HResultValues.S_OK;
        }

        public int GetCredentialAt(uint dwIndex, out ICredentialProviderCredential ppcpc)
        {
            Log.LogMethodCall();
            ppcpc = (ICredentialProviderCredential)credentialTile;
            return HResultValues.S_OK;
        }
    }
}
