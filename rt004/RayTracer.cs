using OpenTK.Mathematics;


namespace rt004
{

    internal static class RayTracer
    {
        public const double EPSILON = 1.0e-6;
        public const double MAXIMON = 1.0e+16;
        struct rayPackage
        {
            public float[] color;
            public double? distance;
            public ISolids solid;
            public Matrix4d trans;

            public rayPackage(float[] color, double? distance, ISolids solid,Matrix4d trans)
            {
                this.color = color;
                this.distance = distance;
                this.solid = solid;
                this.trans = trans;
        }
        }

        /*
        static public float[] RayTracing(ISolids solid, double? distance, Scene scene, Ray transRay, Matrix4d reverseTrans)
        {
            int numOfReps = 5;
            float[] returnColor = new float[3];
            Ray tempRay = MathHelp.RayTransform(transRay, reverseTrans.Inverted()); ;
            bool failState = false;
            bool BackfailState = false;

            bool reflectance = false;
            rayPackage tempPackage = new rayPackage();
            rayPackage backwards = new rayPackage();
            Ray backRay = new Ray();
            ISolids tempSolid = null;
            //ray = solid.Transform.MultiplyR(ray);


            //return Phong(solid, distance, scene, transRay, reverseTrans, false);

            for (int i = 0; i < numOfReps; i++)
            {
                //Vector3d refRay = 2 * normal * Vector3d.Dot(normal, ldirection) - ldirection;
                if (i > 0)
                {
                    backwards = CastRay(backRay, scene, out BackfailState, reflectance, tempSolid);
                }
                tempPackage = CastRay(tempRay, scene,out failState, reflectance, tempSolid); 

                if (!failState)
                {
                    

                    tempSolid = tempPackage.solid;
                    double? tempDistance = tempPackage.distance;
                    Vector3d pointReal = tempRay.origin3d + tempRay.direction3d * (double)tempDistance;
                    Vector3d point = new Vector3d(reverseTrans.Inverted() * new Vector4d(pointReal, 1));
                    Matrix4d NormM = reverseTrans.Inverted();
                    NormM.Transpose();
                    Vector3d solidNormal = new Vector3d((NormM * new Vector4d(solid.GetNormal(point), 0))).Normalized();

                    Vector3d refRay = tempRay.direction3d - 2 * Vector3d.Dot(tempRay.direction3d, solidNormal) * solidNormal;

                    tempRay.origin3d = pointReal;
                    tempRay.direction3d = refRay;

                    backRay.origin3d = pointReal;
                    backRay.direction3d = Vector3d.Normalize(pointReal - Vector3d.Zero);

                    for (int j = 0; j < 3; j++)
                    {
                        returnColor[j] += (float)(tempPackage.color[j]* (1/Math.Pow(2, i)));
                        //if(i > 0) { returnColor[j] += (float)(backwards.color[j] * (1 / Math.Pow(2, i))); }
                    }
                }

                if (!BackfailState && i > 0)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        //returnColor[j] += (float)(backwards.color[j] * (1 / Math.Pow(2, i+1)));
                    }
                }
                if (failState) { break; }
            }
            return returnColor;
        }
        */

