namespace TestConsoleApp
{
    using System;
    using System.Net;

    class Program
    {
        static void Main(string[] args)
        {
            var networkCredential = CredentialsDialog.GetCredentials("Hey!", "We would like a login.");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
