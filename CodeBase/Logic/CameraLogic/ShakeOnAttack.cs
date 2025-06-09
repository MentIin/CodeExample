using Assets.CodeBase.Logic.AttackLogic;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.CodeBase.Logic.CameraLogic
{
    public class ShakeOnAttack : MonoBehaviour
    {
        [SerializeField] [Required()] private Attack _attack;
        [SerializeField] [Required()] private CameraShake _shake;
        

        private void Awake()
        {
            _attack.Done += AttackOnDone;
        }

        private void AttackOnDone()
        {
            _shake.Play();
        }
    }
}