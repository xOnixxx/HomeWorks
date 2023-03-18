using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using Util;

namespace rt004
{
    public class Camera
    {

        private point3D origin = new point3D(new Vector3d(0,0,0));
        private Vector3d target;
        private Vector3d upguide;
        private double fov;
        private double aspectRatio;

        private Vector3d forward;
        private Vector3d up;
        private Vector3d right;

        private double h;
        private double w;




        private Vector3d directionVec = new Vector3d(0,0,1);
        private ImageFormat format;
        private CameraMode mode = CameraMode.Perspective;
        private double camWid = 1080;
        private double camHei = 1080;
        


        public FloatImage RenderScene(Scene scene)
        {
            double cameraSize = 500;
            FloatImage frame = new FloatImage((int)camWid, (int)camHei, 3);
            ray3D ray = new ray3D();
            ray.origin = origin;

            for (double x = 0; x < camWid; x++)
            {
                for (double y = 0; y < camHei; y++)
                {

                    //WORKS
                    //ray.origin = new point3D(new Vector3d((x - camWid / 2)/cameraSize, (y - camHei/2)/cameraSize, 0));
                    //ray.direction = new Vector3d(0,0,1);


                    //ray.origin = new point3D(new Vector3d(x - camWid / 2, y - camHei / 2, 0));
                    //ray.direction = new Vector3d(x-camWid/2 , 0, 1);

                    //CastRay(ray, scene);
                    //frame.PutPixel((int)x, (int)y, CastRay(ray, scene));
                    

                    frame.PutPixel((int)x, (int)y,CastRay(MakeRay((2.0d * x) / camWid - 1.0d, (-2.0d * y) / camHei + 1.0d), scene));

                    
                }
            }
            Console.WriteLine("####");
            return frame;
        }
        
        private float[] CastRay(ray3D ray, Scene scene)
        {
            Dictionary<double, ISolids> intersections = new Dictionary<double, ISolids>();

            foreach (ISolids solid in scene.scene)
            {
                double? temp = solid.Intersection(ray);
                if (temp != null)
                {
                    intersections.Add((double)solid.Intersection(ray), solid);
                }
            }
            if (intersections.Count() == 0)
            {
                return new float[]{ 1,0,0};
            }

            //################################################################
            float[] color = new float[3];
            intersections[intersections.Min(x => x.Key)].color.CopyTo(color, 0);

            if (intersections[intersections.Min(x => x.Key)] is Spehere3D)
            {
                double distance = intersections.Min(x => x.Key);
                point3D pointOfInt = new point3D(Vector3d.Multiply(ray.direction, distance));
                Vector3d normal = Vector3d.Normalize(pointOfInt.vector3 - intersections[intersections.Min(x => x.Key)].origin.vector3);
                Vector3d ldirection = Vector3d.Normalize(scene.lights[0].origin.vector3 - pointOfInt.vector3);
                color[0] = 1f * 0.8f * (float)Vector3d.Dot(normal, ldirection) + 0.2f;
                color[1] = 1f * 0.8f * (float)Vector3d.Dot(normal, ldirection) + 0.2f;
                color[2] = 1f * 0.8f * (float)Vector3d.Dot(normal, ldirection) + 0.2f;


                //Console.WriteLine((float)Vector3d.Dot(normal, ldirection)*0.8);
            }

            if (intersections[intersections.Min(x => x.Key)] is Plane3D)
            {
                double distance = intersections.Min(x => x.Key);
                point3D pointOfInt = new point3D(Vector3d.Multiply(ray.direction, distance));
                Vector3d normal = Vector3d.Normalize((intersections[intersections.Min(x => x.Key)] as Plane3D).normal);
                Vector3d ldirection = Vector3d.Normalize((scene.lights[0].origin.vector3 - pointOfInt.vector3) - 2 * (Vector3d.Normalize(Vector3d.Dot(Vector3d.Subtract(scene.lights[0].origin.vector3, pointOfInt.vector3), normal) * normal)));

                color[0] = 1f * 0.8f * (float)Vector3d.Dot(normal, ldirection) * color[0] + 0.2f * color[0];
                color[1] = 1f * 0.8f * (float)Vector3d.Dot(normal, ldirection) * color[1] + 0.2f * color[1];
                color[2] = 1f * 0.8f * (float)Vector3d.Dot(normal, ldirection) * color[2] + 0.2f * color[2];
                //Console.WriteLine(color[0]);
            }
            

            return color;
        }

        private float[] diffuseC(Scene scene, ray3D ray)
        {

            return new float[] { };
        }

        private ray3D MakeRay(double x, double y)
        {
            Vector3d direction = new Vector3d(forward + x * w * right + y * h * up);
            return new ray3D(origin, Vector3d.Normalize(direction));
        }

        public Camera(point3D origin, Vector3d target, Vector3d upguide, double fov, double aspectRatio)
        {
            this.origin = origin;
            this.target = target;
            this.upguide = upguide;
            this.fov = fov;
            this.aspectRatio = aspectRatio;

            forward = Vector3d.Normalize(Vector3d.Subtract(target, origin.vector3));
            //forward = new Vector3d(0, 0, 1);
            //right = new Vector3d(1, 0, 0);
            right = Vector3d.Normalize(Vector3d.Cross(forward, upguide));
            up = Vector3d.Cross(right, forward);

            h = Math.Tan(fov);
            w = h * aspectRatio;
        }
    }


}
