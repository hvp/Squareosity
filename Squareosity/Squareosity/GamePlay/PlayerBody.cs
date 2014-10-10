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
using FarseerPhysics.SamplesFramework;
namespace Squareosity
{
    enum PowerUp
    {
        None,
        PowerLaserBlast,
        ElectricBlast

    };
    class PlayerBody
    {
      

         PowerUp currentPower = PowerUp.None;
         Vector2 pos = new Vector2(1024/2f, 768 / 2f); // graphics pos
         Vector2 orgin = new Vector2(10, 10);
         Texture2D tex;
         public bool isAlive = true;

         bool laserActive = true;
         bool scoreActive = true;

         double fireRate = 300;
         double timer = 300; // start at 300???
         KeyboardState oldKeyState;
         int health = 100;
         int score;

         bool choicePoint = false;

         int attachedObjCount = 0;

         bool wantsToPickUp = false;
         bool hasPickedUp = false;
         bool wantsToDrop = false;

         bool firePowerUp = false;
         bool drawPowerUpAnimation = false;
         bool hasFiredOne = false;
         float timerPowerUp = 0;
         float frameTimePowerUp = 200; // each frame lasts 200 ms  
         float totalPowerUpTime = 2000; // so the animation will play for 2 secs 
         int powerUpIndex = 0;
         Body powerUpSensor;
         List<Body> appliedHitByPowerUp = new List<Body>();


         Texture2D playerLaser;
         Texture2D powerUpTex;
         Texture2D powerUpDebug;
        
         Vector2 playerPosPowerUp;
         int frameSizeX = 205; 


         Vector2 veloPowerUp = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;
         float rotPowerUp;


         bool enterWithKeyboard = false;

        

         List<Texture2D> electricAnimation = new List<Texture2D>();

         int choice = 99;
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
            playerBody.OnCollision += new OnCollisionEventHandler(playerBody_OnCollision);
            score = 0;
            this.currentPower = PowerUp.None;

            powerUpTex = content.Load<Texture2D>("Animation/Electric/electirc");
            powerUpDebug = content.Load<Texture2D>("powerUpDebug");
            playerLaser = content.Load<Texture2D>("orangeLaser");
         





        }

        public void update(GameTime gameTime)
        {
            if (hasPickedUp)
            {
                laserActive = false;
            }
            else
            {
                laserActive = true; // will need to re code this if I want to set laser active else where.
            }
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

            

            if (scoreActive)
            {
                SpriteFont font = content.Load<SpriteFont>("gamefont");
                Vector2 textPosition = playerBody.Position * 64 - new Vector2(100, 100);
                batch.DrawString(font, score.ToString(), textPosition, Color.White,0f,new Vector2(0,0),1f,SpriteEffects.None,1f);
            }
           batch.Draw(tex, playerBody.Position * 64, null,Color.White, playerBody.Rotation, orgin, 1f, SpriteEffects.None, 1f);
           foreach (playerLaser laser in playerLasers)
           {
               laser.Draw(batch);

           }

           if (firePowerUp)
           {
               drawPowerUpAnimation = true;
               firePowerUp = false;
               veloPowerUp = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;
               veloPowerUp = new Vector2(veloPowerUp.X, veloPowerUp.Y);
               rotPowerUp = VectorToAngle(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right);
             
           }



           if (drawPowerUpAnimation) 
           {
               if (frameSizeX < 2048)
               {
                   // for some reason it's off by 90 degrees
                   batch.Draw(powerUpTex, playerBody.Position * 64f, new Rectangle(frameSizeX, 0, 205, 180), Color.White, rotPowerUp - (1.570f), new Vector2(0, 90f), 1f, SpriteEffects.None, 1f);
                  // batch.Draw(powerUpDebug, powerUpSensor.Position * 64f, null, Color.White, powerUpSensor.Rotation, new Vector2(powerUpDebug.Width / 2, powerUpDebug.Height / 2), 1f, SpriteEffects.None, 1f);

                   timerPowerUp += 100;

                   if (timerPowerUp > frameTimePowerUp)
                   {

                       frameSizeX += 205;
                       timerPowerUp = 0;




                   }


               }
               else
               {
                   drawPowerUpAnimation = false;
                   frameSizeX = 0;
                   timerPowerUp = 0;
                   world.RemoveBody(powerUpSensor);

               }

           }
         
            
            

      
        }

