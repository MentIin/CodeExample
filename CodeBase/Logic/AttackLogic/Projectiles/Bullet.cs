using System;
using System.Collections.Generic;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Infrastructure.Factory;
using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using Assets.CodeBase.Logic.Effects;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.CodeBase.Logic.AttackLogic.Projectiles
{
    public class Bullet : MonoBehaviour, IBullet
    {
        [SerializeField] [Required()] private ProjectileDataHolder _dataHolder;
        
        [SerializeField] private float _speed=1f;
        [SerializeField] private int _pierce = 0;
        
        [SerializeField] [ShowIf("ShowInstantlyDieLayerInInspector")]private LayerMask _instantlyDie;
        [FormerlySerializedAs("mask")][SerializeField] protected LayerMask attackMask;
        [SerializeField] protected LayerMask alwaysPierceMask;

        [SerializeField] private bool _doNotDealDamage = false;
        
        private float _rangeLeft;
        private ITilemapService _tilemapService;

        private int _pierceLeft;
        protected RaycastHit2D[] hits = new RaycastHit2D[10];
        private List<Collider2D> _hittedColliders = new List<Collider2D>();
        protected int raycastHitsAmount;

        private bool ShowInstantlyDieLayerInInspector { get => _pierce > 0; }
        public Team OwnerTeam { get; set; }
        
        public event Action Destroyed;

        public void Construct(ITilemapService factory)
        {
            _tilemapService = factory;
            _pierceLeft = _pierce;
        }

        private void Start()
        {
            _rangeLeft = _dataHolder.Stats[StatType.Range].Value;
        }

        private void Update()
        {
            float distance = _speed * Time.deltaTime;
            if (_rangeLeft <= 0)
            {
                Die();
                return;
            }
            else if (_rangeLeft < distance)
            {
                distance = _rangeLeft;
                _rangeLeft = 0;
            }
            _rangeLeft -= distance;
            

            raycastHitsAmount = Raycast(distance);
            
            for (int i = 0; i < raycastHitsAmount; i++)
            {
                RaycastHit2D hit2D = hits[i];

                if (!_hittedColliders.Contains(hit2D.collider))
                {
                    Health health;
                    _pierceLeft -= 1;
                    _hittedColliders.Add(hit2D.collider);
                    if (hit2D.collider.TryGetComponent(out health))
                    {
                        if (health.Team != OwnerTeam) DealDamage(health, hit2D);
                        if (health.Team == OwnerTeam || alwaysPierceMask.CheckIfLayerInMask(hit2D.collider.gameObject.layer)) {
                            _pierceLeft++;
                        }
                    }else if (alwaysPierceMask.CheckIfLayerInMask(hit2D.collider.gameObject.layer))
                    {
                        _pierceLeft++;
                    }
                    if (_dataHolder.Stats.ContainsKey(StatType.DestructiveCapacity)) _tilemapService.DamageHitBlock(hit2D,
                        _dataHolder.Stats[StatType.DestructiveCapacity].Value);
                    
                    
                }
                if (_instantlyDie.CheckIfLayerInMask(hit2D.collider.gameObject.layer)) Die();
                
                if (_pierceLeft < 0)
                {
                    Die();
                }
            }
            
            Move(distance);
        }

        protected virtual void Move(float distance)
        {
            transform.Translate(Vector2.right * distance);
        }

        protected virtual int Raycast(float distance)
        {
            return Physics2D.RaycastNonAlloc(transform.position, transform.right,
                hits, distance, attackMask);
        }

        private void DealDamage(Health health, RaycastHit2D hit)
        {
            if (_doNotDealDamage) return;
            health.TakeDamage(_dataHolder.Stats[StatType.Damage].Value);
            if (_dataHolder.Stats.ContainsKey(StatType.Knockback))
            {
                Knocknack component = hit.collider.gameObject.AddComponent<Knocknack>();
                component.Construct(_dataHolder.Stats[StatType.Knockback].Value * -hit.normal, hit.collider.attachedRigidbody);
            }
        }

        private void Die()
        {
            Destroyed?.Invoke();
            Destroy(this.gameObject);
        }
    }
}