using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static kmty.geom.d3.Utils;

namespace kmty.geom.d3 {
    public struct Line {
        public double3 pos;
        public double3 vec;

        public Line(Vector3 a, Vector3 b): this(V3D3(a), V3D3(b)) { }
        public Line(double3 p, double3 v) {
            if (Equals(v, (double3)0)) Debug.LogWarning("not creating a line");
            pos = p;
            vec = normalize(v);
        }
    }
}
