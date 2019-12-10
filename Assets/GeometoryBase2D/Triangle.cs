using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace kmty.geom.d2 {
    public struct Triangle {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;

        public Triangle(Edge e, Vector2 c) : this(e.a, e.b, c) { }

        public Triangle(Vector2 a, Vector2 b, Vector2 c) { // counterclockwise
            if (a == b || b == c || c == a) throw new ArgumentException();
            bool flag = Vector3.Cross(b - a, c - b).z > 0;
            this.a = a;
            this.b = flag ? b : c;
            this.c = flag ? c : b;
        }

        public bool ContainsEdge(Edge e) {
            return (a == e.a && b == e.b) || (b == e.a && a == e.b) ||
                   (b == e.a && c == e.b) || (c == e.a && b == e.b) ||
                   (c == e.a && a == e.b) || (a == e.a && c == e.b);
        }

        public Vector2 RemainingPoint(Edge e) {
            if      (a != e.a && a != e.b) return a;
            else if (b != e.a && b != e.b) return b;
            else if (c != e.a && c != e.b) return c;
            throw new ArgumentOutOfRangeException();
        }

        public bool Includes(Vector2 p, bool close) {
            CrossForEachVertices(p, out float c1, out float c2, out float c3);
            bool f1 = (c1 > 0 && c2 > 0 && c3 > 0) || (c1 < 0 && c2 < 0 && c3 < 0);
            bool f2 = c1 * c2 * c3 == 0;
            return close ? f1 || f2 : f1;
        }

        public bool OnEdge(Vector2 p) {
            CrossForEachVertices(p, out float c1, out float c2, out float c3);
            return c1 * c2 * c3 == 0;
        }

        public bool Excludes(Vector2 p) => Includes(p, true) == false;

        void CrossForEachVertices(Vector2 p, out float crossA, out float crossB, out float crossC) {
            crossA = Vector3.Cross(b - a, p - b).z;
            crossB = Vector3.Cross(c - b, p - c).z;
            crossC = Vector3.Cross(a - c, p - a).z;
        }

        public Circle GetCircumscribedCircle() {
            var xa2 = a.x * a.x; var ya2 = a.y * a.y;
            var xb2 = b.x * b.x; var yb2 = b.y * b.y;
            var xc2 = c.x * c.x; var yc2 = c.y * c.y;
            var k = 2 * ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x));
            var cntr = new Vector2(
                ((c.y - a.y) * (xb2 - xa2 + yb2 - ya2) + (a.y - b.y) * (xc2 - xa2 + yc2 - ya2)) / k,
                ((a.x - c.x) * (xb2 - xa2 + yb2 - ya2) + (b.x - a.x) * (xc2 - xa2 + yc2 - ya2)) / k
            );
            return new Circle(cntr, Vector2.Distance(a, cntr));
        }

    }
}
