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
            catch (Exception)
            {
                try
                {
                    Console.WriteLine("(Skipped logging)");
                }
                catch (Exception)
                {
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LogMethodCall()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            var methodBase = sf.GetMethod();
#pragma warning disable 4014
            LogText(methodBase.DeclaringType?.Name + "::" + methodBase.Name);
#pragma warning restore 4014
        }
    }
}
