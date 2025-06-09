using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using Assets.CodeBase.Logic.Enemies;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.CodeBase.Logic.AI.Machine
{
    public class DisableComponentsIfDontSee : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] _components;
        [SerializeField] [Required()] private MobDataHolder mobDataHolder;
        [SerializeField] [Required()]private CheckIfSeeBounds _checkIfSeeBounds;
        private Collider2D _target;

        private void Start()
        {
            _target = mobDataHolder.PlayerMachineBase.GetComponent<Collider2D>();
        }
        private void Update()
        {
            foreach (var behaviour in _components)
            {
                behaviour.enabled = _checkIfSeeBounds.See;
            }
        }
    }
}