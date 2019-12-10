using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kmty.geom.d3;

namespace demo {
    public class DemoTetrahedra : MonoBehaviour {

        [SerializeField] protected Material m;
        [SerializeField] protected float scale;
        protected Tetrahedra t;

        void Start() {
            Init();
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) Init();

        }

        void Init() {
            t = new Tetrahedra(
                Random.onUnitSphere * scale, 
                Random.onUnitSphere * scale,
                Random.onUnitSphere * scale,
                Random.onUnitSphere * scale
                );
        }

        void OnRenderObject() {
            m.SetPass(0);
            t.Draw();
        }
    }
}
