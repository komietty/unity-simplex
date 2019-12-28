using UnityEngine;
using System;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d3 {
    public struct Triangle {
        public double3 a;
        public double3 b;
        public double3 c;
        //public double3x3 points => double3x3(a, b, c);
        public double3 normal => normalize(cross(b - a, c - a));

        public Triangle(double3 a, double3 b, double3 c) {
            if (Equals(a, b) || Equals(b, c) || Equals(c, a)) Debug.LogWarning("not creating a triangle");
            this.a = a;
            this.b = b;
            this.c = c;
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
            // using cramer's rule
            double3 origin = l.pos, ray = l.vec, e1 = b - a, e2 = c - a; 
            var denominator = determinant(double3x3(e1, e2, -ray));
            if (denominator == 0) Debug.LogWarning("line is paralleled");

            var d = origin - a;
            var u = determinant(double3x3(d, e2, -ray)) / denominator;
            var v = determinant(double3x3(e1, d, -ray)) / denominator;
            var t = determinant(double3x3(e1, e2, d))   / denominator;

            p = origin + ray * t;
            return u > 0d && u < 1d && v > 0d && v < 1d && u + v < 1d;
        }

        public bool Intersects(Segment e, out double3 p, bool inclusive) {
            // using cramer's rule
            double3 origin = e.a, ray = normalize(e.b - e.a), e1 = b - a, e2 = c - a;
            var denominator = determinant(double3x3(e1, e2, -ray));
            if (denominator == 0) Debug.LogWarning("line is paralleled");

            var d = origin - a;
            var u = determinant(double3x3(d, e2, -ray)) / denominator;
            var v = determinant(double3x3(e1, d, -ray)) / denominator;
            var t = determinant(double3x3(e1, e2, d))   / denominator;

            p = origin + ray * t;
            bool f1 = u > 0d && u < 1d && v > 0d && v < 1d && u + v < 1d;
            bool f2 = t >  0 && t <  length(e.b - e.a); 
            bool f3 = t >= 0 && t <= length(e.b - e.a);
            return inclusive ? f1 && f3 : f1 && f2;
        }

        public double3 GetCircumCenter(double threshold) {
            var p1 = lerp(a, b, 0.5d);
            var p2 = lerp(b, c, 0.5d);
            var vecAB = b - a;
            var vecBC = c - b;
            var axis = normalize(cross(vecAB, vecBC));
            var dir1 = normalize(cross(vecAB, axis));
            var dir2 = normalize(cross(vecBC, axis));
            return Util3D.GetIntersectionPoint(new Line(p1, dir1), new Line(p2, dir2), threshold);
        }

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
            var tsl = (float3)GetCircumCenter(1e-10d);
            var qut = Quaternion.FromToRotation(Vector3.forward, (float3)nrm);

            GL.Begin(GL.LINE_STRIP);
            for (float j = 0; j < Mathf.PI * 2.1f; j += Mathf.PI * 0.03f) {
                var pnt = double3(cos(j), sin(j), 0) * distance(tsl, a);
                var pos = qut * (float3)pnt + (Vector3)tsl;
                GL.Vertex(pos);
            }
            GL.End();
        }
    }
}
