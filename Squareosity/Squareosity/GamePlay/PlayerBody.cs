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
        enum PowerUp
        {
            None,
            PowerLaserBlast

        };

         PowerUp currentPower = PowerUp.PowerLaserBlast;
         Vector2 pos = new Vector2(1024/2f, 768 / 2f); // graphics pos
         Vector2 orgin = new Vector2(10, 10);
         Texture2D tex;
         public bool isAlive = true;

         KeyboardState oldKeyState;
         int health = 100;
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
            playerBody.CollidesWith = Category.All ^ Category.Cat5;
            playerBody.OnCollision +=new OnCollisionEventHandler(playerBody_OnCollision);
            score = 0;
            


        }

        public void update(GameTime gameTime)
        {
            if (health <= 0)
            {
                isAlive = false;
            }
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

        public void detectInput(KeyboardState keyboardState, MouseState mouse, Vector2 camPos)
        {
            
            // movement with gamePad.
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                float x = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
                float y = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;
                playerBody.ApplyLinearImpulse(new Vector2(x, -y));
                playerBody.LinearDamping = 1f;

               
                if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right != Vector2.Zero)
                {
                    Vector2 velo = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;

                    velo = new Vector2(velo.X, -velo.Y);
                    float rot = VectorToAngle(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right);
                    playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), velo, playerBody.Position, rot, 2 ,world));



                }



            }
            else
            {
                playerBody.LinearDamping = 1f;
               
              
                if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                {
                    this.playerBody.ApplyLinearImpulse(new Vector2(-1, 0));
                }
                if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                {
                    this.playerBody.ApplyLinearImpulse(new Vector2(1, 0));
                }
                if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                {
                    this.playerBody.ApplyLinearImpulse(new Vector2(0, 1));
                }
                if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                {
                    this.playerBody.ApplyLinearImpulse(new Vector2(0, -1));
                }

                if (keyboardState.IsKeyDown(Keys.Space) && oldKeyState.IsKeyUp(Keys.Space))
                {
                    if (currentPower == PowerUp.PowerLaserBlast)
                    {

                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), getVectorFromRads(AngleToRads(45)), playerBody.Position, -AngleToRads(45), 50,world));
                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), -getVectorFromRads(AngleToRads(45)), playerBody.Position, -AngleToRads(45),50, world));
                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), -getVectorFromRads(AngleToRads(135)), playerBody.Position, -AngleToRads(135),50 ,world));
                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), getVectorFromRads(AngleToRads(135)), playerBody.Position, -AngleToRads(135), 50,world));

                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), new Vector2(1,0), playerBody.Position, VectorToAngle(new Vector2(1,0)), 50,world));
                        
                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), new Vector2(0, 1), playerBody.Position, VectorToAngle(new Vector2(0, 1)), 50 ,world));

                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), new Vector2(0, -1), playerBody.Position, VectorToAngle(new Vector2(0, -1)), 50,world));

                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), new Vector2(-1, 0), playerBody.Position, VectorToAngle(new Vector2(-1, 0)),50,world));

                        currentPower = PowerUp.None;
                            
                    }

                }

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                   
                  
                     Vector2 mousePos = new Vector2 (mouse.X, mouse.Y);
                   
                    
                    Vector2 target = mousePos + camPos - new Vector2(1024f / 2f, 768f / 2f);

                    
                     Vector2 direction = target - playerBody.Position * 64; // velocity 
                     float angle = Angle(playerBody.Position * 64, target);


                   
                   
                    playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), direction, playerBody.Position, angle,2 ,world));

                   

               
                }

                oldKeyState = keyboardState;

            }





        }

        public List<playerLaser> getLasers
        {

            get { return playerLasers; }
        }
       
        public Vector2 setPostion
        {
            set { playerBody.Position = value / 64; } 
        }
        public int getHealth
        {
            get{return health;}

        }
        public float Angle(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(from.X - to.X,to.Y - from.Y);
        }

        float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.X, vector.Y);
        }

        float AngleToRads(float angle) // you have to flip this to get the correct amount 
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

        public Vector2 getVectorFromRads(double radians)
        {

            return new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));
         
        
        }

        public bool playerBody_OnCollision(Fixture fix1, Fixture fix2, Contact contact)
        {

            if (fix1.Body.BodyId == 1 && fix2.Body.BodyId == 15)
            {

                isAlive = false;

            }
            else if (fix1.Body.BodyId == 1 && fix2.Body.BodyId == 18) // seeker drone laser hitting player
            {
                health -= 10;

            }
            else if (fix1.Body.BodyId == 1 && fix2.Body.BodyId == 3) // shape collision
            {
                isAlive = false;

            }

            
            return true;

        }
    
    }


 }


