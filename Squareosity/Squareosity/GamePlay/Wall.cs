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

namespace Squareosity
{
    /// <summary>
    /// Wall class is used to create static 'walls' for mazes. 
    /// Postion vector passed is the GRAPHICS postion not the PHYSICS
    /// </summary>
    class Wall
    {
        Vector2 pos;
        Texture2D tex;
        Vector2 orgin;
        Body wallBody;

        bool isRightAngle = false;


        public Wall(Texture2D tex, Vector2 pos, bool isRightAngle, World world )
        {
            this.tex = tex;
            this.pos = pos;
            this.isRightAngle = isRightAngle; 

            wallBody = BodyFactory.CreateRectangle(world, tex.Width / 64.0f, tex.Height / 64.0f, 1f, pos / 64.0f);
            wallBody.BodyId = 4;
            wallBody.BodyType = BodyType.Static;
            wallBody.CollisionCategories = Category.Cat3;
            wallBody.CollidesWith = Category.All ^ Category.Cat2;

            orgin = new Vector2(tex.Width / 2.0f, tex.Height / 2.0f);

            if (isRightAngle)
            {
                wallBody.Rotation = 1.570796f; // need a function for deg to rads

            }
        }
        public Wall(Texture2D tex, Vector2 pos, float rot, World world)
        {
            this.tex = tex;
            this.pos = pos;
         
            wallBody = BodyFactory.CreateRectangle(world, tex.Width / 64.0f, tex.Height / 64.0f, 1f, pos / 64.0f);
            wallBody.BodyId = 4;
            wallBody.BodyType = BodyType.Static;
            wallBody.CollisionCategories = Category.Cat3;
            wallBody.CollidesWith = Category.All ^ Category.Cat2;

            orgin = new Vector2(tex.Width / 2.0f, tex.Height / 2.0f);


            wallBody.Rotation = rot;

            


        }

        public void Update() { }
        public void Draw(SpriteBatch batch)
        {

            batch.Draw(tex, wallBody.Position * 64, null, Color.White, wallBody.Rotation, orgin, 1f, SpriteEffects.None, 0f);


        }

        public float getRot()
        {
            return wallBody.Rotation;
        }

        public Vector2 getPhysicsPos()
        {
            return wallBody.Position;
        }
            
    }
}
