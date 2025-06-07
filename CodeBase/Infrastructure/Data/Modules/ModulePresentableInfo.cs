using System;
using Assets.CodeBase.StaticData;

namespace CodeBase.Infrastructure.Data.Modules
{
    [Serializable]
    public class ModulePresentableInfo
    {
        public int Place;
        public MachineModuleStaticData ModuleStaticData;
        public int Level = 0;
    }
}