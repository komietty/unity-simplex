using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace kmty.geom.d3 {
    public struct Triangle {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Triangle(Vector3 a, Vector3 b, Vector3 c) {
            if (a == b || b == c || c == a)
                throw new ArgumentException();
            this.a = a;
            this.b = b;
            this.c = c;
        }

        //public static bool operator ==(Triangle t1, Triangle t2) {
        //    Equation(t1, t2, out bool f1, out bool f2, out bool f3);
        //    return f1 || f2 || f3;
        //}

        //public static bool operator !=(Triangle t1, Triangle t2) {
        //    Equation(t1, t2, out bool f1, out bool f2, out bool f3);
        //    return !(f1 || f2 || f3);
        //}

        public bool Equals(Triangle t) {
            var f1 = t.a == a && t.b == b && t.c == c;
            var f2 = t.b == a && t.c == b && t.a == c;
            var f3 = t.c == a && t.a == b && t.b == c;
            var f4 = t.a == a && t.c == b && t.b == c;
            var f5 = t.b == a && t.a == b && t.c == c;
            var f6 = t.c == a && t.b == b && t.a == c;
            return f1 || f2 || f3 || f4 || f5 || f6;
        }

        public Edge Remaining(Vector3 p) {
            if (p == a) return new Edge(b, c);
            else if (p == b) return new Edge(c, a);
            else if (p == c) return new Edge(a, b);
            throw new ArgumentOutOfRangeException();
        }

        public bool SameSide(Vector3 p1, Vector3 p2, bool includeOnPlane) {
            var n = GetNormal();
            var d = Vector3.Dot(n, p1 - a) * Vector3.Dot(n, p2 - a);
            if (includeOnPlane) return d >= 0;
            else return d > 0;
        }

        float det3x3(Vector3 vecA, Vector3 vecB, Vector3 vecC) {
            return ((vecA.x * vecB.y * vecC.z)
                  + (vecA.y * vecB.z * vecC.x)
                  + (vecA.z * vecB.x * vecC.y)
                  - (vecA.x * vecB.z * vecC.y)
                  - (vecA.y * vecB.x * vecC.z)
                  - (vecA.z * vecB.y * vecC.x)
                  );
        }

        public bool Intersects(Line l, out Vector3 pos) {
            // using cramer's rule
            var origin = l.pos;
            var ray = l.vec;
            var v0 = a;
            var v1 = b;
            var v2 = c;
            var invRay = -ray;
            var edge1 = v1 - v0;
            var edge2 = v2 - v0;

            var denominator = det3x3(edge1, edge2, invRay);
            if (denominator == 0) Debug.LogError("line is paralleled");

            var d = origin - v0;
            var u = det3x3(d, edge2, invRay) / denominator;
            var v = det3x3(edge1, d, invRay) / denominator;
            var t = det3x3(edge1, edge2, d) / denominator;

            pos = origin + ray * t;
            return u > 0 && u < 1 && v > 0 && v < 1 && u + v < 1;

            //            var v1 = b - a;
            //            var v2 = c - a;
            //            var v3 = - l.vec;
            //            var v4 = l.pos - a;
            //
            //            var matU = Matrix4x4.identity;
            //            matU.SetColumn(0, v4);
            //            matU.SetColumn(1, v2);
            //            matU.SetColumn(2, v3);
            //
            //            var matV = Matrix4x4.identity;
            //            matV.SetColumn(0, v1);
            //            matV.SetColumn(1, v4);
            //            matV.SetColumn(2, v3);
            //
            //            var matO = Matrix4x4.identity;
            //            matO.SetColumn(0, v1);
            //            matO.SetColumn(1, v2);
            //            matO.SetColumn(2, v3);
            //
            //            var detU = matU.determinant;
            //            var detV = matV.determinant;
            //            var detO = matO.determinant;
            //
            //            var u = detU / detO;
            //            var v = detV / detO;
            //
            //            pos = u * v1 + v * v2;
            //
            //            // TODO: Consider degenerate cases like line crosses edges of the triangle i.e. u == 0...
            //            var f1 = u > 0 && u < 1;
            //            var f2 = v > 0 && v < 1;
            //            var f3 = u + v > 0 && u + v < 1;
            //
            //            return f1 && f2 && f3;
        }

        public Vector3 GetNormal() { // TODO : Define which rotation determine normal side
            return Vector3.Cross(b - a, c - b).normalized;
        }

        public Vector3 GetCircumCenter() {
            var p1 = Vector3.Lerp(a, b, 0.5f);
            var p2 = Vector3.Lerp(b, c, 0.5f);
            var vecAB = b - a;
            var vecBC = c - b;
            var axis = Vector3.Cross(vecAB, vecBC).normalized;
            var dir1 = Vector3.Cross(vecAB, axis).normalized;
            var dir2 = Vector3.Cross(vecBC, axis).normalized;
            return Util3D.GetIntersectionPoint(new Line(p1, dir1), new Line(p2, dir2));
        }

    }
}
