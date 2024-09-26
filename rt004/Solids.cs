using System.Text.Json.Serialization;
using Microsoft.VisualBasic;
using OpenTK.Mathematics;
using Util;
using System.Drawing;


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

        public ISolids? GetFace(Vector3d point, bool inside = false)
        {
            foreach (ISolids primitive in primitives)
            {
                if (primitive.GetUnion(point, inside))
                {
                    return primitive;
                }
            }
            return null;
        }

        public virtual Vector3d GetTexture(Vector3d pointOfIntersection)
        {
            //return color;
            if (material.albedo != null)
            {
                ISolids? face = GetFace(pointOfIntersection);
                if (face == null)
                {
                    return Vector3d.Zero;
                }
                return face.GetTexture(pointOfIntersection);
            }
            else { return color; }

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

            Vector3d normal = Vector3d.Subtract(intersection, Vector3d.Zero).Normalized();
            normal = Vector3d.Dot(normal, incidentDirection) > 0 ? normal : -normal;
            if (material.normals == null)
            {
                return normal;
            }


            double u = 0.5 + Math.Atan2(intersection.Z, intersection.X) / (2 * Math.PI);
            double v = 0.5 - Math.Asin(intersection.Y) / Math.PI;

            Bitmap normalMap = Controller.textures[material.normals + ".png"];

            Color clr = normalMap.GetPixel((int)(Math.Floor(u * normalMap.Width)), (int)(Math.Floor(v * normalMap.Height)));
            Vector3d tmpNormal = new Vector3d((double)(clr.R) / 255.0, (double)(clr.G) / 255.0, (double)(clr.B) / 255.0);
            if (Math.Floor(u * normalMap.Width + 1) < normalMap.Width &&
                Math.Floor(u * normalMap.Width - 1) > 0 &&
                Math.Floor(v * normalMap.Width + 1) < normalMap.Height &&
                Math.Floor(v * normalMap.Width - 1) > 0
                )
            {
                Color clr1 = normalMap.GetPixel((int)(Math.Floor(u * normalMap.Width + 1)), (int)(Math.Floor(v * normalMap.Height)));
                Color clr2 = normalMap.GetPixel((int)(Math.Floor(u * normalMap.Width)), (int)(Math.Floor(v * normalMap.Height + 1)));
                Color clr3 = normalMap.GetPixel((int)(Math.Floor(u * normalMap.Width - 1)), (int)(Math.Floor(v * normalMap.Height)));
                Color clr4 = normalMap.GetPixel((int)(Math.Floor(u * normalMap.Width)), (int)(Math.Floor(v * normalMap.Height - 1)));

                double R = clr.R + clr1.R + clr2.R + clr3.R + clr4.R;
                double G = clr.G + clr1.G + clr2.G + clr3.G + clr4.G;
                double B = clr.B + clr1.B + clr2.B + clr3.B + clr4.B;


                tmpNormal = new Vector3d((double)(R) / (255.0*5), (double)(G) / (255.0 * 5), (double)(B) / (255.0 * 5));
            }

            double theta = u * (2 * Math.PI);
            double phi = v * (Math.PI);
            Vector3d Tangent = new Vector3d(-Math.Sin(theta)*Math.Sin(phi), Math.Cos(theta)*Math.Sin(phi), 0);
            Vector3d Bitangent = new Vector3d(Math.Cos(theta) * Math.Cos(phi), Math.Sin(theta) * Math.Cos(phi), -Math.Sin(phi));

            tmpNormal = tmpNormal * 2.0 - Vector3d.One;

            Vector3d trueNormal = Tangent*tmpNormal.X + Bitangent*tmpNormal.Y + normal*tmpNormal.Z;

            return normal;


        }

        public Vector3d GetTexture(Vector3d intersection)
        {
            if (material.albedo == null)
            {
                return color;
            }
            double u = 0.5 + Math.Atan2(intersection.Z, intersection.X)/(2*Math.PI);
            double v = 0.5 - Math.Asin(intersection.Y) / Math.PI;
            //return color;

            Bitmap albedoMap = Controller.textures[material.albedo + ".png"];

            Color clr = albedoMap.GetPixel((int)(Math.Floor(u*albedoMap.Width)),(int)(Math.Floor(v * albedoMap.Height)));
            Vector3d tmpAlbedo = new Vector3d((double)(clr.R) / 255.0, (double)(clr.G) / 255.0, (double)(clr.B) / 255.0);

            if (Math.Floor(u * albedoMap.Width + 1) < albedoMap.Width &&
                Math.Floor(u * albedoMap.Width - 1) > 0 &&
                Math.Floor(v * albedoMap.Width + 1) < albedoMap.Height &&
                Math.Floor(v * albedoMap.Width - 1) > 0
                )
            {
                Color clr1 = albedoMap.GetPixel((int)(Math.Floor(u * albedoMap.Width + 1)), (int)(Math.Floor(v * albedoMap.Height)));
                Color clr2 = albedoMap.GetPixel((int)(Math.Floor(u * albedoMap.Width)), (int)(Math.Floor(v * albedoMap.Height + 1)));
                Color clr3 = albedoMap.GetPixel((int)(Math.Floor(u * albedoMap.Width - 1)), (int)(Math.Floor(v * albedoMap.Height)));
                Color clr4 = albedoMap.GetPixel((int)(Math.Floor(u * albedoMap.Width)), (int)(Math.Floor(v * albedoMap.Height - 1)));

                double R = clr.R + clr1.R + clr2.R + clr3.R + clr4.R;
                double G = clr.G + clr1.G + clr2.G + clr3.G + clr4.G;
                double B = clr.B + clr1.B + clr2.B + clr3.B + clr4.B;


                tmpAlbedo = new Vector3d((double)(R) / (255.0 * 5), (double)(G) / (255.0 * 5), (double)(B) / (255.0 * 5));
            }

            return tmpAlbedo;

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

        private Vector2d uv0 = new Vector2d(0, 0);
        private Vector2d uv1 = new Vector2d(2*MathHelp.EQUILATERAL_CONST, 0);
        private Vector2d uv2 = new Vector2d(MathHelp.EQUILATERAL_CONST, 1.5);

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


        public Vector3d GetTexture(Vector3d intersection)
        {
            if (material.albedo == null)
            {
                return color;
            }
            Vector2d uv = MathHelp.MapToUVTriangle(intersection, v0, v1, v2, uv0, uv1, uv2);
            Bitmap albedoMap = Controller.textures[material.albedo+".png"];
            double u = uv.X - (Math.Floor(uv.X));
            double v = uv.Y - (Math.Floor(uv.Y));
            u *= albedoMap.Width - 1;
            v *= albedoMap.Height - 1;

            Color clr = albedoMap.GetPixel((int)u,(int)v);
            return new Vector3d((double)(clr.R) / 255.0, (double)(clr.G) / 255.0, (double)(clr.B) / 255.0);
            // return color;
        }


        public  Vector3d GetNormal(Vector3d intersection, Vector3d incidentDirection)
        {
            Vector3d edge1 = v1 - v0;
            Vector3d edge2 = v2 - v0;
            Vector3d normal = Vector3d.Cross(edge1, edge2).Normalized();
            normal = Vector3d.Dot(normal, incidentDirection) > 0 ? normal : -normal;
            if (material.normals == null)
            {
                return normal;
            }
            Vector2d uv = MathHelp.MapToUVTriangle(intersection, v0, v1, v2, uv0, uv1, uv2);
            Bitmap normalMap = Controller.textures[material.normals + ".png"];
            double u = uv.X - (Math.Floor(uv.X));
            double v = uv.Y - (Math.Floor(uv.Y));
            u *= normalMap.Width - 1;
            v *= normalMap.Height - 1;

            Color clr = normalMap.GetPixel((int)u, (int)v);
            Vector3d tmpNormal = new Vector3d((double)(clr.R) / 255.0, (double)(clr.G) / 255.0, (double)(clr.B) / 255.0);

            Vector3d Tangent = new Vector3d(1,0,0);
            Vector3d Bitangent = new Vector3d(0, 1, 0);
            Vector3d trueNormal = Tangent * tmpNormal.X + Bitangent * tmpNormal.Y + normal * tmpNormal.Z;

            return trueNormal;
        }

        internal Triangle3D() { }
        internal Triangle3D(Vector3d v0, Vector3d v1, Vector3d v2, Vector2d uv0, Vector2d uv1, Vector2d uv2, Material mat)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;

            this.uv0 = uv0;
            this.uv1 = uv1;
            this.uv2 = uv2;

            this.material = mat;
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
            

        internal Square3D() { }

        //V0 left lower point
        //V3 right upper point
        internal Square3D(Vector3d v0, Vector3d v1, Vector3d v2, Vector3d v3){

            Vector2d uv0 = v0.Xy + v3.Xy;
            Vector2d uv1 = v1.Xy + v3.Xy;
            Vector2d uv2 = v2.Xy + v3.Xy;
            Vector2d uv3 = v3.Xy + v3.Xy;

            this.primitives = new Triangle3D[2]
            {new Triangle3D(v0,v1,v2, uv0, uv1, uv2, material),
            new Triangle3D(v1,v3,v2, uv1, uv3, uv2, material)};
    
        }


        [JsonConstructor]
        public Square3D(float[] color, Material material)
        {
            this.color = MathHelp.ArrayToVec(color);
            this.material = material;
            this.primitives = new ISolids[2]
            {new Triangle3D(new Vector3d(-0.5, -0.5,0),new Vector3d(0.5, -0.5,0),new Vector3d(-0.5, 0.5,0), new Vector2d(0,0), new Vector2d(1,0),new Vector2d(0,1), material ),
            new Triangle3D(new Vector3d(0.5, -0.5,0),new Vector3d(0.5, 0.5,0),new Vector3d(-0.5, 0.5,0),  new Vector2d(1,0), new Vector2d(1,1),new Vector2d(0,1), material ) };
        }
    }

    public class Prism3D : ComplexSolid3D
    {
        public override Matrix4d Transform { get; set; }
        public override Material material { get; set; }
        public override Vector3d color { get; set; }
        public override double[] size_param { get; set; }
        public override ISolids[] primitives { get; set; }

        internal Prism3D() { }

        public override Vector3d GetTexture(Vector3d pointOfIntersection)
        {
            return color;
        }

        [JsonConstructor]
        public Prism3D(float[] color, Material material)
        {
            this.color = MathHelp.ArrayToVec(color);
            this.material = material;
            this.primitives = new ISolids[5]
            {
            //Back face
            new Triangle3D(new Vector3d(-MathHelp.EQUILATERAL_CONST,-0.5,0.5), new Vector3d(MathHelp.EQUILATERAL_CONST,-0.5,0.5),new Vector3d(0,1,0.5), new Vector2d(0, 0),new Vector2d(2*MathHelp.EQUILATERAL_CONST, 0), new Vector2d(MathHelp.EQUILATERAL_CONST, 1.5), material ),

            //Front face
            new Triangle3D( new Vector3d(0,1,-0.5),new Vector3d(MathHelp.EQUILATERAL_CONST,-0.5,-0.5), new Vector3d(0,1,0.5), new Vector2d(0, 0),new Vector2d(2*MathHelp.EQUILATERAL_CONST, 0), new Vector2d(MathHelp.EQUILATERAL_CONST, 1.5), material),
            
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
            double denom = Vector3d.Dot(normal, ray.direction3d);
            if (Math.Abs(denom) > MathHelp.EPSILON)
            {
                double t = Vector3d.Dot((Vector3d.Zero - ray.origin3d),normal) / denom;
                if (t >= 0) return t;
            }
            return null;
        }
        public Vector3d GetNormal(Vector3d intersection, Vector3d incidentDirection)
        {
            Vector3d normal = Vector3d.Dot(this.normal, incidentDirection) > 0 ? this.normal : -this.normal;
            if (material.albedo == null)
            {
                return normal;
            }
            //normal = 
            return normal;
        }

        public Vector3d GetTexture(Vector3d intersection)
        {
            if (material.albedo == null)
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


            Bitmap albedoMap = Controller.textures[material.albedo + ".png"];
            double u = (int)intersection.X % albedoMap.Width;
            double v = (int)intersection.Y % albedoMap.Height;
            Color clr = albedoMap.GetPixel((int)u, (int)v);


            Vector3d tmpAlbedo = new Vector3d((double)(clr.R) / 255.0, (double)(clr.G) / 255.0, (double)(clr.B) / 255.0);

            if (Math.Floor(u * albedoMap.Width + 1) < albedoMap.Width &&
                Math.Floor(u * albedoMap.Width - 1) > 0 &&
                Math.Floor(v * albedoMap.Width + 1) < albedoMap.Height &&
                Math.Floor(v * albedoMap.Width - 1) > 0)
            {
                Color clr1 = albedoMap.GetPixel((int)(Math.Floor(u * albedoMap.Width + 1)), (int)(Math.Floor(v * albedoMap.Height)));
                Color clr2 = albedoMap.GetPixel((int)(Math.Floor(u * albedoMap.Width)), (int)(Math.Floor(v * albedoMap.Height + 1)));
                Color clr3 = albedoMap.GetPixel((int)(Math.Floor(u * albedoMap.Width - 1)), (int)(Math.Floor(v * albedoMap.Height)));
                Color clr4 = albedoMap.GetPixel((int)(Math.Floor(u * albedoMap.Width)), (int)(Math.Floor(v * albedoMap.Height - 1)));

                double R = clr.R + clr1.R + clr2.R + clr3.R + clr4.R;
                double G = clr.G + clr1.G + clr2.G + clr3.G + clr4.G;
                double B = clr.B + clr1.B + clr2.B + clr3.B + clr4.B;


                tmpAlbedo = new Vector3d((double)(R) / (255.0 * 5), (double)(G) / (255.0 * 5), (double)(B) / (255.0 * 5));
            }
            return tmpAlbedo;
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
