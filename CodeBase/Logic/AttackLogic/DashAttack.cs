using System;
using System.Collections;
using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic
{
    public class DashAttack : Attack
    {
        [SerializeField] private float DashDuration; // should be less then reload
        [Required()][SerializeField] private TriggerObserver _attackTriggerObserver;
        [Required()][SerializeField] private Rigidbody2D _rigidbody;

        private bool _active=false;
        public event Action Ended;
        public event Action Started;
        private void Start()
        {
            _attackTriggerObserver.TriggerEnter += AttackTriggerObserverOnTriggerEnter;
        }

        private void AttackTriggerObserverOnTriggerEnter(Collider2D obj)
        {
            if (_active)
            {
                Health health;
                if (obj.TryGetComponent<Health>(out health))
                {
                    DealDamage(health, Damage);
                }
            }
        }

        protected override void OnAttack()
        {
            Started?.Invoke();
            _rigidbody.AddForce(transform.right * DataHolder.Stats[StatType.DashSpeed].Value * _rigidbody.mass, ForceMode2D.Impulse);
            StartCoroutine(EndDash());
            _active = true;
        }

        private IEnumerator EndDash()
        {
            yield return new WaitForSeconds(DashDuration);
            _rigidbody.linearVelocity = _rigidbody.linearVelocity * 0.1f;
            _active = false;
            Ended?.Invoke();
        }
    }
}