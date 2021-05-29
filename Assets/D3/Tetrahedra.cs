using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static kmty.geom.d3.Util3D;

namespace kmty.geom.d3 {
    public class Tetrahedra {
        public double3 a;
        public double3 b;
        public double3 c;
        public double3 d;
        public double3x4 points => double3x4(a, b, c, d);
        public Triangle[] triangles => new Triangle[] { new Triangle(a, b, c), new Triangle(b, c, d), new Triangle(c, d, a), new Triangle(d, a, b) };
        public Sphere circumscribedSphere;

        public Tetrahedra(Vector3 a, Vector3 b, Vector3 c, Vector3 d) : this(CastV3D3(a), CastV3D3(b), CastV3D3(c), CastV3D3(d)) { }
        public Tetrahedra(double3 a, double3 b, double3 c, double3 d) {
            if (Equals(a, b) || Equals(a, c) || Equals(a, d) || 
                Equals(b, c) || Equals(b, d) || Equals(c, d))
                Debug.LogWarning("not creating a tetrahedra");
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.circumscribedSphere = GetCircumscribedSphere();
        }

        public bool HasFace(Triangle t) {
            var f0 = t.Equals(triangles[0]);
            var f1 = t.Equals(triangles[1]);
            var f2 = t.Equals(triangles[2]);
            var f3 = t.Equals(triangles[3]);
            return f0 || f1 || f2 || f3;
        }

        public bool Contains(double3 p, bool includeOnFacet) {
            var f1 = new Triangle(a, b, c).IsSameSide(d, p, includeOnFacet);
            var f2 = new Triangle(b, c, d).IsSameSide(a, p, includeOnFacet);
            var f3 = new Triangle(c, d, a).IsSameSide(b, p, includeOnFacet);
            var f4 = new Triangle(d, a, b).IsSameSide(c, p, includeOnFacet);
            return f1 && f2 && f3 && f4;
        }

        public double3 RemainingPoint(Triangle t) {
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
            var detA = determinant(new double4x4(
                a.x, a.y, a.z, 1,
                b.x, b.y, b.z, 1,
                c.x, c.y, c.z, 1,
                d.x, d.y, d.z, 1));
            var detX = determinant(new double4x4(
                a2ex, a.y, a.z, 1,
                b2ex, b.y, b.z, 1,
                c2ex, c.y, c.z, 1,
                d2ex, d.y, d.z, 1));
            var detY = -determinant(new double4x4(
                a2ex, a.x, a.z, 1,
                b2ex, b.x, b.z, 1,
                c2ex, c.x, c.z, 1,
                d2ex, d.x, d.z, 1));
            var detZ = determinant(new double4x4(
                a2ex, a.x, a.y, 1,
                b2ex, b.x, b.y, 1,
                c2ex, c.x, c.y, 1,
                d2ex, d.x, d.y, 1));
            var ctr = new double3(detX, detY, detZ) / (2 * detA);
            var rad = distance(ctr, a);
            return new Sphere(ctr, rad);
        }

        public void Draw() {
            GL.Begin(GL.LINE_STRIP);
            GL.Vertex((float3)a);
            GL.Vertex((float3)b);
            GL.Vertex((float3)c);
            GL.Vertex((float3)a);
            GL.End();
            GL.Begin(GL.LINE_STRIP);
            GL.Vertex((float3)a);
            GL.Vertex((float3)d);
            GL.Vertex((float3)c);
            GL.End();
            GL.Begin(GL.LINES);
            GL.Vertex((float3)d);
            GL.Vertex((float3)b);
            GL.End();
        }
    }
}
