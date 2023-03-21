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
                    if (x == 365 && y == 507)
                    {
                        Console.WriteLine("xxx");
                    }
                    frame.PutPixel((int)x, (int)y,CastRay(MakeRay((2.0d * x) / camWid - 1.0d, (-2.0d * y) / camHei + 1.0d), scene));
                }
            }
            Console.WriteLine("Image generated");
            return frame;
        }
        
        private float[] CastRay(ray3D ray, Scene scene)
        {
            Dictionary<double, ISolids> intersections = new Dictionary<double, ISolids>();

            foreach (ISolids solid in scene.scene)
            {
                double? temp = solid.GetIntersection(ray);
                if (temp != null)
                {
                    intersections.Add((double)solid.GetIntersection(ray), solid);
                }
            }
            if (intersections.Count() == 0)
            {
                return new float[]{ 1,0,0};
            }

            //################################################################
            float[] color = new float[3] { 0,0,0};
            ISolids close = intersections[intersections.Min(x => x.Key)];
            intersections[intersections.Min(x => x.Key)].color.CopyTo(color, 0);

            color = RayTracer.Phong(intersections[intersections.Min(x => x.Key)], intersections.Min(x => x.Key), scene, ray);

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
            right = Vector3d.Normalize(Vector3d.Cross(forward, upguide));
            up = Vector3d.Cross(right, forward);

            h = Math.Tan(fov);
            w = h * aspectRatio;
        }
    }


}