        static public float[] RayTracing(ISolids solid, double? distance, Scene scene, Ray transRay, Matrix4d reverseTrans, uint rayTracingDepth = 0, uint maxDepth = 0, Ray originalRay = new Ray())
        {
            //Calculate color on point

            bool failState = false;
            rayPackage rayPackage = new rayPackage();
            rayPackage = CastRay(originalRay, scene, out failState, false);

            float[] returnColor = new float[] { 0, 0, 0 };

            
            for (int i = 0; i < 3; i++)
            {
                    returnColor[i] += (float)(rayPackage.color[i] * (1 / Math.Pow(1.5,maxDepth - rayTracingDepth)));

            }

            
            if (!failState && rayTracingDepth > 0)
            {
                //Calculate new vector
                Ray reflectedRay = new Ray();

                Vector3d transPoint = transRay.origin3d + transRay.direction3d * (double)distance;
                Vector3d RealPoint = originalRay.origin3d + originalRay.direction3d * (double)distance;

                Matrix4d NormM = reverseTrans.Inverted();
                NormM.Transpose();
                Vector3d solidNormal = new Vector3d((NormM * new Vector4d(solid.GetNormal(transPoint), 0))).Normalized();
                Vector3d reflectionDir = transRay.direction3d - 2 * Vector3d.Dot(transRay.direction3d, solidNormal) * solidNormal;

                reflectedRay.direction3d = reflectionDir;
                reflectedRay.origin3d = RealPoint;


                //Reflections
                float[] tempC = Camera.CastRay(reflectedRay, scene, rayTracingDepth - 1, maxDepth, solid);
                for (int j = 0; j < 3; j++)
                {
                    returnColor[j] += (float)(tempC[j] * (1 / Math.Pow(1.5, maxDepth - rayTracingDepth)))*(float)solid.material.specCoef;
                }

                //Refractions
                if (solid.material.transparent)
                {

                    double n = solid.material.transparentCoef;
                    double cosI = -Vector3d.Dot(solidNormal, transRay.direction3d);
                    double sinT2 = n * n * (1 - cosI * cosI);
                    if (sinT2 > 1) { }
                    else
                    {
                        double cosT = Math.Sqrt(1 - sinT2);

                        ISolids closest;
                        Vector3d refracted = n * transRay.direction3d + (n * cosI - cosT) * solidNormal;
                        Ray refractedRay = new Ray(RealPoint, refracted);

                        distance = MathHelp.GetIntersect(refractedRay, scene, out closest, out transRay, out reverseTrans, solid, true);
                        if (distance == null) { /*Console.WriteLine("ERROR");*/ }
                        else
                        {


                            //We hit something inside TODO
                            if (closest != solid)
                            {
                                //Vector3d PointInside = RealPoint + refracted * (double)distance;
                                //originalRay.origin3d = PointInside;


                            }

                            Vector3d PointBack = RealPoint + refracted * (double)distance;
                            originalRay.origin3d = PointBack;
                            tempC = Camera.CastRay(originalRay, scene, rayTracingDepth - 1, maxDepth, solid);
                            for (int j = 0; j < 3; j++)
                            {
                                returnColor[j] += (float)(tempC[j] * (1 / Math.Pow(1.5, maxDepth - rayTracingDepth)));
                            }




                            //float[] tempC = Camera.CastRay(refractedRay, scene, rayTracingDepth - 1, maxDepth, solid);
                        }

                    }

                }

            }
            
            return returnColor;
        }


