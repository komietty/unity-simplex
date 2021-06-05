using System;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d2 {

    public struct Line : IEquatable<Line> {
        public float2 pos;
        public float2 vec;

        public Line(float2 p, float2 v) {
            this.vec = normalize(v);
            this.pos = p;
            //var tilt = v.y / v.x;
            //this.pos = p - this.vec * p.x;
        }


        public bool Equals(Line other) {
            // fixed later
            return pos.Equals(other.pos) &&
                   vec.Equals(other.vec);
        }

        public override int GetHashCode() {
            int h = -1370888234;
            h = h * -1521134295 + pos.GetHashCode();
            h = h * -1521134295 + vec.GetHashCode();
            return h;
        }

        public override bool Equals(object obj) { return obj is Line line && Equals(line); }
        public static bool operator ==(Line left, Line right) { return left.Equals(right); } 
        public static bool operator !=(Line left, Line right) { return !(left == right); }
    }
}
