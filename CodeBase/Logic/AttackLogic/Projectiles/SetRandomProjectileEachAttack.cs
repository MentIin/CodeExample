using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.CodeBase.Logic.AttackLogic.Projectiles
{
    public class SetRandomProjectileEachAttack : MonoBehaviour
    {
        [SerializeField] private AttackWithProjectile _attackWithProjectile;

        [SerializeField] private SetProjectileInfo[] _projectiles = new SetProjectileInfo[0];

        private void Awake()
        {
            _attackWithProjectile.Done += AttackWithProjectileOnDone;
        }

        private void AttackWithProjectileOnDone()
        {
            int id = Random.Range(0, _projectiles.Length);
            _attackWithProjectile.SetStatsId(_projectiles[id].AdditionalStaticDataId);
            _attackWithProjectile.Bullet = _projectiles[id].Bullet;
        }
    }

    [Serializable]
    internal class SetProjectileInfo
    {
        public int AdditionalStaticDataId = 0;
        public GameObject Bullet;
    }
}