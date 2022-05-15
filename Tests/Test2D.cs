using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Mathematics;
using System.Runtime.InteropServices;
using static Unity.Mathematics.math;

namespace kmty.geom.d2.test {
    using f2 = float2;
    using SG = Segment;
    using TR = Triangle;
    using UR = UnityEngine.Random;

    public class Test2D {
        [Test]
        public void EquatableTest() {
            var ps = Enumerable.Repeat(0, 10).Select(_ => new f2(UR.value, UR.value)).ToArray();
            var a = new f2(0, 0);
            var b = new f2(1, 0);
            var c = new f2(0, 1);
            var d = new f2(1, 1);
            var e = new f2(2, 0);
            var f = new f2(0, 2);
            var g = new f2(2, 2);

            Debug.Log("f2: "  + Marshal.SizeOf(a));
            Debug.Log("sg: "  + Marshal.SizeOf(new SG(a, b)));
            Debug.Log("tr: "  + Marshal.SizeOf(new TR(a, b, c)));

            // primitive
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.Equals(c));
            Assert.IsTrue (a.Equals(a));

            // circle
            var c1 = new Circle(d, 1);
            var c2 = new Circle(d, 1);
            var c3 = new Circle(d, 2);
            Assert.IsTrue(c1 == c2);
            Assert.IsTrue(c1 != c3);

            // segment
            var s1 = new SG(a, b);
            var s2 = new SG(b, a);
            var s3 = new SG(c, a);
            Assert.IsTrue(s1 == s2);
            Assert.IsTrue(s1 != s3);

            // triangle
            var t1 = new TR(a, b, c);
            var t2 = new TR(a, c, b);
            var t3 = new TR(b, c, a);
            Assert.IsTrue(t1 == t2);
            Assert.IsTrue(t1 == t3);
        }

        [Test]
        public void TriangleTest() {
            var t1 = new TR(new f2(0, 0), new f2(1, 0), new f2(0, 1));
            var s1 = new SG(new f2(0, 0), new f2(0, 1));
            var s2 = new SG(new f2(0, 1), new f2(1, 0));
            var s3 = new SG(new f2(1, 0), new f2(0, 0));

            Assert.IsTrue(t1.Contains(s1));
            Assert.IsTrue(t1.Contains(s2));
            Assert.IsTrue(t1.Contains(s3));
            Assert.IsTrue(t1.RemainingPoint(s3).Equals(t1.c));
            Assert.IsTrue(t1.RemainingPoint(s2).Equals(t1.a));
            Assert.IsTrue(t1.RemainingPoint(s1).Equals(t1.b));
        }

        [Test]
        public void TriangleTransCoordTest() {
            var t = new TR(new f2(1, 1), new f2(5, 2), new f2(2, 4));
            var v1 = t.EuclidCord2TriangleCord(new f2(2, 2));
            Assert.IsTrue(math.all(v1 == new f2(2, 3) / 11));
            var v2 = t.TriangleCord2EuclidCord(v1);
            Assert.IsTrue(math.all(v2 == new f2(2, 2)));

            for (var i = 0; i < 100; i++) {
                var t1 = new TR(UR.insideUnitCircle, UR.insideUnitCircle, UR.insideUnitCircle);
                var p1 = UR.insideUnitCircle;
                var f1 = t1.Includes(p1, true);
                var f2 = t1.Includes4Test(p1, true);
                Debug.Log(f2);
                Assert.IsTrue(!(f1 ^ f2));
            }
        }
    }
}
