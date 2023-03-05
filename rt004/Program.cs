using Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
//using System.Numerics;

namespace rt004
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int wid = 600;
            int hei = 450;
            string fileName = "demo.pfm";
            ImageFormat format = ImageFormat.PFM;
            Mode mode = Mode.ConsoleMode;
            // Parameters.
            // TODO: parse command-line arguments and/or your config file.
            if (args.Length == 0)
            {
                Console.WriteLine("Using defualt values");
            }
            if (args.Length is > 0)
            {
                //Read file mode, takes no further arguments
                if (args[0] == "F" && args.Length == 2)
                {
                    //TODO: read info from file;
                    using FileStream stream = File.OpenRead(args[1]);
                    string text = File.ReadAllText(args[1]);
                    Parameter param = JsonSerializer.Deserialize<Parameter>(text);
                    wid = param.wid;
                    hei = param.hei;
                    if (param.format == "hdr")
                    {
                        format = ImageFormat.HDR;
                        fileName = "demo.hdr";
                    }
                    else if (param.format == "pfm")
                    {
                        format = ImageFormat.PFM;
                        fileName = "demo.pfm";
                    }
                }
                //Read arguments from command line, takes additional 3 arguments
                else if (args[0] == "C" && args.Length is  <= 4 and >= 3)
                {
                    try
                    {
                        wid = int.Parse(args[1]);
                        hei = int.Parse(args[2]);
                    }
                    catch
                    {
                        throw new Exception("Invalid resolution arguments.");
                    }
                    if (args.Length > 3)
                    {
                        if (args[3].ToLower() == "hdr")
                        {
                            format = ImageFormat.HDR;
                            fileName = "demo.hdr";
                        }
                        else if (args[3].ToLower() == "pfm")
                        {
                            format = ImageFormat.PFM;
                            fileName = "demo.pfm";
                        }
                    }
                }
                else
                {
                    Console.WriteLine(args[0]);
                    throw new Exception("Wrong arguments");
                }
            }

            // HDR image.
            FloatImage fi = new FloatImage(wid, hei, 3);
            // TODO: put anything interesting into the image.
            // TODO: use fi.PutPixel() function, pixel should be a float[3] array [R, G, B]
            for (int x = 0; x < wid; x++)
            {
                for (int y = 0; y < hei; y++)
                {
                    float[] tempC = new float[] { (x * y) % 255, (x * x) % 255, (y * y) % 255 };
                    fi.PutPixel(x, y, tempC);
                }
            }

            if (format == ImageFormat.HDR)
            {
                fi.SaveHDR(fileName);
                Console.WriteLine("HDR Image generated");
            }
            else if (format == ImageFormat.PFM)
            {
                fi.SavePFM(fileName);
                Console.WriteLine("PFM Image generated");
            }
        }
    }
}