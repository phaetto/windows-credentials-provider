namespace WindowsCredentialProviderTest
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;

    public static class Log
    {
        private const string LogContextFormat = @"
{{
    Type: ""LogServer.Contexts.ConsoleLog.Commands.LogToConsole"",
    DataType: ""LogServer.Contexts.ConsoleLog.Commands.Data.LogData"",
    Data: {{ Text: ""{0}"" }}
}}
";

        private static readonly HttpClient Client = new HttpClient();

        public static async Task LogText(string text)
        {
            try
            {
                await Client.PostAsync("http://localhost:5050/log", new StringContent(string.Format(LogContextFormat, text), Encoding.UTF8, "application/json"));
            }
            catch (Exception e)
            {
                try
                {
                    Console.Write("NOT THROWN: ");
                    Console.WriteLine(e);
                }
                catch (Exception)
                {
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }
    }
}
