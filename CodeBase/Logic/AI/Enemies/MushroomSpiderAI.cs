using System;
using System.Collections;
using Assets.CodeBase.Logic.AttackLogic.Activators;
using Assets.CodeBase.Logic.Enemies;
using Assets.CodeBase.Logic.Follow;
using CodeBase.Logic.Follow;
using UnityEngine;

namespace Assets.CodeBase.Logic.AI.Enemies
{
    public class MushroomSpiderAI : MonoBehaviour
    {
        [SerializeField] private float _timeToAwake = 0.3f;
        [SerializeField] private TriggerObserver _awakeZone;
        [SerializeField] private TriggerObserver _aggroZone;
        [SerializeField] private TriggerObserver _attackZone;
        [SerializeField] private AttackActivator _attackActivator;
        [SerializeField] private CheckIfSeeBounds _checkIfSeeBounds;
        [Space(5)]
        [SerializeField] private BaseFollowTarget _follow;

        public event Action StartedAwaking;

        private bool _awake = false;
        private bool _awaked = false;
        private void Update()
        {
            if (!_awake)
            {
                if (_awakeZone.CollidersInside > 0 && _checkIfSeeBounds.See && !_awaked)
                {
                    _awaked = true;
                    StartCoroutine(AwakeFromMushroom());
                }
            }
            else
            {
                if (_checkIfSeeBounds.See && _aggroZone.CollidersInside > 0)
                {
                    _follow.SetTargetPoint(_checkIfSeeBounds.Point);
                }
                
                _attackActivator.Active = _attackZone.CollidersInside > 0;
            }
            
        }

        private IEnumerator AwakeFromMushroom()
        {
            StartedAwaking?.Invoke();
            yield return new WaitForSeconds(_timeToAwake);
            _awake = true;
        }
    }
}