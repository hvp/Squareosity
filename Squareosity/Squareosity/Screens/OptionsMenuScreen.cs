#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace Squareosity
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry tutlevel;
        MenuEntry level1;
        MenuEntry level2;
        MenuEntry level3;

        string tutlevelText = "Welcome to the System.";
        string level1Text = "Level 1: Spaaace!";
        string level2Text = "Level 2: More Spaaace!";
        string level3Text = "Level 3: Test Your Might!";
        
        
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Level Select")
        {
            // Create our menu entries.
            tutlevel = new MenuEntry(tutlevelText);
            level1 = new MenuEntry(level1Text);
            level2 = new MenuEntry(level2Text);
            level3 = new MenuEntry(level3Text);
            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            
            back.Selected += OnCancel;
            tutlevel.Selected += tutlevel_Selected;
            level1.Selected += level1_Selected;
            level2.Selected += level2_Selected;
            level3.Selected += level3_Selected;
            


            // Add entries to the menu.
            MenuEntries.Add(tutlevel);
            MenuEntries.Add(level1);
            MenuEntries.Add(level2);
            MenuEntries.Add(level3);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            
        }


        #endregion

        #region Handle Input


        void tutlevel_Selected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                                new TutLevel1());
        }
        void level1_Selected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                             new GameplayScreen());
        }

        void level2_Selected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                             new level2Screen());
        }
        void level3_Selected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager,true,e.PlayerIndex,
                                new level3Screen());
        }


        #endregion
    }
}
