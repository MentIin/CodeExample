using Assets.CodeBase.Logic;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using CodeBase.Logic.DynamicDataLogic.DataHolders;
using NaughtyAttributes;
using UnityEngine;

namespace CodeBase.Logic.MachineModules
{
    public class AdjustMachineBaseStats : MonoBehaviour, IInitializable
    {
        [Required()]public MachineModuleDataHolder DataHolder;
        [Required()] public MachineModule Module;
        public bool OnStart = true;
        

        public void Initialize()
        {
            if (!OnStart) return;
            if (DataHolder.Stats.ContainsKey(StatType.AdjustMaxHealth))
            {
                Module.AttachedMachineBase.AdjustMaxHealth(DataHolder.Stats[StatType.AdjustMaxHealth].Value);
            }
        }
    }
}