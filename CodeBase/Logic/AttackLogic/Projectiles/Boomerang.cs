using System;
using System.Collections.Generic;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Infrastructure.Factory;
using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic.Projectiles
{
    public class Boomerang : MonoBehaviour, IBullet
    {
        [SerializeField] [Required()] private ProjectileDataHolder _dataHolder;
        [SerializeField][Layer] private int _startReturnLayer;
        [SerializeField] private LayerMask _mask;
        public float Speed=1f;
        [SerializeField] private float _targetReturningDistance = 1f;
        [SerializeField] private bool _rotateToOriginWhenReturning = false;

        private float _rangeLeft;
        private ITilemapService _tilemapService;
        private RaycastHit2D[] _hits;
        private bool _returning;
        private Transform _firepointTransform;
        private List<Health> _hittedHealth;
        private Vector3 _returnPosition;

        public event Action Destroyed;
        public Team OwnerTeam { get; set; }

        public float RangeLeft { get => _rangeLeft; }

        public void Construct(ITilemapService tilemapService, Transform firePoint)
        {
            _firepointTransform = firePoint;
            _returnPosition = _firepointTransform.position;
            _tilemapService = tilemapService;
            _hits = new RaycastHit2D[10];
            _hittedHealth = new List<Health>();
        }

        private void Start()
        {
            _rangeLeft = _dataHolder.Stats[StatType.Range].Value;
        }

        private void Update()
        {
            Vector2 direction;
            if (_firepointTransform) _returnPosition = _firepointTransform.position;
            if (_returning)
            {
                Vector2 dir = -(transform.position - _returnPosition).normalized;
                if (_rotateToOriginWhenReturning)
                {
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                }
                
                direction = dir;
            }
            else
            {
                direction = transform.right;
            }
            
            var distance = HandleDistanceChanging();

            int hitsAmount = Physics2D.RaycastNonAlloc(transform.position, 
                direction, _hits, distance, _mask);

            
            HandleRaycast(distance, hitsAmount);
            transform.position += (Vector3) (direction * distance);
        }

        private float HandleDistanceChanging()
        {
            float distance = Speed * Time.deltaTime;
            if (!_returning)
            {
                if (_rangeLeft <= 0)
                {
                    StartReturning();
                }
                else if (_rangeLeft < distance)
                {
                    distance = _rangeLeft;
                    _rangeLeft = 0;
                }

                _rangeLeft -= distance;
            }
            else
            {
                Vector2 dist = (_returnPosition - transform.position);
                if (dist.magnitude <= _targetReturningDistance)
                {
                    Die();
                }
            }

            return distance;
        }

        private void HandleRaycast(float distance, int hitsAmount)
        {
            
            for (int i = 0; i < hitsAmount; i++)
            {
                RaycastHit2D hit = _hits[i];
                if (hit.collider.gameObject.layer == _startReturnLayer)
                {
                    DealDamage(hit);
                    StartReturning();
                }
                else
                {
                    DealDamage(hit);
                }
            }
            
        }

        private void StartReturning()
        {
            if (_returning) return;
            _returning = true;
            _hittedHealth = new List<Health>();
        }

        private void DealDamage(RaycastHit2D hit)
        {
            Health health;
            if (hit.collider.TryGetComponent(out health))
            {
                if (health.Team != OwnerTeam)
                {
                    if (!_hittedHealth.Contains(health))
                    {
                        health.TakeDamage(_dataHolder.Stats[StatType.Damage].Value);
                        _hittedHealth.Add(health);
                    }
                }
            }
            if (_dataHolder.Stats.ContainsKey(StatType.DestructiveCapacity)) _tilemapService.DamageHitBlock(hit, _dataHolder.Stats[StatType.DestructiveCapacity].Value);
        }

        private void Die()
        {
            Destroyed?.Invoke();
            Destroy(this.gameObject);
        }
    }
}