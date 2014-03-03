﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

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
    class TutLevel1 : GameScreen
    {

        #region Fields

        ContentManager content;
        SpriteFont gameFont;
        PlayerBody playerBody;

        bool laserActive = false;
        bool scoreActive = false;
        MouseState mouse;
        Cam2d cam2D;
        BloomComponent bloom;
        public static int bloomSettingsIndex = 0;
        public static World world;
        float pauseAlpha;

        List<Wall> Walls = new List<Wall>();
        List<Bady> Badies = new List<Bady>();
        List<Square> Squares = new List<Square>();
        List<SeekerDrone> Drones = new List<SeekerDrone>();
        List<Collectable> Collectables = new List<Collectable>();
        List<DistanceJoint> Joints = new List<DistanceJoint>();
        List<Pickupable> pickuables = new List<Pickupable>();
        List<Area> Areas = new List<Area>();

        Area pinkArea;

        Texture2D reticle;
        InputAction pauseAction;

        ChoiceDisplay UI;


        SoundEffect WelcomeSFX, BlueSFX, PinkSFX, QuestionsSFX, iniProgressSFX,progressLoadedSFX,
            OneSFX,TwoSFX,ThreeSFX,OpoSFX,SystemSFX,EntryPointSFX;

        SoundEffectInstance WelcomeSFXInstance, BlueSFXInstance, PinkSFXInstance, QuestionsSFXInstance,
            iniProgressSFXInstance, progressLoadedSFXInstance, OneSFXInstance, TwoSFXInstance, ThreeSFXInstance,
            OpoSFXInstance, SystemSFXInstance, EntryPointSFXInstance;


   

        double Timer = 0;

        // variables for level progression 
        double timerForGreenTest = 750;
        bool GreenTestStarted = false;
        bool GreenTestComplete = false;


        bool BlueTestStarted = false;
        bool BlueTestComplete = false;

        bool PinkTestStarted = false;
        bool PinkTestCompleted = false;

        bool questionsSpoken = false;

        bool countDownStated = false;

        bool pressedOk = false;
        bool progressLoaded = false;

        bool questionOneAsked = false;
        bool questionTwoAsked = false;

        bool sayOne = false;
        bool sayTwo = false;
        bool sayThree = false;
        int choice = 0;






        #endregion

        #region Initialization


        /// <summary>
        /// Media player is lagging as hell. Use soundFX instead.
        /// </summary>

        public TutLevel1()
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
                /*
                 * Welcome, Blue, Pink, iniProgress, progressLoadedSFX, 
            Questions, One, Two, Three, Opo, System, entryPoint;
                 * 
                 */

                WelcomeSFX = content.Load<SoundEffect>("Audio/SoundFX/Vivi_Tut/Vivi_Welcome_UK_WAV");
                BlueSFX = content.Load<SoundEffect>("Audio/SoundFX/Vivi_Tut/Vivi_BlueSquare_UK_WAV");
                PinkSFX = content.Load<SoundEffect>("Audio/SoundFX/Vivi_Tut/Vivi_PinkSquare_UK_WAV");
                QuestionsSFX = content.Load<SoundEffect>("Audio/SoundFX/Vivi_Tut/Vivi_Questions_UK_WAV");
                progressLoadedSFX = content.Load<SoundEffect>("Audio/SoundFX/Vivi_Tut/Vivi_ProgessLoaded_WAV");
                iniProgressSFX = content.Load<SoundEffect>("Audio/SoundFX/Vivi_Tut/Vivi_Progress_Tracker_WAV");

                OneSFX = content.Load<SoundEffect>("Audio/SoundFX/Vivi_Tut/Vivi_One_WAV");
                TwoSFX = content.Load<SoundEffect>("Audio/SoundFX/Vivi_Tut/Vivi_Two_WAV");
                ThreeSFX = content.Load<SoundEffect>("Audio/SoundFX/Vivi_Tut/Vivi_Three_WAV");
                OpoSFX = content.Load<SoundEffect>("Audio/SoundFX/Vivi_Tut/Vivi_You_are_opo_UK_WAV");
                SystemSFX = content.Load<SoundEffect>("Audio/SoundFX/Vivi_Tut/Vivi_System_UK_WAV");
                EntryPointSFX = content.Load<SoundEffect>("Audio/SoundFX/Vivi_Tut/Vivi_EntryPoint_WAV");



                WelcomeSFXInstance = WelcomeSFX.CreateInstance();
                BlueSFXInstance = BlueSFX.CreateInstance();
                PinkSFXInstance = PinkSFX.CreateInstance();
                QuestionsSFXInstance = QuestionsSFX.CreateInstance();
                iniProgressSFXInstance = iniProgressSFX.CreateInstance();
                progressLoadedSFXInstance = progressLoadedSFX.CreateInstance();
                OneSFXInstance = OneSFX.CreateInstance();
                TwoSFXInstance = TwoSFX.CreateInstance();
                ThreeSFXInstance = ThreeSFX.CreateInstance();
                OpoSFXInstance = OpoSFX.CreateInstance();
                SystemSFXInstance = SystemSFX.CreateInstance();
                EntryPointSFXInstance = EntryPointSFX.CreateInstance();

            
               
                
                UI = new ChoiceDisplay(content.Load<Texture2D>("a-Button"), content.Load<Texture2D>("b-Button"),
                    content.Load<Texture2D>("x-Button"), null, "No, Let's go!.", "Who am I?", "Where am I?!", null, content);
                UI.Acitve = false;



                cam2D = new Cam2d(ScreenManager.GraphicsDevice);
                reticle = content.Load<Texture2D>("redReticle");
                /// player 
                playerBody = new PlayerBody(content.Load<Texture2D>("redPlayer"), world, content);
                playerBody.getSetLaserStatus = false;
                playerBody.getSetDrawScore = false;
                playerBody.getSetChoicePoint = false;

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

                foreach (Pickupable pickupable in pickuables)
                {
                    if (pickupable.getSetIsTouchingPlayer && playerBody.getSetWantsToPickUp && !playerBody.getSetHasPickedUp)
                    {
                        // joints.Add ( JointFactory.CreateRevoluteJoint(world,playerBody.playerBody,
                        //   pickupable.getBody, new Vector2(0,0)));

                        Joints.Add(JointFactory.CreateDistanceJoint(world, playerBody.playerBody, pickupable.getBody, new Vector2(0, 0), new Vector2(0, 0)));


                        playerBody.getSetWantsToPickUp = false;
                        pickupable.getSetIsAttachedToPlayer = true;



                        playerBody.getSetHasPickedUp = true;

                    }

                    if (Joints.Count > 0 && playerBody.getSetWantsTodrop && playerBody.getSetHasPickedUp && pickupable.getSetIsAttachedToPlayer)
                    {
                        // for some reason the on seperation dosn't work when removing a joint. So we force the pickupable to not touching the player 
                        world.RemoveJoint(Joints[0]);
                        Joints.RemoveAt(0);
                        playerBody.getSetHasPickedUp = false;
                        pickupable.getSetIsAttachedToPlayer = false;
                        pickupable.getSetIsTouchingPlayer = false;


                    }
                }


                if (playerBody.isAlive == false)
                {
                    bloom.Visible = false;

                    world.Clear();


                    this.ExitScreen();

                    LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new DeadScreen(0));

                }



                //game script 


                if (GreenTestStarted == false)
                {
                    Timer += gameTime.ElapsedGameTime.Milliseconds;
                }
                if (Timer > timerForGreenTest && GreenTestStarted == false && MediaPlayer.State == MediaState.Stopped)
                {

                    Timer = 0;
                    GreenTestStarted = true;


                    UI.setSub = "Welcome my name is Vivi. Move to and touch the red square to continue.";
                    Squares.Add(new Square(content.Load<Texture2D>("Squares/redSquare"), new Vector2(800, 100), true, world));
                    //      MediaPlayer.Play(Welcome);
                    WelcomeSFXInstance.Play();
                }

                if (GreenTestStarted == true && GreenTestComplete == false)
                {
                    if (Squares[0].isTouching)
                    {
                        GreenTestComplete = true;
                        BlueTestStarted = true;
                        UI.setSub = "Wonderful, now fire your laser at the blue square.";
                        //  MediaPlayer.Play(Blue);
                        if (WelcomeSFXInstance.State == SoundState.Stopped)
                        {
                            WelcomeSFXInstance.Stop();
                            BlueSFX.Play();
                        }

                        Squares.Add(new Square(content.Load<Texture2D>("Squares/blueSquare"), new Vector2(100, 500), true, world));
                        playerBody.getSetLaserStatus = true;
                        world.RemoveBody(Squares[0].squareBody);
                        Squares.RemoveAt(0);
                    }


                }


                if (BlueTestStarted && BlueTestComplete == false)
                {
                    if (Squares[0].isTouchingLaser)
                    {
                        BlueTestComplete = true;
                        world.RemoveBody(Squares[0].squareBody);
                        Squares.RemoveAt(0);

                        UI.setSub = "Excellent, now pick up and move the pink square into the pink area.";
                        //    MediaPlayer.Play(Pink);

                        PinkSFX.Play();
                        pickuables.Add(new Pickupable(content.Load<Texture2D>("Squares/pinkSquare"), new Vector2(10, 50), world));
                        Areas.Add(new Area(content.Load<Texture2D>("pinkArea"), new Vector2(800, 500), 1.57f, world));

                        Timer = 0;

                    }

                }

                if (BlueTestComplete && GreenTestComplete)
                {
                    PinkTestStarted = true;

                    if (PinkTestCompleted == false)
                    {
                        if (Areas[0].getIsPickUpTouching && playerBody.getSetHasPickedUp == false)
                        {
                            PinkTestCompleted = true;
                            pickuables.RemoveAt(0);
                            Areas.RemoveAt(0);

                        }
                    }


                }

                // need to add bool for sound fx here. 
                if (PinkTestCompleted && UI.Acitve == false && countDownStated == false)
                {

                    if (questionsSpoken == false)
                    {
                        UI.setSub = "You are confirmed operational. Remember only blue, red and pink neon is safe to touch. Questions?";
                        QuestionsSFX.Play();
                        questionsSpoken = true;
                    }
                    Timer += gameTime.ElapsedGameTime.Milliseconds;
                    if (Timer > 2500)
                    {
                        UI.Acitve = true;
                        playerBody.getSetChoicePoint = true;
                        countDownStated = true;
                        Timer = 0;

                    }
                }
                if (BlueTestComplete && GreenTestComplete && PinkTestCompleted && pressedOk == false)
                {


                    if (playerBody.choiceValue == 2 && questionOneAsked == false)
                    {
                        UI.setSub = "You are designated as an Operational Perpetuating Organism. Or Opo.";
                        UI.setTextA = "Ok, let's go!";
                        OpoSFXInstance.Play();
                        questionOneAsked = true;


                    }
                    else if (playerBody.choiceValue == 3 && questionTwoAsked == false)
                    {
                        UI.setSub = "You are in the System.";
                        UI.setTextA = "Ok, let's go!";
                        SystemSFXInstance.Play();
                        questionTwoAsked = true;

                    }
                    else if (playerBody.choiceValue == 1 )
                    {
                        pressedOk = true;
                        UI.Acitve = false;
                        UI.setSub = "Loading progress tracker.";
                        Timer = 0;
                        iniProgressSFXInstance.Play();


                    }

                }

                if (pressedOk && progressLoaded == false)
                {
                    Timer += gameTime.ElapsedGameTime.Milliseconds;
                    if (iniProgressSFXInstance.State != SoundState.Playing && sayThree == false)
                    {
                        UI.setSub = "Three...";
                        ThreeSFXInstance.Play();
                        sayThree = true;

                    }
                    if (ThreeSFXInstance.State != SoundState.Playing && sayTwo == false)
                    {
                        UI.setSub = "Two...";
                        TwoSFXInstance.Play();
                        sayTwo = true;

                    }
                    
                    if (TwoSFXInstance.State != SoundState.Playing && sayOne == false)
                    {
                        UI.setSub = "One...";
                        OneSFXInstance.Play();
                        sayOne = true;

                    }
                    if (OneSFXInstance.State != SoundState.Playing)
                    {
                        UI.setSub = "Progress module loaded.";
                        progressLoadedSFXInstance.Play();
                        playerBody.getSetDrawScore = true;
                        progressLoaded = true;
                        Timer = 0;
                    }

                }

                if (progressLoaded)
                {
                    Timer += gameTime.ElapsedGameTime.Milliseconds;
                    if (Timer > 2000 && Timer < 4000 && EntryPointSFXInstance.State != SoundState.Playing)
                    {
                        UI.setSub = "Loading courpution entry point.";
                        EntryPointSFXInstance.Play();


                    }
                    if (Timer > 4000 && Timer < 6000 && ThreeSFXInstance.State != SoundState.Playing)
                    {

                        UI.setSub = "Three...";
                        ThreeSFXInstance.Play();
                    }
                    if (Timer > 6000 && Timer < 8000 && TwoSFXInstance.State != SoundState.Playing)
                    {
                        UI.setSub = "Two...";
                        TwoSFXInstance.Play();

                    }
                    if (Timer > 8000 && Timer < 10000 && OneSFXInstance.State != SoundState.Playing)
                    {
                        UI.setSub = "One...";
                        OneSFXInstance.Play();


                    }
                    if (Timer > 10000)
                    {
                        bloom.Visible = false;
                        ExitScreen();
                        LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new ChapterOne());
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
                if (MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Pause();
                }

                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);

            }
            else
            {
                if (MediaPlayer.State == MediaState.Paused)
                {
                    MediaPlayer.Resume();
                }
                playerBody.detectInput(keyboardState, mouse, cam2D.Position, gameTime);
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
            foreach (SeekerDrone drone in Drones)
            {
                drone.draw(spriteBatch);
            }

            foreach (Square square in Squares)
            {
                square.Draw(spriteBatch);
                if (square.isTouching)
                {

                }
            }
            foreach (Area area in Areas)
            {
                area.Draw(spriteBatch);
            }
            foreach (Wall wall in Walls)
            {
                wall.Draw(spriteBatch);

            }
            foreach (Pickupable pickupable in pickuables)
            {
                pickupable.Draw(spriteBatch);

            }

            if ((!GamePad.GetState(PlayerIndex.One).IsConnected) && playerBody.getSetLaserStatus)
            {

                Vector2 mousePos = new Vector2(mouse.X, mouse.Y);
                spriteBatch.Draw(reticle, mousePos + cam2D.Position - new Vector2(1024 / 2, 768 / 2), null, Color.White, 0f, new Vector2(10, 10), 1f, SpriteEffects.None, 1f);

            }


            playerBody.draw(spriteBatch);




            spriteBatch.End();

            spriteBatch.Begin();

            UI.Draw(spriteBatch);
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
