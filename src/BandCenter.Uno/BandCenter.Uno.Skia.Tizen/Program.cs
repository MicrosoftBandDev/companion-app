using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace BandCenter.Uno.Skia.Tizen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new BandCenter.Uno.App(), args);
            host.Run();
        }
    }
}
