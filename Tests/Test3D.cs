using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TestTools;

namespace kmty.geom.d3.test {
    using f3 = float3;
    using UR = UnityEngine.Random;

    public class Test3D {

        [Test]
        public void ConvexTestStatic() {
            List<f3> sample1 = new List<f3> {
                new f3(0, 0, 0),
                new f3(1, 0, 0),
                new f3(0, 1, 0),
                new f3(0, 0, 1),
            };
            var c = new Convex(sample1);
            Assert.IsTrue(c.Contains(new f3(0.1f, 0.1f, 0.1f)));
            Assert.IsTrue(c.Contains(new f3(0, 0, 0)));
            Assert.IsTrue(c.Contains(new f3(1, 0, 0)));
            Assert.IsTrue(c.Contains(new f3(0, 1, 0)));
            Assert.IsTrue(c.Contains(new f3(0, 0, 1)));
            Assert.IsFalse(c.Contains(new f3(2.0f, 0.1f, 0.1f)));
        }


        [Test]
        public void ConvexTestRandom() {
            var points = Enumerable.Repeat(0, 200).Select(_ => (f3)UR.insideUnitSphere).ToList();
            var convex = new Convex(points);
            convex.ExpandLoop();

            int itr = 0;
            while (itr < int.MaxValue) {
                itr++;
                var f = convex.Expand(); 
                if (!f) break;
            }

            foreach (var n in convex.nodes) {
                Assert.IsTrue(n.neighbors.Count == 3);
            }

            foreach (var p in points) {
                Assert.IsTrue(convex.Contains(p));
            }
        }

        [Test]
        public void TriangleSegmentIntersectionTest() {
            for (var i = 0; i < 1000; i++) {
                var t = new Triangle(UR.insideUnitSphere, UR.insideUnitSphere, UR.insideUnitSphere);
                var s = new Segment(UR.insideUnitSphere, UR.insideUnitSphere);
                var f1 = t.Intersects(s, out double3 p1, out bool e1);
                var f2 = t.IntersectsUsingMtx(s, out double3 p2, out bool e2);
                Assert.IsTrue(f1 == f2);
                Assert.IsTrue(e1 == e2);
                //Assert.IsTrue(math.all(p1 == p2));
            }
        }

        [Test]
        public void TriangleTriangleIntersectionTest() {
        }
    }
}
