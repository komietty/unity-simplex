using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d2 {
    public class Rectangle : AABB {
    public Segment Left => l;
    public Segment Top => t;
    public Segment Right => r;
    public Segment Bottom => b;
    protected Segment l, t, r, b;

    public Rectangle(Vector2 min, Vector2 max) : base(min, max) {
        l = new Segment(float2(this.min.x, this.max.y), float2(this.min.x, this.min.y));
        t = new Segment(float2(this.min.x, this.max.y), float2(this.max.x, this.max.y));
        r = new Segment(float2(this.max.x, this.max.y), float2(this.max.x, this.min.y));
        b = new Segment(float2(this.min.x, this.min.y), float2(this.max.x, this.min.y));
    }

    public void DrawGizmos() {
        l.DrawGizmos();
        t.DrawGizmos();
        r.DrawGizmos();
        b.DrawGizmos();
    }
    }
}
