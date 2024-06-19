using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    internal static class MathHelp
    {
        public static double? GetIntersect(Ray ray, Scene scene, out ISolids closest, out Ray transRay, out Matrix4d reverseTransform, ISolids self = null, bool inside = false)
        {

            SolidHierarchy hierarchy = scene.solidHierarchy;
            SolidHierarchyContainer container = new SolidHierarchyContainer(hierarchy.root);

            double? tempDistance = null;
            double? outDistance = double.MaxValue;

            transRay = new Ray();
            closest = new Sphere3D();
            reverseTransform = new Matrix4d();
            Ray originalRay = ray;

            foreach (Node node in container)
            {
                foreach (ISolids solid in node.solids)
                {   
                    ray = RayTransform(ray, solid.Transform);
                    
                    if (inside && solid == self)
                    {
                        tempDistance = solid.GetIntersection(ray, inside);

                        if (tempDistance < outDistance && tempDistance > RayTracer.EPSILON)
                        {
                            transRay = ray;
                            outDistance = tempDistance;
                            closest = solid;
                            reverseTransform = solid.Transform;
                        }
                        ray = originalRay;
                    }
                    else 
                    { 
                        tempDistance = solid.GetIntersection(ray);

                        if (tempDistance < outDistance && solid != self && tempDistance > RayTracer.EPSILON)
                        {
                            transRay = ray;
                            outDistance = tempDistance;
                            closest = solid;
                            reverseTransform = solid.Transform;
                        }
                        ray = originalRay;
                    }

                }

            }
            if (outDistance == double.MaxValue)
            {
                return null;
            }
            return outDistance;
        }


        public static Ray RayTransform(Ray ray, Matrix4d transform)
        {
            Ray outRay = new Ray();
            Matrix4d rayTrans = transform.Inverted();

            outRay.direction3d = new Vector3d(rayTrans * new Vector4d(ray.direction3d, 0)).Normalized();
            outRay.origin3d = new Vector3d(rayTrans * new Vector4d(ray.origin3d, 1));
            


            return outRay;
        }
    }


}
