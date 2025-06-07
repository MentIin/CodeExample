using System;
using Assets.CodeBase.StaticData.Level;
using CodeBase.Infrastructure.Data;

namespace Assets.CodeBase.Infrastructure.Data
{
    [Serializable]
    public class OreGenerationData
    {
        public MapBlockStaticData Ore;
        public MinMaxRange VeinSize;
        public MinMaxRange VeinAmount;
    }
}