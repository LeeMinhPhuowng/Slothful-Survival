using NUnit.Framework;
using UnityEngine;
using Core.Foundation.Extensions;

namespace Core.Foundation.Tests
{

    [TestFixture]
    public class VectorExtensionsTests
    {
        // --- Vector3 ---

        [Test]
        public void Vector3_WithX_ReplacesOnlyX()
        {
            var v = new Vector3(1f, 2f, 3f);

            var result = v.WithX(10f);

            Assert.AreEqual(new Vector3(10f, 2f, 3f), result);
        }

        [Test]
        public void Vector3_WithY_ReplacesOnlyY()
        {
            var v = new Vector3(1f, 2f, 3f);

            var result = v.WithY(10f);

            Assert.AreEqual(new Vector3(1f, 10f, 3f), result);
        }

        [Test]
        public void Vector3_WithZ_ReplacesOnlyZ()
        {
            var v = new Vector3(1f, 2f, 3f);

            var result = v.WithZ(10f);

            Assert.AreEqual(new Vector3(1f, 2f, 10f), result);
        }

        [Test]
        public void Vector3_With_DoesNotMutateOriginal()
        {
            var v = new Vector3(1f, 2f, 3f);

            v.WithX(99f);

            Assert.AreEqual(new Vector3(1f, 2f, 3f), v,
                "WithX should return a new Vector3, not mutate the original.");
        }

        // --- Vector2 ---

        [Test]
        public void Vector2_WithX_ReplacesOnlyX()
        {
            var v = new Vector2(1f, 2f);

            var result = v.WithX(10f);

            Assert.AreEqual(new Vector2(10f, 2f), result);
        }

        [Test]
        public void Vector2_WithY_ReplacesOnlyY()
        {
            var v = new Vector2(1f, 2f);

            var result = v.WithY(10f);

            Assert.AreEqual(new Vector2(1f, 10f), result);
        }

        [Test]
        public void Vector2_With_DoesNotMutateOriginal()
        {
            var v = new Vector2(1f, 2f);

            v.WithY(99f);

            Assert.AreEqual(new Vector2(1f, 2f), v,
                "WithY should return a new Vector2, not mutate the original.");
        }
    }
}
