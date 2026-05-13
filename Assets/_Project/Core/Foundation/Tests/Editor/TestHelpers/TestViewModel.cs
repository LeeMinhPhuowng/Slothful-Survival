using Core.Foundation.MVVM;
using Core.Foundation.Reactive;

namespace Core.Foundation.Tests.TestHelpers
{

    public class TestViewModel : BaseViewModel
    {
        public DisposableBag Bag => DisposableBag;
    }
}
