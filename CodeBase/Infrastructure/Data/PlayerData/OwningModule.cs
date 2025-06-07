using System;

namespace Assets.CodeBase.Infrastructure.Data.PlayerData
{
    [Serializable]
    public class OwningModule
    {
        public string Id;

        public int Level = 0;

        public OwningModule(string id, int level)
        {
            Id = id;
            Level = level;
        }
    }
}