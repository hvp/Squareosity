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
    class playerLaser
    {
        Vector2 velocity;
        Texture2D texture;
       public bool hasCollied;
        int speed = 10;
       public Body laserBody;
       float decayTime = 1000;
       public bool hasDecayed = false;
       float counter = 0;

       int damage; 

        public playerLaser(Texture2D tex,Vector2 velocity,Vector2 pos ,float roatation, int damage,World world)
        {
            laserBody = BodyFactory.CreateRectangle(world, 4f / 64f, 10f / 64f, 1f, pos);
            texture = tex;
            
            this.velocity = velocity;

            hasCollied = false;
            this.damage = damage;

            laserBody.Rotation = roatation;
            laserBody.BodyType = BodyType.Dynamic;
            laserBody.FixedRotation = true;
            laserBody.CollisionCategories = Category.Cat5;
            laserBody.CollidesWith = Category.All  ^ Category.Cat1 ^ Category.Cat7 ^  Category.Cat5 ^  Category.Cat8;
            laserBody.BodyId = 10;


            laserBody.OnCollision += new OnCollisionEventHandler(OnCollision);

                 


        }

        public void Update(GameTime gameTime)
        {
                velocity.Normalize();
                laserBody.LinearVelocity = velocity * speed;

                counter += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (counter >= decayTime)
                {
                    hasDecayed = true;

                }

            

        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(texture, laserBody.Position * 64, null, Color.White, laserBody.Rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0.8f);


        }
        public bool OnCollision(Fixture FixtureA, Fixture FixtureB, Contact contact)
        {
            hasCollied = true;

            return true;
        }
        public int getDamage
        {
            get { return damage; }
        }
    
    }
}
