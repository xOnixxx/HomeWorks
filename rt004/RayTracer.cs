﻿using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rt004
{
    static class RayTracer
    {
        public const double EPSILON = 1.0e-6;
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
                if (!CheckShadow(scene, shadowRay, light))
                {

                    double lightComp = LightDisComp(light, pointOfInt, 0.04d, 0.05d, 1.0e-4d);
                    Ediff = PhongDiff(light, solid.material, normal, pointOfInt, ldirection);
                    Ediff /= lightComp;

                    returnColor[0] += Ediff * matColor[0];
                    returnColor[1] += Ediff * matColor[1];
                    returnColor[2] += Ediff * matColor[2];

                    
                    Vector3d refRay = 2*normal*(Vector3d.Dot(normal, ldirection)) - ldirection;
                    Espec = PhongSpec(light, solid.material, scene.specularC, ray.direction, refRay);
                    Espec /= lightComp;
                    returnColor[0] += Espec * light.color[0];
                    returnColor[1] += Espec * light.color[1];
                    returnColor[2] += Espec * light.color[2];
                    
                }

            }

            returnColor[0] +=  0.2f * matColor[0];
            returnColor[1] +=  0.2f * matColor[1];
            returnColor[2] +=  0.2f * matColor[2];
            return OverflowCheck(returnColor);
        }

        static private double PhongDiff(ILight light, Material material, Vector3d normal, point3D pointOfInt, Vector3d ldirection)
        {
            double diffC = material.diffuseCoef;
            return (light.intensity * diffC * Vector3d.Dot(normal, ldirection) <  EPSILON ? 0 : light.intensity * diffC * Vector3d.Dot(normal, ldirection));
        }

        static private double PhongSpec(ILight light, Material material, double specCoef, Vector3d viewRay, Vector3d refRay)
        {
            return (light.intensity * material.specCoef * Math.Pow(Vector3d.Dot(refRay, viewRay), material.gloss));

        }

        static private float[] OverflowCheck(double[] color)
        {
            float[] returnColor = new float[color.Length];
            returnColor[0] = (color[0] > 1) ? 1 : (float)color[0];
            returnColor[1] = (color[1] > 1) ? 1 : (float)color[1];
            returnColor[2] = (color[2] > 1) ? 1 : (float)color[2];

            return returnColor;
        }

        static private bool CheckShadow(Scene scene, ray3D ray, ILight light)
        {
            double lightDistance = Vector3d.Distance(ray.origin.vector3, light.origin.vector3);
           
            foreach (ISolids solid in scene.scene)
            {
                double? temp = solid.GetIntersection(ray);
                if ((temp != null && temp > EPSILON) && (temp < lightDistance - EPSILON))
                {
                    return true;
                }
            }
            return false;
        }

        static private double LightDisComp(ILight light, point3D pointOfInt, double c0, double c1, double c2)
        {
            double distance = Vector3d.Distance(light.origin.vector3, pointOfInt.vector3);
            return (c0 + c1 * distance + c2 * distance * distance);
        }




    }
}
