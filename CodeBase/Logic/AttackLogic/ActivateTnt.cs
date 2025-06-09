using Assets.CodeBase.Logic.DynamicDataLogic;
using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic
{
    public class ActivateTnt : Attack
    {
        [SerializeField] private Health _tntHealth;

        protected override void OnAttack()
        {
            _tntHealth.TakeDamage(1);
        }
    }
}