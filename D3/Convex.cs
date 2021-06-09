using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d3 {
    using f3 = float3;
    using d3 = double3;
    using SG = Segment;
    using TN = TriangleNode;

    public class Convex { 
        public IEnumerable<f3> originals;
        public f3[] points     { get; protected set; }
        public AABB aabb       { get; protected set; }
        public List<TN> nodes  { get; protected set; }

        public Convex(IEnumerable<f3> originals) {
            var xsrt = originals.OrderBy(p => p.x);
            var ysrt = originals.OrderBy(p => p.y);
            var zsrt = originals.OrderBy(p => p.z);
            var fars = new f3[] {
                xsrt.First(), xsrt.Last(),
                ysrt.First(), ysrt.Last(),
                zsrt.First(), zsrt.Last()
            };

            var maxd = float.MinValue;
            var maxi = new int[2];
            for (int i = 0; i < fars.Length; i++) {
                for (int j = 0; j < fars.Length; j++) {
                    if (i == j) continue;
                    var d = distancesq(fars[i], fars[j]);
                    if (d > maxd) {
                        maxd = d;
                        maxi = new int[] { i, j };
                    }
                }
            }

            var f0 = fars[maxi[0]];
            var f1 = fars[maxi[1]];
            var ss = fars.Where(f => !f.Equals(f0) && !f.Equals(f1))
                         .OrderByDescending(f => distancesq(f, f0) + distancesq(f, f1))
                         .ToArray();
            var f2 = ss[0];
            var f3 = ss[1];
            var n0 = new TN(f0, f1, f2);
            var n1 = new TN(f1, f2, f3);
            var n2 = new TN(f2, f3, f0);
            var n3 = new TN(f3, f0, f1);
            n0.neighbors = new List<TN>() { n1, n2, n3 };
            n1.neighbors = new List<TN>() { n2, n3, n0 };
            n2.neighbors = new List<TN>() { n3, n0, n1 };
            n3.neighbors = new List<TN>() { n0, n1, n2 };
            this.nodes = new List<TN>() { n0, n1, n2, n3 };
            this.points = originals.ToArray();
        }

        public void ExpandLoop() { }

        public void Expand() { 

        }


        public void Draw() {
            for (int i = 0; i < nodes.Count; i++) { nodes[i].t.Draw(); }
        }


    }

    public class TriangleNode {
        public Triangle t { get; }
        public List<TN> neighbors { get; set; }

        public TriangleNode(d3 p1, d3 p2, d3 p3) {
            this.t = new Triangle(p1, p2, p3);
            this.neighbors = new List<TN>();
        }
    }
}
