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
    class Area
    {
        Texture2D tex;
        Vector2 pos;
        World world;
        Body AreaBody;

        bool isTouchingPickUp = false;
        public Area(Texture2D tex, Vector2 pos, float rot,World world)
        {
            this.tex = tex;
            this.pos = pos;
            this.world = world;

            AreaBody = BodyFactory.CreateRectangle(world, tex.Width / 64f, tex.Height / 64f, 1f, pos / 64f);
            AreaBody.BodyType = BodyType.Static;
            AreaBody.OnCollision += AreaBody_OnCollision;
            AreaBody.Rotation = rot;
            AreaBody.IsSensor = true;
            AreaBody.OnSeparation += new OnSeparationEventHandler(AreaBody_OnSeparation);

            
        }
        public void Draw(SpriteBatch batch)
        {
            batch.Draw(tex,AreaBody.Position *64,null,Color.White,AreaBody.Rotation,new Vector2(tex.Width / 2, tex.Height /2),1f,SpriteEffects.None,0.1f);
        }
            public bool AreaBody_OnCollision(Fixture fixa, Fixture FixB, Contact contact)
        {
            if (FixB.Body.BodyId == 1)
            {
               // Console.WriteLine("Is touching player");
            }
            if (FixB.Body.BodyId == 19)
            {
                isTouchingPickUp = true;
            }
            return true;

        }
        public void AreaBody_OnSeparation(Fixture fixa, Fixture fixb)
            {
                if (fixb.Body.BodyId == 19)
                {
                    isTouchingPickUp = false;
                }
            }

        public bool getIsPickUpTouching
        {
            get { return isTouchingPickUp; }

        }


    }


}
