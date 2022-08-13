using System;

namespace Minicraft
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MinicraftGame())
                game.Run();
        }
    }
}
