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
 
    /// <summary>
    /// This class holds non - squares. It should take a texture and a position. 
    /// It will then create a shape based off of the texture given to it. 
    /// Might be an idea to use two textures one for genrating the mesh and the other for rendering
    /// </summary>
    class Shape
    {
        Texture2D tex;
        Vector2 pos;
        Vector2 orgin;
      public Body shapeBody;
       public bool isTouching = false;
       bool isStatic = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tex">Texture of shape</param>
        /// <param name="pos">Postion in physics units</param>
        /// <param name="isStatic">True for static</param>
        /// <param name="world">World</param>
        public Shape(Texture2D tex, Vector2 pos, bool isStatic, World world)
        {
            this.tex = tex;
            this.pos = pos;

            uint[] data = new uint[tex.Width * tex.Height];

            tex.GetData(data);

            Vertices verts = PolygonTools.CreatePolygon(data, tex.Width);

            // How do we work out what the correct scale factor is? - Trial and error 
            Vector2 scale = new Vector2(0.025f, 0.025f);
            verts.Scale(ref scale);

            Vector2 centroid = -verts.GetCentroid();
           // orgin = centroid; // just a little test
            verts.Translate(ref centroid);

            var decomposedVertices = BayazitDecomposer.ConvexPartition(verts);

            shapeBody = BodyFactory.CreateCompoundPolygon(world, decomposedVertices, 1);
            if (this.isStatic)
            {
                shapeBody.BodyType = BodyType.Static;
            }
            else
            {
                shapeBody.BodyType = BodyType.Dynamic;
            }
            
            shapeBody.Position = pos / 64;
            shapeBody.BodyId = 3;
            shapeBody.CollisionCategories = Category.Cat3;
            shapeBody.CollidesWith = Category.All ^ Category.Cat2;
            orgin = new Vector2(tex.Width / 2, tex.Height / 2); // this seems to work well
           
        }

        public void update() 
        {
            shapeBody.OnCollision += new OnCollisionEventHandler(OnCollision);
            shapeBody.OnSeparation += new OnSeparationEventHandler(OnSeparation);
        }
        public bool OnCollision(Fixture FixtureA, Fixture FixtureB, Contact contact)
        {
            Body fixA = FixtureA.Body;
            Body fixB = FixtureB.Body;

            if (fixA.BodyId == 3 && fixB.BodyId == 1)
            {
                isTouching = true;
            }

            return true;
        }

        public void OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            Body fixA = fixtureA.Body;
            Body fixB = fixtureB.Body;

            if (fixA.BodyId == 3 && fixB.BodyId == 1)
            {
                isTouching = false;
            }


        }
        public void Draw(SpriteBatch batch) 
        {
          
            // the shapes don't render clean and the edges aren't perfect
            batch.Draw(tex,shapeBody.Position * 64, null, Color.White, shapeBody.Rotation, 
                orgin, 1f, SpriteEffects.None, 1f);

        
        }
       
    }
}
