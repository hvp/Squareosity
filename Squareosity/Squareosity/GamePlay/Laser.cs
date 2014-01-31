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
    class Laser
    {
      public  bool hasHit = false;
      public bool hasHitElse = false;
      public Body laserBody;

      Texture2D tex;
      float rads;

      float decayTime = 250;
      public bool hasDecayed = false;
      float counter = 0;

        public Laser(Body badyBody, Texture2D tex, World world)
        {
            laserBody = BodyFactory.CreateRectangle(world, 4f / 64f, 10f / 64f, 1f, badyBody.Position);

            this.tex = tex;
            laserBody.CollisionCategories = Category.Cat2;
            laserBody.BodyType = BodyType.Dynamic;
            laserBody.Rotation = badyBody.Rotation;
            laserBody.FixedRotation = true;
            laserBody.BodyId = 7;

            rads = laserBody.Rotation;

            if (rads <= 1.57079633f) // 90
            {
                laserBody.LinearVelocity = new Vector2(5, 0);
            }
            else if(rads <= 3.14159265f) // 180
            {
                laserBody.LinearVelocity = new Vector2(0, 5);
            }

           

            laserBody.OnCollision += new OnCollisionEventHandler(OnCollision);

        }

        public Laser(Texture2D tex, Vector2 pos,Vector2 velocity, float rot, World world)
        {
            laserBody = BodyFactory.CreateCircle(world,2.5f / 64, 1f, pos);
                   
            this.rads = rot;
            this.tex = tex;
            laserBody.CollisionCategories = Category.Cat2;
            laserBody.CollidesWith = Category.All ^ Category.Cat2;
            laserBody.BodyType = BodyType.Dynamic;
            laserBody.Rotation = rads;
            laserBody.FixedRotation = true;
            laserBody.BodyId = 18;

            laserBody.LinearVelocity = velocity;

            laserBody.OnCollision += new OnCollisionEventHandler(OnCollision);
        }

        public bool OnCollision(Fixture FixtureA, Fixture FixtureB, Contact contact)
        {
          Body fixa = FixtureA.Body;
          Body fixb = FixtureB.Body;

          if (fixa.BodyId == 7 && fixb.BodyId == 1)
          {
              hasHit = true;
          }
          else
          {
              hasHitElse = true;
          }
            
            

            return true;
        }
        public void Update(GameTime gameTime)
        {

            counter += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (counter >= decayTime)
            {
                hasDecayed = true;

            }

        }
        public void Draw(SpriteBatch batch)
        {
            
            batch.Draw(tex, laserBody.Position * 64, null, Color.White, laserBody.Rotation, new Vector2(tex.Width / 2f, tex.Height / 2f), 1f, SpriteEffects.None, 1f);


        }
    }
}
