
namespace rt004
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Controller.LoadFromJson();
            Controller.GenerateCaptures();
            Console.WriteLine("Finnished");
        }
    }
}