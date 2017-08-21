namespace WindowsCredentialProviderTest
{
    public static class HResultValues
    {
        public const int S_OK = 0x00000000;
        public const int S_FALSE = 0x00000001;
        public const int E_NOTIMPL = unchecked((int) 0x80004001);
        public const int E_FAIL = unchecked((int) 0x80004005);
        public const int E_INVALIDARG = unchecked ((int) 0x80070057);
    }
}
