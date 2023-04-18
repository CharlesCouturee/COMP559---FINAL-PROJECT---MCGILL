using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Library;
using System.Collections.Specialized;

namespace Project_Final
{
    public class Project : Game
    {
        // Fields
        private GraphicsDeviceManager graphics;
        private Screen screen;
        private Shapes shapes;
        private Sprites sprites;
        private Camera camera;                      // ###

        private List<Color> colors;
        private List<Color> colors_contour;
        private Vector2[] buffer;

        private int substep;

        // Environment object
        private Environment environment;
        private float padding;
        private float l;
        private float r;
        private float b;
        private float t;

        public Project()
        {
            // Setup graphics
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = true;

            // Setup root directory
            Content.RootDirectory = "Content";

            // Setup mouse and IsFixedTimeStep
            IsMouseVisible = true;
            IsFixedTimeStep = true;

            // Setup for 
            double UpdatesPerSecond = 60d;
            TargetElapsedTime = TimeSpan.FromTicks((long)Math.Round((double)TimeSpan.TicksPerSecond / UpdatesPerSecond));
        }

        protected override void Initialize()
        {
            // Initialization for window projection + camera setup
            Util.SetRelativeBackBufferSize(graphics, 0.85f);
            screen = new Screen(this, 1200, 768);
            shapes = new Shapes(this);
            sprites = new Sprites(this);
            camera = new Camera(screen);
            camera.Zoom = 22;
            substep = 20;

            // Get the camera size
            camera.GetExtents(out float left, out float right, out float bottom, out float top);
            l = left;
            r = right;
            b = bottom;
            t = top;

            colors = new List<Color>();
            colors_contour = new List<Color>();
            padding = MathF.Abs(right - left) * 0.10f;

            // Create environment
            environment = new Environment();

            // Create a body that will be kind of the "ground" of our game
            RigidBody.RigidSquareBody(r - l - (padding * 2), 2f, new Vector(-5, -10), 1f, true, 0.5f, out RigidBody ground);

            // Extra static bodies 
            RigidBody.RigidSquareBody(18f, 1f, new Vector(-19, 5), 1f, true, 0.5f, out RigidBody static_body_1);
            static_body_1.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);

            // Add static bodies to the environment
            environment.AddRigidBody(ground);
            environment.AddRigidBody(static_body_1);

            // Colors for static bodies
            colors.Add(Color.White);
            colors_contour.Add(Color.White);

            colors.Add(Color.White);
            colors_contour.Add(Color.White);

            // MonoGame initialization
            base.Initialize();
        }

