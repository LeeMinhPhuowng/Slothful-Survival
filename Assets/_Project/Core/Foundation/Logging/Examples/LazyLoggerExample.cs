using UnityEngine;

namespace Core.Foundation.Logging.Examples
{

    public class LazyLoggerExample : MonoBehaviour
    {
        private static ILogger _sLazyLogger;

        // Property with null-coalescing assignment (initializes on first access)
        private static ILogger LazyLogger => _sLazyLogger ??= LogManager.GetLogger<LazyLoggerExample>();

        void Start()
        {
            LazyLogger.Info("✅ Lazy property pattern works!");
        }
    }
}
