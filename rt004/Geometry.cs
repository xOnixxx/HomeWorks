using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    enum AngleType
    {
        Rad,
        Degrees,
    }

    //Represents solids which will be saved in the scene, every solid is parametrically described with its parametric function
    internal interface ISolids
    {
        public bool Intersection(Vector3 ray);
        public LambdaExpression parametricFun { get;}
        public point3D origin { get; set; }
    }

    //Used to distinguishe between vector/point
    internal struct point3D
    {
        public Vector3 vector3;
        public point3D(Vector3 vector3)
        {
            this.vector3 = vector3;
        }
    }

    internal struct ray3D
    {
        public point3D origin;
        public Vector3 direction;
    }

    internal struct angle
    {
        public AngleType type;
        public float value;

        public angle(AngleType type, float value)
        {
            this.type = type;
            this.value = value;
        }

        public void Convert()
        {
            if (type == AngleType.Degrees)
            {
                type = AngleType.Rad;
                value *= float.Pi / 180; 
            }
            switch (type)
            {
                case AngleType.Degrees:
                    type = AngleType.Rad;
                    value *= float.Pi / 180;
                    break;
                case AngleType.Rad:
                    type = AngleType.Degrees;
                    value /= float.Pi / 180;
                    break;
            }
        }
    }
}
