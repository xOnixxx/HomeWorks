using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace rt004
{

    public interface ILights
    {
        public double[] jsonORIGIN { get; set; }
        public Vector3d origin { get; set; }
        public Vector3d color { get; set; }
        public double intensity { get; set; }
    }


    internal class Light : ILights
    {
        public double[] jsonORIGIN { get { return new double[] { }; } set { origin = new Vector3d(value[0], value[1], value[2]); } }
        public Vector3d origin { get; set; }
        public double intensity { get; set; }
        public Vector3d color { set; get; }

        [JsonConstructor]
        public Light(Vector3 origin, float intensity, float[] color)
        {
            this.origin = origin;
            this.intensity = intensity;
            this.color = MathHelp.ArrayToVec(color);
        }
    }
}
