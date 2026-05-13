using UnityEngine;

namespace Core.Foundation.Logging.Examples
{

    public class NamedLoggerExample : MonoBehaviour
    {
        private ILogger _namedLogger;

        private void Awake()
        {
            _namedLogger = LogManager.GetLogger("NamedLogger");
        }

        void Start()
        {
            _namedLogger.Info("✅ Named logger works!");
        }
    }
}