        //Calculates colors using the Phong functions 
        static public float[] Phong(ISolids solid, double? distance, Scene scene, Ray transRay, Matrix4d ReverseTrans, bool reflectance)
        {
            double[] matColor = new double[3];
            double[] returnColor = new double[3];
            solid.color.CopyTo(matColor, 0);
            double Ediff = 0;
            double Espec = 0;
            Ray shadowRay = new Ray();

            //ray = solid.Transform.MultiplyL(ray);
            Ray viewer = transRay;
            Ray original = MathHelp.RayTransform(transRay, ReverseTrans.Inverted());
            //Point with solid at 0,0,0
            Vector3d point = viewer.origin3d + viewer.direction3d * (double)distance;

            Vector3d pointReal = new Vector3d(ReverseTrans * new Vector4d(point,1)) ;
            


            //Gives global normal
            Matrix4d NormM = ReverseTrans.Inverted();
            NormM.Transpose();
            Vector3d solidNormal = new Vector3d(NormM * new Vector4d(solid.GetNormal(point), 0));
            solidNormal = solidNormal.Normalized();
            NormM.Transpose();

            //Global Intersection
            //Console.WriteLine("In phong" + original.direction3d);
            Vector3d view = (original.origin3d - original.direction3d);//(new Vector3d(ReverseTrans * new Vector4d( Vector3d.Normalize(viewer.origin3d - viewer.direction3d),0))).Normalized();
            view = Vector3d.Normalize(original.origin3d - original.direction3d);

            foreach (var light in scene.lights) 
            {
                double lightComp =  LightDisComp(light, pointReal, 0.04d, 0.05d, 1.0e-4d);
                Vector3d lightDir = Vector3d.Normalize(Vector3d.Subtract(light.origin, pointReal));
                double dotDiffuseElement = Vector3d.Dot(solidNormal, lightDir); dotDiffuseElement = dotDiffuseElement > EPSILON ? dotDiffuseElement : 0;


                
                //double dotReflection =  -Vector3d.Dot(2 * Vector3d.Dot(solidNormal, lightDir) * solidNormal - lightDir, view); dotReflection = dotReflection > 0 ? dotReflection : 0;
                if (CheckShadow(scene, new Ray(pointReal ,lightDir), light, solid)) { 
                    returnColor[0] += 0.1f * matColor[0] / scene.lights.Length / lightComp;
                    returnColor[1] += 0.1f * matColor[1] / scene.lights.Length / lightComp;
                    returnColor[2] += 0.1f * matColor[2] / scene.lights.Length / lightComp;


                    return new float[] { (float)returnColor[0], (float)returnColor[1], (float)returnColor[2] };
                }

                
                //The object itself makes the shadow
                if (Vector3d.Dot(solidNormal, lightDir) < 0) {
                    returnColor[0] += 0.1f * matColor[0] / scene.lights.Length / lightComp;
                    returnColor[1] += 0.1f * matColor[1] / scene.lights.Length / lightComp;
                    returnColor[2] += 0.1f * matColor[2] / scene.lights.Length / lightComp;

                    return new float[] { (float)returnColor[0], (float)returnColor[1], (float)returnColor[2] };
                }
                

                returnColor[0] += light.intensity * solid.material.diffuseCoef * dotDiffuseElement * matColor[0] / lightComp;
                returnColor[1] += light.intensity * solid.material.diffuseCoef * dotDiffuseElement * matColor[1] / lightComp;
                returnColor[2] += light.intensity * solid.material.diffuseCoef * dotDiffuseElement * matColor[2] / lightComp;
                

                lightDir = (Vector3d.Subtract(light.origin, pointReal));
                Vector3d H = Vector3d.Normalize(lightDir + view);
                double NdotH = Vector3d.Dot(solidNormal, H);
                //NdotH = Vector3d.Dot(NdotH, view);
                //Vector3d NdotH = 2 * solidNormal * Vector3d.Dot(solidNormal, lightDir) - lightDir;
                double specEl = Math.Pow(NdotH, solid.material.gloss);



                if (Vector3d.Dot(solidNormal, lightDir) < 0) { specEl =  0; }
                returnColor[0] += light.intensity * solid.material.specCoef * specEl * light.color[0] / lightComp;
                returnColor[1] += light.intensity * solid.material.specCoef * specEl * light.color[1] / lightComp;
                returnColor[2] += light.intensity * solid.material.specCoef * specEl * light.color[2] / lightComp;
                

                /*
                Vector3d ldirection = lightDir;//Vector3d.Normalize(Vector3d.Subtract(light.origin,intersection));

                
                shadowRay.direction3d = ldirection;

                //Checks if the ray is blocked by another solid
                if (!CheckShadow(scene, shadowRay, light, solid))
                {

                    //double lightComp = LightDisComp(light, Vector3d.Zero, 0.04d, 0.05d, 1.0e-4d);
                    Ediff = PhongDiff(light, solid.material, solidNormal, ldirection);
                    Ediff /= lightComp;
                    //if (Ediff < 0.5) { Ediff = 0; }
                    returnColor[0] += Ediff * matColor[0];
                    returnColor[1] += Ediff * matColor[1];
                    returnColor[2] += Ediff * matColor[2];

                    double dotReflection = Vector3d.Dot(2 * Vector3d.Dot(solidNormal, lightDir) * solidNormal - lightDir, view); dotReflection = dotReflection > 0 ? dotReflection : 0;
                    Vector3d refRay = solidNormal * (2*Vector3d.Dot(ldirection, solidNormal)) - ldirection ;
                    double specEl = Math.Pow(dotReflection, solid.material.gloss);
                    
                    Espec = PhongSpec(light, solid.material, view, refRay);
                    if (Math.Abs(Espec - specEl) > EPSILON)
                    {
                        //Console.WriteLine(specEl - Espec);
                    }

                    Espec /= lightComp;
                    //Espec = 0;
                    returnColor[0] += Espec * light.color[0];
                    returnColor[1] += Espec * light.color[1];
                    returnColor[2] += Espec * light.color[2];   
                    
                }
                */
                returnColor[0] += 0.1f * matColor[0] / scene.lights.Length / lightComp;
                returnColor[1] += 0.1f * matColor[1] / scene.lights.Length / lightComp;
                returnColor[2] += 0.1f * matColor[2] / scene.lights.Length / lightComp;
            }
            


            if (reflectance)
            {
                returnColor[0] += solid.material.diffuseCoef * matColor[0];
                returnColor[1] += solid.material.diffuseCoef * matColor[1];
                returnColor[2] += solid.material.diffuseCoef * matColor[2];
            }

            return new float[] { (float)returnColor[0], (float)returnColor[1], (float)returnColor[2] };
        }

