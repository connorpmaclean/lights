namespace Executable
{
    using System;
    using System.Threading.Tasks;
    using Lights.Core;

    public class LightsExecutable
    {
        public static async Task Main(string[] args)
        {
            await LightsCore.Run();
        }
    }
}
