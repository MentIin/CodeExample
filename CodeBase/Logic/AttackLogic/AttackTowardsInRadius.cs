using Assets.CodeBase.Logic.DynamicDataLogic;
using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic
{
    public class AttackTowardsInRadius : Attack
    {
        [SerializeField] private LayerMask _layerMask;
        public int MaxHitsOnAttack = 10;
        private Collider2D[] _hits;

        protected override void OnAttack()
        {
            _hits = new Collider2D[MaxHitsOnAttack];
            for (int i = 0; i < Hit(); i++)
            {
                DealDamage(_hits[i].transform.GetComponent<Health>(), Damage);
            }
        }

        private int Hit()
        {
            int amount = Physics2D.OverlapCircleNonAlloc(GetHitPoint(), Range / 2, _hits, _layerMask);
            return amount;
        }

        private Vector3 GetHitPoint()
        {
            return transform.position + transform.right * (Range / 4);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(GetHitPoint(), Range / 2);
        }
    }
}