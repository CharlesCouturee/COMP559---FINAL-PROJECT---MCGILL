using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Final
{
    // Struct that will keep all necessary information of a collision between 2 bodies
    internal struct CollisionsInformation
    {
        // Fields
        public RigidBody body1;
        public RigidBody body2;

        public Vector normal;
        public float depth;

        public Vector point_of_contact1;
        public Vector point_of_contact2;
        public int point_of_contact_count;

        // Constructor
        public CollisionsInformation(RigidBody a, RigidBody b, Vector n, float d, Vector poc1, Vector poc2, int pocc)
        {
            // Initialization
            body1 = a;
            body2 = b;
            normal = n;
            depth = d;
            point_of_contact1 = poc1;
            point_of_contact2 = poc2;
            point_of_contact_count = pocc;
        }
    }
}
