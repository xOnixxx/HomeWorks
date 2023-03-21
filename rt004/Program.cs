using Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using OpenTK.Mathematics;
//using System.Numerics;

namespace rt004
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int wid = 600;
            int hei = 600;
            string fileName = "demo.pfm";
            ImageFormat format = ImageFormat.PFM;
            Scene scene = new Scene();
            ISolids[] solids = new ISolids[3];
            ILight[] lights = new ILight[2];
            Material metal = new Material();
            metal.gloss = 10;
            Material plastic = new Material();
            plastic.gloss = 500;
            Material plastic2 = new Material();
            plastic2.gloss = 10;
            Sphere3D sphere = new Sphere3D(new Vector3d(0.5d,0.5d,3), 0.5d, new float[] {0,1,1}, metal);
            Sphere3D sphere2 = new Sphere3D(new Vector3d(0d, 0d, 4), 1d, new float[] { 0, 0, 1 }, plastic);
            Plane3D plane = new Plane3D(new point3D(new Vector3d(0, -2, 5)), new Vector3d(0, 1, 0), new float[] { 0.5f, 0, 0.3f }, plastic);
            Light light = new Light(new point3D(new Vector3d(0, 5, -2)), 1, new float[] { 1f, 1f, 1f });
            Light light2 = new Light(new point3D(new Vector3d(5, 5, 0)), 1, new float[] { 1f, 1f, 1f });

            solids[0] = sphere;
            solids[1] = sphere2;
            solids[2] = plane;
            lights[0] = light;
            lights[1] = light2;
            scene.scene = solids;
            scene.lights = lights;
            scene.diffuseC = 0.6d;
            scene.specularC = 0.4d;
            Camera camera = new Camera(new point3D(new Vector3d(0, 0, 0)), new Vector3d(0d, 0d, 1d), new Vector3d(0,1,0), 25*Math.PI/180, 1080/1080);
            FloatImage img = camera.RenderScene(scene);

            img.SavePFM(fileName);

            // Parameters.
            // TODO: parse command-line arguments and/or your config file.
            /*
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
            */
        }
    }
}