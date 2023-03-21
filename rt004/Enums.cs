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
        public double gloss;
        public double specC;

        public Material(double gloss, double specC)
        {
            this.gloss = gloss;
            this.specC = specC;
        }
    }
}
