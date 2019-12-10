using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.geom.d3 {
    public struct Edge {
        public Vector3 a;
        public Vector3 b;

        public Edge(Vector3 a, Vector3 b) {
            this.a = a;
            this.b = b;
        }
    }
}
