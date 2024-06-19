
using OpenTK.Mathematics;
using Util;

namespace rt004
{
    internal class Camera
    {

        private Vector3d origin { get; set; }
        private Vector3d target { get; set; }
        private Vector3d upguide { get; set; }
        private double fov { get; set; }
        private double aspectRatio { get; set; }

        private Vector3d forward;
        private Vector3d up;
        private Vector3d right;

        private double h;
        private double w;




        private Vector3d directionVec = new Vector3d(0,0,1);
        private ImageFormat format;
        private CameraMode mode = CameraMode.Perspective;
        private double camWid { get; set; }
        private double camHei { get; set; }
        


        public FloatImage RenderScene(Scene scene)
        {
            FloatImage frame = new FloatImage((int)camWid, (int)camHei, 3);
            Ray ray = new Ray();
            ray.origin3d = origin;
            uint rayTracingDepth = 10;
            //Passes each to be pixel
            //TODO Anti-Aliasing
            float[] temp;
            for (double x = 0; x < camWid; x++)
            {



                for (double y = 0; y < camHei; y++)
                {               
                    if (x == 335 && y == 544)
                    //if (x == 450 && y == 250)
                    { }
                    temp = CastRay(MakeRay((2.0d * x) / camWid - 1.0d, (-2.0d * y) / camHei + 1.0d), scene,rayTracingDepth, rayTracingDepth);

                    frame.PutPixel((int)x, (int)y,temp);
                }
            }
            Console.WriteLine("Image generated");
            return frame;
        }



        public static float[] CastRay(Ray ray, Scene scene, uint rayTracingDepth = 0, uint maxDepth = 0,  ISolids self = null, bool inside = false)
        {
            float[] color = new float[3] { 0.7f, 0.3f, 0.6f };
            ISolids closest = null;
            Ray transRay = new Ray();
            Matrix4d reverseTrans = Matrix4d.Identity;
            double? distance;

            distance = MathHelp.GetIntersect(ray, scene, out closest, out transRay, out reverseTrans, self);
            if (distance == null) { return color; }
            else { color = RayTracer.RayTracing(closest, distance, scene, transRay, reverseTrans, rayTracingDepth, maxDepth, ray); }
            //Console.WriteLine(ray.direction3d);
            return color;
        }


        private Ray MakeRay(double x, double y)
        {
            Vector3d direction = new Vector3d(forward + x * w * right + y * h * up);
            return new Ray(origin, direction.Normalized());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="upguide"></param>
        /// <param name="fov"> In degrees</param>
        /// <param name="aspectRatio"></param>
        public Camera(Vector3d origin, double[] target, double[] upguide, double fov, int width, int height)
        {
            this.origin = origin;
            this.target = new Vector3d(target[0], target[1], target[2]);
            this.upguide = new Vector3d(upguide[0], upguide[1], upguide[2]);
            this.fov = fov*Math.PI/180;
            this.aspectRatio = width / height;
            this.camHei = height;
            this.camWid = width;

            forward = Vector3d.Normalize(Vector3d.Subtract(this.target, origin));
            
            up = this.upguide;
            right = Vector3d.Cross(forward, up).Normalized();


            h = Math.Abs(Math.Tan(this.fov));
            w = h * aspectRatio;
        }
    }


}
