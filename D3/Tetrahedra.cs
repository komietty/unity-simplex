using System;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static kmty.geom.d3.Util3D;

namespace kmty.geom.d3 {
    using TR  = Triangle;
    using V3  = Vector3;
    using f3  = float3;
    using d3  = double3;
    using d34 = double3x4;
    using d44 = double4x4;

    public struct Tetrahedra {
        public d34  vrts { get; }
        public d3 a => vrts[0]; 
        public d3 b => vrts[1];
        public d3 c => vrts[2];
        public d3 d => vrts[3];

        public TR ta; 
        public TR tb;
        public TR tc;
        public TR td;

        public Tetrahedra(V3 a, V3 b, V3 c, V3 d) : this(CastV3D3(a), CastV3D3(b), CastV3D3(c), CastV3D3(d)) { }
        public Tetrahedra(d3 a, d3 b, d3 c, d3 d) {
            this.vrts = new d34(a, b, c, d);
            ta = new TR(a, b, c);
            tb = new TR(b, c, d);
            tc = new TR(c, d, a);
            td = new TR(d, a, b);

            if (Equals(a, b) || Equals(a, c) || Equals(a, d) || 
                Equals(b, c) || Equals(b, d) || Equals(c, d)) throw new Exception();
        }

        public bool HasFace(TR t) {
            if (t == ta || t == tb || t == tc || t == td) return true;
            return false;
        }

        public bool HasPoint(d3 p) {
            if (p.Equals(a)) return true;
            if (p.Equals(b)) return true;
            if (p.Equals(c)) return true;
            if (p.Equals(d)) return true;
            return false;
        }

        public bool Contains(d3 p, bool includeOnFacet) {
            var f1 = ta.IsSameSide(d, p, includeOnFacet);
            var f2 = tb.IsSameSide(a, p, includeOnFacet);
            var f3 = tc.IsSameSide(b, p, includeOnFacet);
            var f4 = td.IsSameSide(c, p, includeOnFacet);
            return f1 && f2 && f3 && f4;
        }

        public d3 RemainingPoint(TR t) {
            if (t == ta) return d;
            if (t == tb) return a;
            if (t == tc) return b;
            if (t == td) return c;
            throw new Exception();
        }

        public Sphere GetCircumscribedSphere() {
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
            return new Sphere(ctr, distance(ctr, a));
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
