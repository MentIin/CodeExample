using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic
{
    public class MultiplyAttacks : Attack
    {
        [SerializeField] private Attack[] _attacks;

        protected override void OnAttack()
        {
            foreach (var attack in _attacks)
            {
                attack.Activate();
            }
        }
    }
}