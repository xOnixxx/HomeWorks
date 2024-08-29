using OpenTK.Mathematics;

namespace rt004
{
    public struct Ray
    {
        public Vector3d origin3d { get { return origin3d_; } set { origin3d_ = value; } }
        public Vector3d direction3d { get { return direction3d_; } set { direction3d_ = value; } }

        private Vector3d direction3d_;

        private Vector3d origin3d_;


        public Ray(Vector3d origin, Vector3d direction) { this.origin3d = origin; this.direction3d = direction.Normalized(); }

    }

    internal static class MathHelp
    {
        public const double EPSILON = 1.0e-6;
        public const double MAXIMON = 1.0e+16;
        public const double EQUILATERAL_CONST = 0.86602540378d;
        public static List<Vector2d> NRooksSample(Vector2d original, int spp)
        {
            List<Vector2d> samples = new List<Vector2d>(spp);

            List<int> size = Enumerable.Range(0, spp).ToList();

            List<int> permutation1 = GetRandomPermutation(size);
            List<int> permutation2 = GetRandomPermutation(size);


            for (int i = 0; i < spp; i++)
            {
                samples.Add(original + new Vector2d(((double)permutation1[i]) / spp, ((double)permutation2[i]) / spp));
            }
            return samples;
        }

        public static List<Vector2d> Jittering(Vector2d original, int spp)
        {
            int dim = (int)Math.Sqrt(spp);
            List<Vector2d> samples = new List<Vector2d>(dim);
            Random rnd = new Random();

            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    samples.Add(original + new Vector2d(((double)(i) + rnd.NextDouble()) / dim, ((double)(j) + rnd.NextDouble()) / dim));
                }
            }

            return samples;
        }


        static List<int> GetRandomPermutation(List<int> list)
        {
            Random rng = new Random();
            List<int> copy = new List<int>(list);

            for (int i = copy.Count - 1; i > 0; i--)
            {
                int j = rng.Next(0, i + 1);
                Swap(copy, i, j);
            }

            return copy;
        }

        static void Swap(List<int> list, int a, int b)
        {
            int temp = list[a];
            list[a] = list[b];
            list[b] = temp;
        }

        //Todo add soft shadows
        public static double GetShadowMultiplier(Ray ray, Scene scene, ILights light, ISolids intersected)
        {
            SolidHierarchy hierarchy = scene.solidHierarchy;
            SolidHierarchyContainer container = new SolidHierarchyContainer(hierarchy.root);

            double? tempDistance = null;

            double shadowMultiplier = 1.0;
            double lightDistance = Vector3d.Distance(light.origin, ray.origin3d);

            Ray originalRay = ray;

            foreach (Node node in container)
            {
                if (node.solids != null)
                {
                    foreach (ISolids solid in node.solids)
                    {
                        ray = RayTransform(ray, solid.Transform);

                        //Do math for transformed ray
                        if (intersected == solid) { tempDistance = solid.GetIntersection(ray, true); }
                        else {tempDistance = solid.GetIntersection(ray); }


                        if (tempDistance != null && tempDistance is not double.NaN)
                        {
                            Vector3d pointLocal = ray.origin3d + ray.direction3d * (double)tempDistance;
                            // Vector3d pointReal = new Vector3d(ReverseTrans * new Vector4d(point, 1));

                            Vector3d pointGlobal = new Vector3d(solid.Transform * new Vector4d(pointLocal, 1));
                            tempDistance = Vector3d.Distance(pointGlobal, originalRay.origin3d);
                        }



                        //The solid casts shadow
                        if (tempDistance is not null && lightDistance > tempDistance && tempDistance > EPSILON)
                        {
                            if (!solid.material.transparent) {
                                return 0.0; }
                            else { shadowMultiplier *= 0.8; }
                        }
                        ray = originalRay;
                    }
                }


            }

            return shadowMultiplier;
        }

        public static double? GetIntersect(Ray ray, Scene scene, out ISolids closest, out Ray transRay, out Matrix4d reverseTransform, ISolids self = null, bool inside = false)
        {

            SolidHierarchy hierarchy = scene.solidHierarchy;
            SolidHierarchyContainer container = new SolidHierarchyContainer(hierarchy.root);

            double? realDistance = null;
            double? tempDistance = null;
            double? outDistance = double.MaxValue;
            double? returnDistance = double.MaxValue;

            transRay = new Ray();
            closest = new Sphere3D();
            reverseTransform = new Matrix4d();
            Ray originalRay = ray;

            //Traverse the scene solids
            foreach (Node node in container)
            {
                if (node.solids != null)
                {
                    foreach (ISolids solid in node.solids)
                    {   
                        ray = RayTransform(ray, solid.Transform);
                    
                        if (inside && solid == self)
                        {
                            tempDistance = solid.GetIntersection(ray, inside);

                            if (tempDistance < outDistance && tempDistance > EPSILON)
                            {
                                transRay = ray;
                                outDistance = tempDistance;
                                closest = solid;
                                reverseTransform = solid.Transform;
                            }
                        }
                        else 
                        { 
                            tempDistance = solid.GetIntersection(ray);

                            //Transformed point
                            if (tempDistance != null && tempDistance is not double.NaN)
                            {
                                Vector3d pointLocal = ray.origin3d + ray.direction3d*(double)tempDistance;
                                // Vector3d pointReal = new Vector3d(ReverseTrans * new Vector4d(point, 1));

                                Vector3d pointGlobal = new Vector3d(solid.Transform * new Vector4d(pointLocal, 1));
                                realDistance = Vector3d.Distance(pointGlobal,originalRay.origin3d);
                            }
                            else { realDistance = double.MinValue; }
                           


                            if (realDistance < outDistance && realDistance > EPSILON && solid != self)
                            {
                                transRay = ray;
                                outDistance = realDistance;
                                returnDistance = tempDistance;
                                closest = solid;
                                reverseTransform = solid.Transform;
                            }
                        
                        }
                        ray = originalRay;
                    }
                }


            }
            if (returnDistance == double.MaxValue)
            {
                return null;
            }
            return returnDistance;
        }


        public static bool TestSameSide(Vector3d p1, Vector3d a, Vector3d b, Vector3d c)
        {
            
            a -= p1;
            b -= p1;
            c -= p1;
            
            Vector3d u = Vector3d.Cross(b,c);
            Vector3d v = Vector3d.Cross(c, a);
            Vector3d w = Vector3d.Cross(a, b);

            if (Vector3d.Dot(u, v) < 0f)
            {
                return false;
            }
            if (Vector3d.Dot(u, w) < 0.0f)
            {
                return false;
            }

            // All normals facing the same way, return true
            return true;
        }


        public static Ray RayTransform(Ray ray, Matrix4d transform)
        {
            Ray outRay = new Ray();
            Matrix4d rayTrans = transform.Inverted();

            outRay.direction3d = new Vector3d(rayTrans * new Vector4d(ray.direction3d, 0)).Normalized();
            outRay.origin3d = new Vector3d(rayTrans * new Vector4d(ray.origin3d, 1));
            
            return outRay;
        }
        internal static void ArrayToVec(ref Vector3d vector, float[] array)
        {
            vector.X = array[0];
            vector.Y = array[1];
            vector.Z = array[2];
        }

        internal static Vector3d ArrayToVec(float[] array)
        {
            Vector3d vector = new Vector3d();
            vector.X = array[0];
            vector.Y = array[1];
            vector.Z = array[2];
            return vector;
        }
    }


}
