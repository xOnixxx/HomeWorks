
using System.Drawing;

namespace rt004
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Controller.LoadTextures();
            Controller.LoadFromJson();
            Controller.GenerateCaptures();
            Console.WriteLine("Finnished");
        }
    }
}