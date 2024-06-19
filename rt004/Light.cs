using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{

    public interface ILights
    {
        public double[] jsonORIGIN { get; set; }
        public Vector3d origin { get; set; }
        public float[] color { get; set; }
        public double intensity { get; set; }
    }


    internal class Light : ILights
    {
        public double[] jsonORIGIN { get { return new double[] { }; } set { origin = new Vector3d(value[0], value[1], value[2]); } }
        public Vector3d origin { get; set; }
        public double intensity { get; set; }
        public float[] color { set; get; }

        public Light(Vector3d origin, float intensity, float[] color)
        {
            this.origin = origin;
            this.intensity = intensity;
            this.color = color;
        }
    }
}
