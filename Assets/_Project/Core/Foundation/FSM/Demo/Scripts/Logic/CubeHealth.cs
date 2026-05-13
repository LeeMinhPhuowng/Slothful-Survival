using System;

namespace Core.Foundation.FSM.Demo
{

    public class CubeHealth
    {
        private int _current;
        
        public CubeHealth(int health)
        {
            _current = health;    
        }

        public void Damage(int damage)
        {
            _current = Math.Max(_current - damage, 0);
        }

        public bool IsDead => _current == 0;
    }
}
