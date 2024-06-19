using OpenTK.Mathematics;
using System.Text.Json.Serialization;


namespace rt004
{
    [JsonPolymorphic(
    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    [JsonDerivedType(typeof(Translate))]
    public interface ITransformations
    {
        public double[] Transformation { get; set; }
        public Matrix4d tM { get; set; }
        public Matrix4d tmInverse { get; set; }

        //Basic Transformation
        public Ray MultiplyL(Ray ray);

        //Reverse Transformation
        public Ray MultiplyR(Ray ray);
        //public Matrix4d transform { get; set; }
        public Matrix4d Inverse();
    }

    public struct Translate : ITransformations
    {
        public Matrix4d tM { get; set; }
        public Matrix4d tmInverse { get; set; }
        public double[] Transformation
        {
            get {  return new double[] { };  }
            set {  this.tM = new Matrix4d(
                value[0], value[1], value[2], value[3],
                value[4], value[5], value[6], value[7],
                value[8], value[9], value[10], value[11],
                value[12], value[13], value[14], value[15]
                );
            }
        }
        public Matrix4d Inverse()
        {
            return tM.Inverted();
        }

        public Translate(double x, double y, double z)
        {
            tM = new Matrix4d(
            1, 0, 0, x,
            0, 1, 0, y,
            0, 0, 1, z,
            0, 0, 0, 1
            );
        } 

        public Translate(Matrix4d tM) { this.tM = tM;}

        //When translating we do not change direction vector
        public Ray MultiplyL(Ray ray)
        {
            //Vector3d origin = new Vector3d(tM.Inverted() * new Vector4d(ray.origin3d,1));
            //Vector3d direction = new Vector3d(tM.Inverted() * new Vector4d(ray.direction3d, 0));
            Matrix4d tempMatrix = tM.Inverted();

            Vector3d origin = new Vector3d(tempMatrix * new Vector4d(ray.origin3d, 1));
            Vector3d direction = new Vector3d(tempMatrix * new Vector4d(ray.direction3d, 0)).Normalized();


            return new Ray(origin, direction);
        }

        //TODO REVERSE
        public Ray MultiplyR(Ray ray)
        {
            Vector3d origin = new Vector3d(tM * new Vector4d(ray.origin3d, 1));
            Vector3d direction = new Vector3d(tM * new Vector4d(ray.direction3d, 1));

            return new Ray(origin, direction);
        }

    }

}
