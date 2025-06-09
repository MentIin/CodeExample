using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using CodeBase.Logic.Follow;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.CodeBase.Logic.Follow
{
    public class SimpleFollowTarget : CheckableFollowTarget
    {
        private const int ChangingVelocitySpeed = 2;
        [Required()] public MobDataHolder DataHolder;

        [SerializeField] private bool _setTargetToTransformOnAwake=true;
        [Required()] [SerializeField] private Rigidbody2D _rigidbody;
        private Vector3 dif;

        private void Awake()
        {
            if (_setTargetToTransformOnAwake)
            {
                TargetPoint = transform.position;
            }
        }

        private void FixedUpdate()
        {
            dif = TargetPoint - transform.position;
            Vector2 move;
            if (!IsReached())
            {
                move = dif.normalized * (DataHolder.Stats[StatType.MoveSpeed].Value);
            }
            else
            {
                move = Vector2.zero;
            }
            //Vector2 velocityDif = move - _rigidbody.velocity;
            //_rigidbody.AddForce(velocityDif * (Time.fixedDeltaTime * ChangingVelocitySpeed) * _rigidbody.mass, ForceMode2D.Impulse);
            _rigidbody.linearVelocity = move;
        }

        public override bool IsReached()
        {
            return dif.magnitude < TargetDistance;
        }
    }
}