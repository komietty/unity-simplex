using System;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d3 {
    public static class Util3D {
        /// <summary>
        /// https://qiita.com/tmakimoto/items/2da05225633272ef935c
        /// </summary>
        public static double3 GetIntersectionPoint(Line a, Line b, double threshold) {
            var alpha = dot(a.vec, b.vec);
            var r = a.pos - b.pos;
            var rho = dot(r, a.vec - alpha * b.vec) / (alpha * alpha - 1d);
            var tau = dot(r, alpha * a.vec - b.vec) / (alpha * alpha - 1d);
            var posA = mad(rho, a.vec, a.pos);
            var posB = mad(tau, b.vec, b.pos);
            if (lengthsq(posA - posB) < threshold) return posA;
            throw new ArgumentException();
        }

        public static bool IsIntersecting(Edge e1, Edge e2, double threshold) {
            IsIntersecting(e1, e2, out bool f, out double3 p, threshold);
            return f;
        }

        public static double3 GetIntersectionPoint(Edge e1, Edge e2, double threshold) {
            IsIntersecting(e1, e2, out bool f, out double3 p, threshold);
            return p;
        }

        static void IsIntersecting(Edge e1, Edge e2, out bool flag, out double3 pos, double threshold) {
            var v1 = e1.b - e1.a;
            var v2 = e2.b - e2.a;
            var n1 = normalize(v1);
            var n2 = normalize(v2);

            var alpha = dot(n1, n2);
            var r = e1.a - e2.a;
            var rho = dot(r, n1 - alpha * n2) / (alpha * alpha - 1d);
            var tau = dot(r, alpha * n1 - n2) / (alpha * alpha - 1d);
            var pos1 = e1.a + rho * n1;
            var pos2 = e2.a + tau * n2;
            var f1 = lengthsq(pos1 - pos2) < threshold;

            var _rho = rho / length(v1);
            var _tau = tau / length(v2);
            var f2 = _rho >= 0d && _rho <= 1d && _tau >= 0d && _tau <= 1d;
            flag = f1 && f2;
            pos = pos2;
        }
    }
}
