using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{

    public interface ILight
    {
        public point3D origin { get; set; }
        public float[] color { get; set; }
        public double intensity { get; set; }
    }


    internal class Light : ILight
    {
        public point3D origin { set; get; }
        public double intensity { get; set; }
        public float[] color { set; get; }

        public Light(point3D origin, float intensity, float[] color)
        {
            this.origin = origin;
            this.intensity = intensity;
            this.color = color;
        }
    }
}
