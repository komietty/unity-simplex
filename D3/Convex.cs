using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Assertions;
using static Unity.Mathematics.math;

namespace kmty.geom.d3 {
    using f3 = float3;
    using d3 = double3;
    using SG = Segment;
    using TN = TriangleNode;

    public class Convex { 
        private IEnumerable<f3> outsides;
        public AABB aabb       { get; protected set; }
        public List<TN> nodes  { get; protected set; }
        public List<d3> vrtcs  { get; protected set; }
        public d3 centroid     { get; protected set; }

        public Stack<TN> taggeds { get; protected set; } // node that will be removed
        public Stack<(TN n, SG s)> contour { get; protected set; } // node that will be connected

        public Convex(IEnumerable<f3> originals) {
            var xs = originals.OrderBy(p => p.x);
            var p0 = xs.First();
            var p1 = xs.Last();
            var p2 = originals.OrderByDescending(p => lengthsq(cross(p - p0, p1 - p0))).First();
            var p3 = originals.OrderByDescending(f => new TN(p0, p1, p2).DistFactor(f)).First();
            this.centroid = (p0 + p1 + p2 + p3) * 0.25f; // caz center of tir's grv = center of points

            var n0 = new TN(p0, p1, p2); n0.SetNormal(centroid);
            var n1 = new TN(p1, p2, p3); n1.SetNormal(centroid);
            var n2 = new TN(p2, p3, p0); n2.SetNormal(centroid);
            var n3 = new TN(p3, p0, p1); n3.SetNormal(centroid);

            n0.neighbors = new List<TN>() { n1, n2, n3 };
            n1.neighbors = new List<TN>() { n2, n3, n0 };
            n2.neighbors = new List<TN>() { n3, n0, n1 };
            n3.neighbors = new List<TN>() { n0, n1, n2 };

            this.nodes = new List<TN>() { n0, n1, n2, n3 };
            this.vrtcs = new List<d3>() { p0, p1, p2, p3 };
            this.outsides = originals;
            this.taggeds = new Stack<TN>();
            this.contour = new Stack<(TN n, SG s)>();
        }

        public void ExpandLoop(int maxitr = int.MaxValue) {
            int itr = 0;
            while (itr < maxitr) { itr++; if (!Expand()) break; }
        }

        public bool Expand() {
            Assert.IsTrue(taggeds.Count() == 0);
            Assert.IsTrue(contour.Count() == 0);
            outsides = outsides.Where(p => !Contains(p));
            if (outsides.Count() == 0) return false;

            var _f = false;
            TN  _n = default;
            f3  _p = default;

            foreach (var n in nodes) {
                var sort = outsides
                    .Where(p => dot(p - n.center, n.normal) > 0)
                    .OrderByDescending(p => n.DistFactor(p));
                if (sort.Count() > 0) { _f = true; _p = sort.First(); _n = n; }
            }

            if (_f) {
                taggeds.Push(_n);
                foreach (var nei in _n.neighbors) NeighborSearch(nei, _n, _p);

                // remove fase
                while (taggeds.Count > 0) {
                    var n = taggeds.Pop();
                    foreach (var nei in n.neighbors) {
                        nei.neighbors.Remove(n);
                    }
                    nodes.Remove(n);
                    RollbackCentroid(n);
                }

                // create fase
                var list = new List<(TN n, d3 a, d3 b)>();
                while (contour.Count > 0) {
                    var c = contour.Pop();
                    var pair = c.n;
                    var sgmt = c.s;
                    var curr = new TN(_p, sgmt.a, sgmt.b);
                    curr.neighbors.Add(pair);
                    pair.neighbors.Add(curr);

                    foreach (var l in list) {
                        if (sgmt.a.Equals(l.a) || sgmt.a.Equals(l.b) || sgmt.b.Equals(l.a) || sgmt.b.Equals(l.b)) {
                            if(!l.n.neighbors.Contains(curr)) l.n.neighbors.Add(curr);
                            if(!curr.neighbors.Contains(l.n)) curr.neighbors.Add(l.n);
                        }
                    }
                    list.Add((curr, sgmt.a, sgmt.b));
                    UpdateCentroid(curr);
                }

                nodes.AddRange(list.Select(l => l.n));
                foreach (var n in nodes) { n.SetNormal(this.centroid); }
            }

            foreach (var n in nodes) {
                //Assert.IsTrue(n.neighbors.Count == 3);
                if (n.neighbors.Count != 3) {
                    Debug.Log(n.neighbors.Count);
                }
                foreach (var nei in n.neighbors) {
                    Assert.IsTrue(nei != null);
                }
            }
            return _f;
        }


        void NeighborSearch(TN curr, TN pair, f3 p) {
            if (dot(p - curr.a, curr.normal) >= 0) {
                taggeds.Push(curr);
                foreach (var n in curr.neighbors) {
                    if (!taggeds.Contains(n)) NeighborSearch(n, curr, p);
                }
            } else {
                contour.Push((curr, curr.Common(pair)));
            }
        }

        public bool Contains(f3 p) {
            // this make it somehow better but dun know why...
            foreach (var n in nodes) {
                if (dot(p - n.a, n.normal) > 0 &&
                    dot(p - n.b, n.normal) > 0 && 
                    dot(p - n.c, n.normal) > 0) return false;
            }
            return true;
        }

        void UpdateCentroid(TN n) {
            var c = nodes.Count + 1;
            var f = 1f / c;
            centroid *= (c - 1);
            centroid = f * (centroid + n.center);
        }

        void RollbackCentroid(TN n) {
            var c = nodes.Count + 1;
            var f = 1f / (c - 1);
            centroid *= c;
            centroid = f * (centroid - n.center);
        }

        public void Draw() {
            for (int i = 0; i < nodes.Count; i++) {
                var n = nodes[i];
                n.t.Draw();
                //GL.Begin(GL.LINES);
                //GL.Vertex((f3)n.t.GetGravityCenter());
                //GL.Vertex((f3)n.t.GetGravityCenter() + (f3)n.normal * 0.5f);
                //GL.End();
            }
        }
    }

    public class TriangleNode {
        public Triangle t { get; }
        public d3 center => t.GetGravityCenter();
        public d3 normal { get; private set; }
        public d3 a => t.a;
        public d3 b => t.b;
        public d3 c => t.c;
        public List<TN> neighbors { get; set; }

        public TriangleNode(d3 p1, d3 p2, d3 p3) {
            this.t = new Triangle(p1, p2, p3);
            this.neighbors = new List<TN>();
        }

        public void SetNormal(d3 centroid) {
            var v = cross(t.b - t.a, t.c - t.a);
            var s = dot(v, centroid - t.a) > 0 ? -1 : 1;
            this.normal = normalize(v) * s;
        }

        public double DistFactor(d3 p) => lengthsq(cross(p - a, dot(p - b, p - c)));

        public SG Common(TN pair) {
            if (this.t.HasVert(pair.a) && this.t.HasVert(pair.b)) return new SG(pair.a, pair.b); 
            if (this.t.HasVert(pair.b) && this.t.HasVert(pair.c)) return new SG(pair.b, pair.c); 
            if (this.t.HasVert(pair.c) && this.t.HasVert(pair.a)) return new SG(pair.c, pair.a);
            throw new System.Exception();
        }
    }
}
