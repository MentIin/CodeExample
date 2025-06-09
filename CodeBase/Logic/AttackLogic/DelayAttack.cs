using System.Collections;
using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic
{
    public class DelayAttack : Attack
    {
        [SerializeField] private Attack _attack;
        [SerializeField] private float _delay = 0f;
        protected override void OnAttack()
        {
            StartCoroutine(InvokeAttack());
        }

        private IEnumerator InvokeAttack()
        {
            yield return new WaitForSeconds(_delay);
            _attack.Activate();
        }
    }
}