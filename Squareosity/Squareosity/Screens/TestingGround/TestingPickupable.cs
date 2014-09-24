using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

namespace Squareosity
{
    class TestingPickupable: GameScreen
    {

        #region Fields

        ContentManager content;
        SpriteFont gameFont;
        PlayerBody playerBody;
        GamePadState previousGamePadState;
        MouseState mouse;
        Cam2d cam2D;
        BloomComponent bloom;
        public static int bloomSettingsIndex = 0;
        public static World world;
        float pauseAlpha;

        List<Wall> Walls = new List<Wall>();
        List<Bady> Badies = new List<Bady>();
        List<DistanceJoint> joints = new List<DistanceJoint>();
        List<Square> Squares = new List<Square>();
        List<SeekerDrone> Drones = new List<SeekerDrone>();
        List<Collectable> Collectables = new List<Collectable>();
        List<Pickupable> Pickupables = new List<Pickupable>();
        Texture2D reticle;
        InputAction pauseAction;

     

        ParticleManager<ParticleState> ParticleManager;
        Texture2D particleArt;

        Area area;

        public static Grid Grid { get; private set; }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>

        public TestingPickupable()
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

                Art.Load(content);
                gameFont = content.Load<SpriteFont>("gamefont");


                ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);
                particleArt = content.Load<Texture2D>("Laser");
             

                const int maxGridPoints = 1600;
                Vector2 gridSpacing = new Vector2((float)Math.Sqrt(1024 * 768 / maxGridPoints));

                Rectangle posAndSize = new Rectangle(-75, -20, 1140, 640 ); 
              //  Grid = new Grid(this.ScreenManager.GraphicsDevice.Viewport.Bounds, gridSpacing);
                Grid = new Grid(posAndSize, gridSpacing);
                
                
                bloom = new BloomComponent(ScreenManager.Game);
                ScreenManager.Game.Components.Add(bloom);
                area = new Area(content.Load<Texture2D>("pinkArea"),new Vector2(500,500),0f,world);

                cam2D = new Cam2d(ScreenManager.GraphicsDevice);
                reticle = content.Load<Texture2D>("redReticle");
                /// player 
                playerBody = new PlayerBody(content.Load<Texture2D>("redPlayer"), world, content);

                Pickupables.Add(new Pickupable(content.Load<Texture2D>("Squares/pinkSquare"), new Vector2(100, 100), world));
                Pickupables.Add(new Pickupable(content.Load<Texture2D>("Squares/pinkSquare"), new Vector2(100, 80), world));
                Pickupables.Add(new Pickupable(content.Load<Texture2D>("Squares/pinkSquare"), new Vector2(120, 100), world));

                Drones.Add(new SeekerDrone(content.Load<Texture2D>("testArrow"), new Vector2(500, 300), world, content, 20));
                Drones.Add(new SeekerDrone(content.Load<Texture2D>("testArrow"), new Vector2(600, 350), world, content, 25));

                Drones[0].Stationary = true;
               
                int space = 0;

                for (int i = 0; i < 11; i++)
                {
                    Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"), new Vector2(space - 5, 0), true, world));
                    Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"), new Vector2(space - 5, 600), true, world));

                    if (i <= 5)
                    {
                        //vertical walls
                        Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"), new Vector2(-50, space + 50), false, world));
                        Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"), new Vector2(1050, space + 50), false, world));
                    }

                    space += 100;
                }

                Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"), new Vector2(1050, 45), false, world));
                Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"), new Vector2(1050, 555), false, world));

                // set cam track 

