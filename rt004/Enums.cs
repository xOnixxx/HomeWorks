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

    public enum SolidType
    {
        Sphere,
        Plane
    }

    public struct Material
    {
        public double gloss { get; set; }
        public double specCoef { get; set; }
        public double diffuseCoef { get; set; }
        public bool transparent { get; set; }
        public double transparentCoef { get; set; }
        public Material(double gloss, double specCoef, double diffuseCoef, bool transparent, double transparentCoef)
        {
            this.gloss = gloss;
            this.specCoef = specCoef;
            this.diffuseCoef = diffuseCoef;
            this.transparentCoef = transparentCoef;
            this.transparent = transparent;
        }
    }
}
