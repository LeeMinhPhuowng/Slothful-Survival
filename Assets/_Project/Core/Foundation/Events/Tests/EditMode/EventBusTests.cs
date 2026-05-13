using Core.Foundation.Events;
using Core.Foundation.Logging;
using Core.Foundation.Logging.Examples;
using NUnit.Framework;
using System;
using System.Threading;

[TestFixture]
public class EventBusTests
{
    private EventBus _eventBus;
    private ILogger _logger;

    [SetUp]
    public void Setup()
    {
        _logger = LogManager.GetLogger<EventBusTests>();
        _eventBus = new EventBus(_logger);
    }

    [Test]
    public void SubscribeAndPublish_ShouldInvokeHandler()
    {
        bool received = false;
        _eventBus.Subscribe<int>(val => received = val == 42);
        _eventBus.Publish(42);
        Assert.IsTrue(received);
    }

    [Test]
    public void Unsubscribe_ShouldNotInvokeHandler()
    {
        int callCount = 0;
        Action<int> handler = _ => callCount++;

        _eventBus.Subscribe(handler);
        _eventBus.Unsubscribe(handler);
        _eventBus.Publish(10);

        Assert.AreEqual(0, callCount);
    }

    [Test]
    public void ConcurrentPublish_ShouldBeThreadSafe()
    {
        int count = 0;
        _eventBus.Subscribe<int>(_ => Interlocked.Increment(ref count));

        // Giả lập 10 thread cùng Publish
        Thread[] threads = new Thread[10];
        for (int i = 0; i < 10; i++)
        {
            threads[i] = new Thread(() => _eventBus.Publish(1));
            threads[i].Start();
        }

        foreach (var t in threads) t.Join();

        Assert.AreEqual(10, count);
    }
}
