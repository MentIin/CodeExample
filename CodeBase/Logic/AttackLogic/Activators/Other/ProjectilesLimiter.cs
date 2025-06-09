using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic.Activators.Other
{
    public class ProjectilesLimiter : MonoBehaviour
    {
        [SerializeField] private AttackWithProjectile _attackWithProjectile;
        [SerializeField] private AttackActivator _attackActivator;

        public bool HaveAvaliableProjectiles { get; private set; }
        private void Awake()
        {
            HaveAvaliableProjectiles = true;
            _attackActivator.Done += AttackWithProjectileOnDone;
            _attackWithProjectile.ProjectileDied += AttackWithProjectileOnProjectileDied;
        }

        private void AttackWithProjectileOnProjectileDied()
        {
            HaveAvaliableProjectiles = true;
        }

        private void AttackWithProjectileOnDone()
        {
            HaveAvaliableProjectiles = false;
        }
        
    }
}