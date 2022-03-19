using Stateless;
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
            var entityStateMachine = GetEntityStateMachine();

            Console.WriteLine($"Current State:{entityStateMachine}");

            string graph = UmlDotGraph.Format(entityStateMachine.GetInfo());
            var urlGraph = HttpUtility.UrlEncode(graph.Replace(" ", ""));
            var url = "https://edotor.net/?engine=dot#" + urlGraph;

            OpenBrowser(url);

        }

        static StateMachine<EntityState, EntityTrigger> GetEntityStateMachine()
        {
            // TODO: add information about LoadLocallyFailed LoadFromServerFailed
            // TODO: how do you model that?

            // Entity Configuration
            var entityStateMachine = new StateMachine<EntityState, EntityTrigger>(EntityState.NotSet);

            entityStateMachine.Configure(EntityState.NotSet)
                .Permit(EntityTrigger.LoadFromServer, EntityState.Loading);

            // TODO: How do you model load failed
            // TODO: add load locally

            entityStateMachine.Configure(EntityState.Loading)
                .Permit(EntityTrigger.LoadComplete, EntityState.Loaded);

            entityStateMachine.Configure(EntityState.Loaded)
                .Permit(EntityTrigger.ChangeValues, EntityState.Changed)
                .Permit(EntityTrigger.Close, EntityState.NotSet);

            entityStateMachine.Configure(EntityState.Changed)
                .Permit(EntityTrigger.Save, EntityState.Saved)
                .Permit(EntityTrigger.SaveAndClose, EntityState.NotSet)
                .Permit(EntityTrigger.Discard, EntityState.NotSet);

            return entityStateMachine;
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

    public enum EntityState
    {
        NotSet,
        Loaded,
        Loading,
        Changed,
        Saved
    }

    public enum EntityTrigger
    {
        LoadLocally,
        LoadFromServer,
        LoadComplete,
        ChangeValues,
        Save,
        SaveAndClose,
        Discard,
        Close
    }
}
