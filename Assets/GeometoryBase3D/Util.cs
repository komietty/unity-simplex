using UnityEngine;
using System;

namespace kmty.geom.d3 {
    public static class Util3D {
        /// <summary>
        /// https://qiita.com/tmakimoto/items/2da05225633272ef935c
        /// </summary>
        public static Vector3 GetIntersectionPoint(Line a, Line b, float threshold = 0.001f) {
            var alpha = Vector3.Dot(a.vec, b.vec);
            var r = a.pos - b.pos;
            var rho = Vector3.Dot(r, a.vec - alpha * b.vec) / (alpha * alpha - 1);
            var tau = Vector3.Dot(r, alpha * a.vec - b.vec) / (alpha * alpha - 1);
            var posA = a.pos + rho * a.vec;
            var posB = b.pos + tau * b.vec;
            if (Vector3.SqrMagnitude(posA - posB) < threshold) return posA;
            throw new ArgumentException();
        }

        public static bool IsIntersecting(Edge e1, Edge e2, float threshold = 0.001f) {
            IsIntersecting(e1, e2, out bool f, out Vector3 p);
            return f;
        }

        public static Vector3 GetIntersectionPoint(Edge e1, Edge e2, float threshold = 0.001f) {
            IsIntersecting(e1, e2, out bool f, out Vector3 p);
            return p;
        }

        static void IsIntersecting(Edge e1, Edge e2, out bool flag, out Vector3 pos, float threshold = 0.001f) {
            var v1 = e1.b - e1.a;
            var v2 = e2.b - e2.a;
            var n1 = v1.normalized;
            var n2 = v2.normalized;

            var alpha = Vector3.Dot(n1, n2);
            var r = e1.a - e2.a;
            var rho = Vector3.Dot(r, n1 - alpha * n2) / (alpha * alpha - 1);
            var tau = Vector3.Dot(r, alpha * n1 - n2) / (alpha * alpha - 1);
            var pos1 = e1.a + rho * n1;
            var pos2 = e2.a + tau * n2;
            var f1 = Vector3.SqrMagnitude(pos1 - pos2) < threshold;

            var _rho = rho / v1.magnitude;
            var _tau = tau / v2.magnitude;
            var f2 = _rho >= 0 && _rho <= 1 && _tau >= 0 && _tau <= 1;
            flag = f1 && f2;
            pos = pos2;
        }
    }
}
