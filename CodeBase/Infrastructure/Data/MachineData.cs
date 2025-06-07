using System;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.StaticData;
using CodeBase.Logic.MachineModules;

namespace CodeBase.Infrastructure.Data
{
    [Serializable]
    public class MachineData
    {
        public ModulePlacementInfo[] ModulePlacementInfo;
        public float MovingSpeed=20f;
        public float RotationSpeed=90f;
        public MachineModuleInfo[] Modules;

        public float Fuel;
    }

    [Serializable]
    public class MachineModuleInfo
    {
        public MachineModule Module;
        public Direction Direction;
        public int Group;
        public bool CanBeActivated=true;
                
        public float ActivationReloadTick=0f;
        public MachineModuleStaticData StaticData;
    }
}