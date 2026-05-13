using System;
using Core.Foundation.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using Core.Foundation.Logging;

namespace Core.Foundation.Events.Demo
{

    public class MoveCubeRaiser : MonoBehaviour, IPointerClickHandler
    {
        private Logging.ILogger _logger;
        [Header("Event Channel")]
        [SerializeField] IntEventChannelSO moveChannelSo;
        
        [Header("Config")]
        [SerializeField] private int moveStep;

        private void Awake()
        {
            _logger = LogManager.GetLogger<MoveCubeRaiser>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _logger.Debug("Move cube raiser");
            moveChannelSo.EventRaise(moveStep);
        }
    }
}
