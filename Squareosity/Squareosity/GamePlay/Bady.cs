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
    class Bady
    {
        Vector2 pos;
        Texture2D tex;
        Texture2D laserTex;
        Path path;
        bool direction; // true for horiztal 
       public  List<Laser> Lasers;
        Body badyBody;
        public bool isDead = false;

        Body hitByLaser;
       public bool hitByPlayerLaser = false;
        World world;
        float time = 0;
        float timeLaser = 0;
        int health = 100;
        /// <summary>
        /// Creates a bady
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="pos">Physcis units</param>
        /// <param name="pathLenght"></param>
        /// <param name="direction">true for an x - axis movement</param>
        /// <param name="rotation">angle in degrees</param>
        /// <param name="content"></param>
        public Bady(Texture2D tex, Vector2 pos, int pathLenght, bool direction, float rotation ,ContentManager content,World world)
        {
            this.tex = tex;
            this.pos = pos;
            this.world = world;
            Lasers = new List<Laser>();
            laserTex = content.Load<Texture2D>("orangeLaser");
            this.direction = direction;

            // create the path the AI follows
            if (this.direction)
            {
                path = new Path();
                path.Add(pos);
                path.Add(new Vector2(pos.X + pathLenght, pos.Y));
                path.Add(new Vector2(pos.X  + pathLenght, pos.Y + 0.1f));
                path.Add(new Vector2(pos.X, pos.Y + 0.1f));
                path.Closed = true;
            }
            else
            {

                path = new Path();
                path.Add(this.pos);
                path.Add(new Vector2(pos.X, pos.Y  + pathLenght));
                path.Add(new Vector2(pos.X  - 0.1f, pos.Y  + pathLenght));
                path.Add(new Vector2(pos.X - 0.1f, pos.Y / 64));
                path.Closed = true;

            }

            //Badybody
            {
                badyBody = BodyFactory.CreateRectangle(world, tex.Width / 64.0f, tex.Height / 64.0f, 1f, pos);
                badyBody.BodyType = BodyType.Dynamic;
                badyBody.Mass = 10f;
                badyBody.CollisionCategories = Category.Cat4;
                badyBody.CollidesWith = Category.All ^ Category.Cat2;
                badyBody.Rotation = AngleToRads(rotation);
                badyBody.FixedRotation = true;

                badyBody.OnCollision += new OnCollisionEventHandler(OnCollision);
                badyBody.BodyId = 9;
            }   





        }
        public void Update(GameTime gameTime)
        {
         
            if (health <= 0)
            {
                isDead = true;
            }
            time += 0.001f;

            if (time > 1)
            {
                time = 0;
            }

          PathManager.MoveBodyOnPath(path, badyBody, time, 1f, 1f / 60f);
          
  
          timeLaser += 0.05f;

          if (timeLaser > 1f)
          {


              Lasers.Add(new Laser(badyBody,laserTex,world));
              
              timeLaser = 0;
          }

         for(int k = 0; k < Lasers.Count; k++)
         {
             Lasers[k].Update(gameTime);

             if (Lasers[k].hasHit)
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
        
       
        public void Draw(SpriteBatch batch)
        {
            batch.Draw(tex, badyBody.Position * 64, null, Color.White, badyBody.Rotation, new Vector2(tex.Width/2,tex.Height /2), 1f, SpriteEffects.None, 0.2f);

            foreach (Laser laser in Lasers)
            {

                laser.Draw(batch);

            }

        }

        float AngleToRads(float angle)
        {
            return (float)(angle * (Math.PI / 180.0f));

        }

        public bool OnCollision(Fixture FixtureA, Fixture FixtureB, Contact contact)
        {
            Body fixa = FixtureA.Body;
            Body fixb = FixtureB.Body;

            if (fixa.BodyId == 9 && fixb.BodyId == 10) // getting hit by players lasers 
            {
             
                hitByPlayerLaser = true;
                playerLaserBody = fixb;
            }

            return true;

        }

        /// <summary>
        /// Gets and sets the player laser body that just hit the player 
        /// </summary>
        public Body playerLaserBody
        {
            set { hitByLaser = value; }
            get { return hitByLaser; }
        }
        /// <summary>
        /// Removes delta amount of health from the bady
        /// </summary>
        public int deltaHealth
        {

            set { health -= value; }
        }

    }
}
