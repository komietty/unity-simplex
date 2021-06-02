using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d2 {

    public struct Segment {
        public float2 a { get; set; }
        public float2 b { get; set; }
        public void Set(float2 a, float2 b) { this.a = a; this.b = b; }
        public Segment(float2 a, float2 b)  { this.a = a; this.b = b; }

        public Line GetLine() => new Line(a, b - a);

        public bool Intersects(Line l) {
            float t1 = cross(float3(l.vec, 0), float3(a - l.pos, 0)).z;
            float t2 = cross(float3(l.vec, 0), float3(b - l.pos, 0)).z;
            return t1 * t2 <= 0;
        }

        public bool Intersects(Segment s) => Intersects(s.GetLine()) && s.Intersects(GetLine());

        public bool Intersects(Bounds b) {
            var min = float2(b.min.x, b.min.y);
            var max = float2(b.max.x, b.max.y);
            if (Intersects(new Segment(float2(min.x, max.y), float2(min.x, min.y)))) return true;
            if (Intersects(new Segment(float2(min.x, max.y), float2(max.x, max.y)))) return true;
            if (Intersects(new Segment(float2(max.x, max.y), float2(max.x, min.y)))) return true;
            if (Intersects(new Segment(float2(min.x, min.y), float2(max.x, min.y)))) return true;
            return false;
        }

        public float2 GetClosestPoint(float2 p) {
            float a1 = b.y - a.y,
                  b1 = a.x - b.x,
                  c1 = a1 * a.x + b1 * a.y,
                  c2 = -b1 * p.x + a1 * p.y,
                  dt = a1 * a1 - -b1 * b1;
            if (Mathf.Approximately(dt, 0f)) return p;
            return float2(a1 * c1 - b1 * c2, a1 * c2 - -b1 * c1) / dt;
        }

        public float GetDistance(float2 p) => distance(GetClosestPoint(p), p);

        public bool Contains(float2 p) {
            var d0 = b - GetClosestPoint(p);
            var d1 = b - a;
            return (dot(d0, d1) >= 0f) && (lengthsq(d0) <= lengthsq(d1));
        }

        public Vector2 GetPoint(float offset) => a + normalize(b - a) * offset;
        public void DrawGizmos() { Gizmos.DrawLine((Vector2)a, (Vector2)b); }


        public override bool Equals(object obj) { return base.Equals(obj); }
        public override int GetHashCode() { return base.GetHashCode(); }

        public static bool operator !=(Segment s1, Segment s2) => !(s1 == s2);
        public static bool operator ==(Segment s1, Segment s2) {
            if (Object.ReferenceEquals(s1, s2)) return true;
            if (((object)s1 == null) || ((object)s2 == null)) return false;
            return s1.a.x == s2.a.x && s1.a.y == s2.a.y &&
                   s1.b.x == s2.b.x && s1.b.y == s2.b.y;
        }
    }
}
