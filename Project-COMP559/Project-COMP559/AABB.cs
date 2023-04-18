using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Final
{
    internal class AABB
    {
        // Fields
        public Vector minimum;
        public Vector maximum;

        // Constructors
        public AABB(Vector min, Vector max)
        {
            minimum = min;
            maximum = max;
        }

        public AABB(float min_x, float min_y, float max_x, float max_y)
        {
            minimum = new Vector(min_x, min_y);
            maximum = new Vector(max_x, max_y);
        }
    }
}
