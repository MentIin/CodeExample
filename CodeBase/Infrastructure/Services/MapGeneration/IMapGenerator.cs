using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.StaticData.Level;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Services.MapGeneration
{
    public interface IMapGenerator : IService
    {
        MapData GetMap(int seed, LevelStaticData staticData,  Vector2Int[] safeZonesPositions,
            Vector2Int[] safeZoneSizes, int chunk);
    }
}