                cam2D.TrackingBody = playerBody.playerBody;
                cam2D.EnableTracking = true;



                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }

            base.Activate(instancePreserved);

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

          
                mouse = Mouse.GetState();
                // bloom
                bloom.Visible = true;
                bloom.Settings = BloomSettings.PresetSettings[bloomSettingsIndex];
                
                // update entites
                playerBody.update(gameTime);
                Grid.Update();
                ParticleManager.Update();

                
               Grid.ApplyExplosiveForce(5f, playerBody.playerBody.Position * 64, 80);
               foreach (playerLaser laser in playerBody.getLasers)
               {
               //   Grid.ApplyExplosiveForce(5f, laser.laserBody.Position * 64, 50);
                  // Grid.ApplyImplosiveForce(5f, laser.laserBody.Position * 64, 50);
                  Grid.ApplyExplosiveForce(5f, laser.laserBody.Position * 64, 50);
                   
               }


                if (playerBody.isAlive == false)
                {
                    playerBody.isAlive = true;
                }

                foreach (SeekerDrone drone in Drones)
                {
                    drone.setTarget(playerBody.playerBody.Position * 64);


                    drone.update(gameTime);

                    if (drone.getIsDead)
                    {
                        //  if(!cam2D.getShake)
                        cam2D.Shake(300f, 1000f);
                        Grid.ApplyExplosiveForce(20f, drone.getBody.Position * 64, 1000);

                        Random randP = new Random();
                        float hue1 = randP.NextFloat(0, 6);
                        float hue2 = (hue1 + randP.NextFloat(0, 2)) % 6f;
                        Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
                        Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);

                        for (int i = 0; i < 120; i++)
                        {
                            float speed = 18f * (1f - 1 / randP.NextFloat(1f, 10f));
                            var state = new ParticleState()
                            {
                                Velocity = randP.NextVector2(speed, speed),
                                Type = ParticleType.Enemy,
                                LengthMultiplier = 1f
                            };
                            Color color = Color.Lerp(color1, color2, randP.NextFloat(0, 1));
                            ParticleManager.CreateParticle(particleArt, drone.getBody.Position * 64f, color, 190, new Vector2(1.5f), state);
                        }

                        world.RemoveBody(drone.getBody);

                        playerBody.updateScore(2); /// update this to a getter and setter!!!
                    }
                }
                for (int k = 0; k < Drones.Count; ++k)
                {
                    if (Drones[k].getIsDead)
                    {
                        Drones.RemoveAt(k);
                    }
                }
                //game script 

                foreach (Pickupable pickupable in Pickupables)
                {
                   
                    if (pickupable.getSetIsTouchingPlayer && playerBody.getSetWantsToPickUp && !playerBody.getSetHasPickedUp)
                    {
                      // joints.Add ( JointFactory.CreateRevoluteJoint(world,playerBody.playerBody,
                        //   pickupable.getBody, new Vector2(0,0)));


                        Random randP = new Random();
                        float hue1 = randP.NextFloat(0, 6);
                        float hue2 = (hue1 + randP.NextFloat(0, 2)) % 6f;
                        Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
                        Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);

                        for (int i = 0; i < 30; i++)
                        {
                            float speed = 18f * (1f - 1 / randP.NextFloat(1f, 10f));
                            var state = new ParticleState()
                            {
                                Velocity = randP.NextVector2(speed, speed),
                                Type = ParticleType.Enemy,
                                LengthMultiplier = 0.4f
                            };
                            Color color = Color.Lerp(color1, color2, randP.NextFloat(0, 1));
                            ParticleManager.CreateParticle(particleArt, pickupable.getBody.Position * 64f, color, 190, new Vector2(1.5f), state);
                        }
                       joints.Add( JointFactory.CreateDistanceJoint(world, playerBody.playerBody, pickupable.getBody, new Vector2(0, 0), new Vector2(0, 0)));


                        playerBody.getSetWantsToPickUp = false;
                        pickupable.getSetIsAttachedToPlayer = true;

                       

                        playerBody.getSetHasPickedUp = true;
                        
                    }

                    if (joints.Count > 0 && playerBody.getSetWantsTodrop && playerBody.getSetHasPickedUp && pickupable.getSetIsAttachedToPlayer )
                    {
                        // for some reason the on seperation dosn't work when removing a joint.
                        world.RemoveJoint(joints[0]);
                        joints.RemoveAt(0);
                        playerBody.getSetHasPickedUp = false;
                        pickupable.getSetIsAttachedToPlayer = false;
                        pickupable.getSetIsTouchingPlayer = false;

                        
                    }
                   
                }

              

                // limts on the cam. 

                cam2D.MaxRotation = 0.001f;
                cam2D.MinRotation = -0.001f;

                cam2D.MaxPosition = new Vector2(((playerBody.playerBody.Position.X) * 64 + 1), ((playerBody.playerBody.Position.Y) * 64) + 1);
                cam2D.MinPosition = new Vector2(((playerBody.playerBody.Position.X) * 64) + 2, ((playerBody.playerBody.Position.Y) * 64) + 1);
                cam2D.Update(gameTime);



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
            MouseState mouse = Mouse.GetState();

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
                playerBody.detectInput(keyboardState, mouse, cam2D.Position, gameTime);
                if(keyboardState.IsKeyDown(Keys.Space))
                {
                    Random rand = new Random();
                    float randX = rand.NextFloat(120, 1000);
                    float randY = rand.NextFloat(100, 500);
                    float ranD = rand.NextFloat(7, 30);

                    Drones.Add(new SeekerDrone(content.Load<Texture2D>("testArrow"), new Vector2(randX, randY), world, content, ranD));

                }
            }
        }

        // <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            bloom.BeginDraw();

            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.FrontToBack,
                            BlendState.AlphaBlend,
                            null,
                            null,
                            null,
                            null,
                            cam2D.View);

            ParticleManager.Draw(spriteBatch);
            Grid.Draw(spriteBatch);
            foreach (SeekerDrone drone in Drones)
            {
                drone.draw(spriteBatch);
            }

            foreach (Pickupable pickupable in Pickupables)
            {
                pickupable.Draw(spriteBatch);
            }

            foreach (Square square in Squares)
            {
                square.Draw(spriteBatch);
                if (square.isTouching)
                {
                    ScreenManager.GraphicsDevice.Clear(Color.White);
                }  
            }

            area.Draw(spriteBatch);

            foreach (Wall wall in Walls)
            {
                wall.Draw(spriteBatch);

            }

            if (!GamePad.GetState(PlayerIndex.One).IsConnected)
            {
              
                Vector2 mousePos = new Vector2(mouse.X, mouse.Y);
                spriteBatch.Draw(reticle, mousePos + cam2D.Position - new Vector2(1024 / 2, 768 / 2), null, Color.White, 0f, new Vector2(10, 10), 1f, SpriteEffects.None, 1f);
        
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