        // This is where the Rendering of the screen happens
        protected override void Update(GameTime gameTime)
        {
            // Handle keyboard and mouse input
            Library.Keyboard Fk = Library.Keyboard.Instance;
            Fk.Update();

            Library.Mouse Fm = Library.Mouse.Instance;
            Fm.Update();

            /* Mouse and Keyboard function */

            // Add square when user left click
            if (Fm.IsLeftMouseButtonPressed())
            {
                // Get mouse position
                Vector mouse_pos = Vector.ToVector(Fm.GetMouseWorldPosition(this, screen, camera));

                // Create body and add it to the list of bodies
                RigidBody.RigidSquareBody(2f, 2f, mouse_pos, 2f, false, 0.5f, out RigidBody body);
                environment.AddRigidBody(body);

                // Add colors
                //colors.Add(RandomHelper.RandomColor());
                colors.Add(Color.Black);
                colors_contour.Add(Color.White);
            }

            // Add circle when user right click
            if (Fm.IsRightMouseButtonPressed())
            {
                // Get mouse position
                Vector mouse_pos = Vector.ToVector(Fm.GetMouseWorldPosition(this, screen, camera));

                // Create body and add it to the list of bodies
                RigidBody.RigidCircleBody(1f, mouse_pos, 2f, false, 0.5f, out RigidBody body);
                environment.AddRigidBody(body);

                // Add colors
                //colors.Add(RandomHelper.RandomColor());
                colors.Add(Color.Black);
                colors_contour.Add(Color.White);
            }

            // Basic Keyboard functions
            if (Fk.IsKeyAvailable)
            {

                if (Fk.IsKeyClicked(Keys.Escape))
                {
                    Exit();
                }

                else if (Fk.IsKeyClicked(Keys.R))
                {
                    // Clear screen
                    environment.bodyList.Clear();
                    colors.Clear();
                    colors_contour.Clear();
                }

                else if (Fk.IsKeyDown(Keys.Up))
                {
                    substep += 1;
                    Console.WriteLine($"Time step: {substep}");
                }

                else if (Fk.IsKeyDown(Keys.Down))
                {
                    if (substep - 1 > 0)
                    {
                        substep -= 1;
                    }
                  
                    Console.WriteLine($"Time step: {substep}");
                }

                // First test
                else if (Fk.IsKeyClicked(Keys.T))
                {
                    // Clear screen
                    environment.bodyList.Clear();
                    colors.Clear();
                    colors_contour.Clear();

                    // Create Polygons
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-14, 5), 1f, true, 0.5f, out RigidBody s1);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-20, 8), 1f, true, 0.5f, out RigidBody s2);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(16, 0), 1f, true, 0.5f, out RigidBody s3);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(12, -10), 1f, true, 0.5f, out RigidBody s4);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(0, -5), 1f, true, 0.5f, out RigidBody s5);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-5, -2), 1f, true, 0.5f, out RigidBody s6);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-7, -8), 1f, true, 0.5f, out RigidBody s7);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(5, 10), 1f, true, 0.5f, out RigidBody s8);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(3, 3), 1f, true, 0.5f, out RigidBody s9);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(0, 14), 1f, true, 0.5f, out RigidBody s10);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-4, 4), 1f, true, 0.5f, out RigidBody s11);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-15, -8), 1f, true, 0.5f, out RigidBody s12);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-22, -3), 1f, true, 0.5f, out RigidBody s13);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-16, -1), 1f, true, 0.5f, out RigidBody s14);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-10, -0), 1f, true, 0.5f, out RigidBody s15);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(8, -2), 1f, true, 0.5f, out RigidBody s16);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-8, 10), 1f, true, 0.5f, out RigidBody s17);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(14, 5), 1f, true, 0.5f, out RigidBody s18);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(5, -13), 1f, true, 0.5f, out RigidBody s19);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(14, 12), 1f, true, 0.5f, out RigidBody s20);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(20, 10), 1f, true, 0.5f, out RigidBody s21);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(23, 6), 1f, true, 0.5f, out RigidBody s22);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(22, -4), 1f, true, 0.5f, out RigidBody s23);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-17, -13), 1f, true, 0.5f, out RigidBody s24);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-10, -15), 1f, true, 0.5f, out RigidBody s25);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-14, 12), 1f, true, 0.5f, out RigidBody s26);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(19, -12), 1f, true, 0.5f, out RigidBody s27);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-23, -14), 1f, true, 0.5f, out RigidBody s28);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-3, -13), 1f, true, 0.5f, out RigidBody s29);

                    // Add them to the list
                    environment.bodyList.Add(s1);
                    environment.bodyList.Add(s2);
                    environment.bodyList.Add(s3);
                    environment.bodyList.Add(s4);
                    environment.bodyList.Add(s5);
                    environment.bodyList.Add(s6);
                    environment.bodyList.Add(s7);
                    environment.bodyList.Add(s8);
                    environment.bodyList.Add(s9);
                    environment.bodyList.Add(s10);
                    environment.bodyList.Add(s11);
                    environment.bodyList.Add(s12);
                    environment.bodyList.Add(s13);
                    environment.bodyList.Add(s14);
                    environment.bodyList.Add(s15);
                    environment.bodyList.Add(s16);
                    environment.bodyList.Add(s17);
                    environment.bodyList.Add(s18);
                    environment.bodyList.Add(s19);
                    environment.bodyList.Add(s20);
                    environment.bodyList.Add(s21);
                    environment.bodyList.Add(s22);
                    environment.bodyList.Add(s23);
                    environment.bodyList.Add(s24);
                    environment.bodyList.Add(s25);
                    environment.bodyList.Add(s26);
                    environment.bodyList.Add(s27);
                    environment.bodyList.Add(s28);
                    environment.bodyList.Add(s29);

                    // Rotate object
                    s1.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s2.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s3.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s4.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s5.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s6.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s7.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s8.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s9.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s10.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s11.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s12.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s13.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s14.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s15.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s16.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s17.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s18.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s19.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s20.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s21.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s22.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s23.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s24.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s25.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s26.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s27.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s28.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s29.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);

                    // Add colorss
                    for (int i = 0; i < 29; i++)
                    {
                        colors.Add(Color.White);
                        colors_contour.Add(Color.White);
                    }
                }

                // Second test
                else if (Fk.IsKeyClicked(Keys.Y))
                {
                    // Clear screen
                    environment.bodyList.Clear();
                    colors.Clear();
                    colors_contour.Clear();

                    // Create Polygons
                    RigidBody.RigidCircleBody(1.2f, new Vector(-14, 5), 1f, true, 0.5f, out RigidBody s1);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-20, 8), 1f, true, 0.5f, out RigidBody s2);
                    RigidBody.RigidCircleBody(1.2f, new Vector(16, 0), 1f, true, 0.5f, out RigidBody s3);
                    RigidBody.RigidCircleBody(1.2f, new Vector(12, -10), 1f, true, 0.5f, out RigidBody s4);
                    RigidBody.RigidCircleBody(1.2f, new Vector(0, -5), 1f, true, 0.5f, out RigidBody s5);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-5, -2), 1f, true, 0.5f, out RigidBody s6);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-7, -8), 1f, true, 0.5f, out RigidBody s7);
                    RigidBody.RigidCircleBody(1.2f, new Vector(5, 10), 1f, true, 0.5f, out RigidBody s8);
                    RigidBody.RigidCircleBody(1.2f, new Vector(3, 3), 1f, true, 0.5f, out RigidBody s9);
                    RigidBody.RigidCircleBody(1.2f, new Vector(0, 14), 1f, true, 0.5f, out RigidBody s10);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-4, 4), 1f, true, 0.5f, out RigidBody s11);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-15, -8), 1f, true, 0.5f, out RigidBody s12);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-22, -3), 1f, true, 0.5f, out RigidBody s13);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-16, -1), 1f, true, 0.5f, out RigidBody s14);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-10, -0), 1f, true, 0.5f, out RigidBody s15);
                    RigidBody.RigidCircleBody(1.2f, new Vector(8, -2), 1f, true, 0.5f, out RigidBody s16);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-8, 10), 1f, true, 0.5f, out RigidBody s17);
                    RigidBody.RigidCircleBody(1.2f, new Vector(14, 5), 1f, true, 0.5f, out RigidBody s18);
                    RigidBody.RigidCircleBody(1.2f, new Vector(5, -13), 1f, true, 0.5f, out RigidBody s19);
                    RigidBody.RigidCircleBody(1.2f, new Vector(14, 12), 1f, true, 0.5f, out RigidBody s20);
                    RigidBody.RigidCircleBody(1.2f, new Vector(20, 10), 1f, true, 0.5f, out RigidBody s21);
                    RigidBody.RigidCircleBody(1.2f, new Vector(23, 6), 1f, true, 0.5f, out RigidBody s22);
                    RigidBody.RigidCircleBody(1.2f, new Vector(22, -4), 1f, true, 0.5f, out RigidBody s23);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-17, -13), 1f, true, 0.5f, out RigidBody s24);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-10, -15), 1f, true, 0.5f, out RigidBody s25);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-14, 12), 1f, true, 0.5f, out RigidBody s26);
                    RigidBody.RigidCircleBody(1.2f, new Vector(19, -12), 1f, true, 0.5f, out RigidBody s27);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-23, -14), 1f, true, 0.5f, out RigidBody s28);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-3, -13), 1f, true, 0.5f, out RigidBody s29);

                    // Add them to the list
                    environment.bodyList.Add(s1);
                    environment.bodyList.Add(s2);
                    environment.bodyList.Add(s3);
                    environment.bodyList.Add(s4);
                    environment.bodyList.Add(s5);
                    environment.bodyList.Add(s6);
                    environment.bodyList.Add(s7);
                    environment.bodyList.Add(s8);
                    environment.bodyList.Add(s9);
                    environment.bodyList.Add(s10);
                    environment.bodyList.Add(s11);
                    environment.bodyList.Add(s12);
                    environment.bodyList.Add(s13);
                    environment.bodyList.Add(s14);
                    environment.bodyList.Add(s15);
                    environment.bodyList.Add(s16);
                    environment.bodyList.Add(s17);
                    environment.bodyList.Add(s18);
                    environment.bodyList.Add(s19);
                    environment.bodyList.Add(s20);
                    environment.bodyList.Add(s21);
                    environment.bodyList.Add(s22);
                    environment.bodyList.Add(s23);
                    environment.bodyList.Add(s24);
                    environment.bodyList.Add(s25);
                    environment.bodyList.Add(s26);
                    environment.bodyList.Add(s27);
                    environment.bodyList.Add(s28);
                    environment.bodyList.Add(s29);

                    // Add colorss
                    for (int i = 0; i < 29; i++)
                    {
                        colors.Add(Color.White);
                        colors_contour.Add(Color.White);
                    }
                }

                // Third test
                else if (Fk.IsKeyClicked(Keys.U))
                {
                    // Clear screen
                    environment.bodyList.Clear();
                    colors.Clear();
                    colors_contour.Clear();

                    // Create Polygons
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-14, 5), 1f, true, 0.5f, out RigidBody s1);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-20, 8), 1f, true, 0.5f, out RigidBody s2);
                    RigidBody.RigidCircleBody(1.2f, new Vector(16, 0), 1f, true, 0.5f, out RigidBody s3);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(12, -10), 1f, true, 0.5f, out RigidBody s4);
                    RigidBody.RigidCircleBody(1.2f, new Vector(0, -5), 1f, true, 0.5f, out RigidBody s5);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-5, -2), 1f, true, 0.5f, out RigidBody s6);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-7, -8), 1f, true, 0.5f, out RigidBody s7);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(5, 10), 1f, true, 0.5f, out RigidBody s8);
                    RigidBody.RigidCircleBody(1.2f, new Vector(3, 3), 1f, true, 0.5f, out RigidBody s9);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(0, 14), 1f, true, 0.5f, out RigidBody s10);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-4, 4), 1f, true, 0.5f, out RigidBody s11);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-15, -8), 1f, true, 0.5f, out RigidBody s12);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-22, -3), 1f, true, 0.5f, out RigidBody s13);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-16, -1), 1f, true, 0.5f, out RigidBody s14);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-10, -0), 1f, true, 0.5f, out RigidBody s15);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(8, -2), 1f, true, 0.5f, out RigidBody s16);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-8, 10), 1f, true, 0.5f, out RigidBody s17);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(14, 5), 1f, true, 0.5f, out RigidBody s18);
                    RigidBody.RigidCircleBody(1.2f, new Vector(5, -13), 1f, true, 0.5f, out RigidBody s19);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(14, 12), 1f, true, 0.5f, out RigidBody s20);
                    RigidBody.RigidCircleBody(1.2f, new Vector(20, 10), 1f, true, 0.5f, out RigidBody s21);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(23, 6), 1f, true, 0.5f, out RigidBody s22);
                    RigidBody.RigidCircleBody(1.2f, new Vector(22, -4), 1f, true, 0.5f, out RigidBody s23);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-17, -13), 1f, true, 0.5f, out RigidBody s24);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-10, -15), 1f, true, 0.5f, out RigidBody s25);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-14, 12), 1f, true, 0.5f, out RigidBody s26);
                    RigidBody.RigidCircleBody(1.2f, new Vector(19, -12), 1f, true, 0.5f, out RigidBody s27);
                    RigidBody.RigidSquareBody(2f, 2f, new Vector(-23, -14), 1f, true, 0.5f, out RigidBody s28);
                    RigidBody.RigidCircleBody(1.2f, new Vector(-3, -13), 1f, true, 0.5f, out RigidBody s29);

                    // Add them to the list
                    environment.bodyList.Add(s1);
                    environment.bodyList.Add(s2);
                    environment.bodyList.Add(s3);
                    environment.bodyList.Add(s4);
                    environment.bodyList.Add(s5);
                    environment.bodyList.Add(s6);
                    environment.bodyList.Add(s7);
                    environment.bodyList.Add(s8);
                    environment.bodyList.Add(s9);
                    environment.bodyList.Add(s10);
                    environment.bodyList.Add(s11);
                    environment.bodyList.Add(s12);
                    environment.bodyList.Add(s13);
                    environment.bodyList.Add(s14);
                    environment.bodyList.Add(s15);
                    environment.bodyList.Add(s16);
                    environment.bodyList.Add(s17);
                    environment.bodyList.Add(s18);
                    environment.bodyList.Add(s19);
                    environment.bodyList.Add(s20);
                    environment.bodyList.Add(s21);
                    environment.bodyList.Add(s22);
                    environment.bodyList.Add(s23);
                    environment.bodyList.Add(s24);
                    environment.bodyList.Add(s25);
                    environment.bodyList.Add(s26);
                    environment.bodyList.Add(s27);
                    environment.bodyList.Add(s28);
                    environment.bodyList.Add(s29);

                    // Rotate object
                    s1.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s2.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s3.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s4.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s5.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s6.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s7.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s8.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s9.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s10.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s11.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s12.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s13.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s14.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s15.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s16.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s17.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s18.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s19.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s20.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s21.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s22.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s23.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s24.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s25.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s26.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s27.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s28.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);
                    s29.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);

                    // Add colorss
                    for (int i = 0; i < 29; i++)
                    {
                        colors.Add(Color.White);
                        colors_contour.Add(Color.White);
                    }
                }

                // Fourth test
                else if (Fk.IsKeyClicked(Keys.I))
                {
                    // Clear screen
                    environment.bodyList.Clear();
                    colors.Clear();
                    colors_contour.Clear();

                    // Create a body that will be kind of the "ground" of our game
                    RigidBody.RigidSquareBody(r - l - (padding * 2), 2f, new Vector(-5, -10), 1f, true, 0.5f, out RigidBody ground);

                    // Extra static bodies 
                    RigidBody.RigidSquareBody(18f, 1f, new Vector(-19, 5), 1f, true, 0.5f, out RigidBody static_body_1);
                    static_body_1.Rotate(Microsoft.Xna.Framework.MathHelper.TwoPi / -10f);

                    // Add static bodies to the environment
                    environment.AddRigidBody(ground);
                    environment.AddRigidBody(static_body_1);

                    // Colors for static bodies
                    colors.Add(Color.White);
                    colors_contour.Add(Color.White);

                    colors.Add(Color.White);
                    colors_contour.Add(Color.White);
                }
            }
                
            
            // Step by the time in our environment
            environment.Step(Util.GetElapsedTimeInSeconds(gameTime), substep);

            // Check if the objects fell of the camera view abd delete them if necessary
            camera.GetExtents(out _, out _, out float bottom, out _);

            // Get the AABB of each body and verify if they fell of screen
            for (int i = 0; i < environment.GetRigidBodyCount(); i++)
            {
                environment.GetRigidBody(i, out RigidBody body);
                //body.NewIntertia(body.RotationalInertia());
                AABB aabb = body.GetAABB();

                // Remove body and its color
                if (aabb.maximum.y < bottom)
                {
                    environment.RemoveRigidBody(body);
                    colors.RemoveAt(i);
                    colors_contour.RemoveAt(i);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Set screen
            screen.Set();

            //GraphicsDevice.Clear(new Color(50, 60, 70));
            GraphicsDevice.Clear(new Color(10, 20, 50));

            // Draw shapes on the screen
            shapes.Begin(camera);

            // Loop through all the bodies in our environment and draw them
            for (int i = 0; i < environment.GetRigidBodyCount(); i++)
            {
                environment.GetRigidBody(i, out RigidBody body);
                Vector2 position = Vector.ToVector2(body.getPosition());

                if (body.GetTypeOfShape() is TypeOfShape.Circle)
                {
                    shapes.DrawCircleFill(position, body.getRadius(), 26, colors[i]);
                    shapes.DrawCircle(position, body.getRadius(), 26, colors_contour[i]);

                    // Draw small line so we can see the actual rotation
                    Vector2 a = Vector2.Zero;
                    Vector2 b = new Vector2(body.getRadius(), 0f);

                    Library.Transform t = new Library.Transform(Vector.ToVector2(body.getPosition()), body.rot, 1f);
                    a = Util.Transform(a, t);
                    b = Util.Transform(b, t);

                    shapes.DrawLine(a, b, Color.White);
                }
                else
                {
                    Vector.ToVector2Array(body.GetTransformedVertices(), ref buffer);
                    shapes.DrawPolygonFill(buffer, body.getTrianglesIndices(), colors[i]);
                    shapes.DrawPolygon(buffer, colors_contour[i]);
                }
            }

            // Just MonoGame draw routine
            shapes.End();
            screen.Unset();
            screen.Present(sprites);
            base.Draw(gameTime);
        }
    }
}