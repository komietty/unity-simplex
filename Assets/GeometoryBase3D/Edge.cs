using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d3 {
    public struct Edge {
        public double3 a;
        public double3 b;
        public double3x2 points => double3x2(a, b);

        public Edge(double3 a, double3 b) {
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
