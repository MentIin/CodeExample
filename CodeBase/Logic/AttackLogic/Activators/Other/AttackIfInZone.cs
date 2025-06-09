using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic.Activators.Other
{
    public class AttackIfInZone : MonoBehaviour
    {
        [SerializeField] private TriggerObserver _triggerObserver;
        [SerializeField] private AttackActivator _attackActivator;

        private void Start()
        {
            _triggerObserver.TriggerEnter += StartAttack;
            _triggerObserver.TriggerExit += EndAttack;
        }

        private void EndAttack(Collider2D obj)
        {
            _attackActivator.Active = false;
        }

        private void StartAttack(Collider2D obj)
        {
            _attackActivator.Active = true;
        }
    }
}