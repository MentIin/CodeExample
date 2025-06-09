using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using UnityEngine;

namespace Assets.CodeBase.Logic.Enemies
{
    [RequireComponent(typeof(MobDataHolder))]
    public class RotateIfSeePlayer : MonoBehaviour
    {
        [SerializeField] private MobDataHolder _dataHolder;
        [SerializeField] private CheckIfSeeBounds _checkIfSeeBounds;
        [SerializeField] private RotateToTarget _rotateToTarget;

        private Transform _target;


        private void Start()
        {
            _target = _dataHolder.PlayerMachineBase.transform;
        }

        private void Update()
        {
            if (_checkIfSeeBounds.See)
            {
                if (_target)
                {
                    Vector2 targetPoint = _target.position;
                    if (_rotateToTarget)
                    {
                        _rotateToTarget.enabled = true;
                        _rotateToTarget.SetTargetPoint(targetPoint);
                    }
                }
            }
            else
            {
                _rotateToTarget.enabled = false;
            }
        }
    }
}