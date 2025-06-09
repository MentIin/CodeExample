using System;
using System.Collections;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic.Projectiles
{
    public class DamageZone : MonoBehaviour, IBullet
    {
        [SerializeField] private ProjectileDataHolder _dataHolder;

        [SerializeField] private float _timeToDestroy;
        [SerializeField] private LayerMask _mask;

        public Team OwnerTeam { get; set; }
        
        private float _attackDelay = 1f;
        public event Action Destroyed;

        private float _leftToDestroy;

        private Collider2D[] _colliders;

        private void Awake()
        {
            _leftToDestroy = _timeToDestroy;
            _colliders = new Collider2D[20];
        }

        private void Start()
        {
            StartCoroutine(DamageOverTime());
        }

        private void Update()
        {
            _leftToDestroy -= Time.deltaTime;
            if (_leftToDestroy <= 0)
            {
                Destroyed?.Invoke();
                Destroy(this.gameObject);
            }
        }

        private IEnumerator DamageOverTime()
        {
            while (true)
            {
                yield return new WaitForSeconds(_attackDelay);
                DealDamage();
            }
        }

        private void DealDamage()
        {
            int numColliders = Physics2D.OverlapCircleNonAlloc(transform.position,
                _dataHolder.Stats[StatType.SplashRadius].Value, _colliders, _mask);
            
            for (int i = 0; i < numColliders; i++)
            {
                Health health;
                if (_colliders[i].TryGetComponent(out health))
                {
                    if (health.Team != OwnerTeam)
                    {
                        health.TakeDamage(_dataHolder.Stats[StatType.Damage].Value);
                    }
                }
            }
        }
    }
}