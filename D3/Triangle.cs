using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static kmty.geom.d3.Utils;

/*
TODO:
1: circumscribed center has to be deterniministically called because
two vertical line has to cross each other so that they are on same triangle
2: Test Distance method, ref: https://cvtech.cc/pointdist/
SUMMARY: 
Borders of non oriented 2-simplex in R^3.
mtx: 三角形空間のe1, e2についての表現行列
inv: e1, e2空間の三角形の2辺についての表現行列
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
        public d3 n { get; }
        double3x3 mtx;
        double3x3 inv;

        public Triangle(SG s, d3 p) : this(s.a, s.b, p) { } 
        public Triangle(V3 p1, V3 p2, V3 p3) : this(V3D3(p1), V3D3(p2), V3D3(p3)) { } 
        public Triangle(d3 p1, d3 p2, d3 p3) {
            if (Equals(p1, p2) || Equals(p2, p3) || Equals(p3, p1)) throw new System.Exception();
            this.a = p1;
            this.b = p2;
            this.c = p3;
            this.n = normalize(cross(b - a, c - a));
            this.mtx = new double3x3(b - a, c - a, cross(b - a, c - a));
            this.inv = math.inverse(mtx);
        }

        public d3 EuclidCord2TriangleCord(d3 p) => math.mul(inv, p - this.a); 
        public d3 TriangleCord2EuclidCord(d3 p) => math.mul(mtx, p) + this.a; 

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

        public d3 Distance(d3 p) {
            var v = length(cross(p - a, dot(p - b, p - c))) / 6d;
            var s = length(cross(b - a, c - a)) / 2d;
            return 3 * v / s;
        }

        /// <summary>
        /// Close Enoughly identical to Intersection algolithm of Cramer's low one,
        /// but some how causes an error in BistellarFllip of 3d.
        /// </summary>
        public bool IntersectsUsingMtx(SG e, out d3 point, out bool onedge) {
            var va = EuclidCord2TriangleCord(e.a);
            var vb = EuclidCord2TriangleCord(e.b);
            point  = default;
            onedge = default;

            if (va.z * vb.z > 0) return false;

            var r = abs(va.z) / (abs(va.z) + abs(vb.z));
            var p = va * (1 - r) + vb * r;

            if (p.x >= 0 && p.y >= 0 && p.x + p.y <= 1) {
                point  = TriangleCord2EuclidCord(p);
                onedge = p.x == 0 && p.y == 0 && p.x + p.y == 1;
                return true;
            }
            return false;
        }


        public d3 GetGravityCenter() =>  (a + b + c) / 3d;

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

        public static (int num, d3 p1, d3 p2) Intersects(Triangle t1, Triangle t2) {
            var f1 = t1.Intersects(new SG(t2.a, t2.b), out d3 p1, out bool o1);
            var f2 = t1.Intersects(new SG(t2.b, t2.c), out d3 p2, out bool o2);
            var f3 = t1.Intersects(new SG(t2.c, t2.a), out d3 p3, out bool o3);
            var f4 = t2.Intersects(new SG(t1.a, t1.b), out d3 p4, out bool o4);
            var f5 = t2.Intersects(new SG(t1.b, t1.c), out d3 p5, out bool o5);
            var f6 = t2.Intersects(new SG(t1.c, t1.a), out d3 p6, out bool o6);

            // intersect one edge (for each other)
            if(f1 && !f2 && !f3) return (num: 1, p1: p1, p2: default);
            if(f2 && !f3 && !f1) return (num: 1, p1: p2, p2: default);
            if(f3 && !f1 && !f2) return (num: 1, p1: p3, p2: default);

            // t1 contains t2, intersect one edge, one vert
            if (f1 && f2 && f3) return (num: 2, p1: p1, p2: all(p1 == p2) ? p3 : p2);

            // t1 contains t2, intersect two edge 
            if (f1 && f2 && !f3) return (num: 2, p1: p1, p2: p2);
            if (f2 && f3 && !f1) return (num: 2, p1: p2, p2: p3);
            if (f3 && f1 && !f2) return (num: 2, p1: p3, p2: p1);
            
            // t2 contains t1, intersect one edge, one vert
            if (f4 && f5 && f6) return (num: 2, p1: p4, p2: all(p4 == p5) ? p6 : p5);

            // t2 contains t1, intersect two edge 
            if (f4 && f5 && !f6) return (num: 2, p1: p4, p2: p5);
            if (f5 && f6 && !f4) return (num: 2, p1: p5, p2: p6);
            if (f6 && f4 && !f5) return (num: 2, p1: p6, p2: p4);

            return (num: 0, p1: default, p2: default);
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
