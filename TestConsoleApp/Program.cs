namespace TestConsoleApp
{
    using System.Net;

    class Program
    {
        static void Main(string[] args)
        {
            //ITestWindowsCredentialProvider pC = new TestWindowsCredentialProvider();
            //IntPtr pUnk = Marshal.GetIUnknownForObject(pC);
            //IntPtr pI;
            //var guidBase = typeof(ICredentialProvider).GUID;
            //var hr = Marshal.QueryInterface(pUnk, ref guidBase, out pI);

            var networkCredential = CredentialsDialog.GetCredentials("Hey!", "We would like a login.");
        }
    }
}
