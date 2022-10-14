using System;

namespace MinicraftGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Minicraft())
                game.Run();
        }
    }
}
