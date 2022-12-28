using RyFern.Clients.DX;
using RyFern.Runner.Demos;

namespace RyFern.Runner
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            using var client = new RandomShapesClient();
            client.Run();
        }
    }
}