using NUnit.Framework;
using Core.Foundation.Extensions;

namespace Core.Foundation.Tests
{

    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void IsNullOrEmpty_NullString_ReturnsTrue()
        {
            string value = null;

            Assert.IsTrue(value.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_EmptyString_ReturnsTrue()
        {
            string value = "";

            Assert.IsTrue(value.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_WhitespaceOnly_ReturnsFalse()
        {
            // Only checks null/empty, NOT whitespace
            string value = "   ";

            Assert.IsFalse(value.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_NonEmptyString_ReturnsFalse()
        {
            string value = "hello";

            Assert.IsFalse(value.IsNullOrEmpty());
        }
    }
}
