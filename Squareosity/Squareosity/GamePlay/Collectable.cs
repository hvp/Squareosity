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

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

namespace Squareosity
{
    class Collectable
    {
        Texture2D tex;
        Vector2 pos;
        Vector2 orgin;

        public bool collected = false;

        public Body collectableBody;
        
        public Collectable(Texture2D tex, Vector2 pos, World world)
        {

            this.tex = tex;
            this.pos = pos;

            collectableBody = BodyFactory.CreateRectangle(world, tex.Width / 64.0f, tex.Height / 64.0f, 1f,pos / 64.0f);
            collectableBody.BodyType = BodyType.Static;
            collectableBody.BodyId = 6;
            collectableBody.CollisionCategories = Category.Cat2;
            collectableBody.OnCollision += new OnCollisionEventHandler(OnCollision);


        }

        public void Update()
        {

           
         


        }

        public bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {

            Body fixA = fixtureA.Body;
            Body fixB = fixtureB.Body;

            if (fixA.BodyId == 6 && fixB.BodyId == 1)
            {
                collected = true;
            }

            return true;
        }

        public void draw(SpriteBatch batch)
        {
            batch.Draw(tex, collectableBody.Position * 64, null, Color.White,
                collectableBody.Rotation, new Vector2(tex.Width / 2, tex.Height / 2), 1f, SpriteEffects.None, 1f);

                

        }
        /// <summary>
        /// True if the item has been collected by the player.
        /// </summary>
        public bool Collected
        {
            get { return collected; }
        }

    }
}
