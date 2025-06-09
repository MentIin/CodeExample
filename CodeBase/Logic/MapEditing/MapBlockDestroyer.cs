using Assets.CodeBase.Infrastructure.Factory;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.CodeBase.Logic.MapEditing
{
    public abstract class MapBlockDestroyer : MonoBehaviour
    {
        [Required()] public BaseDataHolder DataHolder;

        protected ITilemapService tilemapService;

        public void Construct(ITilemapService factory)
        {
            tilemapService = factory;
        }
        
        public abstract void DestroyMap();
    }
}