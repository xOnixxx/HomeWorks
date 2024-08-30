using OpenTK.Mathematics;
using System.Text.Json.Serialization;


namespace rt004
{
    [JsonPolymorphic(
    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    [JsonDerivedType(typeof(Translate))]
    public interface ITransformations
    {
        public Matrix4d tM { get; set; }
        public Matrix4d tmInverse { get; set; }
        public Matrix4d Inverse();
    }

    public abstract class BasicTransforms : ITransformations
    {
        public abstract Matrix4d tM { get; set; }
        public abstract Matrix4d tmInverse { get; set; }

        public Matrix4d Inverse()
        {
            return tM.Inverted();
        }
    }

    public class Translate : BasicTransforms
    {
        public override Matrix4d tM { get; set; }
        public override Matrix4d tmInverse { get; set; }

        internal Translate() { }

        [JsonConstructor]
        public Translate(double x, double y, double z)
        {
            tM = new Matrix4d(
            1, 0, 0, x,
            0, 1, 0, y,
            0, 0, 1, z,
            0, 0, 0, 1
            );
        } 
    }

    public class RotateX : BasicTransforms
    {
        public override Matrix4d tM { get; set; }
        public override Matrix4d tmInverse { get; set; }

        internal RotateX() { }

        [JsonConstructor]
        public RotateX(double angle)
        {
            angle = angle * Math.PI / 180;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            tM = new Matrix4d(
            1, 0, 0, 0,
            0, cos, -sin, 0,
            0, sin, cos,  0,
            0, 0, 0, 1
            );
        }
    }

    public class RotateZ : BasicTransforms
    {
        public override Matrix4d tM { get; set; }
        public override Matrix4d tmInverse { get; set; }

        internal RotateZ() { }

        [JsonConstructor]
        public RotateZ(double angle)
        {
            angle = angle * Math.PI / 180;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            tM = new Matrix4d(
            cos, -sin, 0, 0,
            sin, cos,  0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
            );
        }
    }

    public class RotateY : BasicTransforms
    {
        public override Matrix4d tM { get; set; }
        public override Matrix4d tmInverse { get; set; }

        internal RotateY() { }

        [JsonConstructor]
        public RotateY(double angle)
        {
            angle = angle * Math.PI / 180;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            tM = new Matrix4d(
            cos, 0, sin, 0,
            0, 1, 0, 0,
            -sin, 0, cos, 0,
            0, 0, 0, 1
            );
        }
    }

    public class Resize : BasicTransforms
    {
        public override Matrix4d tM { get; set; }
        public override Matrix4d tmInverse { get; set; }

        internal Resize() { }

        [JsonConstructor]
        public Resize(double x = 1, double y = 1, double z = 1)
        {
            tM = new Matrix4d(
            x, 0, 0, 0,
            0, y, 0, 0,
            0, 0, z, 0,
            0, 0, 0, 1
            );
        }
    }

    public class Shear : BasicTransforms
    {
        public override Matrix4d tM { get; set; }
        public override Matrix4d tmInverse { get; set; }

        internal Shear() { }

        [JsonConstructor]
        public Shear(double xy = 0, double xz = 0, double yz = 0, double yx = 0, double zx = 0, double zy = 0)
        {
            tM = new Matrix4d(
            1, xy, xz, 0,
            yx, 1, yz, 0,
            zx, zy, 1, 0,
            0, 0, 0, 1
            );
        }
    }

}
