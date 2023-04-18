using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Final
{
    // Class for basic attributes of the overall environment/world of the "game"
    internal class Environment
    {
        // Fields
        public List<RigidBody> bodyList;
        private Vector gravity;

        // List for all the points of contacts
        private List<CollisionsInformation> contacts;
        public List<Vector> contact_points;

        // Arrays used for collision response
        private Vector[] contact_points_array;
        private Vector[] impulses;
        private Vector[] impulses_friction;
        private Vector[] rs_1;
        private Vector[] rs_2;
        private float[] j_previous;

        // Constructor
        public Environment()
        {
            // Initialize fields
            gravity = new Vector(0f, -9.81f);
            bodyList = new List<RigidBody>();
            contacts = new List<CollisionsInformation>();
            contact_points = new List<Vector>();
            contact_points_array = new Vector[2];
            impulses = new Vector[2];
            impulses_friction = new Vector[2];
            rs_1 = new Vector[2];
            rs_2 = new Vector[2];
            j_previous = new float[2];
    }

        // Helper methods
        public void AddRigidBody(RigidBody body)
        {
            bodyList.Add(body);
        }

        public void RemoveRigidBody(RigidBody body)
        {
            bodyList.Remove(body);
        }
        public int GetRigidBodyCount()
        {
            return bodyList.Count;
        }

        // To update the simulation correctly
        public void Step(float time, int iterations)
        {  
            // Do multiple subset per time step to have higher precisions
            for (int k = 0; k < iterations; k++)
            {
                // Clear the lists
                contacts.Clear();
                contact_points.Clear();

                // Movement Step
                for (int i = 0; i < bodyList.Count; i++)
                {
                    bodyList[i].MoveStep(time, gravity, iterations);
                }

                // Collision Detection
                for (int i = 0; i < bodyList.Count; i++)
                {
                    RigidBody a = bodyList[i];
                    AABB a_aabb = a.GetAABB();

                    for (int j = 0; j < bodyList.Count; j++)
                    {
                        RigidBody b = bodyList[j];
                        AABB b_aabb = b.GetAABB();

                        // Continue if its the same body or if both are static or if both AABB are intersecting (for performance)
                        if ((a == b) || (a.isStatic && b.isStatic) || !CollisionDetection.CollisionAABB(a_aabb, b_aabb))
                        {
                            continue;
                        }

                        // Check if there is an actual collisions between a and b
                        if (CollisionDetection.IsCollision(a, b, out Vector normal, out float depth))
                        {
                            // Check if one of the objects is static
                            if (a.isStatic)
                            {
                                // Move just b
                                b.Move(normal * depth);
                            }

                            else if (b.isStatic)
                            {
                                // Move just a
                                a.Move(-normal * depth);
                            }

                            else
                            {
                                // Move the 2 bodies apart in opposit direction by half of the depth collision
                                a.Move(-normal * depth / 2f);
                                b.Move(normal * depth / 2f);
                            }

                            // Find the contact point(s)
                            CollisionDetection.ContactPoints(a, b, out Vector contact1, out Vector contact2, out int number_of_contacts);

                            // Store all information of the collision
                            CollisionsInformation collision = new CollisionsInformation(a, b, normal, depth, contact1, contact2, number_of_contacts);
                            contacts.Add(collision);
                        }
                    }
                }

                // Resolve all the collisions
                for (int i = 0; i < contacts.Count; i++)
                {
                    CollisionsInformation contact = contacts[i];
                    CollisionResponse(contact);

                    // Make sure the contact point is not already in the list
                    if (k == iterations - 1)
                    {
                        if (!contact_points.Contains(contact.point_of_contact1))
                        {
                            contact_points.Add(contact.point_of_contact1);
                        }

                        if (contact.point_of_contact_count > 1)
                        {
                            if (!contact_points.Contains(contact.point_of_contact2))
                            {
                                contact_points.Add(contact.point_of_contact2);
                            }
                        }
                    }
                }
            }
        }

        // Resolve the collisions by applying impulses
        public void CollisionResponse(CollisionsInformation collision_info)
        {
            // Reset values inside the arrays for collision response
            impulses[0] = Vector.vec_ZERO;
            impulses[1] = Vector.vec_ZERO;
            impulses_friction[0] = Vector.vec_ZERO;
            impulses_friction[1] = Vector.vec_ZERO;
            rs_1[0] = Vector.vec_ZERO;
            rs_1[1] = Vector.vec_ZERO;
            rs_2[0] = Vector.vec_ZERO;
            rs_2[1] = Vector.vec_ZERO;
            j_previous[0] = 0f;
            j_previous[1] = 0f;

            // Get the 2 bodies currently colliding and the normal
            RigidBody a = collision_info.body1;
            RigidBody b = collision_info.body2;
            Vector normal = collision_info.normal;

            // Get the point of contacts and how many there are
            Vector contact1 = collision_info.point_of_contact1;
            Vector contact2 = collision_info.point_of_contact2;
            int number_of_contacts = collision_info.point_of_contact_count;

            // Put the contact points in corresponding array so we can loop through them
            contact_points_array[0] = contact1;
            contact_points_array[1] = contact2;

            // Restitution (take the smallest one)
            float e = MathF.Min(a.getRestitution(), b.getRestitution());

            // get frictions values by taking the average of both frictions for body a and body b
            float friction_static = (a.friction_static + b.friction_static) / 2f;
            float friction_moving = (a.friction_moving + b.friction_moving) / 2f;

            /* Apply the impulses along the normal direction */

            // Loop through all contact points
            for (int i = 0; i < number_of_contacts; i++)
            {
                // Get "r" values from the center of the object pointing to the point of collision
                Vector r1 = contact_points_array[i] - a.getPosition();
                Vector r2 = contact_points_array[i] - b.getPosition();

                // Get the perpendicular vector to those "r" values
                Vector r1_perpendicular = new Vector(-r1.y, r1.x);
                Vector r2_perpendicular = new Vector(-r2.y, r2.x);

                // Get the current angular velocity for each body
                Vector angular_velocity_a = r1_perpendicular * a.rot_vel;
                Vector angular_velocity_b = r2_perpendicular * b.rot_vel;

                // Relative Velocity
                Vector relative_vel = (b.GetLinearVelocity() + angular_velocity_b) - (a.GetLinearVelocity() + angular_velocity_a);

                // Check if the 2 objects are already moving apart
                if (MyMathHelper.Dot(relative_vel, normal) > 0f)
                {
                    continue;
                }

                // Pre calculate the dot products that we will need inside the impulse calculation
                float r1_perpendicular_dot_n = MyMathHelper.Dot(r1_perpendicular, normal);
                float r2_perpendicular_dot_n = MyMathHelper.Dot(r2_perpendicular, normal);

                // Calculate impulse in vector form
                float j = -(1f + e) * MyMathHelper.Dot(relative_vel, normal);       // Numerator
                j /= ((a.inv_mass + b.inv_mass) + ((r1_perpendicular_dot_n * r1_perpendicular_dot_n) * a.inv_inertia) + ((r2_perpendicular_dot_n * r2_perpendicular_dot_n) * b.inv_inertia));
                j /= (float)number_of_contacts;
                Vector impulse = j * normal;

                // Save impulse
                j_previous[i] = j;

                // Save the impulse and "r" value to corresponding array
                impulses[i] = impulse;
                rs_1[i] = r1;
                rs_2[i] = r2;
            }

            // Apply the impulses
            for (int i = 0; i < number_of_contacts; i++)
            {
                // Get current impulse
                Vector impulse = impulses[i];

                // Update the linear velocities
                a.linear_vel -= impulse * a.inv_mass;
                b.linear_vel += impulse * b.inv_mass;

                // Update angular velocities
                a.rot_vel -= MyMathHelper.Cross(rs_1[i], impulse) * a.inv_inertia;
                b.rot_vel += MyMathHelper.Cross(rs_2[i], impulse) * b.inv_inertia;
            }

            /* Redo the last 2 loops but by applying the impulses along the perpendicular to the normal and opposite direction to relative velocity to simulate friction impulses */

            // Loop through all contact points
            for (int i = 0; i < number_of_contacts; i++)
            {
                // Get "r" values from the center of the object pointing to the point of collision
                Vector r1 = contact_points_array[i] - a.getPosition();
                Vector r2 = contact_points_array[i] - b.getPosition();

                // Get the perpendicular vector to those "r" values
                Vector r1_perpendicular = new Vector(-r1.y, r1.x);
                Vector r2_perpendicular = new Vector(-r2.y, r2.x);

                // Get the current angular velocity for each body
                Vector angular_velocity_a = r1_perpendicular * a.rot_vel;
                Vector angular_velocity_b = r2_perpendicular * b.rot_vel;

                // Relative Velocity
                Vector relative_vel = (b.GetLinearVelocity() + angular_velocity_b) - (a.GetLinearVelocity() + angular_velocity_a);

                // Get the perpendicular vector to the normal and opposite direction of the relative velocity
                Vector frition_vec = relative_vel - MyMathHelper.Dot(relative_vel, normal) * normal;

                // Check if this vector is basically 0, if it is we don't need to go any further
                if (MyMathHelper.EqualVector(frition_vec, Vector.vec_ZERO))
                {
                    continue;
                }

                else
                {
                    // Normalize it
                    frition_vec = MyMathHelper.Normalize(frition_vec);
                }

                // Pre calculate the dot products that we will need inside the impulse calculation
                float r1_perpendicular_dot_fv = MyMathHelper.Dot(r1_perpendicular, frition_vec);
                float r2_perpendicular_dot_fv = MyMathHelper.Dot(r2_perpendicular, frition_vec);

                // Calculate impulse in vector form
                float j = -MyMathHelper.Dot(relative_vel, frition_vec);       // Numerator
                j /= ((a.inv_mass + b.inv_mass) + ((r1_perpendicular_dot_fv * r1_perpendicular_dot_fv) * a.inv_inertia) + ((r2_perpendicular_dot_fv * r2_perpendicular_dot_fv) * b.inv_inertia));
                j /= (float)number_of_contacts;
                Vector impulse = Vector.vec_ZERO;

                // Apply Coulomb's Law
                if (Math.Abs(j) <= (j_previous[i] * friction_static))
                {
                    impulse = j * frition_vec;
                }

                else
                {
                    impulse = -j_previous[i] * frition_vec * friction_moving;
                }

                // Save the impulse and "r" value to corresponding array
                impulses_friction[i] = impulse;
                rs_1[i] = r1;
                rs_2[i] = r2;
            }

            // Apply the impulses
            for (int i = 0; i < number_of_contacts; i++)
            {
                // Get current impulse
                Vector impulse = impulses_friction[i];

                // Update the linear velocities
                a.linear_vel -= impulse * a.inv_mass;
                b.linear_vel += impulse * b.inv_mass;

                // Update angular velocities
                a.rot_vel -= MyMathHelper.Cross(rs_1[i], impulse) * a.inv_inertia;
                b.rot_vel += MyMathHelper.Cross(rs_2[i], impulse) * b.inv_inertia;
            }
        }

        // Getter functions
        public void GetRigidBody(int i, out RigidBody body)
        {
            // Initialize out variables
            body = null;

            // Return if it is impossible
            if (i < 0 || i >= bodyList.Count)
            {
                return;
            }

            // Else, return the body
            body = bodyList[i];
        }
    }
}
