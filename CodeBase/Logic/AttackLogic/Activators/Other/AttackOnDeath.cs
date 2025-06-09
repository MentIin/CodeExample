using Assets.CodeBase.Logic.DynamicDataLogic;
using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic.Activators.Other
{
    public class AttackOnDeath : MonoBehaviour
    {
        [SerializeField] private Death _death;
        [SerializeField] private Attack[] _attacks;

        private void Awake()
        {
            _death.Happen += DeathOnHappen;
        }

        private void DeathOnHappen()
        {
            foreach (var attack in _attacks)
            {
                attack.Activate();
            }
        }
    }
}