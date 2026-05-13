using System;
using NUnit.Framework;
using Core.Foundation.Reactive;
using Core.Foundation.Tests.TestHelpers;

namespace Core.Foundation.Tests
{

    [TestFixture]
    public class MVVMTests
    {
        // --- DISPOSABLE / ADDTO TESTS ---

        [Test]
        public void AddTo_Extension_RegistersCorrectly()
        {
            var vm = new TestViewModel();
            var d1 = new MockDisposable();

            d1.AddTo(vm.Bag);

            Assert.IsFalse(d1.IsDisposed);

            vm.Dispose();

            Assert.IsTrue(d1.IsDisposed);
        }

        [Test]
        public void AddTo_Extension_InternalUsage_Works()
        {
            var vm = new TestViewModel();
            var d = new MockDisposable();

            d.AddTo(vm.Bag);

            vm.Dispose();
            Assert.IsTrue(d.IsDisposed);
        }

        [Test]
        public void AddTo_Extension_ReturnsOriginalInstance()
        {
            var vm = new TestViewModel();
            var original = new MockDisposable();

            var returned = original.AddTo(vm.Bag);

            Assert.AreSame(original, returned, "AddTo must return the original instance");
        }

        [Test]
        public void AddTo_Extension_NullBag_ThrowsException()
        {
            // Test Guard Clause
            var d = new MockDisposable();
            DisposableBag nullBag = null;

            var ex = Assert.Throws<ArgumentNullException>(() => d.AddTo(nullBag));
            Assert.AreEqual("bag", ex.ParamName);
        }

        [Test]
        public void DisposableBag_ExceptionSafety_ContinuesDisposing()
        {
            var vm = new TestViewModel();

            var badItem = new MockDisposable { ThrowOnDispose = true };
            var goodItem = new MockDisposable();

            badItem.AddTo(vm.Bag);
            goodItem.AddTo(vm.Bag);

            vm.Dispose();

            Assert.IsTrue(goodItem.IsDisposed, "The good item must be disposed even if a previous item throws an exception");
        }
    }
}
