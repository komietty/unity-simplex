using UnityEngine;
using System;
using Unity.Mathematics;
using static Unity.Mathematics.math;

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
    public struct Triangle {
        public double3 a;
        public double3 b;
        public double3 c;
        public double3 circumscribedCenter;
        public double3x3 points => double3x3(a, b, c);
        public double3 normal => normalize(cross(b - a, c - a));
        public static double precision = 1e-15d;

        public Triangle(double3 a, double3 b, double3 c) {
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
            var cntr =  Util3D.GetIntersectionPoint( new Line(p1, dir1), new Line(p2, dir2), precision);
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

        public Segment Remaining(double3 p) {
            if      (Equals(p, a)) return new Segment(b, c);
            else if (Equals(p, b)) return new Segment(c, a);
            else if (Equals(p, c)) return new Segment(a, b);
            throw new ArgumentOutOfRangeException();
        }

        public bool IsSameSide(double3 p1, double3 p2, bool includeOnPlane) {
            double d = dot(normal, p1 - a) * dot(normal, p2 - a);
            return includeOnPlane ? d >= 0d : d > 0d;
        }

        public bool Intersects(Line l, out double3 p) {
            if (!CramersLow(l.pos, l.vec, out double3 d, out p)) return false;
            return d.x > 0 && d.x < 1 && d.y > 0 && d.y < 1 && d.x + d.y < 1;
        }

        public bool Intersects(Segment e, out double3 p, bool inclusive) {
            if (!CramersLow(e.a, normalize(e.b - e.a), out double3 d, out p)) return false;
            bool f1 = d.x > 0 && d.x < 1 && d.y > 0 && d.y < 1 && d.x + d.y < 1;
            bool f2 = d.z >  0 && d.z <  length(e.b - e.a); 
            bool f3 = d.z >= 0 && d.z <= length(e.b - e.a);
            return inclusive ? f1 && f3 : f1 && f2;
        }

        bool CramersLow(double3 ogn, double3 ray, out double3 det, out double3 pos) {
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
            det = new double3(u, v, t);
            return true;
        }

        #region drawer
        public void Draw() {
            GL.Begin(GL.LINE_STRIP);
            GL.Vertex((float3)a);
            GL.Vertex((float3)b);
            GL.Vertex((float3)c);
            GL.Vertex((float3)a);
            GL.End();
        }

        public void DrawCircumCircle() {
            var nrm = normalize(cross(b - a, c - a));
            var tsl = (float3)circumscribedCenter;
            var qut = Quaternion.FromToRotation(Vector3.forward, (float3)nrm);

            GL.Begin(GL.LINE_STRIP);
            for (float j = 0; j < Mathf.PI * 2.1f; j += Mathf.PI * 0.03f) {
                var pnt = double3(cos(j), sin(j), 0) * distance(tsl, a);
                var pos = qut * (float3)pnt + (Vector3)tsl;
                GL.Vertex(pos);
            }
            GL.End();
        }
        #endregion
    }
}
