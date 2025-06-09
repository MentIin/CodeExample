using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using Assets.CodeBase.Logic.Follow;
using CodeBase.Logic.Follow;
using UnityEngine;

namespace Assets.CodeBase.Logic.Enemies
{
    public class FollowIfSeePlayer : MonoBehaviour
    {
        [SerializeField] private MobDataHolder mobDataHolder;
        [SerializeField] private CheckIfSeeBounds _checkIfSeeBounds;
        
        [SerializeField] private BaseFollowTarget _followTarget;

        private Collider2D _target;

        private void Start()
        {
            _target = mobDataHolder.PlayerMachineBase.GetComponent<Collider2D>();
        }

        private void Update()
        {
            if (_checkIfSeeBounds.See)
            {
                Vector2 targetPoint = _checkIfSeeBounds.Point;
                if (_followTarget)
                {
                    _followTarget.SetTargetPoint(targetPoint);
                }
            }
        }
    }
}