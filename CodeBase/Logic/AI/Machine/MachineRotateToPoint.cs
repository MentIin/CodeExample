using Assets.CodeBase.Logic.Follow;
using CodeBase.Logic.Follow;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.CodeBase.Logic.AI.Machine
{
    public class MachineRotateToPoint : BaseFollowTarget
    {
        [Required][SerializeField] private MachineBaseReferenceHolder _referenceHolder;
        [SerializeField] private float _speed=5f;
        [SerializeField] private float _offset=0f;

        private void FixedUpdate()
        {
            Vector2 axis = new Vector2(0f, 0f);
            
            Vector2 direction = TargetPoint - transform.position;
            direction = Quaternion.Euler(0f, 0f, _offset) * direction;
            
            Vector3 cross = Vector3.Cross(transform.up, direction.normalized);
            float axisX = Mathf.Clamp(-cross.z * _speed, -1, 1);
            axis.x = axisX;


            _referenceHolder.MachineBase.Move(axis);
        }
    }
}