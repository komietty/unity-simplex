using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TestTools;

namespace kmty.geom.d3.test {
    using f3 = float3;

    public class Test3D {
        List<f3> sample1 = new List<f3> {
            new f3(0, 0, 0),
            new f3(1, 0, 0),
            new f3(0, 1, 0),
            new f3(0, 0, 1),
        };

        [Test]
        public void ConvexTest() {
            var c = new Convex(sample1);
            Assert.IsTrue(c.Contains(new f3(0.1f, 0.1f, 0.1f)));
            Assert.IsTrue(c.Contains(new f3(0, 0, 0)));
            Assert.IsTrue(c.Contains(new f3(1, 0, 0)));
            Assert.IsTrue(c.Contains(new f3(0, 1, 0)));
            Assert.IsTrue(c.Contains(new f3(0, 0, 1)));
            Assert.IsFalse(c.Contains(new f3(2.0f, 0.1f, 0.1f)));
            //c.Expand();

        }


        [Test]
        public void ConvexTestCase1() {
        }
    }
}
