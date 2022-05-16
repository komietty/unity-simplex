using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d3 {
    using f3 = float3;
    using d3 = double3;
    using SG = Segment;
    using TN = TriangleNode;

    public class Convex { 
        private IEnumerable<f3> outsides;
        public List<TN> nodes  { get; protected set; }
        public d3 centroid     { get; protected set; }
        public List<TN>           taggeds { get; protected set; } // node that will be removed
        public List<(TN n, SG s)> contour { get; protected set; } // node that will be connected
        double h = 1e-10d;

        public Convex(IEnumerable<f3> originals) {
            var xs = originals.OrderBy(p => p.x);
            var p0 = xs.First();
            var p1 = xs.Last();
            var p2 = originals.OrderByDescending(p => lengthsq(cross(p - p0, p1 - p0))).First();
            var p3 = originals.OrderByDescending(f => new TN(p0, p1, p2).DistFactor(f)).First();
            this.centroid = (p0 + p1 + p2 + p3) * 0.25f;

            var n0 = new TN(p0, p1, p2); n0.SetNormal(centroid);
            var n1 = new TN(p1, p2, p3); n1.SetNormal(centroid);
            var n2 = new TN(p2, p3, p0); n2.SetNormal(centroid);
            var n3 = new TN(p3, p0, p1); n3.SetNormal(centroid);

            n0.neighbors = new List<TN>() { n1, n2, n3 };
            n1.neighbors = new List<TN>() { n2, n3, n0 };
            n2.neighbors = new List<TN>() { n3, n0, n1 };
            n3.neighbors = new List<TN>() { n0, n1, n2 };

            this.nodes    = new List<TN>() { n0, n1, n2, n3 };
            this.outsides = originals;
            this.taggeds  = new List<TN>();
            this.contour  = new List<(TN n, SG s)>();
        }

        public void ExpandLoop(int maxitr = int.MaxValue) {
            int itr = 0;
            while (itr < maxitr) { itr++; if (!Expand()) break; }
        }

        public bool Expand() {
            taggeds.Clear();
            contour.Clear();
            outsides = outsides.Where(p => !Contains(p));
            if (outsides.Count() == 0) return false;

            if (FindApex(out d3 apex, out TN root)) {
                // tag fase
                taggeds.Add(root);
                foreach (var nei in root.neighbors) NeighborSearch(nei, root, apex);

                // rmv fase
                foreach(var n in taggeds) { 
                    foreach (var nei in n.neighbors) { nei.neighbors.Remove(n); }
                    nodes.Remove(n);
                    RollbackCentroid(n);
                }

                // add fase
                var cone = new (TN n, d3 a, d3 b)[contour.Count];
                for (var i = 0; i < contour.Count; i++) {
                    var c = contour[i];
                    var s = c.s;
                    var pair = c.n;
                    var curr = new TN(apex, s.a, s.b);
                    curr.neighbors.Add(pair);
                    pair.neighbors.Add(curr);

                    foreach (var l in cone) {
                        if (l.n == null) continue;
                        if (s.a.Equals(l.a) || s.a.Equals(l.b) || s.b.Equals(l.a) || s.b.Equals(l.b)) {
                            if (!l.n.neighbors.Contains(curr) && !curr.neighbors.Contains(l.n)) {
                                l.n.neighbors.Add(curr);
                                curr.neighbors.Add(l.n);
                            } else throw new System.Exception();
                        }
                    }
                    UnityEngine.Assertions.Assert.IsTrue(curr != null);
                    cone[i] = (curr, s.a, s.b);
                    UpdateCentroid(curr);
                }

                nodes.AddRange(cone.Select(l => l.n));
                foreach (var n in nodes) n.SetNormal(centroid);
                return true;
            }
            return false;
        }

        bool FindApex(out d3 apex, out TN root) {
            foreach (var n in this.nodes) {
                var s = outsides.Where(p => dot(p - n.center, n.normal) > 0).OrderByDescending(p => n.DistFactor(p));
                if (s.Count() > 0) { apex = s.First(); root = n; return true; }
            }
            apex = default;
            root = default;
            return false;
        }

        void NeighborSearch(TN curr, TN pair, d3 p) {
            if (dot(p - curr.center, curr.normal) > h) {
                taggeds.Add(curr);
                foreach (var n in curr.neighbors) {
                    if (!taggeds.Contains(n)) NeighborSearch(n, curr, p);
                }
            } else {
                var t = (curr, curr.Common(pair));
                if (!contour.Contains(t)) contour.Add(t);
            }
        }

        public bool Contains(d3 p) {
            foreach (var n in nodes) {
                if (dot(p - n.center, n.normal) > h) return false;
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
            }
        }
    }

    public class TriangleNode {
        public Triangle t { get; }
        public d3 center { get; }
        public d3 normal { get; private set; }
        public List<TN> neighbors { get; set; }

        public TriangleNode(d3 p1, d3 p2, d3 p3) {
            this.t = new Triangle(p1, p2, p3);
            this.center = t.GetGravityCenter();
            this.neighbors = new List<TN>();
        }

        public void SetNormal(d3 centroid) {
            var v = cross(t.b - t.a, t.c - t.a);
            var s = dot(v, centroid - t.a) > 0 ? -1 : 1;
            this.normal = normalize(v) * s;
        }

        public double DistFactor(d3 p) => lengthsq(cross(p - t.a, dot(p - t.b, p - t.c)));

        public SG Common(TN pair) {
            if (this.t.HasVert(pair.t.a) && this.t.HasVert(pair.t.b)) return new SG(pair.t.a, pair.t.b); 
            if (this.t.HasVert(pair.t.b) && this.t.HasVert(pair.t.c)) return new SG(pair.t.b, pair.t.c); 
            if (this.t.HasVert(pair.t.c) && this.t.HasVert(pair.t.a)) return new SG(pair.t.c, pair.t.a);
            throw new System.Exception();
        }
    }
}
