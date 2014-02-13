#region using statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GameStateManagement;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Joints;
#endregion

namespace Squareosity
{
    /**
     *Need some way of getting the drones to stop from clustering 
     * 
     */


    class SeekerDrone
    {
        Texture2D tex;
        Vector2 pos;
        World world;
        int health = 100;
        int range;
        Body droneBody;
        ContentManager content;
        List<Laser> Lasers = new List<Laser>();
        Texture2D laserTex;
        Vector2 target = new Vector2(0,0);
        bool isDead = false;
        bool isStationary = false;
        float fireRate = 250;
        float counter = 0;

        KeyboardState keyState;
        // should fire lasers at player 
        public SeekerDrone(Texture2D tex, Vector2 pos, World world, ContentManager content, float dampning) // might add two textures
        {
            this.content = content;
            this.tex = tex;
            this.world = world;
            this.pos = pos;

            Lasers = new List<Laser>();
            laserTex = content.Load<Texture2D>("YellowCircleLaserSmall");

            pos = pos / 64;
            droneBody = BodyFactory.CreateCircle(world, 10.0f / 64, 1f, pos);

            droneBody.BodyType = BodyType.Dynamic;
            droneBody.Mass = 2f;
            droneBody.CollisionCategories = Category.Cat4;
            droneBody.FixedRotation = true;
            droneBody.CollidesWith = Category.All ^ Category.Cat2;
            droneBody.OnCollision+=new OnCollisionEventHandler(droneBody_OnCollision);
            droneBody.BodyId = 15;
            droneBody.Restitution = 1f;
            droneBody.LinearDamping = dampning;

        }

        public void update(GameTime gameTime)
        {
            // if(inRange)
            if (isStationary)
            {
                fireRate = 100;
                droneBody.BodyType = BodyType.Static;
            }
            else
            {
                fireRate = 350;
                droneBody.BodyType = BodyType.Dynamic;
            }

            
            keyState = Keyboard.GetState();
            double deltaX;
            double deltaY;
        
            double rads;
            Vector2 direction; // velocity 


            deltaX = target.X - droneBody.Position.X * 64; // target is in display units
            deltaY = target.Y - droneBody.Position.Y * 64;
            rads = Math.Atan(deltaY / deltaX);
          
            direction = getVectorFromRads(rads);

         //   if (keyState.IsKeyDown(Keys.F))
          //  {

            counter += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (counter > fireRate)
            {
                if (target.X < droneBody.Position.X * 64)
                {
                    Lasers.Add(new Laser(laserTex, droneBody.Position, -direction * 10, (float)rads, world));

                }
                else //if(target.X > droneBody.Position.X)
                {
                    Lasers.Add(new Laser(laserTex, droneBody.Position, direction * 10, (float)rads, world));

                }
                counter = 0;
            }

            // }
            //  drone's seeking 
            {
                direction.Normalize();

                if (!isStationary)
                {
                    if (target.X < droneBody.Position.X * 64)
                    {
                        droneBody.ApplyLinearImpulse(-direction * 0.2f);
                    }
                    else
                    {
                        droneBody.ApplyLinearImpulse(direction * 0.2f);
                    }
                }
            }






            if (health <= 0)
                isDead = true;

            for (int k = 0; k < Lasers.Count; k++)
            {

                Lasers[k].Update(gameTime);
                if (Lasers[k].hasHit || Lasers[k].hasDecayed)
                {
                    world.RemoveBody(Lasers[k].laserBody);
                    Lasers.RemoveAt(k);


                }
                else
                {

                    if (Lasers[k].hasHitElse)
                    {
                        world.RemoveBody(Lasers[k].laserBody);
                        Lasers.RemoveAt(k);

                    }
                }
            }

        }

        public void draw(SpriteBatch batch)
        {
            batch.Draw(tex, droneBody.Position * 64, null, Color.White, droneBody.Rotation, new Vector2(10f,10f), 1f, SpriteEffects.None, 1f);

            foreach (Laser laser in Lasers)
            {

                laser.Draw(batch);

            }

        }

        public bool droneBody_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {

            Body fixA = fixtureA.Body;
            Body fixB = fixtureB.Body;
            if (fixA.BodyId == 15 && fixB.BodyId == 10) // player laser collision 
            {
                health -= 10;

            }

            return true;
        }
        public bool Stationary
        {
            set
            {
                isStationary = value;

            }
            get { return isStationary; }
        }

        public bool getIsDead
        {
            get { return isDead; }

        }
        public Body getBody
        {
            get { return droneBody; }

        }
        public void setTarget(Vector2 target)
        {
            this.target = target;
        }
        public Vector2 getVectorFromRads(double radians)
        {

        return new Vector2((float)Math.Cos((double)radians), (float)Math.Sin((double)radians));
        }
    }
}
