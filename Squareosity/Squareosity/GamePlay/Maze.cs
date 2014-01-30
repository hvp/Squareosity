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
    /// This is the maze class it's used to create complex masezes from textures (it's not working very well)
    /// </summary>
    class Maze
    {

        Texture2D tex; // used to create body 
        Texture2D tex2; // used to render
        Vector2 pos;
        Vector2 orgin;
        Body mazeBody;

        public Maze(Texture2D tex, Texture2D tex2 ,Vector2 pos, World world)
        {
            this.tex = tex;
            this.tex2 = tex2;
            this.pos = pos;

            uint[] data = new uint[tex.Width * tex.Height];

            tex.GetData(data);

            Vertices verts = PolygonTools.CreatePolygon(data, tex.Width);

            // How do we work out what the correct scale factor is? - Trial and error 
            Vector2 scale = new Vector2(0.02f, 0.02f);
            verts.Scale(ref scale);

            Vector2 centroid = -verts.GetCentroid();
            verts.Translate(ref centroid);

            var decomposedVertices = BayazitDecomposer.ConvexPartition(verts);

            mazeBody = BodyFactory.CreateCompoundPolygon(world, decomposedVertices, 1);
            mazeBody.BodyType = BodyType.Static;
            mazeBody.Position = pos / 64;
            mazeBody.BodyId = 5;


            orgin = new Vector2(tex.Width / 2, tex.Height / 2);

        }

        public void draw(SpriteBatch batch)
        {

            // the shapes don't render clean and the edges aren't perfect
            batch.Draw(tex2, mazeBody.Position * 64, null, Color.White, mazeBody.Rotation,
                orgin, 1f, SpriteEffects.None, 1f);


        }

    }
}
