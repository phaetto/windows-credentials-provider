namespace TestConsoleApp
{
    using System;

    class Program
    {
        static void Main()
        {
            var networkCredential = CredentialsDialog.GetCredentials("Hey!", "We would like a login.");

            if (networkCredential != null)
            {
                Console.WriteLine($"Username: \'{networkCredential.UserName}\'");
            }
            else
            {
                Console.WriteLine("No credential detected.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
