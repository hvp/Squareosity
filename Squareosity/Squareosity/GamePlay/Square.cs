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
    class Square
    {
        Texture2D tex;
        Vector2 pos;
        Vector2 orgin = new Vector2(5, 5);
        Body squareBody;
        float size = 10.0f / 64.0f;

      public  bool isTouching = false;

        public Square(Texture2D tex, Vector2 pos, World world)
        {
            this.tex = tex;
            squareBody = BodyFactory.CreateRectangle(world, size, size, 1f, pos / 64);
            squareBody.BodyType = BodyType.Static;
            squareBody.BodyId = 2;
            squareBody.CollisionCategories = Category.Cat7;
            squareBody.CollidesWith = Category.All ^ Category.Cat2;
        }

        public void Update()
        {
            squareBody.OnCollision += new OnCollisionEventHandler(OnCollision);
            squareBody.OnSeparation +=new OnSeparationEventHandler(OnSeparation);

            if (isTouching)
            {
                //GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
            }
            else
            {
                //GamePad.SetVibration(PlayerIndex.One, 0f, +0f);
            }


        }
        public bool OnCollision(Fixture FixtureA, Fixture FixtureB, Contact contact)
        {
            Body fixA = FixtureA.Body;
            Body fixB = FixtureB.Body;

            if (fixA.BodyId == 2 && fixB.BodyId == 1)
            {
                isTouching = true;
            }

            return true;
        }

        private void OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            Body body1 = fixtureA.Body;
            Body body2 = fixtureB.Body;
            if (body1.BodyId == 2 && body2.BodyId == 1)
            {
                isTouching = false;
            }



        }


        public void Draw(SpriteBatch batch)
        {

            batch.Draw(tex, squareBody.Position * 64, null, Color.White, 0f, orgin, 1f, SpriteEffects.None, 0f);

        }



    }
}
