﻿using OpenTK.Mathematics;
using System.Drawing;
using static System.Formats.Asn1.AsnWriter;


namespace rt004
{

    internal static class RayTracer
    {
        struct rayPackage
        {
            public Vector3d color;
            public double? distance;
            public ISolids solid;
            public Matrix4d trans;
            public Ray transRay;
            public Ray originalRay;

            public rayPackage(Vector3d color, double? distance, ISolids solid, Matrix4d trans, Ray transRay, Ray originalRay)
            {
                this.color = color;
                this.distance = distance;
                this.solid = solid;
                this.trans = trans;
                this.transRay = transRay;
                this.originalRay = originalRay;
            }
        }


        static public Vector3d RayTracing(ISolids solid, double? distance, Scene scene, Ray transRay, Matrix4d reverseTrans, uint rayTracingDepth = 0, uint maxDepth = 0, Ray originalRay = new Ray(), double n1 = 1)
        {
            //Calculate color on point

            bool failState = false;
            rayPackage rayPackage;
            Vector3d returnColor = new Vector3d();
            rayTracingDepth++;

            //Basics


            //Normal shading
            //TODO revamp shading, different equations etc.
            returnColor += Phong(solid, distance, scene, transRay, reverseTrans, false);

            //Ray tracing
            //Get new ray

            Ray reflectedRay = ReflectRay(originalRay, transRay, distance, reverseTrans, solid);
            rayPackage = CastRay(reflectedRay, scene, out failState, false, solid, false, n1);
            if (!failState && rayTracingDepth < maxDepth)
            {
                returnColor += RayTracing(rayPackage.solid, rayPackage.distance, scene, rayPackage.transRay, rayPackage.trans, rayTracingDepth, maxDepth, rayPackage.originalRay, n1) / Math.Pow(3, rayTracingDepth + 1);
            }
            
            
            
            //Transparency
            if (solid.material.transparent)
            {
                returnColor += GetRefraction(solid, originalRay, transRay, reverseTrans, (double)distance, scene, n1, rayTracingDepth);
            }


 
            return returnColor;
        }


        //Calculates colors using the Phong functions 
        static public Vector3d Phong(ISolids solid, double? distance, Scene scene, Ray transRay, Matrix4d ReverseTrans, bool reflectance)
        {

            //ray = solid.Transform.MultiplyL(ray);
            Ray viewer = transRay;
            Ray original = MathHelp.RayTransform(transRay, ReverseTrans.Inverted());
            //Point with solid at 0,0,0
            Vector3d point = viewer.origin3d + viewer.direction3d * (double)distance;

            Vector3d pointReal = new Vector3d(ReverseTrans * new Vector4d(point, 1));
            Vector3d matColor = solid.GetTexture(pointReal);

            //Ambient Light
            Vector3d returnColor = new Vector3d();


            //Gives global normal



            foreach (var light in scene.lights)
            {
                //Global light direction
                Vector3d lightDir = Vector3d.Normalize(Vector3d.Subtract(light.origin, pointReal));
               
                Ray lightRay = new Ray(light.origin, lightDir);
                Ray localLight = MathHelp.RayTransform(lightRay, ReverseTrans);

                Matrix4d NormM = ReverseTrans.Inverted();
                NormM.Transpose();

               
                Vector3d solidNormal = new Vector3d(NormM * new Vector4d(solid.GetNormal(point, localLight.direction3d), 0));

                solidNormal = solidNormal.Normalized();
                NormM.Transpose();

                //Global Intersection
                //Console.WriteLine("In phong" + original.direction3d);
                Vector3d view = (original.origin3d - original.direction3d);//(new Vector3d(ReverseTrans * new Vector4d( Vector3d.Normalize(viewer.origin3d - viewer.direction3d),0))).Normalized();
                view = Vector3d.Normalize(original.origin3d - original.direction3d);



                double lightComp = LightDisComp(light, pointReal, 0.04d, 0.05d, 1.0e-4d);
                
                double dotDiffuseElement = Vector3d.Dot(solidNormal, lightDir); dotDiffuseElement = dotDiffuseElement > MathHelp.EPSILON ? dotDiffuseElement : 0;


                returnColor += light.intensity * solid.material.diffuseCoef * dotDiffuseElement * matColor / lightComp;


                lightDir = (Vector3d.Subtract(light.origin, pointReal));
                Vector3d H = Vector3d.Normalize(lightDir + view);
                double NdotH = Vector3d.Dot(solidNormal, H);
                //NdotH = Vector3d.Dot(NdotH, view);
                //Vector3d NdotH = 2 * solidNormal * Vector3d.Dot(solidNormal, lightDir) - lightDir;
                double specEl = Math.Pow(NdotH, solid.material.gloss);


                if (CheckShadow(scene, new Ray(pointReal, lightDir), light, solid) <= 0.21)
                {
                    return new Vector3d(0, 0, 0);
                }
            

                if (Vector3d.Dot(solidNormal, lightDir) < 0) { specEl = 0; }

                returnColor += (light.intensity * solid.material.specCoef * specEl * light.color / lightComp);

                returnColor += 0.1f * matColor / scene.lights.Length / lightComp;
            }



            if (reflectance)
            {
                returnColor += solid.material.diffuseCoef * matColor;
            }
            returnColor += matColor * 0.2;
            return returnColor;
        }

