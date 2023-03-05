using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Util;

namespace rt004
{
    internal class Camera
    {
        
        private point3D focusPoint = new point3D(new System.Numerics.Vector3(0,0,0));
        private CameraMode mode = CameraMode.Perspective;
        private angle viewAngle = new angle(AngleType.Degrees, 180);
        
        public FloatImage RenderScene(Scene scene)
        {
            FloatImage frame = new FloatImage(scene.width, scene.height, 3);
            for (float x = 0; x < scene.width; x++)
            {
                for (float y = 0; y < scene.height; y++)
                {
                    frame.PutPixel((int)x, (int)y, CastRay(x, y));
                }
            }

            return new FloatImage(RenderScene(scene));
        }
        
        private float[] CastRay(float x, float y)
        {

            return new float[] { 0};
        }
    }


}
