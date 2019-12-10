using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.geom.d2 {
    public struct Edge {
        public Vector2 a;
        public Vector2 b;

        public Edge(Vector2 a, Vector2 b) {
            this.a = a;
            this.b = b;
        }
    }
}
