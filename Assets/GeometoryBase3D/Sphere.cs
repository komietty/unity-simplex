using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d3 {
    public struct Sphere {
        public double3 center;
        public double radius;

        public Sphere(double3 c, double r) {
            center = c;
            radius = r;
        }

        public bool Contains(double3 p, bool inclusive) {
            if (inclusive) return lengthsq(p - center) <= radius * radius;
            else           return lengthsq(p - center) <  radius * radius;
        }
    }
}
