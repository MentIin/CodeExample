using Assets.CodeBase.Logic.Follow;
using CodeBase.Logic.Follow;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.CodeBase.Logic.Enemies
{
    public class RotateToTarget : BaseFollowTarget
    {
        public float Speed = 5f;
        [SerializeField] private bool _useRigidbody = true;
        [SerializeField] [ShowIf("_useRigidbody")] private Rigidbody2D _rigidbody;
        [SerializeField] private float _offset;
        private void Update()
        {
            Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 direction = (Vector2)TargetPoint - currentPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + _offset;

            if (_useRigidbody)
            {
                _rigidbody.MoveRotation(angle);
            }
            else
            {
                transform.eulerAngles = new Vector3(0f, 0f, angle);
            }
            
        }

        public override void SetTargetPoint(Vector3 targetPoint)
        {
            base.SetTargetPoint(targetPoint);
            Update();
        }
    }
}