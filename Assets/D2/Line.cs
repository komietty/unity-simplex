using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d2 {

    public struct Line {
        public float2 pos;
        public float2 vec;

        public Line(float2 pos, float2 vec) {
            this.pos = pos;
            this.vec = normalize(vec);
        }
    }
}
