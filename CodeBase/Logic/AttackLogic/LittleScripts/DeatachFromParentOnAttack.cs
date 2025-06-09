using NaughtyAttributes;
using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic.LittleScripts
{
    public class DetachFromParentOnAttack : MonoBehaviour
    {
        [SerializeField] [Required()] private Attack _attack;
        [SerializeField] private bool _destroy=false;
        [SerializeField] private float _destroyTime=10f;

        private void Awake()
        {
            _attack.Done += AttackOnDone;
        }

        private void AttackOnDone()
        {
            if (_destroy)
            {
                Destroy(this.gameObject, _destroyTime);
            }
            
            transform.SetParent(null);
        }
    }
}