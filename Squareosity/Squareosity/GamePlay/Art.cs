using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Squareosity
{
    static class Art
    {
        public static Texture2D Player { get; private set; }
    
        public static Texture2D Pixel { get; private set; }		// a single white pixel

    

        public static void Load(ContentManager content)
        {
            Player = content.Load<Texture2D>("Player");
           

            Pixel = new Texture2D(Player.GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });

          
        }
    }
}
