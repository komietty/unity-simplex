using System;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d2 {
    using f2 = float2;
    using f3 = float3;
    using SG = Segment;

    public struct Triangle : IEquatable<Triangle> {
        public f2 a { get; }
        public f2 b { get; }
        public f2 c { get; }
        public SG[] sgms { get; }

        public Triangle(SG e, f2 c) : this(e.a, e.b, c) { }
        public Triangle(f2 a, f2 b, f2 c) { // counterclockwise
            if (Equals(a, b) || Equals(b, c) || Equals(c, a)) throw new Exception();
            bool f = cross(float3(b - a, 0), float3(c - b, 0)).z > 0;
            this.a = a;
            this.b = f ? b : c;
            this.c = f ? c : b;
            this.sgms = new SG[] {
                new SG(this.a, this.b),
                new SG(this.b, this.c),
                new SG(this.c, this.a)
            };
        }

        public bool Contains(SG e) => Array.Exists(sgms, s => s == e);

        public f2 RemainingPoint(SG e) {
            if (e.Equals(sgms[0])) return c;
            if (e.Equals(sgms[1])) return a;
            if (e.Equals(sgms[2])) return b;
            throw new Exception();
        }

        public bool Includes(f2 p, bool close) {
            CrossForEachEdge(p, out float c1, out float c2, out float c3);
            bool f1 = (c1 > 0 && c2 > 0 && c3 > 0) || (c1 < 0 && c2 < 0 && c3 < 0);
            bool f2 = c1 * c2 * c3 == 0;
            return close ? f1 || f2 : f1;
        }

        public bool OnEdge(f2 p) {
            CrossForEachEdge(p, out float c1, out float c2, out float c3);
            return c1 * c2 * c3 == 0;
        }

        void CrossForEachEdge(f2 p, out float ca, out float cb, out float cc) {
            ca = cross(new f3(b - a, 0), new f3(p - b, 0)).z;
            cb = cross(new f3(c - b, 0), new f3(p - c, 0)).z;
            cc = cross(new f3(a - c, 0), new f3(p - a, 0)).z;
        }

        public Circle GetCircumscribledCircle() { 
            float xa2 = a.x * a.x, ya2 = a.y * a.y;
            float xb2 = b.x * b.x, yb2 = b.y * b.y;
            float xc2 = c.x * c.x, yc2 = c.y * c.y;
            float k = 2 * ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x));
            var ctr = new f2(
                ((c.y - a.y) * (xb2 - xa2 + yb2 - ya2) + (a.y - b.y) * (xc2 - xa2 + yc2 - ya2)) / k,
                ((a.x - c.x) * (xb2 - xa2 + yb2 - ya2) + (b.x - a.x) * (xc2 - xa2 + yc2 - ya2)) / k
            );
            return new Circle(ctr, distance(a, ctr));
        }

        #region IEquitable
        public override bool Equals(object obj) { return obj is Triangle triangle && Equals(triangle); }
        public static bool operator ==(Triangle left, Triangle right) { return left.Equals(right); }
        public static bool operator !=(Triangle left, Triangle right) { return !(left == right); }

        public bool Equals(Triangle other) {
            var f1 =  a.Equals(other.a) && b.Equals(other.b) && c.Equals(other.c);
            var f2 =  a.Equals(other.b) && b.Equals(other.c) && c.Equals(other.a);
            var f3 =  a.Equals(other.c) && b.Equals(other.a) && c.Equals(other.b);
            return f1 || f2 || f3;
        }

        public override int GetHashCode() {
            int h = 1474027755;
            h = h * -1521134295 + a.GetHashCode();
            h = h * -1521134295 + b.GetHashCode();
            h = h * -1521134295 + c.GetHashCode();
            return h;
        }
        #endregion
    }
}
