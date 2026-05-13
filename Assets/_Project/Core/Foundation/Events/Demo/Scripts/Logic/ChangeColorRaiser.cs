using System;
using Core.Foundation.Logging;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Core.Foundation.Events.Demo
{

    public class ChangeColorRaiser : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private VoidEventChannelSO changeColorChannel;
        
        private Core.Foundation.Logging.ILogger _logger;

        private void Awake()
        {
            _logger = LogManager.GetLogger<ChangeColorRaiser>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _logger.Debug("OnMouseDown");
            changeColorChannel.EventRaise();
        }
    }
}
