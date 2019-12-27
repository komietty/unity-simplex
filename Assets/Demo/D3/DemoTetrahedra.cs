using UnityEngine;
using kmty.geom.d3;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace demo {
    public class DemoTetrahedra : MonoBehaviour {

        [SerializeField] protected Material m;
        [SerializeField] protected float scale;
        [SerializeField, Range(-1, 3)] protected int drawTriangleId;
        protected Tetrahedra t;
        protected Sphere circumscribedSphere;

        void Start() {
            t = Init();
            circumscribedSphere = t.GetCircumscribedSphere(1e-18d);
        }

        void Update() {
            if (Input.GetKey(KeyCode.Space)) {
                t = Init();
                circumscribedSphere = t.GetCircumscribedSphere(1e-18d);
            }
        }

        Tetrahedra Init() {
            return new Tetrahedra(
                (float3)UnityEngine.Random.insideUnitSphere * scale, 
                (float3)UnityEngine.Random.insideUnitSphere * scale,
                (float3)UnityEngine.Random.insideUnitSphere * scale,
                (float3)UnityEngine.Random.insideUnitSphere * scale
                );
        }

        void OnGUI() {
            var c = circumscribedSphere.center;
            var r = circumscribedSphere.radius;
            GUI.Label(new Rect(10,  10, 300, 20), "UnitCirclePrecision");
            GUI.Label(new Rect(10,  35, 300, 20), (length(t.a - c) - r).ToString("F14"));
            GUI.Label(new Rect(10,  60, 300, 20), (length(t.b - c) - r).ToString("F14"));
            GUI.Label(new Rect(10,  85, 300, 20), (length(t.c - c) - r).ToString("F14"));
            GUI.Label(new Rect(10, 110, 300, 20), (length(t.d - c) - r).ToString("F14"));
        }

        void OnRenderObject() {
            if (drawTriangleId == -1) {
                m.SetPass(1);
                t.Draw();
            } else {
                m.SetPass(0);
                t.Draw();
                m.SetPass(1);
                t.triangles[drawTriangleId].Draw();
                m.SetPass(2);
                t.triangles[drawTriangleId].DrawCircumCircle();
            }
        }

        void OnDrawGizmos() {
            if (Application.isPlaying) {
                var cs = t.GetCircumscribedSphere(1e-15d);
                Gizmos.color = Color.white;
                Gizmos.DrawCube((float3)cs.center, Vector3.one * 0.1f);
                Gizmos.DrawWireSphere((float3)cs.center, (float)cs.radius);
            }
        }
    }
}
