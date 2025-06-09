using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using CodeBase.Logic.DynamicDataLogic.DataHolders;
using NaughtyAttributes;
using UnityEngine;

namespace CodeBase.Logic.MachineModules
{
    public class BatteryCC : MonoBehaviour
    {
        [SerializeField] [Required()] private MachineModuleDataHolder _dataHolder;
        [SerializeField] [Required()] private MachineModule _machineModule;


        private void Awake()
        {
            _machineModule.Activated += AttackActivatorOnDone;
        }


        private void AttackActivatorOnDone()
        {

                _machineModule.AttachedMachineBase.SpeedUpCCReload(
                    _dataHolder.Stats[StatType.SpeedUpCCReload].Value);

        }
    }
}