using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Study.CustomUrlScheme
{
    class Program
    {
        static string ProcessInput(string s)
        {
            // TODO Verify and validate the input 
            // string as appropriate for your application.
            return s;
        }

        static void Main(string[] args)
        {
            //https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/platform-apis/aa767914(v=vs.85)
            //https://stackoverflow.com/questions/27248542/how-do-i-properly-register-a-protocol-handler-on-windows-8
            //https://stackoverflow.com/questions/35626050/registering-custom-url-handler-in-c-sharp-on-windows-8
            const String PROTOCOL = "banana";
            const String ProgramName = "Study.CustomUrlScheme.exe";
            Console.WriteLine(ProgramName + " invoked with the following parameters.\r\n");
            Console.WriteLine("Raw command-line: \n\t" + Environment.CommandLine);

            Console.WriteLine("\n\nArguments:\n");
            
            foreach (string s in args)
            {
                Console.WriteLine("\t" + ProcessInput(s));
            }
            
            if (args != null && args.Length == 1)
            {
                if (args[0] == "-u")
                {
                    Console.WriteLine("UnRegister " + ProgramName);
                    UrlProtocolHelper.UnRegister(PROTOCOL);
                }

                if (args[0] == "-i")
                {
                    Console.WriteLine("Register " + ProgramName);
                    UrlProtocolHelper.Register(PROTOCOL, Process.GetCurrentProcess().MainModule.FileName, "Banana Test Program");
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

    }
}
