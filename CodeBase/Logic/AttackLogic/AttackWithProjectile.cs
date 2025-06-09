using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.CodeBase.Infrastructure.Data.Stats;
using Assets.CodeBase.Infrastructure.Factory;
using Assets.CodeBase.Logic.AttackLogic.Projectiles;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic
{
    public class AttackWithProjectile : Attack
    {
        [Required()] public Transform FirePoint;
        public GameObject Bullet;

        [Space(10)]
        [SerializeField] private bool _useAdditionalStaticData = false;
        [SerializeField] [ShowIf("_useAdditionalStaticData")]
        private int _id;
        
        private IGameFactory _gameFactory;

        public event Action ProjectileDied;

        public void Construct(IGameFactory factory)
        {
            _gameFactory = factory;
        }

        protected override void OnAttack()
        {
            Dictionary<StatType,Stat> stats;
            if (_useAdditionalStaticData)
            {
                stats = DataHolder.AdditionalStats[_id];
            }
            else
            {
                stats = DataHolder.Stats;
            }
            Create(stats);
        }

        private async Task Create(Dictionary<StatType, Stat> stats)
        {
            IBullet bullet = await _gameFactory.CreateProjectile(FirePoint, Bullet,
                stats, OwnerHealth);
            OnTaskComplete(bullet);
        }

        private void OnTaskComplete(IBullet bullet)
        {
           bullet.Destroyed += ResultOnDestroyed;
        }

        private void ResultOnDestroyed()
        {
            ProjectileDied?.Invoke();
        }

        public void SetStatsId(int id)
        {
            _id = id;
        }
    }
}