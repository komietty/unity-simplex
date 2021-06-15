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
    }
}
