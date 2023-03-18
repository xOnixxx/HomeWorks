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
    }


    internal class Light : ILight
    {
        public point3D origin { set; get; }
        public float intensity;
        public float[] color;

        public Light(point3D origin, float intensity, float[] color)
        {
            this.origin = origin;
            this.intensity = intensity;
            this.color = color;
        }
    }
}
