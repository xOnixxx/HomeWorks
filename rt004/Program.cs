using Util;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text.Json;
using Newtonsoft.Json;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using System.Text.Json.Serialization;
using System.Numerics;
using OpenTK.Windowing.Common.Input;
using System.Xml.Linq;
//using System.Numerics;

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