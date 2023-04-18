using System;
using Microsoft.Xna.Framework;

namespace Library
{
    public readonly struct Box
    {
        public readonly Vector2 Min;
        public readonly Vector2 Max;

        public static readonly Box Empty = new Box(Vector2.Zero, Vector2.Zero);

        public Box(Vector2 min, Vector2 max)
        {
            this.Min = min;
            this.Max = max;
        }

        public Box(Vector2 center, float width, float height)
        {
            float left = center.X - width * 0.5f;
            float right = left + width;
            float bottom = center.Y - height * 0.5f;
            float top = bottom + height;

            this.Min = new Vector2(left, bottom);
            this.Max = new Vector2(right, top);
        }

        public Box(float minX, float maxX, float minY, float maxY)
        {
            this.Min = new Vector2(minX, minY);
            this.Max = new Vector2(maxX, maxY);
        }
    }
}
