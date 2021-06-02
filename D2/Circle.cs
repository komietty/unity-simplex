using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d2 {

    public struct Circle {
        public float2 center;
        public float radius;

        public Circle(float2 c, float r) {
            center = c;
            radius = r;
        }

        public bool Contains(float2 p) => lengthsq(p - center) < radius * radius; 
    }
}
