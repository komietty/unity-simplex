using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d2 {
    using f2 = float2;
    using f3 = float3;
    using SG = Segment;

    /// <summary>
    /// non oriented 1-simplex.
    /// </summary>
    public struct Segment : System.IEquatable<SG> {
        public f2 a { get; }
        public f2 b { get; }
        public Line line { get; }

        public Segment(f2 a, f2 b)  {
            this.a = a;
            this.b = b;
            this.line = new Line(a, b - a);
        }

        public bool Intersects(Line l) {
            var t1 = cross(new f3(l.vec, 0), new f3(a - l.pos, 0)).z;
            var t2 = cross(new f3(l.vec, 0), new f3(b - l.pos, 0)).z;
            return t1 * t2 <= 0;
        }

        public bool Intersects(SG s) => Intersects(s.line) && s.Intersects(line);

        public bool Intersects(Bounds b) {
            if (Intersects(new SG(new f2(b.min.x, b.max.y), new f2(b.min.x, b.min.y)))) return true;
            if (Intersects(new SG(new f2(b.min.x, b.max.y), new f2(b.max.x, b.max.y)))) return true;
            if (Intersects(new SG(new f2(b.max.x, b.max.y), new f2(b.max.x, b.min.y)))) return true;
            if (Intersects(new SG(new f2(b.min.x, b.min.y), new f2(b.max.x, b.min.y)))) return true;
            return false;
        }

        public f2 GetClosestPoint(f2 p) {
            float a1 = b.y - a.y,
                  b1 = a.x - b.x,
                  c1 = a1 * a.x + b1 * a.y,
                  c2 = -b1 * p.x + a1 * p.y,
                  dt = a1 * a1 - -b1 * b1;
            if (Mathf.Approximately(dt, 0f)) return p;
            return float2(a1 * c1 - b1 * c2, a1 * c2 - -b1 * c1) / dt;
        }

        public bool Contains(f2 p) {
            var d0 = b - GetClosestPoint(p);
            var d1 = b - a;
            return (dot(d0, d1) >= 0f) && (lengthsq(d0) <= lengthsq(d1));
        }

        public float GetDistance(f2 p) => abs(cross(new f3(p - a, 0), new f3(normalize(b - a), 0)).z);

        public Vector2 GetPoint(float offset) => a + normalize(b - a) * offset;

        #region IEqatable
        public override bool Equals(object obj) { return obj is SG segment && Equals(segment); }
        public bool Equals(SG other) {
            var f1 = a.Equals(other.a) && b.Equals(other.b);
            var f2 = a.Equals(other.b) && b.Equals(other.a);
            return f1 || f2;
        }
        public static bool operator ==(SG left, SG right) { return left.Equals(right); }
        public static bool operator !=(SG left, SG right) { return !(left == right); }
        public override int GetHashCode() {
            int hashCode = 2118541809;
            hashCode = hashCode * -1521134295 + a.GetHashCode();
            hashCode = hashCode * -1521134295 + b.GetHashCode();
            return hashCode;
        }
        #endregion
    }
}