        static private double PhongDiff(ILights light, Material material, Vector3d normal, Vector3d ldirection)
        {
            double diffC = material.diffuseCoef;
            double x = light.intensity * diffC * Vector3d.Dot(normal, ldirection);
            return (x < MathHelp.EPSILON ? 0 : x);

        }

        static private double PhongSpec(ILights light, Material material, Vector3d viewRay, Vector3d refRay)
        {

            if (Vector3d.Dot(refRay, viewRay) < MathHelp.EPSILON)
            {
                //Console.WriteLine(Vector3d.Dot(refRay, viewRay));
                return 0;
            }

            if (Vector3d.Dot(refRay, Vector3d.Normalize(viewRay)) > MathHelp.EPSILON)
            {
                //Console.WriteLine(Vector3d.Dot(refRay, Vector3d.Normalize(viewRay)));
            }
            return (light.intensity * material.specCoef * Math.Pow(Vector3d.Dot(refRay, Vector3d.Normalize(viewRay)), material.gloss));
        }

        static private double FresnelSpec(ILights light, Material material, double specCoef, Vector3d viewRay, Vector3d refRay)
        {
            double S = Vector3d.Dot(refRay, viewRay);
            //double Sh = S/(material)
            return 0;
        }


        //TODO if the solids are transparent the shadow is less "dark"
        //TODO If translucent solid is shadowed by non translucent one
        static private double CheckShadow(Scene scene, Ray ray, ILights light, ISolids intersected)
        {
            return MathHelp.GetShadowMultiplier(ray, scene, light, intersected);
        }

        static private double LightDisComp(ILights light, Vector3d intersection, double c0, double c1, double c2)
        {
            double distance = Vector3d.Distance(light.origin, intersection);//Vector4d.Distance(light.origin.origin, pointOfInt.origin);
            return (c0 + c1 * distance + c2 * distance * distance);
        }



        private static rayPackage CastRay(Ray ray, Scene scene, out bool fail, bool reflectance, ISolids self, bool inside, double n1)
        {


            ISolids closest = null;
            Ray transRay = new Ray();
            Matrix4d reverseTrans = Matrix4d.Identity;
            double? distance;
            Vector3d color;
            distance = MathHelp.GetIntersect(ray, scene, out closest, out transRay, out reverseTrans, self, inside);
            if (distance == null)
            {
                color = new Vector3d();
                fail = true;
            }
            else { color = RayTracer.Phong(closest, distance, scene, transRay, reverseTrans, false); fail = false; }

            return new rayPackage(color, distance, closest, reverseTrans, transRay, ray);
        }

