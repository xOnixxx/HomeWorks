
namespace rt004
{
    internal class Program
    {
        /*TODOS
         * Refraction inside transparent material
         */
        static void Main(string[] args)
        {

            Controller generator = new Controller();
            generator.LoadFromJson();
            generator.GenerateCaptures();
            Console.WriteLine("Finnished");
        }
    }
}