using System;

namespace Assets.CodeBase.Infrastructure.Data.Modules
{
    [Serializable]
    public class ModuleSerializableInfo
    {
        public string Id;
        public int Place;
        public int Level = 0;

        public ModuleSerializableInfo Copy()
        {
            ModuleSerializableInfo copy = new ModuleSerializableInfo
            {
                Id = Id,
                Place = Place,
            };
            return copy;
        }
    }
}