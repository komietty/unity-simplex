using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d2 {
    using f2 = float2;
    using f3 = float3;
    using SG = Segment;

    public class Convex {
        public IEnumerable<f2> originals;
        public f2[] points { get; protected set; }
        public SG[] segments { get; protected set; }
        public AABB aabb { get; protected set; }

        public Convex(IEnumerable<f2> originals) {
            var xsrt = originals.OrderBy(p => p.x);
            var xmin = xsrt.First();
            var xmax = xsrt.Last();
            var dsrt = originals.OrderBy(p => Dist(new SG(xmin, xmax), p));
            var dmin = dsrt.First();
            var dmax = dsrt.Last();
            this.originals = originals;
            this.points = new f2[] { xmin, dmin, xmax, dmax };
            Reset();
        }

        void Reset() {
            this.segments = new SG[points.Length];
            var l = points.Length;
            var xmin = float.MaxValue;
            var ymin = float.MaxValue;
            var xmax = float.MinValue;
            var ymax = float.MinValue;
            for (int i = 0; i < l; i++) {
                segments[i] = new SG(points[i], points[(i + 1) % l]);
                xmin = min(xmin, points[i].x);
                ymin = min(ymin, points[i].y);
                xmax = max(xmax, points[i].x);
                ymax = max(ymax, points[i].y);
            }
            this.aabb = new AABB(new f2(xmin, ymin), new f2(xmax, ymax));
        }

        public float Dist(SG s, f2 p) => cross(new f3(s.b - s.a, 0), new f3(p - s.a, 0)).z;

        public bool Contains(f2 p) {
            foreach (var s in segments) {
                if (cross(new f3(s.b - s.a, 0), new f3(p - s.a, 0)).z < 0) return false;
            }
            return true;
        }

        public void ExpandLoop(int maxitr = int.MaxValue) {
            int itr = 0;
            while (itr < maxitr) {
                itr++;
                if (!Expand()) break;
            }
        }

        public bool Expand() {
            var outs = originals.Where(p => !Contains(p));
            if (outs.Count() == 0) return false;
            var list = new List<f2>();
            foreach (var s in segments) {
                var srt = outs.OrderBy(p => Dist(s, p));
                list.Add(s.a);
                if (srt.Count() > 0) {
                    var f = srt.First();
                    if (Dist(s, f) < 0) list.Add(f);
                }
            }
            points = list.ToArray();
            Reset();
            return true;
        }


        public void Draw() {
            var l = points.Length;
            GL.Begin(GL.LINE_STRIP);
            for (int i = 0; i <= l; i++)
                GL.Vertex(new f3(points[i % l], 0));
            GL.End();
        }
    }
}
