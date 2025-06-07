using System;
using System.Collections.Generic;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Infrastructure.Data.Modules;
using Assets.CodeBase.StaticData;
using CodeBase.Logic;
using CodeBase.UI.Services.GameStopOver;

namespace CodeBase.Infrastructure.Data
{
    [Serializable]
    public class MachineModel
    {
        public int MachineBasementId;
        public List<ModuleSerializableInfo> MachineModules = new List<ModuleSerializableInfo>();

        public List<UpgradeType> Upgrades = new List<UpgradeType>();

        public MachineModel(MachineStaticData data)
        {
            MachineBasementId = data.BasementStaticData.Id;

            foreach (var info in data.Modules)
            {
                MachineModules.Add(new ModuleSerializableInfo
                {
                     Id = info.ModuleStaticData.Id, Place = info.Place, Level = info.Level
                });
            }
        }

        public MachineModel() { }

        public event Action MachineModulesChanged;

        public void SwapModules(int place1, int place2)
        {
            ModuleSerializableInfo module1 = null;
            ModuleSerializableInfo module2 = null;
            foreach (ModuleSerializableInfo info in MachineModules)
            {
                if (info.Place == place1)
                {
                    module1 = info;
                }
                if (info.Place == place2)
                {
                    module2 = info;
                }
            }

            if (module1 != null) module1.Place = place2;
            if (module2 != null) module2.Place = place1;
            MachineModulesChanged?.Invoke();
        }
        

        public bool HaveFreePlace(int place)
        {
            foreach (var info in MachineModules)
            {
                if (info.Place == place) return false;
            }
            return true;
        }

        public void RemoveMachineModule(int place)
        {
            foreach (var info in MachineModules)
            {
                if (info.Place == place)
                {
                    MachineModules.Remove(info);
                    MachineModulesChanged?.Invoke();
                    return;
                }
            }
        }

        public string GetMachineModuleId(int place)
        {
            foreach (var info in MachineModules)
            {
                if (info.Place == place) return info.Id;
            }

            return null;
        }

        public void InstallModule(string id, int level, int place)
        {
            if (HaveFreePlace(place))
            {
                MachineModules.Add(new ModuleSerializableInfo
                {
                    Id = id,
                    Place = place,
                    Level = level
                });
                MachineModulesChanged?.Invoke();
            }
        }

        public ModuleSerializableInfo GetMachineModuleInfo(int place)
        {
            foreach (var serializableInfo in MachineModules)
            {
                if (serializableInfo.Place == place) return serializableInfo;
            }

            return null;
        }

        public int GetUpgradeLevel(UpgradeType upgradeType)
        {
            int res = 0;
            foreach (var type in Upgrades)
            {
                if (type == upgradeType) res++;
            }

            return res;
        }

        
        //Legacy
        private void MergeModules(int to, int from)
        {
            if (to == from) return;
            if (GetMachineModuleInfo(to).Id == GetMachineModuleInfo(from).Id
                && GetMachineModuleInfo(to).Level == GetMachineModuleInfo(from).Level)
            {
                MachineModules.Remove(GetMachineModuleInfo(from));
                GetMachineModuleInfo(to).Level++;
            }
        }
    }
}