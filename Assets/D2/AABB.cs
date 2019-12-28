using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d2 {

    [System.Serializable]
    public class AABB {
        public float2 Min => min;
        public float2 Max => max;
        public float2 Center => center;
        public float2 Size => float2(max.x - min.x, max.y - min.y);
        [SerializeField] protected Vector2 min, max, center;

        public AABB(float2 min, float2 max) {
            this.min = float2(math.min(min.x, max.x), math.min(min.y, max.y));
            this.max = float2(math.max(min.x, max.x), math.max(min.y, max.y));
            this.center = (this.min + this.max) * 0.5f;
        }

        public AABB(float3 min, float3 max) : this(float2(min.x, min.y), float2(max.x, max.y)) { }

        public bool Contains(float2 p, float offset = 0f) {
            return
                min.x + offset <= p.x && p.x <= max.x - offset &&
                min.y + offset <= p.y && p.y <= max.y - offset;
        }

        public bool ContainsX(float x, float offset = 0f) => min.x + offset <= x && x <= max.x - offset;
        public bool ContainsY(float y, float offset = 0f) => min.y + offset <= y && y <= max.y - offset;
    }
}
