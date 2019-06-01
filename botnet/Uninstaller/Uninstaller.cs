using System.Threading;

namespace Botnet
{
    class Uninstaller
    {
        static void Main(string[] args)
        {
            Thread.Sleep(10000);
            Utility.Uninstall();
        }
    }
}