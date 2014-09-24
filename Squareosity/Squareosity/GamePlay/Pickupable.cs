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
    /// These objects can be moved by the player, the player can 'grab' them.
    /// </summary>
    class Pickupable
    {
        Texture2D tex;
        Vector2 pos;
        float distanceFromPlayer;
        World world;
        Body pickupableBody;
        bool isTouchingPlayer = false;
        bool isAttachedToPlayer = false;
       public Pickupable(Texture2D tex, Vector2 pos, World world)
        {
            this.pos = pos;
            this.tex = tex; 
           this.world = world;
           pickupableBody = BodyFactory.CreateRectangle(this.world,tex.Width / 64f,tex.Height/ 64f,1f,this.pos / 64f);
           pickupableBody.BodyType = BodyType.Dynamic;
           pickupableBody.Mass = 0.5f; // 4 is good 
           pickupableBody.BodyId = 19;
           pickupableBody.CollisionCategories = Category.Cat7;
           pickupableBody.CollidesWith = Category.All;/*^ Category.Cat2; Would remove player laser collision*/
           pickupableBody.OnCollision +=new OnCollisionEventHandler(pickupableBody_OnCollision);
           pickupableBody.OnSeparation +=new OnSeparationEventHandler(pickupableBody_OnSeparation);


            /*
             squareBody = BodyFactory.CreateRectangle(world, size, size, 1f, pos / 64);
             squareBody.BodyType = BodyType.Static;
             squareBody.BodyId = 2;
             squareBody.CollisionCategories = Category.Cat7;
             squareBody.CollidesWith = Category.All ^ Category.Cat2;
             squareBody.OnCollision += new OnCollisionEventHandler(OnCollision);
             squareBody.OnSeparation += new OnSeparationEventHandler(OnSeparation);
             */


        }
        public void Draw(SpriteBatch batch)
       {
           batch.Draw(tex, pickupableBody.Position * 64, null, Color.White, pickupableBody.Rotation, new Vector2(tex.Width / 2, tex.Height / 2), 1f, SpriteEffects.None, 0.5f);
       }
        public void getDistanceFromPlayer()
        {
        

        }
        public bool getSetIsTouchingPlayer
        {
            get { return isTouchingPlayer; }
            set { isTouchingPlayer = value; }
        }
        public bool getSetIsAttachedToPlayer
        {
            get { return isAttachedToPlayer; }
            set { isAttachedToPlayer = value; }
        }


   

        public bool pickupableBody_OnCollision( Fixture fixa, Fixture fixb, Contact contact)
        {
            if (fixb.Body.BodyId == 1)
            {
                isTouchingPlayer = true;
            }
            return true;
        }
        public void pickupableBody_OnSeparation(Fixture fixa, Fixture fixb)
        {
            if (fixb.Body.BodyId == 1)
            {
                isTouchingPlayer = false;
            }
            
        }

        public Body getBody
        {
            get { return pickupableBody; }
        }
    }
}
