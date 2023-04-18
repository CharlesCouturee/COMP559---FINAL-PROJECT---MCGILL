using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Project_Final
{
    internal class CollisionDetection
    {
        // Check for collisions given two RigidBody
        public static bool IsCollision(RigidBody body1, RigidBody body2, out Vector n, out float depth)
        {
            // Initialize values for the two out variables
            n = Vector.vec_ZERO;
            depth = 0f;

            // Get the type of shape for each body
            TypeOfShape shape1 = body1.GetTypeOfShape();
            TypeOfShape shape2 = body2.GetTypeOfShape();

            // Check for all cases of collisions in our environment
            if (shape1 is TypeOfShape.Square)
            {
                // shape1 = square      shape2 = square
                if (shape2 is TypeOfShape.Square)
                {
                    return CollisionDetection.CollisionPolygone(body1.getPosition(), body1.GetTransformedVertices(),
                        body2.getPosition(), body2.GetTransformedVertices(), out n, out depth);
                }

                // shape1 = square      shape2 = circle
                else if (shape2 is TypeOfShape.Circle)
                {
                    bool result = CollisionDetection.CollisionsPolygonsCircles(body2.getPosition(), body2.getRadius(), body1.getPosition(), body1.GetTransformedVertices(), out n, out depth);

                    // Reverse normal so the bodies are pushed apart correctly
                    n = -n;

                    // return result after
                    return result;
                }
            }

            else if (shape1 is TypeOfShape.Circle)
            {
                // shape1 = circle      shape2 = square
                if (shape2 is TypeOfShape.Square)
                {
                    return CollisionDetection.CollisionsPolygonsCircles(body1.getPosition(), body1.getRadius(), body2.getPosition(), body2.GetTransformedVertices(), out n, out depth);
                }

                // shape1 = circle      shape2 = circle
                else if (shape2 is TypeOfShape.Circle)
                {
                    return CollisionDetection.CollisionCircle(body1.getPosition(), body1.getRadius(), body2.getPosition(), body2.getRadius(), out n, out depth);
                }
            }

            // If we get there then there was no collisions
            return false;
        }
        // Collisions between circles
        public static bool CollisionCircle(Vector c1, float r1, Vector c2, float r2, out Vector n, out float depth)
        {
            // Initialize out values to null
            n = Vector.vec_ZERO;
            depth = 0f;

            float dist = Vector.DistanceBetweenVectors(c1, c2);
            float sum_of_radius = r1 + r2;

            // If there is no collision
            if (dist >= sum_of_radius)
            {
                return false;
            }

            /* THERE IS A COLLISION */

            // Normal c1->c2
            n = Vector.Normalize(c2 - c1);

            // How much depth we need to move apart
            depth = sum_of_radius - dist;

            return true;
        }

        // Collision between polygons (Separating Axis Theorem)
        public static bool CollisionPolygone(Vector center1, Vector[] vertices1, Vector center2, Vector[] vertices2, out Vector n, out float depth)
        {
            // Initialize out variables
            n = Vector.vec_ZERO;
            depth = float.MaxValue;

            // Loop through vertices of the first polygon
            for (int i = 0; i < vertices1.Length; i++)
            {
                // Take the vertices of the triangle
                Vector v1 = vertices1[i];
                Vector v2 = Vector.vec_ZERO;

                if (i == vertices1.Length - 1)
                {
                    v2 = vertices1[0];
                }

                else
                {
                    v2 = vertices1[i + 1]
;
                }

                // Create an edge based on thos vertices
                Vector edge = v2 - v1;

                // Normal of the edge (axis we will use to test separation)
                Vector axis = new Vector(-edge.y, edge.x);
                axis = MyMathHelper.Normalize(axis);

                // Project
                CollisionDetection.ProjectSquare(vertices1, axis, out float min1, out float max1);
                CollisionDetection.ProjectSquare(vertices2, axis, out float min2, out float max2);

                // Check if the min of the first polygon is greater than the max of the second polygon
                if (min1 >= max2 || min2 >= max1)
                {
                    // They are separated
                    return false;
                }

                // Get the depth of the collision on the current axis
                float depth_on_axis = MathF.Min(max2 - min1, max1 - min2);

                // Keep the smallest depth and save normal
                if (depth_on_axis < depth)
                {
                    depth = depth_on_axis;
                    n = axis;
                }
            }

            // Loop through vertices of second polygon
            for (int i = 0; i < vertices2.Length; i++)
            {
                // Take the vertices of the triangle
                Vector v1 = vertices2[i];
                Vector v2 = Vector.vec_ZERO;

                if (i == vertices2.Length - 1)
                {
                    v2 = vertices2[0];
                }

                else
                {
                    v2 = vertices2[i + 1]
;
                }

                // Create an edge based on thos vertices
                Vector edge = v2 - v1;

                // Normal of the edge (axis we will use to test separation)
                Vector axis = new Vector(-edge.y, edge.x);
                axis = MyMathHelper.Normalize(axis);

                // Project
                CollisionDetection.ProjectSquare(vertices1, axis, out float min1, out float max1);
                CollisionDetection.ProjectSquare(vertices2, axis, out float min2, out float max2);

                // Check if the min of the first polygon is greater than the max of the second polygon
                if (min1 >= max2 || min2 >= max1)
                {
                    // They are separated
                    return false;
                }

                // Get the depth of the collision on the current axis
                float depth_on_axis = MathF.Min(max2 - min1, max1 - min2);

                // Keep the smallest depth and save normal
                if (depth_on_axis < depth)
                {
                    depth = depth_on_axis;
                    n = axis;
                }
            }

            // Get vector direction from first polygon to second polygon
            Vector dir = center2 - center1;

            // Check if direction is correct (from 1->2)
            if (MyMathHelper.Dot(dir, n) < 0f)
            {
                n = -n;
            }

            return true;

        }

        // Collision between a circle and a square
        public static bool CollisionsPolygonsCircles(Vector c_center, float c_radius, Vector poly_center, Vector[] vertices, out Vector n, out float depth)
        {
            // Initialize out variables
            n = Vector.vec_ZERO;
            depth = float.MaxValue;
            Vector axis = Vector.vec_ZERO;
            float depth_on_axis = 0f;
            float min1 = 0f;
            float max1 = 0f;
            float min2 = 0f;
            float max2 = 0f;

            // Loop through vertices of the first polygon
            for (int i = 0; i < vertices.Length; i++)
            {
                // Take the vertices of the triangle
                Vector v1 = vertices[i];
                Vector v2 = Vector.vec_ZERO;

                if (i == vertices.Length - 1)
                {
                    v2 = vertices[0];
                }

                else
                {
                    v2 = vertices[i + 1]
;
                }

                // Create an edge based on thos vertices
                Vector edge = v2 - v1;

                // Normal of the edge (axis we will use to test separation)
                axis = new Vector(-edge.y, edge.x);
                axis = MyMathHelper.Normalize(axis);

                // Project
                CollisionDetection.ProjectSquare(vertices, axis, out min1, out max1);
                CollisionDetection.ProjectCircle(c_center, c_radius, axis, out min2, out max2);

                // Check if the min of the first polygon is greater than the max of the second polygon
                if (min1 >= max2 || min2 >= max1)
                {
                    // They are separated
                    return false;
                }

                // Get the depth of the collision on the current axis
                depth_on_axis = MathF.Min(max2 - min1, max1 - min2);

                // Keep the smallest depth and save normal
                if (depth_on_axis < depth)
                {
                    depth = depth_on_axis;
                    n = axis;
                }
            }

            // Find index of the closest point and the actual vector corresponding to the index

            int closest_point_index = CollisionDetection.ClosestPoint(c_center, vertices);
            Vector closest_pont = vertices[closest_point_index];

            // Get the axis
            axis = closest_pont - c_center;
            axis = MyMathHelper.Normalize(axis);

            // Project
            CollisionDetection.ProjectSquare(vertices, axis, out min1, out max1);
            CollisionDetection.ProjectCircle(c_center, c_radius, axis, out min2, out max2);

            // Check if the min of the first polygon is greater than the max of the second polygon
            if (min1 >= max2 || min2 >= max1)
            {
                // They are separated
                return false;
            }

            // Get the depth of the collision on the current axis
            depth_on_axis = MathF.Min(max2 - min1, max1 - min2);

            // Keep the smallest depth and save normal
            if (depth_on_axis < depth)
            {
                depth = depth_on_axis;
                n = axis;
            }

            // Get vector direction from first polygon to second polygon
            Vector dir = poly_center - c_center;

            // Check if direction is correct (from polygon->circle)
            if (MyMathHelper.Dot(dir, n) < 0f)
            {
                n = -n;
            }

            return true;
        }

        // Helper function to find the closest point to the polygon
        private static int ClosestPoint(Vector c_center, Vector[] vertices)
        {
            // Initialize variables
            int result = -1;
            float minD = float.MaxValue;

            // Check for distance betweem center of circles and all the vertices
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector v = vertices[i];
                float dist = MyMathHelper.DistanceBetweenVectors(v, c_center);

                // Save the min distance
                if (dist < minD)
                {
                    minD = dist;
                    result = i;
                }
            }
 
            return result;
        }

        // Helper function to get the min and max projection
        private static void ProjectSquare(Vector[] vertices, Vector axis, out float min_projection, out float max_projection)
        {
            // Set default values for min and max projection
            min_projection = float.MaxValue;
            max_projection = float.MinValue;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector v = vertices[i];

                // Project the vertix on the axis
                float projection = MyMathHelper.Dot(v, axis);

                // Check if projection is smaller than min or greater than max
                if (projection < min_projection)
                {
                    min_projection = projection;
                }

                if (projection > max_projection)
                {
                    max_projection = projection;
                }
            }
        }

        public static void ProjectCircle(Vector center, float radius, Vector axis, out float min_projection, out float max_projection)
        {
            // Get the direction 
            Vector dir = MyMathHelper.Normalize(axis);

            // Get the amount of distance we need to move towards the direction to get to the edge of the circle
            Vector distance_in_dir = dir * radius;

            // Get the 2 points that are on the edge of the circle according to direction vector
            Vector p1 = center + distance_in_dir;
            Vector p2 = center - distance_in_dir;

            // Project these points on the axis
            min_projection = MyMathHelper.Dot(p1, axis);
            max_projection = MyMathHelper.Dot(p2, axis);

            // Check to be sure that min and max values are correct
            if (min_projection > max_projection)
            {
                float temp = min_projection;
                min_projection = max_projection;
                max_projection = temp;
            }
        }

        // Get contact points of a collision
        public static void ContactPoints(RigidBody a, RigidBody b, out Vector contact1, out Vector contact2, out int number_of_contacts)
        {
            // Default values for out variables
            contact1 = Vector.vec_ZERO;
            contact2 = Vector.vec_ZERO;
            number_of_contacts = 0;

            // Get the type of shape for each body
            TypeOfShape shape1 = a.GetTypeOfShape();
            TypeOfShape shape2 = b.GetTypeOfShape();

            // Check for all cases of collisions in our environment and get the contact points
            if (shape1 is TypeOfShape.Square)
            {
                // shape1 = square      shape2 = square
                if (shape2 is TypeOfShape.Square)
                {
                    CollisionDetection.ContactPointSquares(a.GetTransformedVertices(), b.GetTransformedVertices(), out contact1, out contact2, out number_of_contacts);
                }

                // shape1 = square      shape2 = circle
                else if (shape2 is TypeOfShape.Circle)
                {
                    CollisionDetection.ContactPointCircleSquare(b.getPosition(), b.getRadius(), a.getPosition(), a.GetTransformedVertices(), out contact1);
                    number_of_contacts = 1;
                }
            }

            else if (shape1 is TypeOfShape.Circle)
            {
                // shape1 = circle      shape2 = square
                if (shape2 is TypeOfShape.Square)
                {
                    CollisionDetection.ContactPointCircleSquare(a.getPosition(), a.getRadius(), b.getPosition(), b.GetTransformedVertices(), out contact1);
                    number_of_contacts = 1;
                }

                // shape1 = circle      shape2 = circle
                else if (shape2 is TypeOfShape.Circle)
                {
                    CollisionDetection.ContactPointCircles(a.getPosition(), a.getRadius(), b.getPosition(), out contact1);
                    number_of_contacts = 1;
                }
            }
        }

        // Collision point between 2 squares
        public static void ContactPointSquares(Vector[] vertices_a, Vector[] vertices_b, out Vector contact1, out Vector contact2, out int number_of_contacts)
        {
            // Default values for out variables
            contact1 = Vector.vec_ZERO;
            contact2 = Vector.vec_ZERO;
            number_of_contacts = 0;

            // Variable for the current minimum distance that we found
            float minimum_distance_squared = float.MaxValue;

            // Loop through all vertices in a and compare with edges of b
            for (int i = 0; i < vertices_a.Length; i++)
            {
                // Get the point
                Vector point = vertices_a[i];

                for (int j = 0; j < vertices_b.Length; j++)
                {
                    // Get edges
                    Vector v1 = vertices_b[j];
                    Vector v2 = Vector.vec_ZERO;

                    if (j == vertices_b.Length - 1)
                    {
                        v2 = vertices_b[0];
                    }

                    else
                    {
                        v2 = vertices_b[j + 1];
                    }

                    // Find closest point between a point and a line segment
                    CollisionDetection.PointToSegmentDistance(point, v1, v2, out float distance_squared, out Vector closest_point);

                    // Check if minimum distance found is actually equal to the current minimum distance squared
                    if (MyMathHelper.EquaFloat(distance_squared, minimum_distance_squared))
                    {
                        // Make sure that this new contact point is not equal to contact1 (almost equal because of floating point accuracy)
                        if (!MyMathHelper.EqualVector(closest_point, contact1))
                        {
                            // We just found another contact points 
                            contact1 = closest_point;
                            number_of_contacts = 2;
                        }
                    }

                    // Checks if distance_squared is smaller than current minimum distance squared
                    else if (distance_squared < minimum_distance_squared)
                    {
                        minimum_distance_squared = distance_squared;

                        // Save the first point of contact we just found
                        number_of_contacts = 1;
                        contact1 = closest_point; contact2 = closest_point;
                    }
                }
            }

            // Loop through all vertices in b and compare with edges of a
            for (int i = 0; i < vertices_b.Length; i++)
            {
                // Get the point
                Vector point = vertices_b[i];

                for (int j = 0; j < vertices_a.Length; j++)
                {
                    // Get edges
                    Vector v1 = vertices_a[j];
                    Vector v2 = Vector.vec_ZERO;

                    if (j == vertices_a.Length - 1)
                    {
                        v2 = vertices_a[0];
                    }

                    else
                    {
                        v2 = vertices_a[j + 1];
                    }

                    // Find closest point between a point and a line segment
                    CollisionDetection.PointToSegmentDistance(point, v1, v2, out float distance_squared, out Vector closest_point);

                    // Check if minimum distance found is actually equal to the current minimum distance squared
                    if (MyMathHelper.EquaFloat(distance_squared, minimum_distance_squared))
                    {
                        // Make sure that this new contact point is not equal to contact1 (almost equal because of floating point accuracy)
                        if (!MyMathHelper.EqualVector(closest_point, contact1))
                        {
                            // We just found another contact points 
                            contact1 = closest_point;
                            number_of_contacts = 2;
                        }
                    }

                    // Checks if distance_squared is smaller than current minimum distance squared
                    else if (distance_squared < minimum_distance_squared)
                    {
                        minimum_distance_squared = distance_squared;

                        // Save the first point of contact we just found
                        number_of_contacts = 1;
                        contact1 = closest_point; contact2 = closest_point;
                    }
                }
            }
        }

        // Collision point between square and circle
        public static void ContactPointCircleSquare(Vector c_center, float c_radius, Vector poly_center, Vector[] poly_vertices, out Vector contact_point)
        {
            // Initialize the minimum distance squared and contact_point
            float min_distance_squared = float.MaxValue;
            contact_point = Vector.vec_ZERO;

            // Loop through every edge of the polygon
            for (int i = 0; i < poly_vertices.Length; i++)
            {
                // Gwet the 2 vertices of the edge
                Vector v1 = poly_vertices[i];
                Vector v2 = Vector.vec_ZERO;

                if (i == poly_vertices.Length - 1)
                {
                    v2 = poly_vertices[0];
                }

                else
                {
                    v2 = poly_vertices[i + 1];
                }

                // Get relative point to line segment distance
                CollisionDetection.PointToSegmentDistance(c_center, v1, v2, out float distance_squared, out Vector contact);

                // Adjust minimum distance squared variable if necesary and get contact point
                if (distance_squared < min_distance_squared)
                {
                    min_distance_squared = distance_squared;
                    contact_point = contact;
                }
            }
        }

        // Collision point of contact between 2 circles
        public static void ContactPointCircles(Vector center1, float radius1, Vector center2, out Vector contact_point)
        {
            // Find vector that points from center1 to center2 and normalize it
            Vector v12 = center2 - center1;
            v12 = MyMathHelper.Normalize(v12);

            // Determine point of contact
            contact_point = center1 + (v12 * radius1);
        }

        // Checking for "collision" between AABB
        public static bool CollisionAABB(AABB a, AABB b)
        {
            // No collision  between AABB 
            if (a.maximum.x <= b.minimum.x || b.maximum.x <= a.minimum.x || a.maximum.y <= b.minimum.y || b.maximum.y <= a.minimum.y)
            {
                return false;
            }

            // Else, there is a collision between AABB
            return true;
        }

        // Calculate relative distance from point to line segment
        public static void PointToSegmentDistance(Vector point, Vector a, Vector b, out float distance_squared, out Vector contact_point)
        {
            // Find the necessary Vectors
            Vector ab = b - a;
            Vector ap = point - a;

            // Get the projection
            float projection = MyMathHelper.Dot(ap, ab);

            // Get the length squared of vector ab
            float ab_length_squared = MyMathHelper.LengthSquaredOfVector(ab);
            float d = projection / ab_length_squared;

            if (d <= 0f)
            {
                contact_point = a;
            }

            else if (d >= 1f)
            {
                contact_point = b;
            }

            else
            {
                contact_point = a + (ab * d);
            }
            
            // Get distance squared
            distance_squared = MyMathHelper.DistanceSquaredBetweenVectors(point, contact_point);
        }
    }
}
