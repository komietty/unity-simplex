using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.geom.d3 {
    public struct Line {
        public Vector3 pos;
        public Vector3 vec;

        public Line(Vector3 p, Vector3 v) {
            pos = p;
            vec = v.normalized;
        }
    }
}
