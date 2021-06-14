using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d2 {
    using f2 = float2;
    using f3 = float3;

    public struct AABB {
        public f2 size => new f2(max.x - min.x, max.y - min.y);
        public f2 min { get; }
        public f2 max { get; }
        public f2 center { get; }

        public AABB(f2 _min, f2 _max) {
            this.min = new f2(min(_min.x, _max.x), min(_min.y, _max.y));
            this.max = new f2(max(_min.x, _max.x), max(_min.y, _max.y));
            this.center = (this.min + this.max) * 0.5f;
        }

        public AABB(f3 min, f3 max) : this(new f2(min.x, min.y), new f2(max.x, max.y)) { }

        public bool Contains(f2 p, float offset = 0f) {
            return
                min.x + offset <= p.x && p.x <= max.x - offset &&
                min.y + offset <= p.y && p.y <= max.y - offset;
        }

        public bool ContainsX(float x, float offset = 0f) => min.x + offset <= x && x <= max.x - offset;
        public bool ContainsY(float y, float offset = 0f) => min.y + offset <= y && y <= max.y - offset;
    }

    //public class Rectangle : AABB {
    //    public Segment Left => l;
    //    public Segment Top => t;
    //    public Segment Right => r;
    //    public Segment Bottom => b;
    //    protected Segment l, t, r, b;

    //    public Rectangle(Vector2 min, Vector2 max) : base(min, max) {
    //        l = new Segment(float2(this.min.x, this.max.y), float2(this.min.x, this.min.y));
    //        t = new Segment(float2(this.min.x, this.max.y), float2(this.max.x, this.max.y));
    //        r = new Segment(float2(this.max.x, this.max.y), float2(this.max.x, this.min.y));
    //        b = new Segment(float2(this.min.x, this.min.y), float2(this.max.x, this.min.y));
    //    }

    //    public void DrawGizmos() {
    //        l.DrawGizmos();
    //        t.DrawGizmos();
    //        r.DrawGizmos();
    //        b.DrawGizmos();
    //    }
    //}
}
