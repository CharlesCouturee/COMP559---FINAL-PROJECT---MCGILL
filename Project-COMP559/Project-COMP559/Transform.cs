using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Final
{
    internal struct Transform
    {
        // Fields
        public float posX;
        public float posY;
        public float cos;
        public float sin;

        // Create a basic Transform object with everything set at 0
        public static Transform transform_ZERO = new Transform(0f, 0f, 0f);

        public Transform(Vector position, float angle)
        {
            // Store the position
            posX = position.x;
            posY = position.y;

            // Get the properties of the angle
            cos = MathF.Cos(angle);
            sin = MathF.Sin(angle); 
        }

        public Transform(float x, float y, float angle)
        {
            // Store the position
            posX = x;
            posY = y;

            // Get the properties of the angle
            cos = MathF.Cos(angle);
            sin = MathF.Sin(angle);
        }
    }
}
