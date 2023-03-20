using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    public enum LoadMode
    {
        FileMode,
        ConsoleMode,
    }

    public enum CameraMode
    {
        Perspective,
        Fish,
        Cylindrical,
    }
    public enum ImageFormat
    {
        HDR,
        PFM
    }

    public struct Material
    {
        public string name;
        public double gloss;
        public Material(string name, double gloss)
        {
            this.name = name;
            this.gloss = gloss;
        }
    }
}
