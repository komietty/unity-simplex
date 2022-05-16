using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static kmty.geom.d3.Utils;

namespace kmty.geom.d3 {
    public struct Segment {
        public double3 a;
        public double3 b;
        public double3x2 points => double3x2(a, b);

        public Segment(Vector3 a, Vector3 b): this(V3D3(a), V3D3(b)) { }
        public Segment(double3 a, double3 b) {
            if (Equals(a, b)) Debug.LogWarning("not creating an edge");
            this.a = a;
            this.b = b;
        }

        public bool Contains(double3 s){
            return math.all(a == s) || math.all(b == s);
        }
        
        public bool EqualsIgnoreDirection(Segment pair){
            return this.Equals(pair) || this.Equals(new Segment(pair.b, pair.a));
        }

        public void Draw() {
            GL.Begin(GL.LINES);
            GL.Vertex((float3)a);
            GL.Vertex((float3)b);
            GL.End();
        }

/*
        #region IEquatable
        public override bool Equals(object obj) { return obj is Segment pair && Equals(pair); }
        public static bool operator ==(Segment left, Segment right) { return left.Equals(right); }
        public static bool operator !=(Segment left, Segment right) { return !(left == right); }

        public bool Equals(Segment s) {
            if(Equals(s.a, a) && Equals(s.b, b)) return true;
            return false;
        }

        public override int GetHashCode() {
            int hashCode = 1474027755;
            hashCode = hashCode * -1521134295 + a.GetHashCode();
            hashCode = hashCode * -1521134295 + b.GetHashCode();
            return hashCode;
        }
        #endregion
*/
    }
}
