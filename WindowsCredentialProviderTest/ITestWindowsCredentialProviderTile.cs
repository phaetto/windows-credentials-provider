namespace WindowsCredentialProviderTest
{
    using System.Runtime.InteropServices;
    using CredentialProvider.Interop;

    [ComVisible(true)]
    [Guid("5419A510-AB52-40AD-9D95-A8F2E58AAE6B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITestWindowsCredentialProviderTile : ICredentialProviderCredential
    {
    }
}