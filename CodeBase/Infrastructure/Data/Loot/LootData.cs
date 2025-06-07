using System;
using Assets.CodeBase.Infrastructure.Services.StaticData;
using NaughtyAttributes;

namespace Assets.CodeBase.Infrastructure.Data.Loot
{
    [Serializable]
    
    public class LootData
    {
        public LootType Type = LootType.CraftingResource;
        [ShowIf("_show")][AllowNesting]public CraftingResourceType CraftingResourceType;

        private bool _show => Type == LootType.CraftingResource;
        public int Amount=1;
    }
}