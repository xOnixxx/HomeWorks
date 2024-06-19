using OpenTK.Mathematics;

namespace rt004
{
    public struct Ray
    {
        public Vector3d origin3d { get { return origin3d_; } set { origin3d_ = value; } }
        public Vector3d direction3d { get { return direction3d_; } set { direction3d_ = value; } }

        private Vector3d direction3d_;

        private Vector3d origin3d_;


        public Ray(Vector3d origin, Vector3d direction) { this.origin3d = origin; this.direction3d = direction; }

    }
}
