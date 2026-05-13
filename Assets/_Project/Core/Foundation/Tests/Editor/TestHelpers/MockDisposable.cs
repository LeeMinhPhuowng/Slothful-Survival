using UnityEngine;
using System;

namespace Core.Foundation.Tests.TestHelpers
{

    public class MockDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public bool ThrowOnDispose { get; set; }

        public void Dispose()
        {
            if (ThrowOnDispose) throw new Exception("Dispose Error Intentional");
            IsDisposed = true;
        }
    }
}
