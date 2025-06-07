using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.CodeBase.Infrastructure.Factory
{
    public interface ITilemapService
    {
        void SetData(Vector2Int mapSize, Tilemap[] tilemaps, Dictionary<Vector2Int, DestructibleTileData> mapTiles,
            List<Vector2Int> freeTiles, SpriteRenderer ambientOcclusion, int chunks);
        Task DamageBlocksInBounds(Bounds bounds, int damage);
        Task DamageHitBlock(RaycastHit2D hit, int damage);
        Task DamageTile(int damage, int x, int y);
        Task DamageWithBlast(Vector2 position, int damage, int range);
    }
}