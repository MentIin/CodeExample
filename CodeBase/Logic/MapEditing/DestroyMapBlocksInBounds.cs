using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using UnityEngine;

namespace Assets.CodeBase.Logic.MapEditing
{
    public class DestroyMapBlocksInBounds : MapBlockDestroyer
    {
        [SerializeField] private Collider2D _collider;

        public override void DestroyMap()
        {
            tilemapService.DamageBlocksInBounds(_collider.bounds, DataHolder.Stats[StatType.DestructiveCapacity].Value);
        }
    }
}