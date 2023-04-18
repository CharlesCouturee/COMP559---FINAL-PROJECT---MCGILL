using System;
using Microsoft.Xna.Framework;

namespace Library
{
    public readonly struct Transform
    {
        internal readonly float PositionX;
        internal readonly float PositionY;
        internal readonly float CosScaleX;
        internal readonly float SinScaleX;
        internal readonly float CosScaleY;
        internal readonly float SinScaleY;

        private readonly float ScaleX;
        private readonly float ScaleY;
        private readonly float angle;

        public static readonly Transform Identity = new Transform(Vector2.Zero, 0f, Vector2.One);

        public static readonly int SizeInBytes = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Transform));

        public Transform(Vector2 position, float angle, Vector2 scale)
        {
            this.ScaleX = scale.X;
            this.ScaleY = scale.Y;
            this.angle = angle;

            float sin = MathF.Sin(angle);
            float cos = MathF.Cos(angle);

            this.PositionX = position.X;
            this.PositionY = position.Y;
            this.CosScaleX = cos * scale.X;
            this.SinScaleX = sin * scale.X;
            this.CosScaleY = cos * scale.Y;
            this.SinScaleY = sin * scale.Y;
        }

        public Transform(Vector2 position, float angle, float scale)
        {
            this.ScaleX = scale;
            this.ScaleY = scale;
            this.angle = angle;

            float sin = MathF.Sin(angle);
            float cos = MathF.Cos(angle);

            this.PositionX = position.X;
            this.PositionY = position.Y;
            this.CosScaleX = cos * scale;
            this.SinScaleX = sin * scale;
            this.CosScaleY = cos * scale;
            this.SinScaleY = sin * scale;
        }

        public Vector2 GetScale()
        {
            return new Vector2(this.ScaleX, this.ScaleY);
        }

        public float GetRotation()
        {
            return this.angle;
        }

        public Vector2 GetTranslation()
        {
            return new Vector2(this.PositionX, this.PositionY);
        }

        public Matrix ToMatrix()
        {
            Matrix result = Matrix.Identity;
            result.M11 = this.CosScaleX;
            result.M12 = this.SinScaleX;
            result.M21 = -this.SinScaleY;
            result.M22 = this.CosScaleY;
            result.M41 = this.PositionX;
            result.M42 = this.PositionY;

            return result;
        }

        public bool Equals(Transform other)
        {
            return this.PositionX == other.PositionX &&
                this.PositionY == other.PositionY &&
                this.CosScaleX == other.CosScaleX &&
                this.SinScaleX == other.SinScaleX &&
                this.CosScaleY == other.CosScaleY &&
                this.SinScaleY == other.SinScaleY;
        }
    }
}
