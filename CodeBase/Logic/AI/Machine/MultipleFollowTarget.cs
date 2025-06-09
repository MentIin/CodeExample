using Assets.CodeBase.Logic.Follow;
using CodeBase.Logic.Follow;
using UnityEngine;

namespace Assets.CodeBase.Logic.AI.Machine
{
    public class MultipleFollowTarget : CheckableFollowTarget
    {
        [SerializeField] private BaseFollowTarget[] FollowTarget;
        [Header("ONLY CHECK! THEY DONT ACTIVATE!")]
        [SerializeField] private CheckableFollowTarget[] FollowTargetToCheck;
        [SerializeField] private bool _setTargetToTransformOnStart = true;

        private void Start()
        {
            if (_setTargetToTransformOnStart)
            {
                SetTargetPoint(transform.position);
            }
        }

        public override void SetTargetPoint(Vector3 targetPoint)
        {
            base.SetTargetPoint(targetPoint);
            foreach (var followTarget in FollowTarget)
            {
                followTarget.SetTargetPoint(targetPoint);
            }
        }

        public override bool IsReached()
        {
            foreach (var followTarget in FollowTargetToCheck)
            {
                if (!followTarget.IsReached()) return false;
            }
            return true;
        }
    }
}