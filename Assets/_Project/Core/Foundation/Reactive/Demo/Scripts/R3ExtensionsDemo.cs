using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Core.Foundation.Logging;

namespace Core.Foundation.Reactive.Demo.Scripts
{

    public class R3ExtensionsDemo : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private Image _loadingImage;
        [SerializeField] private GameObject _targetObject;
        [SerializeField] private Button _triggerButton;
        [SerializeField] private Button _switchSceneButton;

        // Reactive Properties (State)
        [Header("Reactive Properties")]
        [SerializeField] private SerializableReactiveProperty<string> _statusProp = new SerializableReactiveProperty<string>("Initializing...");
        [SerializeField] private SerializableReactiveProperty<float> _fillProp = new SerializableReactiveProperty<float>(0f);
        [SerializeField] private SerializableReactiveProperty<bool> _activeProp = new SerializableReactiveProperty<bool>(true);

        private readonly DisposableBag _disposables = new DisposableBag();
        private Logging.ILogger _logger;

        void Awake()
        {
            _logger = LogManager.GetLogger<R3ExtensionsDemo>();
            _switchSceneButton.onClick.AddListener(SwitchScene);
        }

        void Start()
        {
            _logger.Info("--- Bắt đầu Test ---");

            _statusProp.BindToText(_statusText)
                .AddTo(_disposables);


            _fillProp.BindToFillAmount(_loadingImage)
                .AddTo(_disposables);


            _activeProp.BindToActive(_targetObject)
                .AddTo(_disposables);


            R3Extensions.SafeTimer(TimeSpan.FromSeconds(0.1f))
                .Subscribe(tick =>
                {
                    if (_fillProp.Value >= 1f) _fillProp.Value = 0f;
                    else _fillProp.Value += 0.05f;

                    _statusProp.Value = $"Loading... {Math.Round(_fillProp.Value * 100)}% (Tick: {tick})";
                    _logger.Info($"<color=green>[SafeTimer] Đang chạy ở: {SceneManager.GetActiveScene().name} - Tick: {tick}</color>");
                })
                .AddTo(_disposables);


            R3Extensions.ThrottleFirst(_triggerButton.OnClickAsObservable(), TimeSpan.FromSeconds(1.0f))
                .Subscribe(_ =>
                {
                    _activeProp.Value = !_activeProp.Value;

                    _logger.Info($"[R3Demo] Button Clicked at {Time.time}");
                })
                .AddTo(_disposables);
        }

        public void SwitchScene()
        {
            _logger.Info("<color=yellow>--- ĐANG CHUYỂN SCENE ---</color>");

            SceneManager.LoadScene("SCN_EmptyScene");
        }

        void OnDestroy()
        {
            _disposables.Dispose();
            _statusProp.Dispose();
            _fillProp.Dispose();
            _activeProp.Dispose();
        }
    }
}
