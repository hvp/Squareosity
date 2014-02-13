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
    class ActOneScreen : MenuScreen
    {
        #region Fields

        MenuEntry tut;
        MenuEntry chapterOne;


        string tutText = "Welcome to the System.";
        string chapterOneText = "Training wheels.";
        
        
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public ActOneScreen()
            : base("Act One")
        {
            // Create our menu entries.
             tut = new MenuEntry(tutText);
           chapterOne = new MenuEntry(chapterOneText);
          
 
            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            
            back.Selected += OnCancel;
            tut.Selected +=  tut_Selected;
            chapterOne.Selected += chapterOne_Selected;


            // Add entries to the menu.
            MenuEntries.Add(tut);
            MenuEntries.Add(chapterOne);
            
         
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

        void  tut_Selected(object sender, PlayerIndexEventArgs e)
        {
         LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new TutLevel1());

        }
        void chapterOne_Selected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new ChapterOne());

        }
        
    


        #endregion
    }
}
