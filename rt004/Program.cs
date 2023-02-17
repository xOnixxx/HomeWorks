using Util;
//using System.Numerics;

namespace rt004
{
  internal class Program
  {
    static void Main(string[] args)
    {
      // Parameters.
      // TODO: parse command-line arguments and/or your config file.
      int wid = 600;
      int hei = 450;
      string fileName = "demo.pfm";

      // HDR image.
      FloatImage fi = new FloatImage(wid, hei, 3);

      // TODO: put anything interesting into the image.
      // TODO: use fi.PutPixel() function, pixel should be a float[3] array [R, G, B]

      //fi.SaveHDR(fileName);   // Doesn't work well yet...
      fi.SavePFM(fileName);

      Console.WriteLine("HDR image is finished.");
    }
  }
}
