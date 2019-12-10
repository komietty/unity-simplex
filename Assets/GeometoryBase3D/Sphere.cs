using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.geom.d3 {
    public struct Sphere {
        public Vector3 center;
        public float radius;

        public Sphere(Vector3 c, float r) {
            center = c;
            radius = r;
        }

        public bool Contains(Vector3 p) => (p - center).sqrMagnitude < radius * radius;
    }
}
