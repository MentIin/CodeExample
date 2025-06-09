using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using Assets.CodeBase.Logic.Enemies;
using Assets.CodeBase.Logic.Follow;
using CodeBase.Logic.Follow;
using UnityEngine;

namespace Assets.CodeBase.Logic.AI.Machine
{
    public class SetTargetIfDontSeePlayer : MonoBehaviour
    {
        [SerializeField] private MobDataHolder mobDataHolder;
        [SerializeField] private CheckIfSeeBounds _checkIfSeeBounds;
        
        [SerializeField] private BaseFollowTarget _followTarget;
        [SerializeField] private float _timeToNotSee=5f;
        [SerializeField] private bool _dontResetTimeWhenSee = true;

        private Collider2D _target;
        private float _left;

        private void Start()
        {
            _target = mobDataHolder.PlayerMachineBase.GetComponent<Collider2D>();
            _left = _timeToNotSee;
        }

        private void Update()
        {
            _left -= Time.deltaTime;
            if (_checkIfSeeBounds.See)
            {
                if (_dontResetTimeWhenSee)
                {
                    _left += Time.deltaTime * 2;
                    if (_left > _timeToNotSee) _left = _timeToNotSee;
                }
                else
                {
                    _left = _timeToNotSee;
                }
            }

            if (_left <= 0)
            {
                _left = 0f;
                if (mobDataHolder.PlayerMachineBase != null) _followTarget.SetTargetPoint(mobDataHolder.PlayerMachineBase.transform.position);
            }
        }
    }
}