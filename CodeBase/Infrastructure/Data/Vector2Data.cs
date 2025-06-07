using System;

namespace Assets.CodeBase.Infrastructure.Data
{
    [Serializable]
    public class Vector2Data
    {
        public Vector2Data(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X;
        public float Y;
    }
}