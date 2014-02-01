using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GameStateManagement;

namespace Squareosity
{
    class DeadScreen : GameScreen
    {
        ContentManager content;
        SpriteBatch batch;

        float pauseAlpha;

        InputAction pauseAction;
        BloomComponent bloom;
        public static int bloomSettingsIndex = 0;
        Texture2D tex;

        int levelFrom;

        public DeadScreen(int levelfrom)
        {
            this.levelFrom = levelfrom;
            // define and action 
            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);
        
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);


        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                {
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                }
                bloom = new BloomComponent(ScreenManager.Game);
                ScreenManager.Game.Components.Add(bloom);
                tex = content.Load<Texture2D>("DeadScreen/redDeathScreen");




                ScreenManager.Game.ResetElapsedTime();
        
            }

            
            base.Activate(instancePreserved);
        }
        public override void Deactivate()
        {
            base.Deactivate();
        }
        public override void Unload()
        {
            content.Unload();
        }
        public override void Update(GameTime gameTime, bool otherScreenHasFocus
            ,bool coveredByOtherScreen)
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


            }

        }

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
                
                 bloom.Visible = false;

                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);

             }
            else
            {
                
                if (keyboardState.IsKeyDown(Keys.A) || gamePadState.Buttons.A == ButtonState.Pressed)
                {
                    if (levelFrom == 1)
                    {

                        ExitScreen();
                       
                    LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new GameplayScreen());
                       
                    }
                    else if (levelFrom == 2)
                    {
                        ExitScreen();
                      
                        LoadingScreen.Load(ScreenManager, false,ControllingPlayer, new level2Screen());
                    
                    }
                    else if (levelFrom == 3)
                    {
                        ExitScreen();

                        LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new level3Screen());
                    }
                    
                }
               // enter or a is continue 
               // b or esc is back ect 
            }
        }

        public override void Draw(GameTime gameTime)
        {
            bloom.BeginDraw();
            batch = ScreenManager.SpriteBatch;
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            batch.Begin();

            
                batch.Draw(tex,new Vector2(512,384),null,Color.White,0f,new Vector2(tex.Width / 2, tex.Height / 2),1f,SpriteEffects.None,1f);


            batch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);

            }
            base.Draw(gameTime);
        } 

    }
}
