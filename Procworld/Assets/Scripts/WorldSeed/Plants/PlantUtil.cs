using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

namespace WorldSeed.Plants
{
    public class IntersectionUtil
    {
        public static float GetVolumeOfIntersection(BoundingSphere node, ParallelQuery<BoundingSphere> allNodes)
        {
            return ParallelEnumerable.Sum(
                ParallelEnumerable.Select(
                    allNodes,
                    b => BoundingSphere.IntersectionVolume(node, b))
            );
        }
    }

    public class VectorUtil
    {

        /// <summary>
        /// Scalar projection of a onto b
        /// negative sign is angle between is less than 90 degrees,
        /// positive sign if angle between is greater than 90 degrees,
        /// zero if angle is 90 degrees
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float ScalarProjection(Vector3 a, Vector3 b)
        {
            return Vector3.Dot(a, b) / Vector3.Magnitude(b);
        }


        public static Vector3 Midpoint(Vector3 a, Vector3 b)
        {
            return (a + b) / 2.0f;
        }


        // from https://math.stackexchange.com/a/1586185/155079
        public static Quaternion RandomQuaternion(System.Random randomGen)
        {
            // From https://stackoverflow.com/a/44031492/2924421
            float u = (float)randomGen.NextDouble();
            float v = (float)randomGen.NextDouble();
            float w = (float)randomGen.NextDouble();

            return new Quaternion(
                Sqrt(1 - u) * Sin(2 * PI * v),
                Sqrt(1 - u) * Cos(2 * PI * v),
                Sqrt(u) * Sin(2 * PI * w),
                Sqrt(u) * Cos(2 * PI * w)
                );
        }

        // from https://math.stackexchange.com/a/1586185/155079
        public static Vector3 RandomNormalVector(System.Random randomGen)
        {
            double u1 = randomGen.NextDouble();
            double u2 = randomGen.NextDouble();
            double latitude = System.Math.Acos(2 * u1 - 1) - System.Math.PI / 2.0;
            double longitude = 2 * System.Math.PI * u2;
            double x = System.Math.Cos(latitude) * System.Math.Cos(longitude);
            double y = System.Math.Cos(latitude) * System.Math.Sin(longitude);
            double z = System.Math.Sin(latitude);
            return new Vector3((float)x, (float)y, (float)z);
        }
    }

    public class PlantUtil
    {
        public static float CalculateLight(BoundingSphere node, ParallelQuery<BoundingSphere> allNodes)
        {
            return Exp(-IntersectionUtil.GetVolumeOfIntersection(node, allNodes));
        }

        // Called Y in the paper
        public static float ChangeInPhysiologicalAge(float v, float vigorMin, float vigorMax, float growthRate)
        {
            // I added the Max to clamp to vigorMin because I don't clamp vigor to vigorMin since those branches will be shedded in one step anyway
            return SigmoidLikeCurve((Max(v, vigorMin) - vigorMin) / (vigorMax - vigorMin)) * growthRate;
        }

        // 0 at x = 0, 1 at x = 1, smoothly interpolate between those two points
        // Called S in the paper
        public static float SigmoidLikeCurve(float x)
        {
            return 3 * x * x - 2 * x * x * x;
        }
    }

    // From https://stackoverflow.com/a/914198/2924421
    public static class MoreLinq
    {
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, null);
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            comparer = comparer ?? Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var min = sourceIterator.Current;
                var minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }
    }
}

