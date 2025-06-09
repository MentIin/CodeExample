using Assets.CodeBase.Logic.DynamicDataLogic;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.CodeBase.Logic.Raycasters
{
    public abstract class Raycaster : MonoBehaviour
    {
        public LayerMask CastLayerMask;
        public float Distance;
        
        public bool _ignoreMyTeam = false;
        [ShowIf("_ignoreMyTeam")] public Health Health;

        
        public abstract int RaysCount { get; }

        public abstract RaycastHit2D[] Cast();


        protected virtual void OnDrawGizmosSelected()
        {
            foreach (var raycastHit2D in Cast())
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(raycastHit2D.point, 0.3f);
            }
        }
    }
}