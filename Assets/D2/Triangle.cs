using System;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d2 {
    public struct Triangle {
        public float2 a;
        public float2 b;
        public float2 c;

        public Triangle(Segment e, float2 c) : this(e.a, e.b, c) { }

        public Triangle(float2 a, float2 b, float2 c) { // counterclockwise
            if (Equals(a, b) || Equals(b, c) || Equals(c, a)) throw new ArgumentException();
            bool flag = cross(float3(b - a, 0), float3(c - b, 0)).z > 0;
            this.a = a;
            this.b = flag ? b : c;
            this.c = flag ? c : b;
        }

        public bool ContainsSegment(Segment e) {
            return Equals(a, e.a) && Equals(b, e.b) || Equals(b, e.a) && Equals(a, e.b) ||
                   Equals(b, e.a) && Equals(c, e.b) || Equals(c, e.a) && Equals(b, e.b) ||
                   Equals(c, e.a) && Equals(a, e.b) || Equals(a, e.a) && Equals(c, e.b);
        }

        public float2 RemainingPoint(Segment e) {
            if      (!Equals(a, e.a) && !Equals(a, e.b)) return a;
            else if (!Equals(b, e.a) && !Equals(b, e.b)) return b;
            else if (!Equals(c, e.a) && !Equals(c, e.b)) return c;
            throw new ArgumentOutOfRangeException();
        }

        public bool Includes(float2 p, bool close) {
            CrossForEachVertices(p, out float c1, out float c2, out float c3);
            bool f1 = (c1 > 0 && c2 > 0 && c3 > 0) || (c1 < 0 && c2 < 0 && c3 < 0);
            bool f2 = c1 * c2 * c3 == 0;
            return close ? f1 || f2 : f1;
        }

        public bool OnEdge(float2 p) {
            CrossForEachVertices(p, out float c1, out float c2, out float c3);
            return c1 * c2 * c3 == 0;
        }

        public bool Excludes(float2 p) => Includes(p, true) == false;

        void CrossForEachVertices(float2 p, out float crossA, out float crossB, out float crossC) {
            crossA = cross(float3(b - a, 0), float3(p - b, 0)).z;
            crossB = cross(float3(c - b, 0), float3(p - c, 0)).z;
            crossC = cross(float3(a - c, 0), float3(p - a, 0)).z;
        }

        public Circle GetCircumscribedCircle() {
            var xa2 = a.x * a.x; var ya2 = a.y * a.y;
            var xb2 = b.x * b.x; var yb2 = b.y * b.y;
            var xc2 = c.x * c.x; var yc2 = c.y * c.y;
            var k = 2 * ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x));
            var cntr = float2(
                ((c.y - a.y) * (xb2 - xa2 + yb2 - ya2) + (a.y - b.y) * (xc2 - xa2 + yc2 - ya2)) / k,
                ((a.x - c.x) * (xb2 - xa2 + yb2 - ya2) + (b.x - a.x) * (xc2 - xa2 + yc2 - ya2)) / k
            );
            return new Circle(cntr, distance(a, cntr));
        }

    }
}
