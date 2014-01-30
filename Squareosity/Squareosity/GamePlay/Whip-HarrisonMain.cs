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

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

namespace Squareosity
{
    class Whip
    {
        public List<Body> chainLinks; // need to create a class called 'chainlink'
        Texture2D tex;
        Vector2 start;
        Vector2 end;
        public   bool isTouching = false;
   
        public Whip(Texture2D tex, Vector2 start, Vector2 end, World world )
        {
            this.tex = tex;
           
            this.start = start / 64;
            this.end = end / 64;

            Path path = new Path();

            path.Add(this.start);
            path.Add(this.end);

            //A single chainlink
            PolygonShape shape = new PolygonShape(PolygonTools.CreateRectangle(0.125f, 0.125f),20);

            //Use PathFactory to create all the chainlinks based on the chainlink created before.
            chainLinks = PathManager.EvenlyDistributeShapesAlongPath(world, path, shape, BodyType.Dynamic,30);

            foreach (Body chainLink in chainLinks)
            {
                chainLink.BodyId = 8;
                foreach (Fixture f in chainLink.FixtureList)
                {
                    f.Friction = 0.2f;
                    f.CollisionCategories = Category.Cat3;
                    Category whipMask = Category.All ^ Category.Cat2;
                    f.CollidesWith = whipMask;
                    f.Body.BodyId = 8;
                    f.OnCollision += new OnCollisionEventHandler(OnCollision); 
                    
                }

            }

            //Fix the first chainlink to the world
          //  FixedRevoluteJoint fixedJoint = new FixedRevoluteJoint(chainLinks[0], Vector2.Zero, chainLinks[0].Position);
//            Game1.world.AddJoint(fixedJoint);

            

            //Attach all the chainlinks together with a revolute joint. This is the spacing between links
            List<RevoluteJoint> joints = PathManager.AttachBodiesWithRevoluteJoint(world, chainLinks,
                                                                                   new Vector2(0, -0.1f),
                                                                                   new Vector2(0, 0.1f),
                                                                                   false, false);

            //The chain is breakable
            for (int i = 0; i < joints.Count; i++)
            {
                RevoluteJoint r = joints[i];
                r.Breakpoint = 10000f;
            }


        }

        public void Draw(SpriteBatch batch)
        {

            foreach (Body chainLink in chainLinks)
            {
                batch.Draw(tex, chainLink.Position * 64, null, Color.White, chainLink.Rotation, new Vector2(tex.Width/2,tex.Height/2), 1f, SpriteEffects.None, 1f);
                

            }

        }
        public bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            Body fixa = fixtureA.Body;
            Body fixb = fixtureB.Body;

            if (fixa.BodyId == 8 && fixb.BodyId == 1)
            {

                isTouching = true;
            }
            
                


        return true;
        }

       


        
           
    }

}

