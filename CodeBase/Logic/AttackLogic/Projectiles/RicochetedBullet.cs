using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic.Projectiles
{
    class RicochetedBullet : Bullet
    {
        [SerializeField] private LayerMask _ricochetMask;
        protected override void Move(float distance)
        {
            float left = distance;
            int index = 0;
            foreach (var hit in hits)
            {
                index++;
                if (index > raycastHitsAmount) break;
                if (hit.collider == null) continue;
                if (_ricochetMask.CheckIfLayerInMask(hit.collider.gameObject.layer))
                {
                    transform.position = hit.point + hit.normal * 0.1f;
                    left = 0f;

                    transform.right = Vector2.Reflect(transform.right, hit.normal);
                    break;
                }
            }
            transform.Translate(Vector2.right * left);
        }
        
    }
}