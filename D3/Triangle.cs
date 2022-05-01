using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static kmty.geom.d3.Util3D;

/*
TODO:
1.
circumscribed center has to be deterniministically called because
two vertical line has to cross each other because they are on same triangle
2.
hopefully create sorted list in certain order.
ex.
var s = new List<double3> {a, b, c}.OrderBy(v => v.x).ToList();
var f = s[0] == s[1];
if (f.x) {
    if (f.y) s = s.OrderBy(v => v.z).ToList(); 
    else     s = s.OrderBy(v => v.y).ToList();
}
and then assign b, c as certain rotation order
*/

namespace kmty.geom.d3 {
    using V3 = Vector3;
    using f3 = float3;
    using d3 = double3;
    using SG = Segment;

    public struct Triangle : System.IEquatable<Triangle> {
        public d3 a { get; }
        public d3 b { get; }
        public d3 c { get; }
        public d3 n => normalize(cross(b - a, c - a));

        public Triangle(SG s, d3 p) : this(s.a, s.b, p) { } 
        public Triangle(V3 p1, V3 p2, V3 p3) : this(CastV3D3(p1), CastV3D3(p2), CastV3D3(p3)) { } 
        public Triangle(d3 p1, d3 p2, d3 p3) {
            if (Equals(p1, p2) || Equals(p2, p3) || Equals(p3, p1)) throw new System.Exception();
            this.a = p1;
            this.b = p2;
            this.c = p3;
        }

        public bool HasVert(d3 p) => p.Equals(a) || p.Equals(b) || p.Equals(c);

        public SG Remaining(d3 p) {
            if (p.Equals(a)) return new SG(b, c);
            if (p.Equals(b)) return new SG(c, a);
            if (p.Equals(c)) return new SG(a, b);
            throw new System.Exception();
        }

        public bool IsSameSide(d3 p1, d3 p2, bool includeOnPlane) {
            double d = dot(n, p1 - a) * dot(n, p2 - a);
            return includeOnPlane ? d >= 0 : d > 0;
        }

        public bool Intersects(Line l, out d3 p, out bool isOnEdge) {
            if (!CramersLow(l.pos, l.vec, out d3 d, out p)) {
                isOnEdge = default;
                return false;
            }
            isOnEdge = d.x == 0 || d.x == 1 || d.y == 0 || d.y == 1 || d.x + d.y == 1;
            return d.x >= 0 && d.x <= 1 && d.y >= 0 && d.y <= 1 && d.x + d.y <= 1;
        }

        public bool Intersects(SG e, out d3 p, out bool isOnEdge) {
            if (!CramersLow(e.a, normalize(e.b - e.a), out d3 d, out p)) {
                isOnEdge = default;
                return false;
            }
            bool f1 = d.x >= 0 && d.x <= 1 && d.y >= 0 && d.y <= 1 && d.x + d.y <= 1;
            bool f2 = d.z >= 0 && d.z <= length(e.b - e.a);
            isOnEdge = d.x == 0 || d.x == 1 || d.y == 0 || d.y == 1 || d.x + d.y == 1;
            return f1 && f2;
        }

        bool CramersLow(d3 ogn, d3 ray, out d3 det, out d3 pos) {
            // using cramer's rule
            var e1 = b - a;
            var e2 = c - a;
            var denominator = determinant(double3x3(e1, e2, -ray));
            if (denominator == 0) {
                Debug.LogWarning("parallele");
                det = default;
                pos = default;
                return false;
            }
            var d = ogn - a;
            var u = determinant(double3x3(d, e2, -ray)) / denominator;
            var v = determinant(double3x3(e1, d, -ray)) / denominator;
            var t = determinant(double3x3(e1, e2, d))   / denominator;
            pos = ogn + ray * t;
            det = new d3(u, v, t);
            return true;
        }

        // TODO: Test
        // ref : https://cvtech.cc/pointdist/
        public d3 Distance(d3 p) {
            var v = length(cross(p - a, dot(p - b, p - c))) / 6d;
            var s = length(cross(b - a, c - a)) / 2d;
            return 3 * v / s;
        }

        public d3 GetGravityCenter() { return (a + b + c) / 3d; }
        public d3 GetCircumCenter() {
            var pab = lerp(a, b, 0.5);
            var pbc = lerp(b, c, 0.5);
            var vab = b - a;
            var vbc = c - b;
            var axis = normalize(cross(vab, vbc));
            var d1 = normalize(cross(vab, axis));
            var d2 = normalize(cross(vbc, axis));
            var precision = 1e-15d;
            return GetIntersectionPoint(new Line(pab, d1), new Line(pbc, d2), precision);
        }

        #region drawer
        public void Draw() {
            GL.Begin(GL.LINE_STRIP);
            GL.Vertex((f3)a);
            GL.Vertex((f3)b);
            GL.Vertex((f3)c);
            GL.Vertex((f3)a);
            GL.End();
        }

        public void DrawCircumCircle() {
            var nrm = normalize(cross(b - a, c - a));
            var crc = (f3)GetCircumCenter();
            var qut = Quaternion.FromToRotation(V3.forward, (f3)nrm);

            GL.Begin(GL.LINE_STRIP);
            for (float j = 0; j < Mathf.PI * 2.1f; j += Mathf.PI * 0.03f) {
                var pnt = double3(cos(j), sin(j), 0) * distance(crc, a);
                var pos = qut * (f3)pnt + (V3)crc;
                GL.Vertex(pos);
            }
            GL.End();
        }
        #endregion

        #region IEquatable
        public override bool Equals(object obj) { return obj is Triangle triangle && Equals(triangle); }
        public static bool operator ==(Triangle left, Triangle right) { return left.Equals(right); }
        public static bool operator !=(Triangle left, Triangle right) { return !(left == right); }

        public bool Equals(Triangle t) {
            if     (Equals(t.a, a) && Equals(t.b, b) && Equals(t.c, c)) return true;
            else if(Equals(t.b, a) && Equals(t.c, b) && Equals(t.a, c)) return true;
            else if(Equals(t.c, a) && Equals(t.a, b) && Equals(t.b, c)) return true;
            else if(Equals(t.a, a) && Equals(t.c, b) && Equals(t.b, c)) return true;
            else if(Equals(t.b, a) && Equals(t.a, b) && Equals(t.c, c)) return true;
            else if(Equals(t.c, a) && Equals(t.b, b) && Equals(t.a, c)) return true;
            return false;
        }

        public override int GetHashCode() {
            int hashCode = 1474027755;
            hashCode = hashCode * -1521134295 + a.GetHashCode();
            hashCode = hashCode * -1521134295 + b.GetHashCode();
            hashCode = hashCode * -1521134295 + c.GetHashCode();
            return hashCode;
        }
        #endregion
    }
}
