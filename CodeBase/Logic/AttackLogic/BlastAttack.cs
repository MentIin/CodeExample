using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic
{
    public class BlastAttack : Attack
    {
        [SerializeField] private LayerMask _layerMask;
        public int MaxHitsOnAttack = 20;
        private Collider2D[] _hits;

        protected override void OnAttack()
        {
            _hits = new Collider2D[MaxHitsOnAttack];
            for (int i = 0; i < Hit(); i++)
            {
                Health health;
                
                Collider2D hittedCollider = _hits[i];
                if (hittedCollider.transform.TryGetComponent(out health))
                {
                    float dist = (hittedCollider.transform.position - transform.position).magnitude;
                    float value = DataHolder.Stats[StatType.SplashRadius].Value;
                    float k = (value - dist) / value;
                    float realDamage = (float) Damage * k;
                    realDamage = Mathf.Clamp(realDamage, 0, Damage);
                    DealDamage(health, Mathf.RoundToInt(realDamage));
                }
            }
        }

        private int Hit()
        {
            int amount = Physics2D.OverlapCircleNonAlloc(transform.position, DataHolder.Stats[StatType.SplashRadius].Value, _hits, _layerMask);
            return amount;
        }
    }
}