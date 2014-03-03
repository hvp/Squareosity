#region using statements

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
#endregion

namespace Squareosity
{
    class LevelEditor : GameScreen
    {
        Cam2d cam2D;
        ContentManager content;
        BloomComponent bloom;

        public static World world;

        float pauseAlpha;

        InputAction pauseAction;

        public static int bloomSettingsIndex = 0;

        int assetIndex = 0; // 0 = blueWall, 1 = greenMine 

        float assetRotation = 0f;
        float scroolWheelRot;

        Texture2D playerSpwan;
        Texture2D blueWall; 
        MouseState mouse;
        MouseState oldMouse;

        bool resetSave = false;

        List<Wall> walls = new List<Wall>();

        Vector2 assetPos;


        public LevelEditor()
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



                bloom = new BloomComponent(ScreenManager.Game);
                ScreenManager.Game.Components.Add(bloom);


                cam2D = new Cam2d(ScreenManager.GraphicsDevice);
            //    cam2D.MoveCamera(new Vector2(1024 / 2, 768 / 2));
                playerSpwan = content.Load<Texture2D>("redPlayer");
                blueWall = content.Load<Texture2D>("Walls/blueWallMedium");
                    
                // set cam track 
                // don't need to for the moment 



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
                // bloom
                bloom.Visible = true;
                bloom.Settings = BloomSettings.PresetSettings[bloomSettingsIndex];
                // mouse input
                {
                    mouse = Mouse.GetState();



                    if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                    {

                        walls.Add(new Wall(blueWall,assetPos, assetRotation,world));

                    }




                    oldMouse = mouse;

                }
                cam2D.MaxRotation = 0.001f;
                cam2D.MinRotation = -0.001f;
                cam2D.Update(gameTime);
                world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            }
        }
        bool isRotated = false;//TODO : rename this variable appropriately , or create input manager? to handle "key pressed once" event?
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
                Dictionary<Tuple<Keys, Keys>, Vector2> dirMoves = new Dictionary<Tuple<Keys, Keys>, Vector2>()
                {
                    {new Tuple<Keys,Keys> (Keys.A ,Keys.Left), new Vector2(-.1f,0) },
                    {new Tuple<Keys,Keys> (Keys.D ,Keys.Right),new Vector2(.1f,0) },
                    {new Tuple<Keys,Keys> (Keys.W ,Keys.Up), new Vector2(0,-.1f) },
                    {new Tuple<Keys,Keys> (Keys.S ,Keys.Down), new Vector2(0,.1f) }
                };
                foreach (var k in dirMoves)
                {
                    if (keyboardState.IsKeyDown(k.Key.Item1) || keyboardState.IsKeyDown(k.Key.Item2))
                        cam2D.MoveCamera(k.Value);
                }


                if (!isRotated && keyboardState.IsKeyDown(Keys.R))
                {
                    isRotated = true;
                    if (assetRotation == 1.57f)
                    {
                        assetRotation = 0;
                    }
                    else
                    {
                        assetRotation = 1.57f;
                    }
                }
                else if (keyboardState.IsKeyUp(Keys.R))
                {
                    isRotated = false;
                }

                if (keyboardState.IsKeyDown(Keys.Y))
                {
                   
                        assetRotation += 0.1f;
                  
                  
                }
                if (keyboardState.IsKeyDown(Keys.T))
                {

                    assetRotation -= 0.1f;


                }
                if (keyboardState.IsKeyDown(Keys.T))
                {

                    assetRotation -= 0.1f;


                }
                if (keyboardState.IsKeyDown(Keys.Enter) && resetSave == false)
                {
                    writeToFile();
                    resetSave = true;
                }
                if(keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    resetSave = false;
                }
               

            }
        }

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

            spriteBatch.Draw(playerSpwan,new Vector2(0,0),Color.White);

            foreach (Wall wall in walls)
            {
                wall.Draw(spriteBatch);
            }
           assetPos = new Vector2(mouse.X, mouse.Y) + cam2D.Position - new Vector2(1024 / 2, 768 / 2);

            spriteBatch.Draw(blueWall,assetPos ,null, Color.White,assetRotation,new Vector2(blueWall.Width / 2,blueWall.Height / 2),1f,SpriteEffects.None,1f);

            spriteBatch.End();


            // If the game is transitioning on or off, fade it out to black.
              if (TransitionPosition > 0 || pauseAlpha > 0)
              {
                  float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                  ScreenManager.FadeBackBufferToBlack(alpha);
              }
        }

        public void writeToFile()
        {
            foreach (Wall wall in walls)
            {
                Vector2 pos = wall.getPhysicsPos();
                Console.Write(assetIndex + " ");
                Console.Write(pos.X + " " + pos.Y + " ");
                Console.Write(wall.getRot());
                Console.WriteLine("");
            }

        }
    }
}
