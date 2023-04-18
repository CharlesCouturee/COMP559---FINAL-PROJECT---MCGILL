using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Final
{
    public enum TypeOfShape
    {
        Circle = 0,
        Square = 1
    }

    internal class RigidBody
    {
        // General Fields
        private Vector pos;
        public Vector linear_vel;
        private float density;
        private float mass;
        public float inv_mass;
        private float restitution;
        private float area;
        public float rot; // angle
        public float rot_vel; // angular velocity
        public bool isStatic;

        // Fields for TypeOfShape
        private float r; // Radius
        private float w; // Width
        private float h; // Height
        private TypeOfShape tos; // Type

        // Fields for the vertices, the Transformed vertices and vertices for TrianglesIndices
        private Vector[] vertices;
        private Vector[] verticesTransformed;
        private int[] trianglesIndices;
        private bool updateRequiredTransformed;
        private bool updateRequiredAABB;

        // Fields a bit more about physics stuff
        private Vector force;
        private AABB aabb;
        public float inertia;
        public float inv_inertia;
        public float friction_static;
        public float friction_moving;

        // Constructor
        private RigidBody(Vector p, float d, float m, float rest, float a, bool isStat, float radius, float width, float height, TypeOfShape body)
        {
            // Initialize those fields to 0 initially
            linear_vel = Vector.vec_ZERO;
            rot = 0f;
            rot_vel = 0f;

            // Pass the other valuyes to the corresponding field
            pos = p;
            density = d;
            mass = m;
            restitution = rest;
            area = a;
            isStatic = isStat;
            r = radius; 
            w = width; 
            h = height;
            tos = body;
            force = Vector.vec_ZERO;
            inertia = RotationalInertia();
            friction_static = 0.5f;
            friction_moving = 0.3f;


            // Inverse mass and inertia
            if (!isStatic)
            {
                inv_mass = 1f / mass;
                inv_inertia = 1f / inertia;
            }

            else
            {
                // No mass or inertia if the object is static
                inv_mass = 0f;
                inv_inertia = 0f;
            }

            // Create array of vertices depending on the RigidBody shape

            // Sqaure
            if (tos is TypeOfShape.Square)
            {
                vertices = RigidBody.VerticesForSquare(w, h);
                verticesTransformed = new Vector[vertices.Length];
                trianglesIndices = RigidBody.VerticesForBoxTriangles();
            }

            // Circle
            else
            {
                vertices = null;
                verticesTransformed = null;
                trianglesIndices = null;
            }

            updateRequiredTransformed = true; ;  // set to true
            updateRequiredAABB = true ;
        }

        // Function to create Squares and Circles
        public static void RigidSquareBody(float width, float height, Vector position, float density, bool isStat, float rest, out RigidBody body)        // (*) check actual class for special extra term VIDEO [2]
        {
            // Set the body null first
            body = null;

            // Calculate area of the square (A=w*h)
            float area = width * height;

            // Calculate the mass of the circle
            float mass = area * density;

            // Create the Square
            body = new RigidBody(position, density, mass, rest, area, isStat, 0f, width, height, TypeOfShape.Square);
        }

        public static void RigidCircleBody(float radius, Vector position, float density, bool isStat, float rest, out RigidBody body)        // (*) check actual class for special extra term VIDEO [2]
        {
            // Set the body null first
            body = null;

            // Calculate area of the circle (A=r*r*pi)
            float area = radius * radius * MathF.PI;

            // Calculate the mass of the circle
            float mass = area * density;

            // Create the Circle
            body = new RigidBody(position, density, mass, rest, area, isStat, radius, 0f, 0f, TypeOfShape.Circle);
        }

        // Move things around
        public void Move(Vector amount)
        {
            pos += amount;
            updateRequiredTransformed = true;
            updateRequiredAABB = true ;
        }

        public void NewPosition(Vector v)
        {
            pos = v;
            updateRequiredTransformed = true;
            updateRequiredAABB = true ;
        }

        public void Rotate(float rotation_amount)
        {
            rot += rotation_amount;
            updateRequiredTransformed = true;
            updateRequiredAABB = true ;
        }

        public void NewRotation(float rotation)     // Rotate to
        {
            rot = rotation;
            updateRequiredTransformed = true;
            updateRequiredAABB = true;
        }

        public void NewIntertia(float iner)         // To set inertia
        {
            inertia = iner;
            // Inverse inertia
            if (!isStatic)
            {
                inv_inertia = 1f / inertia;
            }

            else
            {
                // No inertia if the object is static
                inv_inertia = 0f;
            }

            updateRequiredTransformed = true;
            updateRequiredAABB = true;
        }

        // Increment position and rotation based on Forward Euler
        public void MoveStep(float time, Vector gravity, int iterations)
        {
            // Check if the body is static
            if (isStatic)
            {
                return;
            }

            // Divide the current time step by the number of iterations so we get higher precision
            time /= (float)iterations;

            // Increment velocity by gravity
            linear_vel += gravity * time;

            // Increment position and rotation
            pos += linear_vel * time;
            rot += rot_vel * time;

            // Set the force back to 0
            force = Vector.vec_ZERO;

            // Set update to true
            updateRequiredTransformed = true;
            updateRequiredAABB = true;
        }

        // Add force
        public void AddForce(Vector f)
        {
            force = f;
        }

        // Create vertices array for a Square shape
        private static Vector[] VerticesForSquare(float width, float height)
        {
            // Get the dimensions of the Square
            float left = -width / 2f;
            float right = left + width;
            float bottom = -height / 2f;
            float top = bottom + height;

            // Create array of vertices that will form the Square
            Vector[] verts = new Vector[4];
            verts[0] = new Vector(left, top);
            verts[1] = new Vector(right, top);
            verts[2] = new Vector(right, bottom);
            verts[3] = new Vector(left, bottom);

            return verts;
        }

        // Triangulating a square
        private static int[] VerticesForBoxTriangles()
        {
            // Create the triangles indices array
            int[] triangles = new int[6];

            // Upper right Triangle
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            // Down left Triangle
            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;

            // return indices
            return triangles;
        }

        // Calculate the rotational inertia depending on if the body is a square or a circle
        public float RotationalInertia()
        {
            // Circle
            if (tos is TypeOfShape.Circle)
            {
                // Rotational Intertia at the center of a circle
                return (1f / 2f) * mass * r*r;
            }

            // Square
            else
            {
                // Rotational Intertia at the center of a 2D square
                return (1f / 12f) * mass * (w*w + h*h);
            }
        }

        // Getter Functions
        public TypeOfShape GetTypeOfShape()
        {
            return tos;
        }

        public Vector getPosition()
        {

            return pos;
        }

        public float getRadius()
        {
            return r;
        }

        public float getWidth()
        {
            return w;
        }

        public float getHeight()
        {
            return h;
        }

        public int[] getTrianglesIndices()
        {
            return trianglesIndices;
        }
        
        public float getMass()
        {
            return mass;
        }

        public float getRestitution()
        {

            return restitution;
        }

        public Vector GetLinearVelocity()
        {
            return linear_vel;
        }

        public float GetAngularVelocity()
        {
            return rot_vel;
        }

        public Vector[] GetTransformedVertices()
        {
            // Be sure to apply the transformation before returning the result
            if (updateRequiredTransformed)
            {
                Transform transform = new Transform(pos, rot);

                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector vec = vertices[i];
                    verticesTransformed[i] = Vector.TransformVector(vec, transform);   
                }
            }

            // Set boolean for updates to false because we have done the updates necessary
            updateRequiredTransformed = false;

            return verticesTransformed;
        }

        // AABB for our 2 shapes (circle and square)
        public AABB GetAABB()
        {
            if (updateRequiredAABB) 
            {
                // min and max axis initialization
                float min_x = float.MaxValue;
                float min_y = float.MaxValue;
                float max_x = float.MinValue;
                float max_y = float.MinValue;

                // Check if it is a square
                if (tos is TypeOfShape.Square)
                {
                    // Get the vertices
                    Vector[] vertices = GetTransformedVertices();

                    // Loop through vertices and determine dimension of the AABB
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        Vector v = vertices[i];

                        if (v.x < min_x)
                        {
                            min_x = v.x;
                        }

                        if (v.x > max_x)
                        {
                            max_x = v.x;
                        }

                        if (v.y < min_y)
                        {
                            min_y = v.y;
                        }

                        if (v.y > max_y)
                        {
                            max_y = v.y;
                        }

                    }
                }

                // Check if it is a circle
                else if (tos is TypeOfShape.Circle)
                {
                    // Get the dimension of the AABB for a circle
                    min_x = pos.x - r;
                    min_y = pos.y - r;
                    max_x = pos.x + r;
                    max_y = pos.y + r;
                }

                // set AABB field
                aabb = new AABB(min_x, min_y, max_x, max_y);
            }

            // Set update to false
            updateRequiredAABB = false;

            // Return AABB
            return aabb;
           
        }
    }
}
