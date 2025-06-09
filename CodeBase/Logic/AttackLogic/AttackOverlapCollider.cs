using Assets.CodeBase.Logic.DynamicDataLogic;
using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic
{
    public class AttackOverlapCollider : Attack
    {
        [SerializeField] private Collider2D _collider;
        [SerializeField] private ContactFilter2D _contactFilter;
        public int MaxHitsOnAttack = 10;
        private Collider2D[] _hits;

        protected override void OnAttack()
        {
            _hits = new Collider2D[MaxHitsOnAttack];
            for (int i = 0; i < Hit(); i++)
            {
                Health health;
                if (_hits[i].transform.TryGetComponent(out health))
                {
                    DealDamage(health, Damage);
                }
            }
        }

        private int Hit()
        {
            int amount = Physics2D.OverlapCollider(_collider, _contactFilter, _hits);
            return amount;
        }
    }
}