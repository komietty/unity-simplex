using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d2 {
    using f2 = float2;

    public struct Circle : System.IEquatable<Circle> {
        public f2 center;
        public float radius;

        public Circle(f2 c, float r) {
            center = c;
            radius = r;
        }

        public bool Contains(f2 p) => lengthsq(p - center) < radius * radius;

        #region IEquatable
        public override bool Equals(object obj) { return obj is Circle circle && Equals(circle); }
        public bool Equals(Circle other) { return center.Equals(other.center) && radius == other.radius; }
        public static bool operator ==(Circle left, Circle right) { return left.Equals(right); } 
        public static bool operator !=(Circle left, Circle right) { return !(left == right); }
        public override int GetHashCode() {
            int hashCode = 1472534999;
            hashCode = hashCode * -1521134295 + center.GetHashCode();
            hashCode = hashCode * -1521134295 + radius.GetHashCode();
            return hashCode;
        }
        #endregion
    }
}
