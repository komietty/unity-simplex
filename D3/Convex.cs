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
            this.outsides = originals;
            this.taggeds = new Stack<TN>();
            this.contour = new Stack<(TN n, SG s)>();
        }

        public void ExpandLoop(int maxitr = int.MaxValue) {
            int itr = 0;
            while (itr < maxitr) { itr++; if (!Expand()) break; }
        }

        public bool Expand() {
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


                // debug
                var c1 = new HashSet<d3>();
                foreach (var c in contour) {
                    c1.Add(c.s.a);
                    c1.Add(c.s.b);
                }
                if(!(c1.Count == contour.Count)) {
                    Debug.LogWarning(c1.Count);
                    Debug.LogWarning(contour.Count);

                    int i = 0;
                    foreach (var c in contour) {
                        Debug.LogWarning($"{i}, n:{c.n.center}, a: {c.s.a}, b: {c.s.b}");
                        i++;
                    }
                }
                var c2 = c1.ToDictionary(c => c, c => 0);
                foreach (var c in contour) {
                    c2[c.s.a]++;
                    c2[c.s.b]++;
                }
                foreach (var v in c2.Values) {
                    Assert.IsTrue(v == 2);
                }


                // remove fase
                while (taggeds.Count > 0) {
                    var n = taggeds.Pop();
                    foreach (var nei in n.neighbors) { nei.neighbors.Remove(n); }
                    nodes.Remove(n);
                    RollbackCentroid(n);
                    n = null;
                }

                // create cone
                var cone = new List<(TN n, d3 a, d3 b)>();
                while (contour.Count > 0) {
                    var c = contour.Pop();
                    var pair = c.n;
                    var sgmt = c.s;
                    var curr = new TN(_p, sgmt.a, sgmt.b);
                    curr.neighbors.Add(pair);
                    pair.neighbors.Add(curr);

                    //foreach (var l in list) {
                    for(var i = 0; i < cone.Count; i++) {
                        var l = cone[i];
                        Assert.IsTrue(curr.neighbors.Count <= 3);
                        //Assert.IsTrue(l.n.neighbors.Count <= 3);
                        if (sgmt.a.Equals(l.a) || sgmt.a.Equals(l.b) || sgmt.b.Equals(l.a) || sgmt.b.Equals(l.b)) {
                            if (!l.n.neighbors.Contains(curr)) l.n.neighbors.Add(curr);
                            else throw new System.Exception();
                            if(!curr.neighbors.Contains(l.n)) curr.neighbors.Add(l.n);
                            else throw new System.Exception();
                        }
                    }
                    cone.Add((curr, sgmt.a, sgmt.b));
                    foreach (var l in cone) l.n.SetNormal(this.centroid);
                    UpdateCentroid(curr);
                }

                nodes.AddRange(cone.Select(l => l.n));
                foreach (var n in nodes) { n.SetNormal(this.centroid); }
            }

            //check
            foreach (var n in nodes) {
                if (n.neighbors.Count != 3) {
                    var p = new HashSet<d3>();
                    n.neighbors.ForEach(_n => {
                        p.Add(_n.a);
                        p.Add(_n.b);
                        p.Add(_n.c);
                    });
                    Assert.IsTrue(p.Count <= 6 && p.Count >= 4);
                    Debug.Log(p.Count);
                }
                foreach (var nei in n.neighbors) { Assert.IsTrue(nei != null); }
            }
            return _f;
        }


        // consider degenerated case or numerical problem
        double h = 1e-10d;
        void NeighborSearch(TN curr, TN pair, f3 p) {
            if (dot(p - curr.a, curr.normal) > h &&
                dot(p - curr.b, curr.normal) > h &&
                dot(p - curr.c, curr.normal) > h) {
                taggeds.Push(curr);
                foreach (var n in curr.neighbors) {
                    if (!taggeds.Contains(n)) NeighborSearch(n, curr, p);
                }
            } else {
                var t = (curr, curr.Common(pair));
                if (!contour.Contains(t)) contour.Push(t);
            }
        }

        public bool Contains(f3 p) {
            foreach (var n in nodes) {
                //if (dot(p - n.a, n.normal) > 1e-7d) return false;
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