        private static Ray ReflectRay(Ray originalRay, Ray transRay, double? distance, Matrix4d reverseTrans, ISolids solid)
        {
            //Calculate new vector
            Ray reflectedRay = new Ray();

            Vector3d transPoint = transRay.origin3d + transRay.direction3d * (double)distance;
            Vector3d RealPoint = originalRay.origin3d + originalRay.direction3d * (double)distance;


            //Transray is already in local space

            Matrix4d NormM = reverseTrans.Inverted();
            NormM.Transpose();
            Vector3d solidNormal = new Vector3d((NormM * new Vector4d(solid.GetNormal(transPoint, transRay.direction3d), 0))).Normalized();
            Vector3d reflectionDir = transRay.direction3d - 2 * Vector3d.Dot(transRay.direction3d, solidNormal) * solidNormal;




            reflectedRay.direction3d = reflectionDir;
            reflectedRay.origin3d = RealPoint;

            return reflectedRay;
        }

        private static Vector3d GetRefraction(ISolids solid, Ray originalRay, Ray transRay, Matrix4d reverseTrans, double pointDistance, Scene scene, double n1, uint depth)
        {
            Vector3d returnColor = Vector3d.Zero;

            Vector3d TransPoint = transRay.origin3d + transRay.direction3d * (double)pointDistance;
            Vector3d RealPoint = originalRay.origin3d + originalRay.direction3d * (double)pointDistance;

            double n = 1;
            double n2 = solid.material.transparentCoef;

            //From n1 to n2, thus going into solid with n2

            //We are inside
            //We pass n1 as the index of the solid and get n2 through the intersection which is in this case the same solid thus leading to n1 == n2 meaning we are going out
            //TODO Doesnt work if we have the same materials inside one of another 
            if (n1 == n2) { n = n2 / n1; }

            else { n = n1 / n2; }

            Matrix4d NormM = reverseTrans.Inverted();
            NormM.Transpose();

            //transRay is already in Local space
            //#####################################################NEW ADDITION


            Vector3d solidNormal = new Vector3d(NormM * new Vector4d(solid.GetNormal(TransPoint, transRay.direction3d), 0));
            solidNormal = solidNormal.Normalized();

            double cosI = -Vector3d.Dot(solidNormal, originalRay.direction3d);
            double sinT2 = n * n * (1 - cosI * cosI);
            if (sinT2 > 1) { }
            else
            {
                double cosT = Math.Sqrt(1 - sinT2);

                ISolids closest;
                Vector3d refracted = n * originalRay.direction3d + (n * cosI - cosT) * solidNormal;
                Ray refractedRay = new Ray(RealPoint, refracted);

                double? distance = MathHelp.GetIntersect(refractedRay, scene, out closest, out transRay, out reverseTrans, solid, (n1 != n2));

                //No hit this means we should get background
                if (distance < MathHelp.EPSILON || distance is null)
                {
                    returnColor = Camera.CastRay(refractedRay, scene, 0, 3, solid, false);
                }
                //Hit now we need to decide 
                else
                {
                    //We hit something inside TODO
                    if (closest != solid)
                    {
                        //Vector3d PointInside = RealPoint + refracted * (double)distance;
                        //originalRay.origin3d = PointInside;


                    }
                    //The solid is homogenenous (only consists of itself)
                    Vector3d PointBack = RealPoint + refracted * (double)distance;
                    originalRay.origin3d = PointBack;
                    returnColor = Camera.CastRay(refractedRay, scene, depth, 3, solid, true, closest.material.transparentCoef);



                    //float[] tempC = Camera.CastRay(refractedRay, scene, rayTracingDepth - 1, maxDepth, solid);
                }
            }
            return returnColor;
        }
    }
}
