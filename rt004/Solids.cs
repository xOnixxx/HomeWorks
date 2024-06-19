using System.ComponentModel.Design.Serialization;
using System.IO.IsolatedStorage;
using System.Text.Json.Serialization;
using OpenTK.Mathematics;


namespace rt004
{

    public enum AngleType
    {
        Rad,
        Degrees,
    }



    [JsonPolymorphic(
    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    [JsonDerivedType(typeof(Sphere3D))]
    //Represents solids which will be saved in the scene, every solid is parametrically described with its parametric function
    public interface ISolids
    {
        public double? GetIntersection(Ray ray, bool inside = false);
        public Vector3d GetNormal(Vector3d pointOfIntersection);
        public Matrix4d Transform { get; set; }

        //Center point for calculating the intersection
        public float[] color { get; set; }
        public Material material { get; set; }

        //We use this to pass the universal components for different size restrictions, i.e radius for sphere, vector parameters for plane etc.
        public double[] size_param { get; set; }
    }


    public class Sphere3D : ISolids
    {
        
        //public point3D origin { get; set; }
        public float[] color { get; set; }
        public Material material { get; set; }
        public Matrix4d Transform { get; set; }
        //radius
        public double[] size_param { get; set; }
        
        private double? Quadratic(double a, double b, double c)
        {
            if ((b * b - 4 * a * c) <= 0)
            {
                return null;
            }
            
            double delta = Math.Sqrt(b * b - 4 * a * c);
            double x1 = (-b + delta) / 2 * a;
            double x2 = (-b - delta) / 2 * a;


            if (x1 < 0 && x2 < 0) { return null; }
            if (x1 > 0 && x2 < 0) { return x1; }
            if (x1 < 0 && x2 > 0) { return x2; }

            return Math.Min(x1, x2);
        }

        private double? QuadraticMax(double a, double b, double c)
        {
            if ((b * b - 4 * a * c) <= 0)
            {
                return null;
            }

            double delta = Math.Sqrt(b * b - 4 * a * c);
            double x1 = (-b + delta) / 2 * a;
            double x2 = (-b - delta) / 2 * a;


            if (x1 < 0 && x2 < 0) { return null; }
            return Math.Max(x1, x2);
        }


        public double? GetIntersection(Ray ray, bool inside = false)
        {
            //double a = ray.direction3d.LenghtSquared;
            //double b = 2 * (Vector3d.Dot(ray.direction3d, d));

            //Vector3d origin = new Vector3d(Transform.tmInverse * new Vector4d(ray.origin3d, 1));
            //Vector3d direction = new Vector3d(Transform.tmInverse * new Vector4d(ray.direction3d, 0));

            /*
            Vector3d direction = Vector3d.TransformVector(ray.direction3d, tempM);
            tempM.Transpose();
            Vector3d origin = Vector3d.TransformPosition(ray.origin3d, tempM);
            */

            //Vector3d origin = Vector3d.TransformPosition(ray.origin3d, tempMatrix);
            //Vector3d direction = Vector3d.TransformVector(ray.direction3d, tempMatrix);

            Ray viewer = ray;

            Vector3d d = viewer.origin3d;

            double a = Vector3d.Dot(viewer.direction3d, viewer.direction3d);
            double b = 2 * (Vector3d.Dot(viewer.direction3d, d));
            double c = Vector3d.Dot(d,d) - 1;
            double? t;
            if (inside)
            {
                t = QuadraticMax(a, b, c);
            }
            else
            {
                t = Quadratic(a, b, c);
            }



            return t;
        }
        //TODO ADD transformation on the point
        public Vector3d GetNormal(Vector3d intersection)
        {

            return -Vector3d.Subtract(Vector3d.Zero,intersection).Normalized();
        }

        
    }

    public class Plane3D : ISolids
    {
        public Matrix4d Transform { get; set; }
        public Material material { get; set; }
        public float[] color { get; set; }
        public double[] size_param { get; set; }
        public Vector3d normal = new Vector3d(0, 1, 0);

        public double? GetIntersection(Ray ray, bool inside = false)
        {
            /*
            Vector3d origin = new Vector3d();
            double denom = -Vector3d.Dot(origin, -normal);
            double dotProduct = Vector3d.Dot(-normal, origin);
            double t = (Vector3d.Dot(-normal, origin) + denom) / Vector3d.Dot(normal, ray.direction);
            if (dotProduct < RayTracer.EPSILON && t < RayTracer.EPSILON)
            {
                return null;
            }
            */

            Vector3d origin = new Vector3d(0, 0,0);
            double originCheck = Vector3d.Dot(Vector3d.Subtract(origin, ray.origin3d), normal);
            double bottom = Vector3d.Dot(ray.direction3d, normal);
            if (bottom == 0) { return null; }
            double result = originCheck / bottom;
            if (result < 0) { return null; }
            return result;
        }
        public Vector3d GetNormal(Vector3d intersection)
        {
            //intersection = new Vector3d(Transform.tM * new Vector4d(intersection, 1));
            return this.normal;
        }


        public Plane3D(double[] size_param, float[] color, Material material)
        {
            this.normal = new Vector3d(0,1,0);//new Vector3d(size_param[0], size_param[1], size_param[2]);
            this.color = color;
            this.material = material;
        }
        
        public Plane3D() { }


    }
}
