using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.geom.d2 {
    public struct Circle {
        public Vector2 center;
        public float radius;

        public Circle(Vector2 c, float r) {
            center = c;
            radius = r;
        }

        public bool Contains(Vector2 p) {
            return (p - center).sqrMagnitude < radius * radius;
        }
    }
}
