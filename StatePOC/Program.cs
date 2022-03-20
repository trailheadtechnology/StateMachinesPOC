using Stateless.Graph;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Web;

namespace StatePOC
{
    class Program
    {
        static void Main(string[] args)
        {
            var entityStateMachine = new EntityStateMachine();

            Console.WriteLine($"Current State:{entityStateMachine.StateMachine}");

            // Show machine
            string graph = UmlDotGraph.Format(entityStateMachine.StateMachine.GetInfo());
            var urlGraph = HttpUtility.UrlEncode(graph.Replace(" ", ""));
            var url = "https://edotor.net/?engine=dot#" + urlGraph;

            OpenBrowser(url);

        }

        public static void OpenBrowser(string url)
        {
            // https://stackoverflow.com/questions/14982746/open-a-browser-with-a-specific-url-by-console-application
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }


}
