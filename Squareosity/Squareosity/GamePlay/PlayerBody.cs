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
    class PlayerBody
    {
         Vector2 pos = new Vector2(1024/2f, 768 / 2f); // graphics pos
         Vector2 orgin = new Vector2(10, 10);
         Texture2D tex;
         public bool isAlive = true;

         int score;

         List<playerLaser> playerLasers = new List<playerLaser>();
         ContentManager content;
         World world;

        public Body playerBody;

        public PlayerBody(Texture2D tex, World world, ContentManager content)
        {
            this.tex = tex;
            this.world = world;
            this.content = content;
            playerBody = BodyFactory.CreateRectangle(world, 20.0f / 64.0f, 20.0f / 64.0f, 1f, pos / 64.0f);
            playerBody.BodyType = BodyType.Dynamic;
            playerBody.Mass = 2f;
            playerBody.Friction = 0.5f;
            playerBody.BodyId = 1;
            playerBody.CollisionCategories = Category.Cat1;
            playerBody.CollidesWith = Category.All;

            score = 0;
            


        }

        public void update(GameTime gameTime)
        {
            detectInput();
            foreach (playerLaser laser in playerLasers)
            {
                laser.Update(gameTime);
            }

            for (int k = 0; k < playerLasers.Count(); k++)
            {
                if (playerLasers[k].hasCollied || playerLasers[k].hasDecayed)
                {
                    world.RemoveBody(playerLasers[k].laserBody);
                    playerLasers.RemoveAt(k);
                    
                }
            }
        }



        public void draw(SpriteBatch batch)
        {

            SpriteFont font = content.Load<SpriteFont>("gamefont");
            Vector2 textPosition = playerBody.Position * 64 - new Vector2(100, 100);

            batch.DrawString(font,score.ToString(), textPosition, Color.White);

           batch.Draw(tex, playerBody.Position * 64, null, Color.White, playerBody.Rotation, orgin, 1f, SpriteEffects.None, 1f);
           foreach (playerLaser laser in playerLasers)
           {
               laser.Draw(batch);

           }
      
        }

        public void detectInput()
        {
            // movement
            float x = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
            float y = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;
            playerBody.ApplyLinearImpulse(new Vector2(x, -y));
            playerBody.LinearDamping = 1f;

           
           if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right != Vector2.Zero)
            {
               Vector2 velo = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;

               velo = new Vector2(velo.X, -velo.Y);
               float rot = VectorToAngle(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right);
               playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"),velo,playerBody.Position,rot,world));
              
                
               
            }
            
          





        }

        float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.X, vector.Y);
        }

        float AngleToRads(float angle)
        {
            return (float)(angle * (Math.PI / 180.0f));       
    
        }

        public void updateScore(int amount)
        {
            score += amount;
        }

        public int getScore()
        {
            return score;
        }
    
    }


 }


