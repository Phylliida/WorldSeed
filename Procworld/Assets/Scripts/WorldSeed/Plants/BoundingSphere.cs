using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldSeed.Plants
{
    public struct BoundingSphere
    {
        Module module;
        Vector3 pos;
        float radius;
        public BoundingSphere(Module module, Vector3 pos, float radius)
        {
            this.module = module;
            this.pos = pos;
            this.radius = radius;
        }

        public static bool Intersects(BoundingSphere a, BoundingSphere b)
        {
            return Vector3.Distance(a.pos, b.pos) <= a.radius + b.radius;
        }

        public static float IntersectionVolume(BoundingSphere a, BoundingSphere b)
        {
            float vol = 0.0f;
            // From https://mathworld.wolfram.com/Sphere-SphereIntersection.html
            if (a.module != b.module && Intersects(a, b))
            {
                float R = a.radius;
                float r = b.radius;
                float d = Vector3.Distance(a.pos, b.pos);
                float x = (d * d - r * r + R * R) / 2 * d;
                float d1 = x;
                float d2 = d - x;
                float h1 = R - d1;
                float h2 = r - d2;
                vol = (float)(System.Math.PI) * sqr(R + r - d) * (d * d + 2 * d * r - 3 * r * r + 2 * d * R + 6 * r * R - 3 * R * R) / (12 * d);
            }
            return vol;
        }

        static float sqr(float a)
        {
            return a * a;
        }
    }
}