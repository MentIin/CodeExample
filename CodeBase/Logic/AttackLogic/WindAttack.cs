using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using Assets.CodeBase.Logic.Effects;
using Assets.CodeBase.Logic.Raycasters;
using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic
{
    class WindAttack : Attack
    {
        public Raycaster Raycaster;
        private float _pushForce;

        private void Start()
        {
            _pushForce = DataHolder.Stats[StatType.Knockback].Value;
            Raycaster.Distance = DataHolder.Stats[StatType.Range].Value;
        }

        protected override void OnAttack()
        {
            float raysCount = Raycaster.RaysCount;
            foreach (var hit in Raycaster.Cast())
            {
                if (hit.collider)
                {
                    Rigidbody2D component;
                    if (hit.collider.TryGetComponent(out component))
                    {
                        Vector2 force = -hit.normal*(_pushForce / raysCount);

                        Knocknack knocknack = component.gameObject.AddComponent<Knocknack>();
                        knocknack.Construct(force, component);
                    }
                }
            }
        }
    }
}