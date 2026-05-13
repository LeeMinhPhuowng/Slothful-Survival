using NUnit.Framework;
using UnityEngine;
using Core.Foundation.Events;
using Core.Foundation.Tests.TestHelpers;
using UnityEngine.TestTools;
using System.Text.RegularExpressions;

namespace Core.Foundation.Tests
{

    [TestFixture]
    public class EventSystemTests
    {
        // --- VOID CHANNEL TESTS ---
        [Test]
        public void VoidChannel_BasicFlow_Works()
        {
            var channel = ScriptableObject.CreateInstance<VoidEventChannelSO>();
            try
            {
                bool fired = false;
                VoidEventChannelSO.OnHandler handler = () => fired = true;

                channel.AddListener(handler);
                channel.EventRaise();

                Assert.IsTrue(fired);
                
                // Test Remove
                fired = false;
                channel.RemoveListener(handler);
                channel.EventRaise();
                Assert.IsFalse(fired);
            }
            finally
            {
                Object.DestroyImmediate(channel);
            }
        }

        [Test]
        public void VoidChannel_Reentrancy_ProtectsInfiniteLoop()
        {
            var channel = ScriptableObject.CreateInstance<VoidEventChannelSO>();
            try {
                int count = 0;
                
                VoidEventChannelSO.OnHandler recursiveHandler = null;
                recursiveHandler = () => 
                {
                    count++;
                    if (count < 5) channel.EventRaise();
                };

                channel.AddListener(recursiveHandler);

                LogAssert.Expect(LogType.Warning, new Regex(".reentrancy."));

                channel.EventRaise();

                Assert.AreEqual(1, count); 
            }
            finally 
            {
                Object.DestroyImmediate(channel);
            }
        }

        // --- GENERIC CHANNEL & LISTENER TESTS ---
        
        private IntEventChannelSO _channel;
        private GameObject _go1, _go2;
        private IntEventListener _listenerComp1, _listenerComp2;

        [SetUp]
        public void Setup()
        {
            _channel = ScriptableObject.CreateInstance<IntEventChannelSO>();
            
            _go1 = new GameObject("L1");
            _listenerComp1 = _go1.AddComponent<IntEventListener>();
            _listenerComp1.SetChannel(_channel);

            _go2 = new GameObject("L2");
            _listenerComp2 = _go2.AddComponent<IntEventListener>();
            _listenerComp2.SetChannel(_channel);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_channel);
            Object.DestroyImmediate(_go1);
            Object.DestroyImmediate(_go2);
        }

        [Test]
        public void Integrated_FireEvent_TriggersMultipleListeners()
        {
            int val1 = 0, val2 = 0;
            
            // Attach callbacks to the private UnityEvent
            _listenerComp1.AddCallback(x => val1 = x);
            _listenerComp2.AddCallback(x => val2 = x);

            // Simulate OnEnable
            _listenerComp1.InvokePrivate("OnEnable");
            _listenerComp2.InvokePrivate("OnEnable");

            // Act
            _channel.EventRaise(99);

            // Assert
            Assert.AreEqual(99, val1);
            Assert.AreEqual(99, val2);
        }

        [Test]
        public void Integrated_DisableListener_StopsReceiving()
        {
            int val1 = 0;
            _listenerComp1.AddCallback(x => val1 = x);

            _listenerComp1.InvokePrivate("OnEnable");
            _listenerComp1.InvokePrivate("OnDisable");

            _channel.EventRaise(100);

            Assert.AreEqual(0, val1, "Disabled listener should not receive events");
        }

        [Test]
        public void OnEventRaised_Subscriber_ReceivesEvent()
        {
            int received = 0;
            _channel.OnEventRaised += x => received = x;

            _channel.EventRaise(123);

            Assert.AreEqual(123, received);
        }

        [Test]
        public void OnEventRaised_OneSubscriberThrows_OthersStillReceive()
        {
            bool secondSubscriberFired = false;

            _channel.OnEventRaised += _ => throw new System.Exception("OnEventRaised Crash!");
            _channel.OnEventRaised += _ => secondSubscriberFired = true;

            LogAssert.Expect(LogType.Error, new Regex(".*OnEventRaised Crash!.*"));

            Assert.DoesNotThrow(() => _channel.EventRaise(5));
            Assert.IsTrue(secondSubscriberFired, "Second OnEventRaised subscriber should still run.");
        }

        [Test]
        public void OnEventRaised_AndAddListener_BothReceiveSameEvent()
        {
            int onEventRaisedValue = 0;
            int addListenerValue = 0;

            _channel.OnEventRaised += x => onEventRaisedValue = x;

            _listenerComp1.AddCallback(x => addListenerValue = x);
            _listenerComp1.InvokePrivate("OnEnable");

            _channel.EventRaise(777);

            Assert.AreEqual(777, onEventRaisedValue);
            Assert.AreEqual(777, addListenerValue);
        }

        [Test]
        public void Channel_ExceptionInOneListener_DoesNotStopOthers()
        {
            bool listener2Fired = false;

            // Listener 1: Throws an exception
            _listenerComp1.AddCallback(x => throw new System.Exception("Crash!"));
            
            // Listener 2: Normal behavior
            _listenerComp2.AddCallback(x => listener2Fired = true);

            _listenerComp1.InvokePrivate("OnEnable");
            _listenerComp2.InvokePrivate("OnEnable");

            LogAssert.Expect(LogType.Error, new Regex(".*Crash!.*"));

            // Act: Ensure the exception is caught inside the Channel loop
            Assert.DoesNotThrow(() => _channel.EventRaise(1));

            // Assert: Listener 2 should still be executed
            Assert.IsTrue(listener2Fired, "Listener 2 failed to fire due to an exception in Listener 1");
        }

    }
}
