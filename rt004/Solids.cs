using System.Text.Json.Serialization;
using OpenTK.Mathematics;


namespace rt004
{

    [JsonPolymorphic(
    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    [JsonDerivedType(typeof(Sphere3D))]
    //Represents solids which will be saved in the scene, every solid is parametrically described with its parametric function
    public interface ISolids
    {
        public double? GetIntersection(Ray ray, bool inside = false);
        public bool GetUnion(Vector3d point, bool inside = false);
        public Vector3d GetNormal(Vector3d pointOfIntersection, Vector3d incidentDirection);
        public Vector3d GetTexture(Vector3d pointOfIntersection);
        public Matrix4d Transform { get; set; }

        //Center point for calculating the intersection
        public Vector3d color { get; set; }
        public Material material { get; set; }

        //We use this to pass the universal components for different size restrictions, i.e radius for sphere, vector parameters for plane etc.
        //Soon obsolete (transformation matrices will substitute this)
        public double[] size_param { get; set; }
    }

    public abstract class ComplexSolid3D : ISolids
    {
        public abstract Matrix4d Transform { get; set; }
        public abstract Material material { get; set; }
        public abstract Vector3d color { get; set; }
        public abstract double[] size_param { get; set; }

        public abstract ISolids[] primitives { get; set; }


        public bool GetUnion(Vector3d point, bool inside = false)
        {
            foreach (ISolids primitive in primitives)
            {
                if (primitive.GetUnion(point, inside)) {
                    return true;
                }
            }
            return false;
        }

        public double? GetIntersection(Ray ray, bool inside = false)
        {
            double? tempDistance = null;
            double? outDistance = double.MaxValue;

            foreach (ISolids primitive in primitives)
            {
                tempDistance = primitive.GetIntersection(ray, inside);
                if (tempDistance < outDistance && tempDistance > MathHelp.EPSILON)
                {
                    outDistance = tempDistance;
                }
            }
            if (outDistance == double.MaxValue)
            {
                return null;
            }
            return outDistance;

        }

        public Vector3d GetNormal(Vector3d intersection, Vector3d incidentDirection)
        {
            foreach (ISolids primitive in primitives)
            {
                if (primitive.GetUnion(intersection)) { return primitive.GetNormal(intersection, incidentDirection); }
                
            }
            return Vector3d.Zero;
        }

        public abstract Vector3d GetTexture(Vector3d pointOfIntersection);



    }

    public class Sphere3D : ISolids
    {
        
        //public point3D origin { get; set; }
        public Vector3d color { get; set; }
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

        //TODO
        public bool GetUnion(Vector3d point, bool inside = false)
        {
            return false;
        }

        public double? GetIntersection(Ray ray, bool further = false)
        {

            Ray viewer = ray;

            Vector3d d = viewer.origin3d;

            double a = Vector3d.Dot(viewer.direction3d, viewer.direction3d);
            double b = 2 * (Vector3d.Dot(viewer.direction3d, d));
            double c = Vector3d.Dot(d,d) - 1;
            double? t;
            if (further)
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
        public Vector3d GetNormal(Vector3d intersection, Vector3d incidentDirection)
        {

            return Vector3d.Subtract(intersection, Vector3d.Zero).Normalized();
        }

        public Vector3d GetTexture(Vector3d pointOfIntersection)
        {
            return color;
        }

        internal Sphere3D() { }

        [JsonConstructor]
        public Sphere3D(float[] color, Material material)
        {
            this.color = MathHelp.ArrayToVec(color);
            this.material = material;
        }

    }

    public class Triangle3D : ISolids
    {
        public Matrix4d Transform { get; set; }
        public Material material { get; set; }
        public Vector3d color { get; set; }
        public double[] size_param { get; set; }

        //Vertices for equilateral triangle
        private Vector3d v0 = new Vector3d(-MathHelp.EQUILATERAL_CONST, -0.5, 0);
        private Vector3d v1 = new Vector3d(MathHelp.EQUILATERAL_CONST, -0.5, 0);
        private Vector3d v2 = new Vector3d(0, 1, 0);

        public bool GetUnion(Vector3d point, bool inside = false)
        {
            return (MathHelp.TestSameSide(point, v0, v1, v2));
        }


        //Möller–Trumbore
        public double? GetIntersection(Ray ray, bool inside = false)
        {
            Vector3d e1 = v1 - v0;
            Vector3d e2 = v2 - v0;
            Vector3d cross_e2 = Vector3d.Cross(ray.direction3d, e2);
            double det = Vector3d.Dot(e1, cross_e2);

            if (det > -MathHelp.EPSILON && det < MathHelp.EPSILON) { return null; }

            double inv_det = 1 / det;
            Vector3d s = ray.origin3d - v0;
            double u = inv_det * Vector3d.Dot(s, cross_e2);
            
            if (u < 0 || u > 1) { return null; }

            Vector3d cross_e1 = Vector3d.Cross(s, e1);
            double v = inv_det * Vector3d.Dot(ray.direction3d, cross_e1);

            if (v < 0 || u + v > 1) { return null; }

            double t = inv_det * Vector3d.Dot(e2, cross_e1);
            if (t > MathHelp.EPSILON) { return t; }
            else { return null; }
        }


        public Vector3d GetTexture(Vector3d pointOfIntersection)
        {
            return color;
        }


        public  Vector3d GetNormal(Vector3d intersection, Vector3d incidentDirection)
        {
            Vector3d edge1 = v1 - v0;
            Vector3d edge2 = v2 - v0;
            Vector3d normal = Vector3d.Cross(edge1, edge2).Normalized();
            var xx = Vector3d.Dot(normal, incidentDirection);
            return Vector3d.Dot(normal, incidentDirection) > 0 ? normal : -normal;
        }

        internal Triangle3D() { }
        internal Triangle3D(Vector3d v0, Vector3d v1, Vector3d v2)
        {
            this.v0 = v0;
            this.v1 = v1; 
            this.v2 = v2;
        }

        [JsonConstructor]
        public Triangle3D(float[] color, Material material)
        {
            this.color = MathHelp.ArrayToVec(color);
            this.material = material;
        }

    }

    public class Square3D : ComplexSolid3D
    {
        public override Matrix4d Transform { get; set; }
        public override Material material { get; set; }
        public override Vector3d color { get; set; }
        public override double[] size_param { get; set; }

        public override ISolids[] primitives { get; set; }
            

        public override Vector3d GetTexture(Vector3d pointOfIntersection)
        {
            return color;
        }

        internal Square3D() { }
        internal Square3D(Vector3d v0, Vector3d v1, Vector3d v2, Vector3d v3){
          this.primitives = new Triangle3D[2]
          {new Triangle3D(v0,v1,v2),
          new Triangle3D(v1,v3,v2)};
    
        }


        [JsonConstructor]
        public Square3D(float[] color, Material material)
        {
            this.color = MathHelp.ArrayToVec(color);
            this.material = material;
            this.primitives = new ISolids[2]
            {new Triangle3D(new Vector3d(-0.5, -0.5,0),new Vector3d(0.5, -0.5,0),new Vector3d(-0.5, 0.5,0)),
            new Triangle3D(new Vector3d(0.5, -0.5,0),new Vector3d(0.5, 0.5,0),new Vector3d(-0.5, 0.5,0)) };
        }
    }


    public class Prism3D : ComplexSolid3D
    {
        public override Matrix4d Transform { get; set; }
        public override Material material { get; set; }
        public override Vector3d color { get; set; }
        public override double[] size_param { get; set; }
        public override ISolids[] primitives { get; set; }


        public override Vector3d GetTexture(Vector3d pointOfIntersection)
        {
            return color;
        }

        internal Prism3D() { }

        [JsonConstructor]
        public Prism3D(float[] color, Material material)
        {
            this.color = MathHelp.ArrayToVec(color);
            this.material = material;
            this.primitives = new ISolids[5]
            {
            //Back face
            new Triangle3D(new Vector3d(-MathHelp.EQUILATERAL_CONST,-0.5,0.5), new Vector3d(MathHelp.EQUILATERAL_CONST,-0.5,0.5),new Vector3d(0,1,0.5)),

            //Front face
            new Triangle3D( new Vector3d(0,1,-0.5),new Vector3d(MathHelp.EQUILATERAL_CONST,-0.5,-0.5),new Vector3d(-MathHelp.EQUILATERAL_CONST,-0.5,-0.5)),
            
            //Left Side
            new Square3D(new Vector3d(-MathHelp.EQUILATERAL_CONST,-0.5,-0.5), new Vector3d(-MathHelp.EQUILATERAL_CONST,-0.5,0.5),new Vector3d(0,1,-0.5),new Vector3d(0,1,0.5)),

            //Right Side
            new Square3D(new Vector3d(MathHelp.EQUILATERAL_CONST,-0.5,0.5), new Vector3d(MathHelp.EQUILATERAL_CONST,-0.5,-0.5),new Vector3d(0,1,0.5), new Vector3d(0,1,-0.5)), 

            //Bottom Side
            new Square3D(new Vector3d(MathHelp.EQUILATERAL_CONST,-0.5,0.5), new Vector3d(MathHelp.EQUILATERAL_CONST,-0.5,-0.5),new Vector3d(-MathHelp.EQUILATERAL_CONST,-0.5,0.5), new Vector3d(-MathHelp.EQUILATERAL_CONST,-0.5,-0.5))

            };
        }
    }


    public class Plane3D : ISolids
    {
        public Matrix4d Transform { get; set; }
        public Material material { get; set; }
        public Vector3d color { get; set; }
        public double[] size_param { get; set; }
        public Vector3d normal = new Vector3d(0, 1, 0);

        public bool GetUnion(Vector3d point, bool inside = false)
        {
            return false;
        }


        public double? GetIntersection(Ray ray, bool inside = false)
        { 
            Vector3d origin = new Vector3d(0, 0,0);
            double originCheck = Vector3d.Dot(Vector3d.Subtract(origin, ray.origin3d), normal);
            double bottom = Vector3d.Dot(ray.direction3d, normal);
            if (bottom == 0) { return null; }
            double result = originCheck / bottom;
            if (result < 0) { return null; }
            return result;
        }
        public Vector3d GetNormal(Vector3d intersection, Vector3d incidentDirection)
        {
            return Vector3d.Dot(normal, incidentDirection) > 0 ? normal : -normal;
        }

        public Vector3d GetTexture(Vector3d intersection)
        {
            int xi = Math.Abs((int)Math.Floor(intersection.X / 2));
            int zi = Math.Abs((int)Math.Floor(intersection.Z / 2));
            if ((xi + zi) % 2 == 0)
            {
                return Vector3d.One * 0.1;
            }
            else
            {
                return Vector3d.One*0.8;
            }
        }

        [JsonConstructor]
        public Plane3D(float[] color, Material material)
        {
            this.normal = new Vector3d(0,1,0);//new Vector3d(size_param[0], size_param[1], size_param[2]);
            this.color = MathHelp.ArrayToVec(color);
            this.material = material;
        }

    }
}
