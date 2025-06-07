using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.CodeBase.Infrastructure.AssetManagement;
using Assets.CodeBase.Infrastructure.Factory;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Infrastructure.Services.AudioService;
using CodeBase.Logic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CodeBase.Infrastructure.Factory
{
    public class TilemapService : ITilemapService
    {
        private const string DamageLevelTexture = "_DamageLevelTexture";
        private Tilemap[] _tilemaps;
        private int _tilemapsCount;
        private Vector2Int _mapSize;

        private Dictionary<Vector2Int, DestructibleTileData> _mapTiles;

        private List<Vector2Int> _freeTiles;

        private IPersistentProgressService _progressService;
        private readonly IGameFactory _gameFactory;
        private readonly IAudioService _audioService;
        private readonly IAssets _assets;
        private Material[] _tilemapMaterials;
        private Texture2D[] _damageLevelTextures;
        private Texture2D _ambientOcclusionTexture;
        private SpriteRenderer _ambientOcclusion;


        public TilemapService(IPersistentProgressService progressService, IGameFactory gameFactory,
            IAudioService audioService, IAssets assets)
        {
            _progressService = progressService;
            _gameFactory = gameFactory;
            _audioService = audioService;
            _assets = assets;
        }

        /// <summary>
        /// This method supposed that all chunks are the same
        /// </summary>
        public void SetData(Vector2Int mapSize, Tilemap[] tilemaps, Dictionary<Vector2Int, DestructibleTileData> mapTiles,
            List<Vector2Int> freeTiles, SpriteRenderer ambientOcclusion, int chunks)
        {

            _tilemapMaterials = new Material[chunks];
            _mapSize = mapSize;
            _tilemaps = tilemaps;
            _tilemapsCount = chunks;
            _mapTiles = mapTiles;
            _freeTiles = freeTiles;
            _ambientOcclusion = ambientOcclusion;

            for (int i = 0; i < _tilemapsCount; i++)
            {
                _tilemapMaterials[i] = _tilemaps[i].GetComponent<TilemapRenderer>().material;
            }
            
            
            //TODO
            _damageLevelTextures = new Texture2D[chunks];
            for (int i = 0; i < chunks; i++)
            {
                _damageLevelTextures[i] = GenerateDamageTexture(mapSize);
                _tilemapMaterials[i].SetTexture(DamageLevelTexture, _damageLevelTextures[i]);
                _tilemapMaterials[i].SetVector("_MapSize", new Vector4(mapSize.x, mapSize.y, 0, 0));
                _tilemapMaterials[i].SetVector("_pos", new Vector4(0, -mapSize.y * i * GameConstants.MapBlockSize, 0, 0));
            }
            

            
            //GenerateAmbientOcculisionTexture(mapSize);
        }


        public async Task DamageBlocksInBounds(Bounds bounds, int damage)
        {
            bounds = new Bounds(bounds.center / GameConstants.MapBlockSize, bounds.size / GameConstants.MapBlockSize);
            Vector2Int minPoint = new Vector2Int(Mathf.FloorToInt(bounds.min.x),
                Mathf.FloorToInt(bounds.min.y));
            Vector2Int maxPoint = new Vector2Int(Mathf.CeilToInt(bounds.max.x),
                Mathf.CeilToInt(bounds.max.y));

            float blockSize = GameConstants.MapBlockSize / 4;
            
            for (int x = minPoint.x; x < maxPoint.x; x++)
            {
                for (int y = minPoint.y; y < maxPoint.y; y++)
                {
                    if (bounds.Contains(new Vector3(x + blockSize, y + blockSize, 0f)))
                    {
                        await DamageTile(damage, x, y);
                    }
                }
            }
        }

        public async Task DamageHitBlock(RaycastHit2D hit, int damage)
        {
            if (hit.collider.gameObject.layer == GameConstants.MapBlocksLayer)
            {
                Vector2 hitPos = hit.point - hit.normal * 0.01f;
                hitPos = hitPos / GameConstants.MapBlockSize;
                Vector2Int pos = new Vector2Int(Mathf.FloorToInt(hitPos.x), Mathf.FloorToInt(hitPos.y));
                await DamageTile(damage, pos.x, pos.y);
            }
        }

        public async Task DamageTile(int damage, int x, int y)
        {
            if (_mapTiles == null) return;
            int chunk = 0;
            int chunkSize = _mapSize.y;

            while (true)
            {
                if (y >= _mapSize.y)
                {
                    y -= chunkSize;
                    chunk++;
                }
                else
                {
                    break;
                }
            }
            Vector2Int tilePosition = new Vector2Int(x, y);

            Vector2Int globalPos = tilePosition + Vector2Int.up * _mapSize.y * chunk;
            if (_mapTiles.ContainsKey(globalPos))
            {
                DestructibleTileData destructibleTileData = _mapTiles[globalPos];
                destructibleTileData.GetDamage(damage);
                if (destructibleTileData.Destroyed)
                {
                    AudioClip breakingSound =  await _assets.Load<AudioClip>(destructibleTileData.StaticData.BreakingSound);
                    Vector2 pos = new Vector2(x * GameConstants.MapBlockSize, y * GameConstants.MapBlockSize);
                    DestroyTile(tilePosition, destructibleTileData, chunk);
                    _audioService.PlaySound(breakingSound,pos, true, destructibleTileData.StaticData.BreakingSoundVolume,
                        false, false);
                }
                else
                {
                    await SetDamageVisualForTile(x, y, destructibleTileData, chunk);
                }
                await _gameFactory.SpawnTileDestructionParticles(tilePosition + Vector2Int.up * (chunk * chunkSize),
                    destructibleTileData);

               
            }
        }

        private void DestroyTile(Vector2Int tilePosition, DestructibleTileData destructibleTileData,
            int chunk)
        {
            _tilemaps[chunk].SetTile((Vector3Int) tilePosition + Vector3Int.up* _mapSize.y *chunk,
                null);
            if (destructibleTileData.StaticData.HaveDrop)
            {
                _progressService.Progress.CollectResource(destructibleTileData.StaticData.Loot);
            }
            Vector2Int globalPos = tilePosition + Vector2Int.up * _mapSize.y * chunk;
            int chunkLinked = destructibleTileData.LinkedTileChunk;
            if (chunkLinked != null)
            {
                _tilemaps[chunkLinked].SetTile((Vector3Int) globalPos, null);
            }


            
            _mapTiles.Remove(globalPos);
            //_ambientOcclusionTexture.SetPixel(tilePosition.x, tilePosition.y, new Color(0f, 0f, 0f, 0f));
            //_ambientOcclusionTexture.Apply();
        }

        public async Task DamageWithBlast(Vector2 position, int damage, int range)
        {
            range = Mathf.RoundToInt(range / GameConstants.MapBlockSize);
            Vector2Int tilePosition = new Vector2Int(Mathf.RoundToInt(position.x / GameConstants.MapBlockSize),
                Mathf.RoundToInt(position.y / GameConstants.MapBlockSize));
            
            for (int x = tilePosition.x - range; x < tilePosition.x + range; x++)
            {
                for (int y = tilePosition.y - range; y < tilePosition.y + range; y++)
                {
                    float dist = (tilePosition - new Vector2Int(x, y)).magnitude;
                    float k = (range - dist) / range;
                    float realDamage = (float) damage * k * Mathf.Abs(k);
                    realDamage = Mathf.Clamp(realDamage, 0, damage);
                    await DamageTile(Mathf.RoundToInt(realDamage), x, y);
                }
            }
        }

        private async Task SetDamageVisualForTile(int x, int y, DestructibleTileData data, 
            int chunk)
        {
            //Debug.Log(x + " " + y + " " + chunk);
        
            var levelOfDestruction = data.StaticData.GetLevelOfDestruction(data.CurrentDamage);
            if (levelOfDestruction == data.LastLevelOfDestruction)
            {
                return;
            }
            else
            {
                data.LastLevelOfDestruction = levelOfDestruction;
                float destruction =1 - (float)levelOfDestruction / (float)GameConstants.MaxLevelOfDestruction;
                _damageLevelTextures[chunk].SetPixel(x, y, new Color(destruction, destruction, destruction));
                _damageLevelTextures[chunk].Apply();
                _tilemapMaterials[chunk].SetTexture(DamageLevelTexture, _damageLevelTextures[chunk]);
            }
        }

        private void GenerateAmbientOcculisionTexture(Vector2Int size)
        {
            _ambientOcclusionTexture = new Texture2D(size.x, size.y);
            _ambientOcclusionTexture.filterMode = FilterMode.Bilinear;
            _ambientOcclusionTexture.anisoLevel = 1;
            Color transparent = new Color(0f, 0f, 0f, 0.00f);
            for (int y = 0; y < _ambientOcclusionTexture.height; y++)
            {
                for (int x = 0; x < _ambientOcclusionTexture.width; x++)
                {
                    if (!_mapTiles.ContainsKey(new Vector2Int(x, y)))
                    {
                        _ambientOcclusionTexture.SetPixel(x, y, transparent);
                    }
                    else
                    {
                        _ambientOcclusionTexture.SetPixel(x, y, Color.black);
                    }
                    
                }
                _ambientOcclusionTexture.Apply();
                _ambientOcclusion.gameObject.transform.localScale = new Vector3(4, 4, 1f);
                //_ambientOcclusion.material.SetTexture("_Texture2D", _ambientOcclusionTexture);
                Rect rect = new Rect (0, 0, size.x, size.y);
                _ambientOcclusion.sprite = Sprite.Create(_ambientOcclusionTexture, rect,
                    Vector2.zero, 2f, 1, SpriteMeshType.FullRect);
            }
        }

        private Texture2D GenerateDamageTexture(Vector2Int mapSize)
        {
            Texture2D tex = new Texture2D(mapSize.x, mapSize.y);
            
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    tex.SetPixel(x, y, Color.white);
                }
            }
            tex.Apply();
            return tex;
        }
    }
}