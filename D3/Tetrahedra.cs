using UnityEngine;
using static Unity.Mathematics.math;
using static kmty.geom.d3.Util3D;

namespace kmty.geom.d3 {
    using TR = Triangle;
    using V3 = UnityEngine.Vector3;
    using f3 = Unity.Mathematics.float3;
    using d3 = Unity.Mathematics.double3;
    using d34 = Unity.Mathematics.double3x4;
    using d44 = Unity.Mathematics.double4x4;

    public class Tetrahedra {
        public d3 a, b, c, d;
        public d34 points => double3x4(a, b, c, d);
        public TR[] triangles;
        public Sphere circumscribedSphere;

        public Tetrahedra(V3 a, V3 b, V3 c, V3 d) : this(CastV3D3(a), CastV3D3(b), CastV3D3(c), CastV3D3(d)) { }
        public Tetrahedra(d3 a, d3 b, d3 c, d3 d) {
            if (Equals(a, b) || Equals(a, c) || Equals(a, d) || 
                Equals(b, c) || Equals(b, d) || Equals(c, d))
                Debug.LogWarning("not creating a tetrahedra");
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.circumscribedSphere = GetCircumscribedSphere();
            this.triangles = new TR[] {
                new TR(a, b, c),
                new TR(b, c, d),
                new TR(c, d, a),
                new TR(d, a, b)
            };
        }

        public bool HasFace(TR t) {
            var f0 = t.Equals(triangles[0]);
            var f1 = t.Equals(triangles[1]);
            var f2 = t.Equals(triangles[2]);
            var f3 = t.Equals(triangles[3]);
            return f0 || f1 || f2 || f3;
        }

        public bool HasPoint(d3 p) {
            var f0 = Equals(p, a);
            var f1 = Equals(p, b);
            var f2 = Equals(p, c);
            var f3 = Equals(p, d);
            return f0 || f1 || f2 || f3;
        }

        public bool Contains(d3 p, bool includeOnFacet) {
            var f1 = triangles[0].IsSameSide(d, p, includeOnFacet);
            var f2 = triangles[1].IsSameSide(a, p, includeOnFacet);
            var f3 = triangles[2].IsSameSide(b, p, includeOnFacet);
            var f4 = triangles[3].IsSameSide(c, p, includeOnFacet);
            return f1 && f2 && f3 && f4;
        }

        public d3 RemainingPoint(TR t) {
            if      (!Equals(a, t.a) && !Equals(a, t.b) && !Equals(a, t.c)) return a;
            else if (!Equals(b, t.a) && !Equals(b, t.b) && !Equals(b, t.c)) return b;
            else if (!Equals(c, t.a) && !Equals(c, t.b) && !Equals(c, t.c)) return c;
            else if (!Equals(d, t.a) && !Equals(d, t.b) && !Equals(d, t.c)) return d;
            throw new System.ArgumentOutOfRangeException();
        }

        Sphere GetCircumscribedSphere() {
            /// <summary>
            // http://mathworld.wolfram.com/Circumsphere.html
            /// </summary>
            var a2 = a * a; var a2ex = a2.x + a2.y + a2.z; 
            var b2 = b * b; var b2ex = b2.x + b2.y + b2.z;
            var c2 = c * c; var c2ex = c2.x + c2.y + c2.z;
            var d2 = d * d; var d2ex = d2.x + d2.y + d2.z;
            var detA = determinant(new d44(
                a.x, a.y, a.z, 1,
                b.x, b.y, b.z, 1,
                c.x, c.y, c.z, 1,
                d.x, d.y, d.z, 1));
            var detX = determinant(new d44(
                a2ex, a.y, a.z, 1,
                b2ex, b.y, b.z, 1,
                c2ex, c.y, c.z, 1,
                d2ex, d.y, d.z, 1));
            var detY = -determinant(new d44(
                a2ex, a.x, a.z, 1,
                b2ex, b.x, b.z, 1,
                c2ex, c.x, c.z, 1,
                d2ex, d.x, d.z, 1));
            var detZ = determinant(new d44(
                a2ex, a.x, a.y, 1,
                b2ex, b.x, b.y, 1,
                c2ex, c.x, c.y, 1,
                d2ex, d.x, d.y, 1));
            var ctr = new d3(detX, detY, detZ) / (2 * detA);
            var rad = distance(ctr, a);
            return new Sphere(ctr, rad);
        }

        public void Log() { Debug.Log($"a:{a}, b:{b}, c:{c}, d:{d}"); }
        public void Draw() {
            GL.Begin(GL.LINES);
            GL.Vertex((f3)a); GL.Vertex((f3)b);
            GL.Vertex((f3)b); GL.Vertex((f3)c);
            GL.Vertex((f3)c); GL.Vertex((f3)a);
            GL.Vertex((f3)a); GL.Vertex((f3)d);
            GL.Vertex((f3)b); GL.Vertex((f3)d);
            GL.Vertex((f3)c); GL.Vertex((f3)d);
            GL.End();
        }
    }
}
