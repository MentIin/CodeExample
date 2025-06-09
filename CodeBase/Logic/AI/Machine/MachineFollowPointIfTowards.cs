using Assets.CodeBase.Logic.Follow;
using CodeBase.Logic.Follow;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.CodeBase.Logic.AI.Machine
{
    public class MachineFollowPointIfTowards : CheckableFollowTarget
    {
        [Required][SerializeField] private MachineBaseReferenceHolder _referenceHolder;
        [SerializeField] private float _angleStart=2f;
        [SerializeField] private float _angleEnd=10f;
        [Header("Go from point if not towards")]
        [SerializeField] private bool _reverse = false;

        private bool _active;
        private void FixedUpdate()
        {
            if (IsReached()) return;
            
            Vector2 axis = new Vector2(0f, 0f);
            Vector2 direction = TargetPoint - transform.position;
            float angle = Vector2.SignedAngle(transform.up, direction);

            if (Mathf.Abs(angle) <= _angleStart && !_active) _active = true;
            else if (Mathf.Abs(angle) >= _angleEnd && _active) _active = false;
            

            if (_active && !_reverse) axis.y = 1f;
            else if (_reverse && !_active) axis.y = -1f;

            _referenceHolder.MachineBase.Move(axis);
        }

        public override bool IsReached()
        {
            Vector3 dif = TargetPoint - transform.position;
            if (dif.magnitude <= TargetDistance)
            {
                return true;
            }

            return false;
        }
    }
}