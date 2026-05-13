using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Core.Foundation.Extensions;

namespace Core.Foundation.Tests
{

    [TestFixture]
    public class CollectionExtensionsTests
    {
        // --- RandomElement ---

        [Test]
        public void RandomElement_NullList_ThrowsArgumentNullException()
        {
            IReadOnlyList<int> list = null;

            Assert.Throws<ArgumentNullException>(() => list.RandomElement());
        }

        [Test]
        public void RandomElement_EmptyList_ThrowsInvalidOperationException()
        {
            var list = new List<int>();

            Assert.Throws<InvalidOperationException>(() => list.RandomElement());
        }

        [Test]
        public void RandomElement_SingleElement_ReturnsThatElement()
        {
            var list = new List<int> { 42 };

            int result = list.RandomElement();

            Assert.AreEqual(42, result);
        }

        [Test]
        public void RandomElement_MultipleElements_ReturnsElementFromList()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };

            for (int i = 0; i < 50; i++)
            {
                int result = list.RandomElement();
                Assert.IsTrue(list.Contains(result),
                    $"RandomElement returned {result} which is not in the list.");
            }
        }

        // --- Shuffle ---

        [Test]
        public void Shuffle_NullList_ThrowsArgumentNullException()
        {
            IList<int> list = null;

            Assert.Throws<ArgumentNullException>(() => list.Shuffle());
        }

        [Test]
        public void Shuffle_EmptyList_DoesNotThrow()
        {
            var list = new List<int>();

            Assert.DoesNotThrow(() => list.Shuffle());
        }

        [Test]
        public void Shuffle_PreservesAllElements()
        {
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var originalSorted = list.OrderBy(x => x).ToList();

            list.Shuffle();

            var shuffledSorted = list.OrderBy(x => x).ToList();
            CollectionAssert.AreEqual(originalSorted, shuffledSorted);
        }

        [Test]
        public void Shuffle_PreservesCount()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };

            list.Shuffle();

            Assert.AreEqual(5, list.Count);
        }

        [Test]
        public void Shuffle_LargeList_ChangesOrder()
        {
            // With 100 elements, the probability of staying in order is 1/100! ≈ 0
            var list = new List<int>(Enumerable.Range(0, 100));
            var original = new List<int>(list);

            list.Shuffle();

            // Verify at least one element changed position
            bool hasChanged = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != original[i])
                {
                    hasChanged = true;
                    break;
                }
            }

            Assert.IsTrue(hasChanged,
                "Shuffle on 100 elements did not change order — extremely unlikely if Fisher-Yates is correct.");
        }

        // --- IsNullOrEmpty ---

        [Test]
        public void IsNullOrEmpty_NullCollection_ReturnsTrue()
        {
            IReadOnlyCollection<int> collection = null;

            Assert.IsTrue(collection.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_EmptyCollection_ReturnsTrue()
        {
            var collection = new List<int>();

            Assert.IsTrue(collection.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_NonEmptyCollection_ReturnsFalse()
        {
            var collection = new List<int> { 1 };

            Assert.IsFalse(collection.IsNullOrEmpty());
        }
    }
}