        static private double PhongDiff(ILights light, Material material, Vector3d normal, Vector3d ldirection)
        {
            double diffC = material.diffuseCoef;
            double x = light.intensity * diffC * Vector3d.Dot(normal,ldirection);
            return (x <  EPSILON ? 0 : x);

        }

        static private double PhongSpec(ILights light, Material material, Vector3d viewRay, Vector3d refRay)
        {

            if (Vector3d.Dot(refRay, viewRay) < EPSILON)
            {
                //Console.WriteLine(Vector3d.Dot(refRay, viewRay));
                return 0;
            }
            
            if (Vector3d.Dot(refRay, Vector3d.Normalize(viewRay)) > EPSILON)
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

        static private float[] OverflowCheck(double[] color)
        {
            float[] returnColor = new float[color.Length];
            returnColor[0] = (color[0] > 1) ? (float)Math.Truncate(color[0]) : (float)color[0];
            returnColor[1] = (color[1] > 1) ? (float)Math.Truncate(color[1]) : (float)color[1];
            returnColor[2] = (color[2] > 1) ? (float)Math.Truncate(color[2]) : (float)color[2];

            return returnColor;
        }

        static private bool CheckShadow(Scene scene, Ray ray, ILights light, ISolids intersected)
        {

            double lightDistance = Vector3d.Distance(light.origin, ray.origin3d);

            ISolids closest = null;
            Ray transRay = new Ray();
            Matrix4d reverseTrans = Matrix4d.Identity;
            double? distance;

            distance = MathHelp.GetIntersect(ray, scene, out closest, out transRay, out reverseTrans, intersected);

            if (distance == null || distance > lightDistance) { return false; }
            return true;


        }

        static private double LightDisComp(ILights light, Vector3d intersection, double c0, double c1, double c2)
        {
            double distance = Vector3d.Distance(light.origin, intersection);//Vector4d.Distance(light.origin.origin, pointOfInt.origin);
            return (c0 + c1 * distance + c2 * distance * distance);
        }

        

        private static rayPackage CastRay(Ray ray, Scene scene, out bool fail, bool reflectance, ISolids self = null)
        {


            ISolids closest = null;
            Ray transRay = new Ray();
            Matrix4d reverseTrans = Matrix4d.Identity;
            double? distance;
            float[] color;
            distance = MathHelp.GetIntersect(ray, scene, out closest, out transRay, out reverseTrans, self);
            if (distance == null) { 
                color = new float[3] { 0, 0, 0 };
                fail = true; }
            else { color = RayTracer.Phong(closest, distance, scene, transRay, reverseTrans, false); fail = false; }

            //################################################################


            return new rayPackage(color, distance ,closest, reverseTrans);
        }
    }
}
