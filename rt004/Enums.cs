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
        public double specCoef;
        public double diffuseCoef;
        public Material(double gloss, double specCoef, double diffuseCoef)
        {
            this.gloss = gloss;
            this.specCoef = specCoef;
            this.diffuseCoef = diffuseCoef;
        }
    }
}
