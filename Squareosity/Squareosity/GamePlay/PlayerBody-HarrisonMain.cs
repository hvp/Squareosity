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

     



        public Body playerBody;

        public PlayerBody(Texture2D tex, World world)
        {
            this.tex = tex;


            playerBody = BodyFactory.CreateRectangle(world, 20.0f / 64.0f, 20.0f / 64.0f, 1f, pos / 64.0f);
            playerBody.BodyType = BodyType.Dynamic;
            playerBody.Mass = 2f;
            playerBody.Friction = 0.5f;
            playerBody.BodyId = 1;
            playerBody.CollisionCategories = Category.Cat1;
            playerBody.CollidesWith = Category.All;
            
            


        }

        public void update()
        {
            detectInput();

        }



        public void draw(SpriteBatch batch)
        {
           batch.Draw(tex, playerBody.Position * 64, null, Color.White, playerBody.Rotation, orgin, 1f, SpriteEffects.None, 1f);
    
      
        }

        public void detectInput()
        {
            // movement
            float x = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
            float y = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;
            playerBody.ApplyLinearImpulse(new Vector2(x, -y));
            playerBody.LinearDamping = 1f;

            /* Rotation: 
             * This is really rough and needs a rework 
             * like a momentum roatiation that has some elastic to it.
             * That pulls the square back to 0f when not in use)
            */
           // if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right != Vector2.Zero)
            //{
               // playerBody.AngularVelocity = 0;
             //  playerBody.Rotation = VectorToAngle(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right);
               
            //}
            
            // adust bloom 
            if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                Game1.bloomSettingsIndex = 0;
            if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed)
                Game1.bloomSettingsIndex = 1;
            if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed)
                Game1.bloomSettingsIndex = 2;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed)
                Game1.bloomSettingsIndex = 3;








        }

        float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.X, vector.Y);
        }

        float AngleToRads(float angle)
        {
            return (float)(angle * (Math.PI / 180.0f));       
    
        }
    
    }


 }


