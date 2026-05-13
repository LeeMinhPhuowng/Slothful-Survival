using UnityEngine;

namespace Core.Foundation.FSM.Demo
{

    [CreateAssetMenu(fileName = "SO_CubeConfig", menuName = "Scriptable Objects/Demo/FSM/CubeConfig")]
    public class CubeConfigSO : ScriptableObject, ICubeConfig
    {   
        [SerializeField] private int _health;
        [SerializeField] private float _speed;
        [SerializeField] private float _detectRange;
        [SerializeField] private float _attackRange;
        [SerializeField] private float _attackCooldown;
        [SerializeField] private float _attackDuration;


        public int Health => _health;
        public float Speed => _speed;
        public float DetectRange => _detectRange;
        public float AttackRange => _attackRange;
        public float AttackCooldown => _attackCooldown;
        public float AttackDuration => _attackDuration;
    }
}
