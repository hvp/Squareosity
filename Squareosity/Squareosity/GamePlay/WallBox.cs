using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;



namespace Squareosity
{
    /// <summary>
    /// need to make new art that means that laser break on collision with this type of wall.
    /// </summary>
    class WallBox
    {
        int lenght;
        int height;
        Vector2 position;
        List<Wall> Walls = new List<Wall>();
        ContentManager content;
        World world;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lenght"> Number of horzital walls</param>
        /// <param name="height">Number of vertical walls</param>
        /// <param name="position"></param>
        /// <param name="world"></param>
        public WallBox(int lenght, int height, Vector2 position ,World world, ContentManager content)
        {
            this.lenght = lenght;
            this.height = height;
            this.position = position;
            this.world = world;
            int space = 0;
            for (int k = 0; k < lenght; ++k)
            {
                // top row 
                Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"),
                    new Vector2(position.X, position.Y - 50),true, world));
                // bottom row
                Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"),
                  new Vector2(position.X , position.Y + 50 ), true, world));

                space += 100;
            }

            space = 0;
            for (int k = 0; k < height; ++k)
            {
                // left side 
                Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"),
                   new Vector2(position.X - 50, position.Y ), false, world));
               // right side 
                Walls.Add(new Wall(content.Load<Texture2D>("Walls/blueWallMedium"),
                  new Vector2(position.X + 50, position.Y), false, world));
               


            }



        }

        public void Draw(SpriteBatch batch)
        {
            foreach (Wall wall in Walls)
            {
                wall.Draw(batch);
            }
        }
        public void removeWalls()
        {
            foreach (Wall wall in Walls)
            {
                world.RemoveBody(wall.WallBody);

            }

            Walls.Clear();
        }

    }

}
