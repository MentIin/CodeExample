using Assets.CodeBase.Logic.AttackLogic.Activators;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using CodeBase.Logic.DynamicDataLogic.DataHolders;
using NaughtyAttributes;
using UnityEngine;

namespace CodeBase.Logic.MachineModules.Attacks
{
    public class AttachedMachineRegeneration : MonoBehaviour
    {
        [SerializeField] [Required()] private MachineModuleDataHolder _dataHolder;
        [SerializeField] [Required()] private MachineModule _machineModule;
        [SerializeField] private AttackActivator _attackActivator;

        private bool _health = true;
        private bool _energy = true;
        private bool _healthPercent;

        private void Awake()
        {
            _attackActivator.Done += AttackActivatorOnDone;
        }

        private void Start()
        {
            _health = _dataHolder.Stats.ContainsKey(StatType.HealthRegeneration);
            _healthPercent = _dataHolder.Stats.ContainsKey(StatType.HealthRegenerationPercent);
            _energy = _dataHolder.Stats.ContainsKey(StatType.EnergyRegeneration);
        }

        private void AttackActivatorOnDone()
        {
            if (_health) _machineModule.AttachedMachineBase.Health.Heal(_dataHolder.Stats[StatType.HealthRegeneration].Value);
            if (_healthPercent)
            {
                _machineModule.AttachedMachineBase.Health.HealPercent(
                    _dataHolder.Stats[StatType.HealthRegenerationPercent].Value);
            }
        }
    }
}