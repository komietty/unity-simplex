using UnityEngine;
using kmty.geom.d3;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace demo {
    public class DemoTriangle3D : MonoBehaviour {
        [SerializeField] protected Material m;
        [SerializeField] protected float scale;
        [SerializeField, Range(-5, 5)] protected float x;
        [SerializeField, Range(-5, 5)] protected float y;
        Triangle t;
        Segment e;

        void Start() {
            Init();
        }

        void Init() {
            e = new Segment(double3(0, 0, -10), double3(x, y, 10));
            t = new Triangle(
                (float3)UnityEngine.Random.onUnitSphere * scale,
                (float3)UnityEngine.Random.onUnitSphere * scale,
                (float3)UnityEngine.Random.onUnitSphere * scale
                );
        }

        void Update() {
            e.b = double3(x, y, 10);
            if (Input.GetKey(KeyCode.Space)) Init();
        }

        void OnRenderObject() {
            m.SetPass(1);
            t.Draw();
            m.SetPass(2);
            e.Draw();
        }

        void OnDrawGizmos() {
            if (Application.isPlaying) {
                Gizmos.color = Color.cyan;
                if (t.Intersects(e, out double3 p, true)) Gizmos.DrawCube((float3)p, Vector3.one * 0.1f);
            }
        }
    }
}
