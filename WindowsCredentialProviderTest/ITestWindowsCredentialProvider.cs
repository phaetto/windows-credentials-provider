namespace WindowsCredentialProviderTest
{
    using System.Runtime.InteropServices;
    using CredentialProvider.Interop;

    [ComVisible(true)]
    [Guid("093755F9-E026-4F61-84A8-485665F57ED0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITestWindowsCredentialProvider: ICredentialProvider
    {
    }
}