using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace rt004
{
    static class RayTracer
    {
        //Calculates colors using the Phong functions 
        static public float[] Phong(ISolids solid, double distance, Scene scene, ray3D ray)
        {
            double[] matColor = new double[3];
            double[] returnColor = new double[3];
            solid.color.CopyTo(matColor, 0);
            double Ediff = 0;
            double Espec = 0;
            ray3D shadowRay;


            point3D pointOfInt = new point3D(Vector3d.Multiply(ray.direction, distance));
            shadowRay.origin = pointOfInt;
            Vector3d normal = Vector3d.Normalize(solid.GetNormal(pointOfInt));
            //Vector3d ldirection = Vector3d.Normalize(scene.lights[0].origin.vector3 - pointOfInt.vector3);

            foreach(var light in scene.lights) 
            {
                Vector3d ldirection = Vector3d.Normalize(light.origin.vector3 - pointOfInt.vector3);
                shadowRay.direction = ldirection;
                //Checks if the ray is blocked by another solid
                if (!CheckShadow(scene, shadowRay, light, solid))
                {
<<<<<<< Updated upstream
                    double distanceComp = LightDisComp(light, pointOfInt, 0.04, 0.05, 1.0e-3d);
                    Ediff = PhongDiff(light, scene.diffuseC, normal, pointOfInt, ldirection);
                    Ediff /= distanceComp;
=======

                    double lightComp = LightDisComp(light, pointOfInt, 0.04d, 0.05d, 1.0e-4d);
                    Ediff = PhongDiff(light, scene.diffuseC, normal, pointOfInt, ldirection);
                    Ediff /= lightComp;
>>>>>>> Stashed changes
                    returnColor[0] += Ediff * matColor[0];
                    returnColor[1] += Ediff * matColor[1];
                    returnColor[2] += Ediff * matColor[2];

                    
                    Vector3d refRay = 2*normal*(Vector3d.Dot(normal, ldirection)) - ldirection;
                    Espec = PhongSpec(light, solid.material, scene.specularC, ray.direction, refRay);
<<<<<<< Updated upstream
                    Espec /= distanceComp;
=======
                    Espec /= lightComp;
>>>>>>> Stashed changes
                    returnColor[0] += Espec * light.color[0];
                    returnColor[1] += Espec * light.color[1];
                    returnColor[2] += Espec * light.color[2];
                    
                    
                }
                else
                {
                    //returnColor = new double[] { 1,1,1};
                }
            }

            returnColor[0] +=  0.2f * matColor[0];
            returnColor[1] +=  0.2f * matColor[1];
            returnColor[2] +=  0.2f * matColor[2];
            return OverflowCheck(returnColor);

        }

        static private double PhongDiff(ILight light, double diffuseCoef, Vector3d normal, point3D pointOfInt, Vector3d ldirection)
        {
            return (light.intensity * diffuseCoef * Vector3d.Dot(normal, ldirection) < 0 ? 0 : light.intensity * diffuseCoef * Vector3d.Dot(normal, ldirection));
        }

        static private double PhongSpec(ILight light, Material material, double specCoef, Vector3d viewRay, Vector3d refRay)
        {
<<<<<<< Updated upstream

<<<<<<< Updated upstream
            double temp = (light.intensity * specCoef * Math.Pow(Vector3d.Dot(refRay, viewRay), material.gloss));
            return (light.intensity * specCoef * Math.Pow(Vector3d.Dot(refRay, viewRay.Normalized()), material.gloss));
=======
            //double temp = (light.intensity * specCoef * Math.Pow(Vector3d.Dot(refRay, viewRay), material.gloss));
=======
>>>>>>> Stashed changes
            return (light.intensity * specCoef * Math.Pow(Vector3d.Dot(refRay, viewRay), material.gloss));
>>>>>>> Stashed changes
        }

        static private float[] OverflowCheck(double[] color)
        {
            float[] returnColor = new float[color.Length];
            returnColor[0] = (color[0] > 1) ? 1 : (float)color[0];
            returnColor[1] = (color[1] > 1) ? 1 : (float)color[1];
            returnColor[2] = (color[2] > 1) ? 1 : (float)color[2];

            return returnColor;
        }

        static private bool CheckShadow(Scene scene, ray3D ray, ILight light, ISolids thisSolid)
        {
            double lightDistance = Vector3d.Distance(ray.origin.vector3, light.origin.vector3);
            double? temp;

            foreach (ISolids solid in scene.scene)
            {
                temp = solid.GetIntersection(ray);
                if ((temp != null && temp > 1.0e-5) && (temp < lightDistance - 1.0e-6)) {return true;}
            }
            return false;
        }

        static private double LightDisComp(ILight light, point3D pointOfInt, double c0, double c1, double c2)
        {
            double distance = Vector3d.Distance(light.origin.vector3, pointOfInt.vector3);
<<<<<<< Updated upstream
            double compensation = c0 + c1*distance + c2*distance*distance;
            return compensation;
=======
            return (c0 + c1 * distance + c2 * distance * distance);
>>>>>>> Stashed changes
        }




    }
}
