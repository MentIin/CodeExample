using Assets.CodeBase.Logic;
using NaughtyAttributes;
using UnityEngine;

namespace CodeBase.Logic.MachineModules
{
    public class Turbo : MonoBehaviour
    {
        [Required()][SerializeField] private MachineModule Module;
        
        [Required()][SerializeField] private ConstantForce2D _constantForce;
        [Required()][SerializeField] private FixedJoint2D _fixedJoint2D;
        public float force = 10f;
        public bool _reverseForce = false;

        
        private void Update()
        {
            if (Module.Active)
            {
                if (_reverseForce) _constantForce.force = transform.up * (force * -1);
                else _constantForce.force = transform.up * force;
            }
            else
            {
                _constantForce.force = Vector2.zero;
            }
            
        }
    }
}