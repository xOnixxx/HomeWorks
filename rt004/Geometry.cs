﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace rt004
{

    public enum AngleType
    {
        Rad,
        Degrees,
    }

    //Represents solids which will be saved in the scene, every solid is parametrically described with its parametric function
    public interface ISolids
    {
        public double? Intersection(ray3D ray);
        public void Transform(Matrix4d transMatrix);
        public point3D origin { get; set; }
        public float[] color { get; set; }
    }

    //Used to distinguishe between vector/point
    public struct point3D
    {
        public Vector3d vector3;
        public point3D(Vector3d vector3)
        {
            this.vector3 = vector3;
        }
    }

    public struct ray3D
    {
        public point3D origin;
        public Vector3d direction;

        public ray3D(point3D origin, Vector3d direction)
        {
            this.origin = origin;
            this.direction = direction;
        }
    }
   
    public struct angle
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



    public class Spehere3D : ISolids
    {
        public point3D origin { get; set; }
        public float[] color { get; set; }
        private double radius;
        private double? Quadratic(double a, double b, double c)
        {
            if ((b * b - 4 * a * c) <= 0)
            {
                return null;
            }
            
            double delta = Math.Sqrt(b * b - 4 * a * c);
            double x1 = (-b + delta) / 2 * a;
            double x2 = c / (a * x1);
            if (x1 < 0 && x2 < 0) { return null; }
            if (x1 > 0 && x2 < 0) { return x1; }
            if (x1 < 0 && x2 > 0) { return x2; }
            return Math.Min(x1, x2);
        }
        public double? Intersection(ray3D ray)
        {
            Vector3d d = Vector3d.Subtract(ray.origin.vector3,origin.vector3); 
            double a = Vector3d.Dot(ray.direction, ray.direction);
            double b = 2 * (Vector3d.Dot(ray.direction, d));
            double c = Vector3d.Dot(d, d) - radius * radius;
            double? t = Quadratic(a, b, c);
            return t;
        }

        public void Transform(Matrix4d transMatrix)
        {

        }
        public Spehere3D(Vector3d origin, double radius, float[] color) 
        {
            this.origin = new point3D(origin);
            this.radius = radius;
            this.color = color;
        }
    }

    public class Plane3D : ISolids
    {
        public point3D origin { get; set; }
        public float[] color { get; set; }
        public Vector3d normal;

        public double? Intersection(ray3D ray)
        {
            if (Vector3d.Dot(ray.direction, normal) == 0)
            {
                return null;
            }
            double t = Vector3d.Dot((origin.vector3 - ray.origin.vector3), normal)/Vector3d.Dot(ray.direction, normal);
            if (t <= 0)
            {
                return null;
            }
            return t;
        }

        public void Transform(Matrix4d transMatrix)
        {

        }

        public Plane3D(point3D origin, Vector3d normal, float[] color)
        {
            this.origin = origin;
            this.normal = normal;
            this.color = color;
        }
    }
}
