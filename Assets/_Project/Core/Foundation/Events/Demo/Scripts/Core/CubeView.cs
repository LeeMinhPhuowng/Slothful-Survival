using System;
using UnityEngine;

namespace Core.Foundation.Events.Demo
{

    public class CubeView : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            if(_spriteRenderer == null )
                _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
        
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
        
        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }
    }
}
