using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static kmty.geom.d3.Util3D;

namespace kmty.geom.d3 {
    public struct Segment {
        public double3 a;
        public double3 b;
        public double3x2 points => double3x2(a, b);

        public Segment(Vector3 a, Vector3 b): this(CastV3D3(a), CastV3D3(b)) { }
        public Segment(double3 a, double3 b) {
            if (Equals(a, b)) Debug.LogWarning("not creating an edge");
            this.a = a;
            this.b = b;
        }

        public void Draw() {
            GL.Begin(GL.LINES);
            GL.Vertex((float3)a);
            GL.Vertex((float3)b);
            GL.End();
        }
    }
}
