using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Unity.Mathematics.math;
using URnd = UnityEngine.Random;

namespace kmty.geom.d3.test {
    public class Test3D {

        [Test]
        public void TetrahedraCircumscribedSpherePresicion() {
            for (var i = 0; i < 100; i++) {
                var t = new Tetrahedra(
                    URnd.insideUnitSphere,
                    URnd.insideUnitSphere,
                    URnd.insideUnitSphere,
                    URnd.insideUnitSphere
                    );
                var s = t.circumscribedSphere;
                var c = s.center;
                var r = s.radius;
                var h = 1e-10f;
                Assert.IsTrue(length(t.a - c) - r < h);
                Assert.IsTrue(length(t.b - c) - r < h);
                Assert.IsTrue(length(t.c - c) - r < h);
                Assert.IsTrue(length(t.d - c) - r < h);
            }
        }
    }
};
