using System;
using UnityEngine;

namespace Core.Foundation.Events.Demo
{

    public class CubePresenter : MonoBehaviour
    {
        [Header("View")]
        [SerializeField] private CubeView view;

        [Header("Events Channel")] 
        [SerializeField] private VoidEventChannelSO changeColorChannel;

        private CubeVerticalMovement _movement;
        private CubeColorService _colorService;

        private void Awake()
        {
            _movement = new CubeVerticalMovement();
            _colorService = new CubeColorService();
        }

        private void OnEnable()
        {
            changeColorChannel.AddListener(OnChangeColor);
        }

        public void OnChangeColor()
        {
            view.SetColor(_colorService.RandomColor());
        }

        public void OnMoveUp(int step)
        {
            view.SetPosition(_movement.MoveUp(view.GetPosition(), step));
        }

        public void OnMoveDown(int step)
        {
            view.SetPosition(_movement.MoveDown(view.GetPosition(), step));
        }
    }
}
