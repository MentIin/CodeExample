using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Infrastructure.Data.Stats;
using Assets.CodeBase.Infrastructure.Services;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Logic.AttackLogic.Projectiles;
using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using Assets.CodeBase.StaticData;
using Assets.CodeBase.StaticData.Level;
using CodeBase.Infrastructure.Data.Modules;
using CodeBase.Logic.Machine;
using CodeBase.UI.Services.GameStopOver;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Factory
{
    public interface IGameFactory : IService
    {
        List<ISavedProgressReader> ProgressReaders { get; }
        List<ISavedProgress> ProgressWriters { get; }
        void CleanUp();
        Task WarmUp();
        Task GenerateLevel(LevelStaticData levelStaticData, int seed);
        Task InitMap();
        Task SpawnEnemies(LevelStaticData staticData);
        Task<GameObject> CreateHud(LevelStaticData levelStaticData);

        Task<IBullet> CreateProjectile(Transform startPoint, GameObject asset,
            Dictionary<StatType, Stat> stats, Health ownerHealth);

        Task<MachineBase> CreatePlayerMachine(Vector2 playerStartingTilePosition, MachineBasementStaticData basement,
            ModulePresentableInfo[] modules, List<UpgradeType> playerMachineUpgrades);
        Task SpawnTileDestructionParticles(Vector2Int tilePosition, DestructibleTileData tileData);
        Task SpawnModuleCollectable(MachineModuleStaticData moduleStaticData, Vector2 transformPosition);
    }
}