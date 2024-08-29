
using OpenTK.Mathematics;
using System.Text.Json.Serialization;
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


        [JsonConstructor]
        public Camera(double[] origin, double[] target, double[] upguide, double fov, int width, int height)
        {
            this.origin = new Vector3d(origin[0], origin[1], origin[2]);
            this.target = new Vector3d(target[0], target[1], target[2]);
            this.upguide = new Vector3d(upguide[0], upguide[1], upguide[2]);
            this.fov = fov*Math.PI/180;
            this.aspectRatio = width / height;
            this.camHei = height;
            this.camWid = width;

            forward = Vector3d.Normalize(Vector3d.Subtract(this.target, this.origin));
            
            up = this.upguide;
            right = Vector3d.Cross(forward, up).Normalized();


            h = Math.Abs(Math.Tan(this.fov));
            w = h * aspectRatio;
        }


        public FloatImage RenderScene(Scene scene, Func<Vector2d, int, List<Vector2d>> sampler, int spp = 4)
        {
            FloatImage frame = new FloatImage((int)camWid, (int)camHei, 3);
            Ray ray = new Ray();
            ray.origin3d = origin;
            uint maxDepth = 10;

            Vector3d temp = Vector3d.Zero;
            float[] color = new float[3];
            List<Vector2d> samples = new List<Vector2d>();
            Vector2d pixelPosition = Vector2d.Zero;

            for (double x = 0; x < camWid; x++)
            {
                pixelPosition.X = x;
                int i = (int)Math.Floor(x / camWid * 100);
                Console.Write($"\rProgress: {i}%");

                for (double y = 0; y < camHei; y++)
                {               
                    pixelPosition.Y = y;

                    //TODO Add starting medium (for example camera is underwater
                    samples = sampler(pixelPosition, spp);
                    foreach (Vector2d sample in samples)
                    {
                        temp += CastRay(MakeRay((2.0d * sample.X) / camWid - 1.0d, (-2.0d * sample.Y) / camHei + 1.0d), scene, 0, maxDepth)/(double)spp;
                    }

                    color[0] = (float)temp.X;
                    color[1] = (float)temp.Y;
                    color[2] = (float)temp.Z;


                    frame.PutPixel((int)x, (int)y,color);
                    temp = Vector3d.Zero;
                }
            }

            Console.WriteLine("Image generated");

            

            return frame;
        }

        public static FloatImage CameraDebugShowNoise(Func<Vector2d, int, List<Vector2d>> sampler, int spp = 256)
        {
            FloatImage frame = new FloatImage((int)spp, (int)spp, 3);
            Vector2d pixelPosition = Vector2d.Zero;
            List<Vector2d> samples = new List<Vector2d>();
            samples = sampler(pixelPosition, spp);
            float[] color = new float[3];
            color[0] = 1;
            color[1] = 1;
            color[2] = 1;
            foreach (Vector2d sample in samples)
            {
                frame.PutPixel((int)(sample.X * spp), (int)(sample.Y * spp), color);
            }
            Console.WriteLine("Image generated");
            return frame;
        }



        public static Vector3d CastRay(Ray ray, Scene scene, uint rayTracingDepth = 0, uint maxDepth = 0,  ISolids self = null, bool inside = false, double n1 = 1)
        {
            //BGR
            Vector3d color = new Vector3d( 0.7f, 0.3f, 0.6f);
            ISolids closest = null;
            Ray transRay = new Ray();
            Matrix4d reverseTrans = Matrix4d.Identity;
            double? distance;

            //Finds the closest solid, if not return BGR color
            distance = MathHelp.GetIntersect(ray, scene, out closest, out transRay, out reverseTrans, self);
            if (distance == null) { return color; }
            if (rayTracingDepth > maxDepth) { return new Vector3d(); }
            else { color = RayTracer.RayTracing(closest, distance, scene, transRay, reverseTrans, rayTracingDepth, maxDepth, ray, n1); }
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






    }



}