        public void detectInput(KeyboardState keyboardState, MouseState mouse, Vector2 camPos, GameTime gameTime)
        {
            
            // movement with gamePad.
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                float x = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
                float y = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;
                playerBody.ApplyLinearImpulse(new Vector2(x, -y));
                playerBody.LinearDamping = 1f;


                if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right != Vector2.Zero && laserActive)
                {

                 
                    
                        Vector2 velo = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;
                        velo = new Vector2(velo.X, -velo.Y);
                        float rot = VectorToAngle(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right);
                        playerLasers.Add(new playerLaser(playerLaser, velo, playerBody.Position, rot, 2, world));

                        if (GamePad.GetState(PlayerIndex.One).Triggers.Right > 0.5f && drawPowerUpAnimation == false) // Ie it's not already playing
                        {
                            if (powerUpSensor != null)
                            {
                                Console.WriteLine("null sensor");
                            }
                              //  world.RemoveBody(powerUpSensor);
                            
                                firePowerUp = true;
                                appliedHitByPowerUp.Clear();
                                PowerUpSensorIni(playerBody.Position, rot);
                           
                          
                            
                        }
                    
                       
                            
                    

                }
              

                // may need to intrduce a oldState button state for the game pad.
                if (choicePoint)
                {

                    if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                    {
                        choice = 1;
                    }
                    else if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed)
                    {
                        choice = 2;

                    }
                    else if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed)
                    {
                        choice = 3;

                    }
                 

                }

                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                {
                    wantsToPickUp = true;
                }
                else if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed)
                {
                    wantsToDrop = true;
                }

                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Released)
                {
                    wantsToPickUp = false;
                }
                else if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Released)
                {
                    wantsToDrop = false;
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
                if (keyboardState.IsKeyDown(Keys.Q))
                {
                    wantsToPickUp = true;
                }
                else if (keyboardState.IsKeyDown(Keys.E))
                {
                    wantsToDrop = true;
                }
                if (keyboardState.IsKeyUp(Keys.Q))
                {
                    wantsToPickUp = false;
                }
                else if (keyboardState.IsKeyUp(Keys.E))
                {
                    wantsToDrop = false;
                }
                
                
                if (choicePoint)
                {
                    if (keyboardState.IsKeyDown(Keys.Enter) || keyboardState.IsKeyDown(Keys.Enter))
                    {
                        this.choice = 1;
                    }
                    if (keyboardState.IsKeyDown(Keys.Q) || keyboardState.IsKeyDown(Keys.Q))
                    {
                        this.choice = 3;
                    }
                    if (keyboardState.IsKeyDown(Keys.E) || keyboardState.IsKeyDown(Keys.E))
                    {
                        this.choice = 2;
                    }
                }

                if (keyboardState.IsKeyDown(Keys.Space) && oldKeyState.IsKeyUp(Keys.Space))
                {
                    if (currentPower == PowerUp.PowerLaserBlast)
                    {

                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), getVectorFromRads(AngleToRads(45)), playerBody.Position, -AngleToRads(45), 50, world));
                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), -getVectorFromRads(AngleToRads(45)), playerBody.Position, -AngleToRads(45), 50, world));
                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), -getVectorFromRads(AngleToRads(135)), playerBody.Position, -AngleToRads(135), 50, world));
                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), getVectorFromRads(AngleToRads(135)), playerBody.Position, -AngleToRads(135), 50, world));

                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), new Vector2(1, 0), playerBody.Position, VectorToAngle(new Vector2(1, 0)), 50, world));

                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), new Vector2(0, 1), playerBody.Position, VectorToAngle(new Vector2(0, 1)), 50, world));

                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), new Vector2(0, -1), playerBody.Position, VectorToAngle(new Vector2(0, -1)), 50, world));

                        playerLasers.Add(new playerLaser(content.Load<Texture2D>("orangeLaser"), new Vector2(-1, 0), playerBody.Position, VectorToAngle(new Vector2(-1, 0)), 50, world));

                        currentPower = PowerUp.None;

                    }
                    else
                    {

                        if ( drawPowerUpAnimation == false) // Ie it's not already playing
                        {
                            if (powerUpSensor != null)
                            {
                                Console.WriteLine(" not null sensor");
                                world.RemoveBody(powerUpSensor);
                            }
                            //  



                            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);


                            Vector2 target = mousePos + camPos - new Vector2(1024f / 2f, 768f / 2f);


                            Vector2 direction = target - playerBody.Position * 64; // velocity 
                            float angle = Angle(playerBody.Position * 64, target);


                            firePowerUp = true;
                            appliedHitByPowerUp.Clear();
                            PowerUpSensorIni(playerBody.Position, angle);



                        }




                    }

                }

                if (mouse.LeftButton == ButtonState.Pressed && laserActive )
                {

             
                        Vector2 mousePos = new Vector2(mouse.X, mouse.Y);


                        Vector2 target = mousePos + camPos - new Vector2(1024f / 2f, 768f / 2f);


                        Vector2 direction = target - playerBody.Position * 64; // velocity 
                        float angle = Angle(playerBody.Position * 64, target);

                        playerLasers.Add(new playerLaser(playerLaser, direction, playerBody.Position, angle, 2, world));
                     
                    
                }
                else if (mouse.LeftButton == ButtonState.Released)
                {
                    
                }

                oldKeyState = keyboardState;

            }





        }
        /// <summary>
        /// If the value is 99 we are waiting for input from the player 
        /// A = 1
        /// B = 2
        /// X = 3
        /// </summary>
        public int choiceValue
        {
            get { return choice; }


        }
        public bool getSetChoicePoint
        {
            set { choicePoint = value; }
            get { return choicePoint; }
        }
        public double getSetfireRate
        {
            get { return fireRate; }
            set { fireRate = value; }
        }
        public PowerUp getSetPowerUp
        {
            set { currentPower = value; }
            get { return currentPower; }

        }
        public bool getSetDrawScore
        {
            get { return scoreActive; }
            set { scoreActive = value; }

        }

        public bool getSetWantsToPickUp
        {
            set { wantsToPickUp = value; }
            get { return wantsToPickUp; }
        }
        public bool getSetWantsTodrop
        {
            set { wantsToDrop = value; }
            get { return wantsToDrop; }
        }
        public bool getSetHasPickedUp
        {
            set{hasPickedUp = value;}
            get { return hasPickedUp; }
        }
        public void incrementAttachedObjCount()
        {
            this.attachedObjCount += 1;

        }
        public void decrementAttachedObjCount()
        {
            
            this.attachedObjCount -= 1;

        }
     

        public bool getSetLaserStatus
        {
            set { laserActive = value; }
            get { return laserActive; }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos">Sim units</param>
        /// <param name="rot">Rads</param>
        private void PowerUpSensorIni(Vector2 pos, float rot)
        {
            // should have a size setting 
            // powerUpSensor = BodyFactory.CreateRectangle(world, 205, 180, 1f);
        
          
       
            
         //  powerUpSensor = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(205), ConvertUnits.ToSimUnits(180), 1f, pos);

            
            powerUpSensor = BodyFactory.CreateBody(world);
            FixtureFactory.AttachRectangle(185 / 64f,160 / 64f,1f,new Vector2(0),powerUpSensor,pos);
            
            powerUpSensor.Position = pos;

            powerUpSensor.CollidesWith = Category.All ^ Category.Cat1 ^ Category.Cat5 ^ Category.Cat8;
            powerUpSensor.BodyId = 49;
            powerUpSensor.Rotation = rot - 1.57f;
            powerUpSensor.FixedRotation = true;
            powerUpSensor.BodyType = BodyType.Static;
            powerUpSensor.IsSensor = true;  
            powerUpSensor.OnCollision += new OnCollisionEventHandler(powerUpSensor_OnCollision);
            
            
          

        }

        public bool powerUpSensor_OnCollision(Fixture fix1, Fixture fix2, Contact contact)
        {
            if (drawPowerUpAnimation && !appliedHitByPowerUp.Contains(fix2.Body))
            {

                float rot = powerUpSensor.Rotation;

               

                Vector2 angleVector = new Vector2((float)Math.Cos(rot) - (float)Math.Sin(rot));

                angleVector.Normalize();

                fix2.Body.ApplyLinearImpulse(angleVector);

              
                appliedHitByPowerUp.Add(fix2.Body);

            }



            return true;
        }
      


      
    }


 }


