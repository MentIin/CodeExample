using UnityEngine;

namespace Assets.CodeBase.Logic.CameraLogic
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _following;
        public float FollowPositionSpeed = 3f;
        public Vector3 PositionOffset = new Vector3(0f, 0f, -10f);

        private void LateUpdate()
        {
            if (_following == null) return;

            var newPosition = GetNewPosition();

            transform.position = newPosition;
        }

        public void Follow(Transform following)
        {
            _following = following;
        }

        public void TeleportToTarget()
        {
            transform.position = _following.position + PositionOffset;
        }
        private Vector3 GetNewPosition()
        {
            Vector3 followingPosition = _following.position + PositionOffset;
            followingPosition = followingPosition + _following.up * 2;
            Vector3 newPosition =
                Vector3.Lerp(transform.position, followingPosition, FollowPositionSpeed * Time.deltaTime);
            return newPosition;
        }
    }
}