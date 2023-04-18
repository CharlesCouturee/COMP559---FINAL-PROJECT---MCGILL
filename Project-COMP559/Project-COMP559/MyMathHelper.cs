using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Final
{
    internal class MyMathHelper
    {
        // Calculate the length of a Vector
        public static float LengthOfVector(Vector vec)
        {
            return MathF.Sqrt(vec.x * vec.x + vec.y * vec.y);
        }

        // Length squared of a Vector
        public static float LengthSquaredOfVector(Vector vec)
        {
            return vec.x * vec.x + vec.y * vec.y;
        }

        // Calculate the distance betrween 2 Vectors
        public static float DistanceBetweenVectors(Vector vec1, Vector vec2)
        {
            // Get dx and dy
            float dx = vec1.x - vec2.x;
            float dy = vec1.y - vec2.y;

            return MathF.Sqrt(dx * dx + dy * dy);
        }

        public static float DistanceSquaredBetweenVectors(Vector vec1, Vector vec2)
        {
            // Get dx and dy
            float dx = vec1.x - vec2.x;
            float dy = vec1.y - vec2.y;

            return dx * dx + dy * dy;
        }

        // Normalize the given Vector
        public static Vector Normalize(Vector vec)
        {
            float length = MyMathHelper.LengthOfVector(vec);

            float x = vec.x / length;
            float y = vec.y / length;

            return new Vector(x, y);
        }

        // Dot product between 2 Vectors
        public static float Dot(Vector vec1, Vector vec2)
        {
            return vec1.x * vec2.x + vec1.y * vec2.y;
        }

        // Cross product between 2 Vectors
        public static float Cross(Vector vec1, Vector vec2)
        {
            // x and y component of the cross product is 0 because we are in 2D
            return vec1.x * vec2.y - vec1.y * vec2.x;
        }

        // If 2 floating point values are basically equal (not exactly)
        public static bool EquaFloat(float a, float b)
        {
            return MathF.Abs(a - b) < 0.0005f;
        }

        // If 2 Vectors are basically equal (not exactly)
        public static bool EqualVector(Vector a, Vector b)
        {
            return MyMathHelper.EquaFloat(a.x, b.x) && MyMathHelper.EquaFloat(a.y, b.y);
        }
    }
}
