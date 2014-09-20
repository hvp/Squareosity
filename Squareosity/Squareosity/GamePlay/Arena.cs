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

using FarseerPhysics.SamplesFramework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;
namespace Squareosity
{

    /// <summary>
    /// This class holds non - squares. It should take a texture and a position. 
    /// It will then create a shape based off of the texture given to it. 
    /// Might be an idea to use two textures one for genrating the mesh and the other for rendering
    /// </summary>
    class Arena    {
        Texture2D tex;
        Vector2 pos;
        Vector2 orgin;
        public Body arenaBody;
        public bool isTouching = false;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tex">Texture of shape</param>
        /// <param name="pos">Postion in physics units</param>
        /// <param name="world">World</param>
        public Arena(Texture2D tex, Vector2 pos, World world)
        {
            

            
            this.tex = tex;
            this.pos = pos;

            uint[] data = new uint[tex.Width * tex.Height];

            tex.GetData(data);

            Vertices verts = PolygonTools.CreatePolygon(data, tex.Width,false);
            Vector2 centroid = -verts.GetCentroid();
            verts.Translate(ref centroid);
            orgin = -centroid; // just a little test

            verts = SimplifyTools.ReduceByDistance(verts, 4f);
       

           Vector2 scale = ConvertUnits.ToSimUnits(new Vector2(1f)); 
           verts.Scale(ref scale);
          
            
            
            var decomposedVertices = FarseerPhysics.Common.Decomposition.SeidelDecomposer.ConvexPartitionTrapezoid(verts, 0.001f); // USE THIS FOR outlines! 
           
         
            arenaBody = BodyFactory.CreateCompoundPolygon(world, decomposedVertices, 1);
            arenaBody.Position = pos;
            arenaBody.BodyType = BodyType.Static;
            arenaBody.CollisionCategories = Category.Cat3;
            arenaBody.CollidesWith = Category.All ^ Category.Cat2;
            arenaBody.BodyId = 4;
            
           
        }

        public void update()
        {
            arenaBody.OnCollision += new OnCollisionEventHandler(OnCollision);
            arenaBody.OnSeparation += new OnSeparationEventHandler(OnSeparation);
        }
        public Texture2D getTex
        {
            get { return tex; }
        }
        // might cuause problems if we have Walls too. 
        public bool OnCollision(Fixture FixtureA, Fixture FixtureB, Contact contact)
        {
            Body fixA = FixtureA.Body;
            Body fixB = FixtureB.Body;

            if (fixA.BodyId == 4 && fixB.BodyId == 1)
            {
                isTouching = true;
            }

            return true;
        }

        // might cuause problems if we have Walls too. 
        public void OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            Body fixA = fixtureA.Body;
            Body fixB = fixtureB.Body;

            if (fixA.BodyId == 4 && fixB.BodyId == 1)
            {
                isTouching = false;
            }


        }
        public void Draw(SpriteBatch batch)
        {

           
            batch.Draw(tex,ConvertUnits.ToDisplayUnits(arenaBody.Position), null, Color.White, arenaBody.Rotation,
                orgin, 1f, SpriteEffects.None, 0f);


        }

        public float getsetRotation
        {
            get{return arenaBody.Rotation;}
            set { arenaBody.Rotation = value; }
        }

    }
}
