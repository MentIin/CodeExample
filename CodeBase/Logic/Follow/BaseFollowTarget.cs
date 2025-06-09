using UnityEngine;

namespace CodeBase.Logic.Follow
{
    public abstract class BaseFollowTarget : MonoBehaviour
    {
        public float TargetDistance = 1f;
        public Vector3 TargetPoint { get; protected set; }
        public virtual void SetTargetPoint(Vector3 targetPoint)
        {
            TargetPoint = targetPoint;
        }
        
    }

    public abstract class CheckableFollowTarget : BaseFollowTarget
    {
        public abstract bool IsReached();
    }
}