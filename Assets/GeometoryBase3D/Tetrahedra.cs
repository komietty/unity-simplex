using UnityEngine;
using System;

namespace kmty.geom.d3 {
    public class Tetrahedra {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
        public Vector3 d;
        public Triangle[] triangles => new Triangle[] { new Triangle(a, b, c), new Triangle(b, c, d), new Triangle(c, d, a), new Triangle(d, a, b) };

        public Tetrahedra(Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public bool HasFace(Triangle t) {
            var f0 = t.Equals(triangles[0]);
            var f1 = t.Equals(triangles[1]);
            var f2 = t.Equals(triangles[2]);
            var f3 = t.Equals(triangles[3]);
            return f0 || f1 || f2 || f3;
        }

        public bool Contains(Vector3 p, bool includeOnPlane) {
            var f1 = new Triangle(a, b, c).SameSide(d, p, includeOnPlane);
            var f2 = new Triangle(b, c, d).SameSide(a, p, includeOnPlane);
            var f3 = new Triangle(c, d, a).SameSide(b, p, includeOnPlane);
            var f4 = new Triangle(d, a, b).SameSide(c, p, includeOnPlane);
            return f1 && f2 && f3 && f4;
        }

        public Vector3 RemainingPoint(Triangle t) {
            if (a != t.a && a != t.b && a != t.c) return a;
            else if (b != t.a && b != t.b && b != t.c) return b;
            else if (c != t.a && c != t.b && c != t.c) return c;
            else if (d != t.a && d != t.b && d != t.c) return d;
            throw new ArgumentOutOfRangeException();
        }

        public Sphere GetCircumscribedSphere() {
            var triA = new Triangle(a, b, c);
            var triB = new Triangle(b, c, d);
            var cntr = Util3D.GetIntersectionPoint(
                                new Line(triA.GetCircumCenter(), triA.GetNormal()),
                                new Line(triB.GetCircumCenter(), triB.GetNormal()));
            return new Sphere(cntr, Vector3.Distance(cntr, a));

            #region another solver
            /// <summary>
            // http://mathworld.wolfram.com/Circumsphere.html
            /// </summary>
            //var a2 = Vector3.Scale(a, a);
            //var b2 = Vector3.Scale(b, b);
            //var c2 = Vector3.Scale(c, c);
            //var d2 = Vector3.Scale(d, d);
            //var _a = Matrix4x4.zero;
            //var _m = Matrix4x4.zero;
            //_a.SetRow(0, new Vector4(a.x, a.y, a.z, 1));
            //_a.SetRow(1, new Vector4(b.x, b.y, b.z, 1));
            //_a.SetRow(2, new Vector4(c.x, c.y, c.z, 1));
            //_a.SetRow(3, new Vector4(d.x, d.y, d.z, 1));
            //_m.SetRow(0, new Vector4(a2.x + a2.y + a2.z, a.y, a.z, 1));
            //_m.SetRow(1, new Vector4(b2.x + b2.y + b2.z, b.y, b.z, 1));
            //_m.SetRow(2, new Vector4(c2.x + c2.y + c2.z, c.y, c.z, 1));
            //_m.SetRow(3, new Vector4(d2.x + d2.y + d2.z, d.y, d.z, 1));
            //var detA = _a.determinant;
            //var detX = _m.determinant;
            //var detY = _m.determinant * -1;
            //var detZ = _m.determinant;
            //var center = new Vector3(detX, detY, detZ) / (2 * detA);
            //var radius = Vector3.Distance(center, a);
            //return new Sphere(center, radius);
            #endregion
        }

        public void Draw() {
            GL.Begin(GL.LINE_STRIP);
            GL.Vertex(a);
            GL.Vertex(b);
            GL.Vertex(c);
            GL.Vertex(a);
            GL.End();
            GL.Begin(GL.LINE_STRIP);
            GL.Vertex(a);
            GL.Vertex(d);
            GL.Vertex(c);
            GL.End();
            GL.Begin(GL.LINES);
            GL.Vertex(d);
            GL.Vertex(b);
            GL.End();
        }
    }
}
