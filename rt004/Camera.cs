using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics

namespace rt004
{
    internal class Camera
    {
        private point3D focusPoint = new point3D(new System.Numerics.Vector3(0,0,0));
        private CameraMode mode = CameraMode.Perspective;
        private angle viewAngle = new angle(AngleType.Degrees, 180);
        public void RenderScene(Scene scene) 
        {

        }

    }


}
