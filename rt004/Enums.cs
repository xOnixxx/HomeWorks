using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        public double gloss { get; set; }
        public double specCoef { get; set; }
        public double diffuseCoef { get; set; }
        public bool transparent { get; set; }
        public double transparentCoef { get; set; }
        public string albedo {  get; set; }
        public string normals { get; set; }


        [JsonConstructor]
        public Material(double gloss, double specCoef, double diffuseCoef, bool transparent, double transparentCoef, string albedo, string normals)
        {
            this.gloss = gloss;
            this.specCoef = specCoef;
            this.diffuseCoef = diffuseCoef;
            this.transparentCoef = transparentCoef;
            this.transparent = transparent;

            this.albedo = albedo;
            this.normals = normals;
        }
    }
}
