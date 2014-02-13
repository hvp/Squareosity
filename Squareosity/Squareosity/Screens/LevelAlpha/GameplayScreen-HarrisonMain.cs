#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GameStateManagement;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Joints;
#endregion

namespace Squareosity
{
    /// <summary>
    /// Level 1 game screen. Collect the 3 collectables to progress
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        PlayerBody playerBody;
        List<Square> Squares = new List<Square>();
        List<Shape> Shapes = new List<Shape>();
        List<Wall> Walls = new List<Wall>();
        List<Maze> Mazes = new List<Maze>();
        List<Collectable> Collectables = new List<Collectable>();
        List<Whip> Whips = new List<Whip>();
        List<Bady> Badies = new List<Bady>();

        Shape ninjaGate;
        bool ninjaGateRemoved = false;

        Body circleBody;
        Texture2D circleTex;

        Texture2D dead;

        GamePadState previousGamePadState;

        Cam2d cam2D;

        BloomComponent bloom;

        public static int bloomSettingsIndex = 0;

        public static World world;


     

        float pauseAlpha;

        InputAction pauseAction;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);


            world = new World(new Vector2(0, 0));
      

            // define and action 
            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                gameFont = content.Load<SpriteFont>("gamefont");

                bloom = new BloomComponent(ScreenManager.Game);
                ScreenManager.Game.Components.Add(bloom);

                playerBody = new PlayerBody(content.Load<Texture2D>("redPlayer"), world);
                dead = content.Load<Texture2D>("DeadScreen/redDeathScreen");

                cam2D = new Cam2d(ScreenManager.GraphicsDevice);

                // neon whip wheel
                {
                    // instantiate the cirlce body 
                    circleBody = BodyFactory.CreateCircle(world, 20.0f / 64, 1f, new Vector2(1000f / 64f, 1000f / 64f));
                    circleBody.BodyType = BodyType.Static;
                    circleBody.CollisionCategories = Category.Cat2;



                    // fix the circle to the world
                    circleTex = content.Load<Texture2D>("orangeCircle");
                    FixedRevoluteJoint fixedJoint = new FixedRevoluteJoint(circleBody, Vector2.Zero, circleBody.Position);
                    world.AddJoint(fixedJoint);

                    // create the whips
                    Whips.Add(new Whip(content.Load<Texture2D>("orangeChianLinkSmall"), new Vector2(1000, 1000), new Vector2(1350, 1000f), world));
                    Whips.Add(new Whip(content.Load<Texture2D>("orangeChianLinkSmall"), new Vector2(1000, 1000), new Vector2(650, 1000), world));
                    //    Whips.Add(new Whip(content.Load<Texture2D>("orangeChianLinkSmall"), new Vector2(1000, 1000), new Vector2(1000, 1350), world));

                    for (int k = 0; k < Whips.Count; k++)
                    {

                        world.AddJoint(new FixedRevoluteJoint(Whips[k].chainLinks[k], Vector2.Zero, circleBody.Position));
                    }

                }

                // add badies 
                {
                    Badies.Add(new Bady(content.Load<Texture2D>("Badies/orangeBady"), new Vector2(-1, -2), 15, true, 180f, content, world));
                    Badies.Add(new Bady(content.Load<Texture2D>("Badies/pinkBady"), new Vector2(-3, 0.5f), 8, false, 90f, content, world));
                }


                // add some squares and some walls
                {
                    int space = 0;

                    for (int i = 0; i < 10; i++)
                    {
                        Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"), new Vector2(space - 5, 0), true, world));
                        Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"), new Vector2(space - 5, 600), true, world));

                        Squares.Add(new Square(content.Load<Texture2D>("Squares/greenSquare"), new Vector2(space, 100), world));
                        Squares.Add(new Square(content.Load<Texture2D>("Squares/pinkSquare"), new Vector2(space + 50, 200), world));

                        Squares.Add(new Square(content.Load<Texture2D>("Squares/yellowSquare"), new Vector2(space, 300), world));
                        Squares.Add(new Square(content.Load<Texture2D>("Squares/orangeSquare"), new Vector2(space + 50, 400), world));

                        Squares.Add(new Square(content.Load<Texture2D>("Squares/greenSquare"), new Vector2(space, 500), world));


                        if (i <= 5)
                        {
                            //vertical walls
                            Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"), new Vector2(-50, space + 50), false, world));
                            Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"), new Vector2(1050, space + 50), false, world));
                        }

                        space += 100;


                    }

                    // extra wall
                    Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"), new Vector2(space + 5, 0), true, world));
                    Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"), new Vector2(space - 10, 0), true, world)); 

                    // Ninja gate need to change the colour of the ninja gate
                    {
                        ninjaGate = new Shape(content.Load<Texture2D>("NinjaWeapons/blueNinjaWheel"),new Vector2((space - 5), 600f),false,world);
                        Shapes.Add(ninjaGate);
                        Shapes[0].shapeBody.AngularVelocity = 10f;
                      //  Shapes.Add(new Shape(content.Load<Texture2D>("NinjaWeapons/blueNinjaWheel"),new Vector2((space - 5), 600f),false,world));
                        world.AddJoint(new FixedRevoluteJoint(Shapes[0].shapeBody, Vector2.Zero, Shapes[0].shapeBody.Position));

                    }

                }

               
                //add collectables
                {
                    Collectables.Add(new Collectable(content.Load<Texture2D>("redStar"), new Vector2(0, 25), world));
                    Collectables.Add(new Collectable(content.Load<Texture2D>("redStar"), new Vector2(900, 25), world));
                    Collectables.Add(new Collectable(content.Load<Texture2D>("redStar"), new Vector2(900, 625), world));
                }

                // set cam track 

                cam2D.TrackingBody = playerBody.playerBody;
                cam2D.EnableTracking = true;

               

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }


        }


        public override void Deactivate()
        {


            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();


        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                bloom.Visible = true;
                playerBody.update();
               
                //game script 
                {

                    
                    if (Collectables.Count == 1 && ninjaGateRemoved == false)
                    {
                        world.RemoveBody(ninjaGate.shapeBody);
                        Shapes.Remove(ninjaGate);// for some reason I had to create a ninja gate object and not just add an object to the list
                        ninjaGateRemoved = true;
                    }

                    if (Collectables.Count == 0)
                    {
                        Game1.progress = 1;
                      ScreenManager.RemoveScreen(this);
                      
                        // the pause system screws up when we have the reloaded game screen.
                        // but should be ok when we have a new level 
                        LoadingScreen.Load(ScreenManager, true, PlayerIndex.One,
                           new GameplayScreen());
                 
                    }
                                        
                }


                bloom.Settings = BloomSettings.PresetSettings[bloomSettingsIndex];

                foreach (Shape shape in Shapes)
                {
                    if (shape.isTouching)
                    {
                       
                        playerBody.isAlive = false;
                       
                  

                    }
                    shape.update();
                }

                foreach (Square square in Squares)
                {
                    square.Update();
                    if (square.isTouching)
                    {
                        playerBody.isAlive = false;
                    }
                }

                foreach (Bady bady in Badies)
                {
                    
                    foreach (Laser laser in bady.Lasers)
                    {
                        if (laser.hasHit)
                        {
                            playerBody.isAlive = false;

                        }

                    }

                    bady.Update(gameTime);

                }

                foreach (Whip whip in Whips)
                {

                     whip.chainLinks[0].ApplyTorque(50); // gets the wheel spinning
                     // for some reason this is true on start up.
                     
                     if (whip.isTouching) 
                     {
                         playerBody.isAlive = false;

                     }
               }

                // Collectables
                {
                    foreach (Collectable collectable in Collectables)
                    {

                        collectable.Update();


                    }

                    //remove collected collectables 

                    for (int k = 0; k < Collectables.Count; k++)
                    {
                        if (Collectables[k].collected)
                        {
                            world.RemoveBody(Collectables[k].collectableBody);
                            Collectables.RemoveAt(k);
                        }


                    }
                }

                // limts on the cam. 

                cam2D.MaxRotation = 0.001f;
                cam2D.MinRotation = -0.001f;

                cam2D.MaxPosition = new Vector2(((playerBody.playerBody.Position.X) * 64 + 1), ((playerBody.playerBody.Position.Y) * 64) + 1);
                cam2D.MinPosition = new Vector2(((playerBody.playerBody.Position.X) * 64) + 2, ((playerBody.playerBody.Position.Y) * 64) + 1);
                cam2D.Update(gameTime);

                if (!playerBody.isAlive)
                {
                    bloom.Visible = false;
                    LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new DeadScreen(1));
                }

                world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
               // bloom.Dispose();
                bloom.Visible = false;
               
                  ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);

            }
            else
            {

                if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                {
                    playerBody.playerBody.ApplyLinearImpulse(new Vector2(-1, 0));
                }
                if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                {
                    playerBody.playerBody.ApplyLinearImpulse(new Vector2(1, 0));
                }
                if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                {
                    playerBody.playerBody.ApplyLinearImpulse(new Vector2(0, 1));
                }
                if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                {
                    playerBody.playerBody.ApplyLinearImpulse(new Vector2(0, -1));
                }
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            bloom.BeginDraw();

            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);
            
            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.FrontToBack,
                            BlendState.AlphaBlend,
                            null,
                            null,
                            null,
                            null,
                            cam2D.View);


          

            foreach (Square sqaure in Squares)
            {
                sqaure.Draw(spriteBatch);
                if (sqaure.isTouching)
                {
                    ScreenManager.GraphicsDevice.Clear(Color.White);
                   
                }
            }
            

            foreach (Wall wall in Walls)
            {
                wall.Draw(spriteBatch);

            }
            foreach (Collectable collectable in Collectables)
            {
         
                    collectable.draw(spriteBatch);
                

            }
            foreach (Shape shape in Shapes)
            {
                shape.Draw(spriteBatch);
                if (shape.isTouching)
                {
                    ScreenManager.GraphicsDevice.Clear(Color.White);
                   
                    
                }
            }

            foreach (Bady bady in Badies)
            {
                foreach(Laser laser in bady.Lasers )
                {
                    if (laser.hasHit)
                    {
                        ScreenManager.GraphicsDevice.Clear(Color.White);
                   
                    }


                }
                bady.Draw(spriteBatch);
            }
            spriteBatch.Draw(circleTex, circleBody.Position * 64, null, Color.White, circleBody.Rotation, new Vector2(20f, 20f), 1f, SpriteEffects.None, 1f);

            foreach (Whip whip in Whips)
            {
                whip.Draw(spriteBatch);
            }
            playerBody.draw(spriteBatch);

              

            spriteBatch.End();
         

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
                
            }
        }


        #endregion
    }
}
