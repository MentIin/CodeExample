using UnityEngine;
using UnityEngine.Events;

namespace Assets.CodeBase.Logic.AttackLogic.Projectiles
{
    public class UnityEventOnProjectileDeath : MonoBehaviour
    {
        [SerializeField] private UnityEvent _unityEvent;
        [SerializeField] private Bullet _bullet;

        private void Awake()
        {
            _bullet.Destroyed += BulletOnDestroyed;
        }

        private void BulletOnDestroyed()
        {
            _unityEvent.Invoke();
        }
    }
}