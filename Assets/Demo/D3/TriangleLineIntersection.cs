using UnityEngine;
using kmty.geom.d3;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace demo {
    public class TriangleLineIntersection : MonoBehaviour {
        public Material mat;
        public KeyCode reset = KeyCode.R;
        public int triangleSize;
        Triangle t;
        Line l;
        double3 p;
        bool f;

        void Start() {
            SetTestCase();
        }
        void Update() {
            if (Input.GetKeyDown(reset)) SetTestCase();
        }

        void SetTestCase() {
            t = new Triangle(
                (float3)UnityEngine.Random.onUnitSphere * triangleSize, 
                (float3)UnityEngine.Random.onUnitSphere * triangleSize, 
                (float3)UnityEngine.Random.onUnitSphere * triangleSize);
            l = new Line(double3(1), (float3)UnityEngine.Random.onUnitSphere);
            f = t.Intersects(l, out p);
        }

        void OnRenderObject() {
            var t1 = float3(1, 1, 0);
            var t2 = float3(0, 0, 0);
            var t3 = float3(0.4f, 0.1f, 0.9f);
            var _p = float3(1f, 0f, 0);
            var _q = float3(0f, 1f, 0f);

            var f = new Triangle(t1, t2, t3).Intersects(new Line(_p, _q - _p), out double3 intersects);
            Debug.Log(f);

            GL.PushMatrix();
            mat.SetPass(0);
            GL.Begin(GL.TRIANGLES);
            //GL.Vertex(t.a);
            //GL.Vertex(t.b);
            //GL.Vertex(t.c);
            GL.Vertex(t1);
            GL.Vertex(t2);
            GL.Vertex(t3);
            GL.End();

            mat.SetPass(2);
            GL.Begin(GL.LINE_STRIP);
            GL.Vertex(_p);
            GL.Vertex(_q);
            GL.End();

            //  if (f) {
            //      mat.SetPass(2);
            //      GL.Begin(GL.TRIANGLE_STRIP);
            //      GL.Vertex(p + Vector3.left * 0.5f);
            //      GL.Vertex(p + Vector3.up * 0.5f);
            //      GL.Vertex(p + Vector3.right * 0.5f);
            //      GL.Vertex(p + Vector3.down * 0.5f);
            //      GL.Vertex(p + Vector3.left * 0.5f);
            //      GL.End();
            //  }

            GL.Begin(GL.TRIANGLE_STRIP);
            mat.SetPass(2);
            GL.Vertex((float3)intersects + (float3)Vector3.left * 0.02f);
            GL.Vertex((float3)intersects + (float3)Vector3.up * 0.02f);
            GL.Vertex((float3)intersects + (float3)Vector3.right * 0.02f);
            GL.Vertex((float3)intersects + (float3)Vector3.down * 0.02f);
            GL.Vertex((float3)intersects + (float3)Vector3.left * 0.02f);
            GL.End();

            GL.PopMatrix();
        }
    }
}