using UnityEngine;
using System;
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
    using V3 = UnityEngine.Vector3;
    using f3 = Unity.Mathematics.float3;
    using d3 = Unity.Mathematics.double3;
    using d33 = Unity.Mathematics.double3x3;
    using SG = Segment;

    public struct Triangle {
        public d3 a;
        public d3 b;
        public d3 c;
        public d3 circumscribedCenter;
        public d3 normal => normalize(cross(b - a, c - a));
        public d33 points => double3x3(a, b, c);
        public static double precision = 1e-15d;

        public Triangle(SG s, d3 p) : this(s.a, s.b, p) { } 
        public Triangle(V3 a, V3 b, V3 c) : this(CastV3D3(a), CastV3D3(b), CastV3D3(c)) { } 
        public Triangle(d3 a, d3 b, d3 c) {
            if (Equals(a, b) || Equals(b, c) || Equals(c, a)) Debug.LogWarning("not creating a triangle");

            this.a = a;
            this.b = b;
            this.c = c;

            var p1 = lerp(a, b, 0.5);
            var p2 = lerp(b, c, 0.5);
            var vecAB = b - a;
            var vecBC = c - b;
            var axis = normalize(cross(vecAB, vecBC));
            var dir1 = normalize(cross(vecAB, axis));
            var dir2 = normalize(cross(vecBC, axis));
            var cntr =  GetIntersectionPoint( new Line(p1, dir1), new Line(p2, dir2), precision);
            this.circumscribedCenter = cntr;
        }

        public bool Equals(Triangle t) {
            var f1 = Equals(t.a, a) && Equals(t.b, b) && Equals(t.c, c);
            var f2 = Equals(t.b, a) && Equals(t.c, b) && Equals(t.a, c);
            var f3 = Equals(t.c, a) && Equals(t.a, b) && Equals(t.b, c);
            var f4 = Equals(t.a, a) && Equals(t.c, b) && Equals(t.b, c);
            var f5 = Equals(t.b, a) && Equals(t.a, b) && Equals(t.c, c);
            var f6 = Equals(t.c, a) && Equals(t.b, b) && Equals(t.a, c);
            return f1 || f2 || f3 || f4 || f5 || f6;
        }

        public SG Remaining(d3 p) {
            if      (Equals(p, a)) return new SG(b, c);
            else if (Equals(p, b)) return new SG(c, a);
            else if (Equals(p, c)) return new SG(a, b);
            throw new ArgumentOutOfRangeException();
        }

        public bool IsSameSide(d3 p1, d3 p2, bool includeOnPlane) {
            double d = dot(normal, p1 - a) * dot(normal, p2 - a);
            return includeOnPlane ? d >= 0d : d > 0d;
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
            bool f3 = d.z >= 0 && d.z <= length(e.b - e.a);
            isOnEdge = d.x == 0 || d.x == 1 || d.y == 0 || d.y == 1 || d.x + d.y == 1;
            return f1 && f3;
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
            var tsl = (f3)circumscribedCenter;
            var qut = Quaternion.FromToRotation(V3.forward, (f3)nrm);

            GL.Begin(GL.LINE_STRIP);
            for (float j = 0; j < Mathf.PI * 2.1f; j += Mathf.PI * 0.03f) {
                var pnt = double3(cos(j), sin(j), 0) * distance(tsl, a);
                var pos = qut * (f3)pnt + (V3)tsl;
                GL.Vertex(pos);
            }
            GL.End();
        }
        #endregion
    }
}
