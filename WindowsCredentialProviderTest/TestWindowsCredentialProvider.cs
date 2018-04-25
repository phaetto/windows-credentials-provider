namespace WindowsCredentialProviderTest
{
    using System;
    using System.Runtime.InteropServices;
    using CredentialProvider.Interop;

    [ComVisible(true)]
    [Guid(Constants.CredentialProviderUID)]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Rebootify.TestWindowsCredentialProvider")]
    public class TestWindowsCredentialProvider : ITestWindowsCredentialProvider
    {
        private _CREDENTIAL_PROVIDER_USAGE_SCENARIO usageScenario = _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_INVALID;
        private TestWindowsCredentialProviderTile credentialTile = null;
        internal ICredentialProviderEvents CredentialProviderEvents;
        internal uint CredentialProviderEventsAdviseContext = 0;

        public TestWindowsCredentialProvider()
        {
            Log.LogText("TestWindowsCredentialProvider: Created object");
        }

        public int SetUsageScenario(_CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus, uint dwFlags)
        {
            Log.LogMethodCall();

            usageScenario = cpus;

            switch (cpus)
            {
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CREDUI:
                    return HResultValues.S_OK;

                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_LOGON:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_UNLOCK_WORKSTATION:
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
                CredentialProviderEventsAdviseContext = upAdviseContext;
                CredentialProviderEvents = pcpe;
                var intPtr = Marshal.GetIUnknownForObject(pcpe);
                Marshal.AddRef(intPtr);
            }

            return HResultValues.S_OK;
        }

        public int UnAdvise()
        {
            Log.LogMethodCall();

            if (CredentialProviderEvents != null)
            {
                var intPtr = Marshal.GetIUnknownForObject(CredentialProviderEvents);
                Marshal.Release(intPtr);
                CredentialProviderEvents = null;
                CredentialProviderEventsAdviseContext = 0;
            }

            return HResultValues.S_OK;
        }

        public int GetFieldDescriptorCount(out uint pdwCount)
        {
            Log.LogMethodCall();
            pdwCount = (uint)credentialTile.CredentialProviderFieldDescriptorList.Count;
            return HResultValues.S_OK;
        }

        public int GetFieldDescriptorAt(uint dwIndex, [Out] IntPtr ppcpfd) /* _CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR** */
        {
            Log.LogMethodCall();

            if (dwIndex >= credentialTile.CredentialProviderFieldDescriptorList.Count)
            {
                return HResultValues.E_INVALIDARG;
            }

            var listItem = credentialTile.CredentialProviderFieldDescriptorList[(int) dwIndex];
            var pcpfd = Marshal.AllocCoTaskMem(Marshal.SizeOf(listItem)); /* _CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR* */
            Marshal.StructureToPtr(listItem, pcpfd, false); /* pcpfd = &CredentialProviderFieldDescriptorList */
            Marshal.StructureToPtr(pcpfd, ppcpfd, false); /* *ppcpfd = pcpfd */

            return HResultValues.S_OK;
        }

        public int GetCredentialCount(out uint pdwCount, out uint pdwDefault, out int pbAutoLogonWithDefault)
        {
            Log.LogMethodCall();

            pdwCount = 1; // Credential tiles number
            pdwDefault = unchecked ((uint)0);
            pbAutoLogonWithDefault = 0; // Try to auto-logon when all credential managers are enumerated (before the tile selection)
            return HResultValues.S_OK;
        }

        public int GetCredentialAt(uint dwIndex, out ICredentialProviderCredential ppcpc)
        {
            Log.LogMethodCall();

            if (credentialTile == null)
            {
                credentialTile = new TestWindowsCredentialProviderTile(this, usageScenario);
            }

            ppcpc = (ICredentialProviderCredential)credentialTile;
            return HResultValues.S_OK;
        }
    }
}
