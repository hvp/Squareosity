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
    class ActSelectionScreen : MenuScreen
    {
        #region Fields

        MenuEntry actOne;
      

        string actOneText = "Act One.";
    
        
        
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public ActSelectionScreen()
            : base("Act Select")
        {
            // Create our menu entries.
            actOne = new MenuEntry(actOneText);
            
            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            
            back.Selected += OnCancel;
            actOne.Selected += actOne_Selected;
            
            


            // Add entries to the menu.
            MenuEntries.Add(actOne);
           
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


        void actOne_Selected(object sender, PlayerIndexEventArgs e)
        {
            //LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
              //                  new TutLevel1());
            ScreenManager.AddScreen(new ActOneScreen(),PlayerIndex.One);
        }
       


        #endregion
    }
}
