using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using Library;

namespace Project_Final
{
    // Struct of a Vector
    internal struct Vector
    {
        // Fields
        public float x;
        public float y;

        // The Zero Vector
        public static Vector vec_ZERO = new Vector(0f, 0f);

        // Constructor
        public Vector(float x_coord, float y_coord)
        {
            x = x_coord;
            y = y_coord;
        }

        // All possible operation we want to do on Vectors 

        // Addition
        public static Vector operator +(Vector vec1, Vector vec2)
        {
            return new Vector(vec1.x + vec2.x, vec1.y + vec2.y);
        }

        // Substraction
        public static Vector operator -(Vector vec1, Vector vec2)
        {
            return new Vector(vec1.x - vec2.x, vec1.y - vec2.y);
        }

        // Multiplication
        public static Vector operator *(Vector vec, float scalar)
        {
            return new Vector(vec.x * scalar, vec.y * scalar);
        }

        public static Vector operator *(float scalar, Vector vec)
        {
            return new Vector(vec.x * scalar, vec.y * scalar);
        }

        // Division
        public static Vector operator /(Vector vec, float scalar)
        {
            return new Vector(vec.x / scalar, vec.y / scalar);
        }

        // Change sign
        public static Vector operator -(Vector vec)
        {
            return new Vector(-vec.x, -vec.y);
        }

        // Equality
        public bool Equals(Vector vec)
        {
            return x == vec.x && y == vec.y;
        }

        // Calculate the length of a Vector
        public static float LengthOfVector(Vector vec)
        {
            return MathF.Sqrt(vec.x*vec.x + vec.y*vec.y);
        }

        // Calculate the distance betrween 2 Vectors
        public static float DistanceBetweenVectors(Vector vec1, Vector vec2)
        {
            // Get dx and dy
            float dx = vec1.x - vec2.x;
            float dy = vec1.y - vec2.y;

            return MathF.Sqrt(dx*dx + dy*dy);
        }

        // Normalize the given Vector
        public static Vector Normalize(Vector vec)
        {
            float length = Vector.LengthOfVector(vec);

            float x = vec.x / length;
            float y = vec.y / length;

            return new Vector(x, y);
        }

        // Dot product between 2 Vectors
        public static float Dot(Vector vec1, Vector vec2)
        {
            return vec1.x*vec2.x + vec1.y*vec2.y;
        }

        // Cross product between 2 Vectors
        public static float Cross(Vector vec1, Vector vec2)
        {
            // x and y component of the cross product is 0 because we are in 2D
            return vec1.x * vec2.y - vec1.y * vec2.x;
        }

        // Take a struct Vector and return a Vector2
        public static Vector2 ToVector2(Vector vec)
        {
            return new Vector2(vec.x, vec.y);
        }

        // Take a Vector2 and return a struct Vector
        public static Vector ToVector(Vector2 vec)
        {
            return new Vector(vec.X, vec.Y);
        }

        // Take a Vector[] and convert it to a Vector2[] in reference
        public static void ToVector2Array(Vector[] source, ref Vector2[] destination)
        {
            // Safety check
            if (destination is null || source.Length != destination.Length)
            {
                destination = new Vector2[source.Length];
            }

            // Copy the source array into the destination array but in Vector2
            for (int i = 0; i < source.Length; i++)
            {
                Vector vec = source[i];
                destination[i] = new Vector2(vec.x, vec.y);
            }
        }

        // Static function to transform vertices
        public static Vector TransformVector(Vector v, Transform t)
        {
            // Get the 2D rotation "vector"
            float rotation_x = t.cos * v.x - t.sin * v.y;
            float rotation_y = t.sin * v.x + t.cos * v.y;

            // Get the 2D translation "vector"
            float translation_x = rotation_x + t.posX;
            float translation_y = rotation_y + t.posY;

            return new Vector(translation_x, translation_y);
        }

        // Override the basic Equals function to handle equality between vectors
        public override bool Equals(object obj)
        {
            if (obj is Vector vec)
            {
                return Equals(vec);
            }

            return false;
        }
    }
}
