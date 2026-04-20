using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CatMergeRowPaw
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Console.WriteLine("Program.Main start");
                using var game = new Game1();
                Console.WriteLine("Game created");
                game.Run();
                Console.WriteLine("Game.Run returned");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
        }
    }
}
