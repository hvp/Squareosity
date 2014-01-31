
using System;
using GameStateManagement;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Squareosity
{
   public class SquareosityGame : Microsoft.Xna.Framework.Game
    {
         GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        ScreenFactory screenFactory;
       
        public static int progess = 1;



        /// <summary>
        /// The main game constructor.
        /// </summary>
        public SquareosityGame()
        {
            Content.RootDirectory = "Content";

         
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1024;
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            Mouse.WindowHandle = Window.Handle;

            // Create the screen factory and add it to the Services
            screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);


            // On Windows and Xbox we just add the initial screens
            AddInitialScreens();

        }

         private void AddInitialScreens()
        {
            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);

          

            screenManager.AddScreen(new MainMenuScreen(), null);

        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }

   


    }
}
