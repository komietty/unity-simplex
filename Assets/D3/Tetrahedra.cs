using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d3 {
    public class Tetrahedra {
        public double3 a;
        public double3 b;
        public double3 c;
        public double3 d;
        public double3x4 points => double3x4(a, b, c, d);
        public Triangle[] triangles => new Triangle[] { new Triangle(a, b, c), new Triangle(b, c, d), new Triangle(c, d, a), new Triangle(d, a, b) };

        public Tetrahedra(double3 a, double3 b, double3 c, double3 d) {
            if (Equals(a, b) || Equals(a, c) || Equals(a, d) || 
                Equals(b, c) || Equals(b, d) || Equals(c, d))
                Debug.LogWarning("not creating a tetrahedra");
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

        public bool Contains(double3 p, bool includeOnPlane) {
            var f1 = new Triangle(a, b, c).IsSameSide(d, p, includeOnPlane);
            var f2 = new Triangle(b, c, d).IsSameSide(a, p, includeOnPlane);
            var f3 = new Triangle(c, d, a).IsSameSide(b, p, includeOnPlane);
            var f4 = new Triangle(d, a, b).IsSameSide(c, p, includeOnPlane);
            return f1 && f2 && f3 && f4;
        }

        public double3 RemainingPoint(Triangle t) {
            if      (!Equals(a,t.a) && !Equals(a , t.b) && !Equals(a != t.c)) return a;
            else if (!Equals(b,t.a) && !Equals(b , t.b) && !Equals(b != t.c)) return b;
            else if (!Equals(c,t.a) && !Equals(c , t.b) && !Equals(c != t.c)) return c;
            else if (!Equals(d,t.a) && !Equals(d , t.b) && !Equals(d != t.c)) return d;
            throw new System.ArgumentOutOfRangeException();
        }

        public Sphere GetCircumscribedSphere(double precisition) {
            var triA = new Triangle(a, b, c);
            var triB = new Triangle(b, c, d);
            var cntr = Util3D.GetIntersectionPoint(
                                new Line(triA.GetCircumCenter(precisition), triA.normal),
                                new Line(triB.GetCircumCenter(precisition), triB.normal),
                                precisition
                                );
            return new Sphere(cntr, distance(cntr, a));

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
