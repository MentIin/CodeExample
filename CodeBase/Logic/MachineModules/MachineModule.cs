using System;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Infrastructure.Data.Stats;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using Assets.CodeBase.StaticData;
using CodeBase.Infrastructure.Data;
using CodeBase.Logic.DynamicDataLogic.DataHolders;
using CodeBase.Logic.Machine;
using UnityEngine;

namespace CodeBase.Logic.MachineModules
{
    public sealed class MachineModule : MonoBehaviour
    {
        public MachineModuleDataHolder statsHolder;
        [HideInInspector]public MachineBase AttachedMachineBase;

        private bool active = false;
        private bool _canBeActivated;
        private int _group;

        public event Action Activated;
        public event Action Deactivated;
        public bool Active
        {
            get => active;
        }
        public void Construct(MachineBase machineBase, int group)
        {
            _group = group;
            AttachedMachineBase = machineBase;
        }

        public void Deactivate()
        {
            if (!active) return;
            active = false;
            OnDeactivate();
        }

        public void Activate()
        {
            if (active) return;
            active = true;
            OnActivate();
        }
        
        

        public void SetData(MachineModuleStaticData data, int level, float k)
        {
            statsHolder.Stats = data.Stats.ToDictionary();
            
            if (!Mathf.Approximately(k, 1f))
            {
                foreach (var pair in statsHolder.Stats)
                {
                    if (pair.Key == StatType.Damage)
                    {
                        pair.Value.AddModifier(new Modifier{Value = k});
                    }
                }
            }

            foreach (var statLvlModifier in data.StatsLevelModifier)
            {
                if (statsHolder.Stats.ContainsKey(statLvlModifier.Type))
                {
                    Modifier mod = new Modifier();
                    MinMaxRange minMaxRange = new MinMaxRange();
                    minMaxRange.Min = 1;
                    minMaxRange.Max = statLvlModifier.Value;
                    mod.Value = minMaxRange.GetValue((level-1) / (float)(GameConstants.MaxModuleLevel-1));
                    
                    Debug.Log(level + " " + mod.Value);
                    statsHolder.Stats[statLvlModifier.Type].AddModifier(mod);
                }
            }
            
            
            
            _canBeActivated = data.CanBeActivated;

        }

        public void Initialize()
        {
            
        }

        private void OnActivate()
        {
            Activated?.Invoke();
        }

        private void OnDeactivate()
        {
            Deactivated?.Invoke();
        }

        public void ChangeActiveState()
        {
            if (active) Deactivate();
            else Activate();
        }
    }
}